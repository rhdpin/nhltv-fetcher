using NhlTvFetcher.Data;
using NhlTvFetcher.Models;
using NhlTvFetcher.Models.Json.Schedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace NhlTvFetcher {
    public class FeedFetcher {
        private readonly Messenger _messenger;
        private readonly Options _options;
        private readonly Session _session;

        public FeedFetcher(Messenger messenger, Options options, Session session) {
            _messenger = messenger;
            _options = options;
            _session = session;
        }

        public Feed GetLatestFeedForTeam(string teamName, DateTime startDate, DateTime endDate) {
            _messenger.WriteLine(
                $"Getting latest feed for {teamName} for game played between {startDate} and {endDate}",
                LogMessageCategory.Verbose);

            var feeds = GetFeeds(startDate, endDate);

            var feedsWithSelectedTeam = feeds.OrderByDescending(f => f.Date)
                .Where(f => (!f.IsFrench || _options.French) &&
                            f.Away.Equals(teamName) ||
                            f.Home.Equals(teamName)).ToList();
            if (feedsWithSelectedTeam.Count == 0) {
                _messenger.WriteLine(
                    $"No game feed for '{teamName}' was found ({startDate.ToShortDateString()}-{endDate.ToShortDateString()})");
                return null;
            }

            var latestDate = feedsWithSelectedTeam[0].Date;
            var isHomeTeam = feedsWithSelectedTeam[0].Home.Equals(teamName);
            var latestStreams = feedsWithSelectedTeam.Where(f => f.Date.Equals(latestDate));

            Feed chosenFeed = null;

            if (_options.French) {
                if (isHomeTeam)
                    chosenFeed = latestStreams.FirstOrDefault(f =>
                        f.Type.Equals("HOME") && f.IsFrench);
                else
                    chosenFeed = latestStreams.FirstOrDefault(f =>
                        f.Type.Equals("AWAY") && f.IsFrench);

                chosenFeed ??= latestStreams.FirstOrDefault(f => f.IsFrench);
            }

            if (chosenFeed == null) {
                if (isHomeTeam)
                    chosenFeed = latestStreams.FirstOrDefault(f =>
                        f.Type.Equals("HOME") && !f.IsFrench);
                else
                    chosenFeed = latestStreams.FirstOrDefault(f =>
                        f.Type.Equals("AWAY") && !f.IsFrench);
            }

            chosenFeed ??= latestStreams.FirstOrDefault(f => !f.IsFrench);

            return chosenFeed;
        }

        public string GetStreamUrl(Feed feed) {
            _session.LogIn();

            var checkAccessUrl = Session.NhlTv.CheckAccessUri.Replace("%MEDIA_ID%", feed.MediaId);
            _messenger.WriteLine($"Trying to get access token from {checkAccessUrl}",
                LogMessageCategory.Verbose);

            string authCode = null;
            using (var webClient = new WebClient()) {
                webClient.Headers.Add(HttpRequestHeader.UserAgent, Session.NhlTv.UserAgent);
                webClient.Headers.Add(HttpRequestHeader.ContentType,
                    "application/json;charset=UTF-8");
                webClient.Headers.Add(HttpRequestHeader.Cookie, _session.SessionToken);

                webClient.Headers.Add("Origin", Session.NhlTv.OriginUri);

                var json = webClient.UploadString(checkAccessUrl, "{\"type\":\"nhl\"}");
                var model = json.Deserialize<Models.Json.CheckAccess.Rootobject>();
                model = json.Deserialize<Models.Json.CheckAccess.Rootobject>();
                authCode = model.data;
            }

            var playerSettingsUrl =
                Session.NhlTv.PlayerSettingsUri.Replace("%MEDIA_ID%", feed.MediaId);
            _messenger.WriteLine($"Trying to get player settings from {playerSettingsUrl}",
                LogMessageCategory.Verbose);

            string accessUrl = null;
            using (var webClient = new WebClient()) {
                webClient.Headers.Add(HttpRequestHeader.UserAgent, Session.NhlTv.UserAgent);
                webClient.Headers.Add(HttpRequestHeader.Cookie, _session.SessionToken);

                webClient.Headers.Add("Origin", Session.NhlTv.OriginUri);

                var json = webClient.DownloadString(playerSettingsUrl);
                var model = json.Deserialize<Models.Json.Player.Rootobject>();
                accessUrl = !model.streamAccess.Contains('?')
                    ? $"{model.streamAccess}?authorization_code={authCode}"
                    : $"{model.streamAccess}&authorization_code={authCode}";
            }

            _messenger.WriteLine($"Getting the feed...");
            _messenger.WriteLine($"Trying to get stream URL from {accessUrl}",
                LogMessageCategory.Verbose);

            string streamUrl = null;
            using (var webClient = new WebClient()) {
                webClient.Headers.Add(HttpRequestHeader.UserAgent, Session.NhlTv.UserAgent);
                webClient.Headers.Add(HttpRequestHeader.ContentType,
                    "application/json;charset=UTF-8");
                webClient.Headers.Add("Origin", Session.NhlTv.OriginUri);

                var json = webClient.UploadString(accessUrl, "");
                var model = json.Deserialize<Models.Json.StreamUrl.Rootobject>();
                streamUrl = model.data.stream;
            }

            return streamUrl;
        }

        /// <summary>
        /// Retrieves the NHL schedule between the given start and end dates, respectively.
        /// NOTE: The NHL Schedule API returns in pages of 20. So if more than 20 games are
        /// spanned across, more calls are necessary to grab all data.
        /// </summary>
        /// <param name="startDate">The first day & time to get the schedule</param>
        /// <param name="endDate">The last day & time to get the schedule</param>
        /// <returns>An object containing the NHL schedule.</returns>
        private Rootobject GetSchedule(DateTime startDate, DateTime endDate) {
            string startDateString, endDateString;
            DateTime curDate = startDate;
            Rootobject fullModel = new Rootobject();

            startDateString = startDate.ToString("yyyy-MM-dd");
            endDateString = endDate.ToString("yyyy-MM-dd");

            string schedulerUrl =
                Session.NhlTv.SchedulerUri.Replace("%START_DATE%", startDateString)
                    .Replace("%END_DATE%", endDateString);
            _messenger.WriteLine($"Trying to get NHL schedule data from {schedulerUrl}",
                LogMessageCategory.Verbose);

            Rootobject model;
            using (var webClient = new WebClient()) {
                webClient.Headers.Add("User-Agent", Session.NhlTv.UserAgent);
                var json = webClient.DownloadString(schedulerUrl);
                fullModel = json.Deserialize<Rootobject>();

                while (fullModel.meta.current_page < fullModel.meta.last_page) {
                    json = webClient.DownloadString(schedulerUrl +
                                                    $"&page={fullModel.meta.current_page + 1}");
                    model = json.Deserialize<Rootobject>();

                    fullModel.data = [..fullModel.data, ..model.data];
                    fullModel.meta = model.meta;
                }
            }

            return fullModel;
        }

        public IEnumerable<Feed> GetFeeds(DateTime start, DateTime end) {
            var schedule = GetSchedule(start, end);
            var broadcasts = GetBroadcasters(start, end);
            var feeds = new List<Feed>();
            int idCounter = 1;

            foreach (var game in schedule.data) {
                int day = game.startTime.Day;
                foreach (var content in game.content) {
                    if (content.clientContentMetadata.Length != 0) {
                        switch (content.status.id) {
                            case 3: // replay
                            case 4: // live
                                var nationalBroadcaster = "";
                                var isNational = false;
                                var isFrench = false;

                                var broadcasterType = content.clientContentMetadata[0].name =
                                    content.clientContentMetadata[0].name.ToUpper();

                                // determine if Home/Away/National/French
                                if (!broadcasterType.Equals("HOME")
                                    && !broadcasterType.Equals("AWAY")) {
                                    if (broadcasterType.Equals("FRENCH")) {
                                        isFrench = true;
                                    }
                                    else {
                                        isNational = true;
                                    }
                                }

                                // get all broadcasters for the given game
                                var allBroadcasters = broadcasts.Where(b =>
                                    b.HomeTeam.Equals(game.homeCompetitor.shortName)
                                    && b.AwayTeam.Equals(game.awayCompetitor.shortName));

                                // trim to only EN or FR feeds
                                if (isFrench) {
                                    allBroadcasters =
                                        allBroadcasters.Where(b => b.Language == "fr");
                                }
                                else {
                                    allBroadcasters = allBroadcasters.Where(b =>
                                        b.Language == "en"
                                        && GetStreamType(b.Type)
                                            .Equals(isNational ? "NATIONAL" : broadcasterType));
                                }

                                Broadcast broadcaster = null;
                                string broadcasterString = null;

                                if (allBroadcasters.Any()) {
                                    if (allBroadcasters.Count() == 1) {
                                        broadcaster = allBroadcasters.First();
                                    }
                                    else {
                                        broadcaster = allBroadcasters.FirstOrDefault(b =>
                                                          b.Name.Equals(broadcasterType)) ??
                                                      allBroadcasters.FirstOrDefault(b =>
                                                          broadcasterType.Contains(b.Name));
                                    }

                                    //broadcasterType.Contains(b.Name)
                                    if (broadcaster != null) {
                                        broadcasterString = broadcaster.Name;
                                    }
                                    else {
                                        broadcaster = broadcasterType.Equals("SPORTSNET")
                                            ? allBroadcasters.FirstOrDefault(b =>
                                                b.Name.Contains("SN"))
                                            : allBroadcasters.First();

                                        if (isNational || isFrench) {
                                            broadcasterString = broadcaster.Name;
                                        }
                                        else {
                                            broadcasterString = (broadcaster.Name == "NHLN")
                                                ? broadcasterType
                                                : broadcaster.Name;
                                        }

                                        //if (broadcasterType.Equals("HOME") ||
                                        //    broadcasterType.Equals("AWAY")) {
                                        //    broadcasterString = broadcaster.Name;
                                        //}
                                        //else {
                                        //    broadcasterString = (broadcaster.Name == "NHLN")
                                        //        ? broadcasterType
                                        //        : broadcaster.Name;
                                        //}
                                    }
                                }

                                var feed = new Feed() {
                                    Home = game.homeCompetitor.shortName,
                                    Away = game.awayCompetitor.shortName,
                                    Date = game.startTime != DateTime.MinValue
                                        ? game.startTime
                                        : broadcaster.Date,
                                    Id = idCounter,
                                    MediaId = content.id.ToString(),
                                    Type = broadcaster != null
                                        ? GetStreamType(broadcaster.Type)
                                        : broadcasterType,
                                    Broadcaster = broadcasterString,
                                    IsFrench = isFrench,
                                    IsReplay = (content.status.id == 4)
                                };

                                feeds.Add(feed);
                                idCounter++;

                                break;

                            case 2: // future
                            default:
                                break;
                        }
                    }
                }
            }

            return feeds;
        }

        private string GetStreamType(string broadcastType) {
            if (broadcastType == "H") {
                return "HOME";
            }

            if (broadcastType == "A") {
                return "AWAY";
            }

            if (broadcastType == "N") {
                return "NATIONAL";
            }

            return broadcastType;
        }

        /// <summary>
        /// Retrieves the NHL broadcasting networks for games between the given start
        /// and end dates, respectively. NOTE: The NHL API returns in weeks, not days.
        /// So if multiple weeks are spanned across, another call is necessary to grab
        /// each successive week.
        /// </summary>
        /// <param name="startDate">The first day & time to get the schedule</param>
        /// <param name="endDate">The last day & time to get the schedule</param>
        /// <returns>An object containing the NHL schedule.</returns>
        private List<Broadcast> GetBroadcasters(DateTime startDate, DateTime endDate) {
            string startDateString;
            DateTime curDate = DateTime.MinValue;
            var broadcasts = new List<Broadcast>();

            try {
                Models.Json.ScheduleBroadcast.RootObject model;

                do {
                    startDateString = startDate.ToString("yyyy-MM-dd");
                    string schedulerUrl = "https://api-web.nhle.com/v1/schedule/" + startDateString;
                    _messenger.WriteLine($"Trying to get NHL schedule data from {schedulerUrl}",
                        LogMessageCategory.Verbose);

                    using (var webClient = new System.Net.WebClient()) {
                        var json = webClient.DownloadString(schedulerUrl);
                        model = json.Deserialize<Models.Json.ScheduleBroadcast.RootObject>();
                    }

                    foreach (var date in model.GameWeek) {
                        curDate = DateTime.Parse(date.Date);

                        if (DateTime.Compare(curDate, endDate.Date) < 0) {
                            foreach (var game in date.Games) {
                                if (game.TvBroadcasts != null) {
                                    foreach (var broadcastItem in game.TvBroadcasts) {
                                        var broadcast = new Broadcast() {
                                            HomeTeam = game.HomeTeam.Abbrev.ToUpper(),
                                            AwayTeam = game.AwayTeam.Abbrev.ToUpper(),
                                            Type = broadcastItem.Market.ToUpper(),
                                            Name = broadcastItem.Network.ToUpper(),
                                            Date = curDate
                                        };
                                        broadcasts.Add(broadcast);
                                    }
                                }
                            }
                        }
                    }

                    startDateString = curDate.Date.AddDays(1).ToString("yyyy-MM-dd");
                } while (DateTime.Compare(curDate, endDate) < 0);
            }
            catch (Exception ex) {
                _messenger.WriteLine($"Getting broadcasts failed: {ex.Message}",
                    LogMessageCategory.Verbose);
            }

            return broadcasts;
        }
    }
}
