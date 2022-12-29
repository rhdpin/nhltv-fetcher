using NhlTvFetcher.Data;
using NhlTvFetcher.Models;
using NhlTvFetcher.Models.Json;
using NhlTvFetcher.Models.Json.Schedule;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;

namespace NhlTvFetcher
{
    public class FeedFetcher
    {
        public const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/106.0.0.0 Safari/537.36";

        private readonly Messenger _messenger;
        private readonly Options _options;

        public FeedFetcher(Messenger messenger, Options options)
        {
            _messenger = messenger;
            _options = options;
        }

        public Feed GetLatestFeedForTeam(string teamName, DateTime startDate, DateTime endDate)
        {
            _messenger.WriteLine($"Getting latest feed for {teamName} for game played between {startDate} and {endDate}", MessageCategory.Verbose);
                        
            var feeds = GetFeeds(startDate, endDate);

            var feedsWithSelectedTeam = feeds.OrderByDescending(f => f.Date)
                .Where(f => f.Away.Equals(teamName, StringComparison.OrdinalIgnoreCase) ||
                f.Home.Equals(teamName, StringComparison.OrdinalIgnoreCase)).ToList();
            if (feedsWithSelectedTeam.Count == 0)
            {
                _messenger.WriteLine($"No game for '{teamName}' was found ({startDate.ToShortDateString()}-{endDate.ToShortDateString()})");
                return null;
            }

            var latestDate = feedsWithSelectedTeam[0].Date;
            var isHomeTeam = feedsWithSelectedTeam[0].Home.Equals(teamName, StringComparison.OrdinalIgnoreCase);
            var latestStreams = feedsWithSelectedTeam.Where(f => f.Date.Equals(latestDate));

            Feed chosenFeed;
            if (isHomeTeam)
                chosenFeed = latestStreams.FirstOrDefault(f => f.Type.Equals("home", StringComparison.OrdinalIgnoreCase));
            else
                chosenFeed = latestStreams.FirstOrDefault(f => f.Type.Equals("away", StringComparison.OrdinalIgnoreCase));

            chosenFeed ??= latestStreams.FirstOrDefault();

            return chosenFeed;
        }

        public string GetStreamUrl(Feed feed)
        {            
            string loginCookie = "";
            var serializationOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            _messenger.WriteLine($"Logging in...");
            var loginUrl = $"https://nhltv.nhl.com/api/v3/sso/nhl/login";            
            
            string configFilePath = _options.AuthFilePath ?? Path.Combine(Directory.GetCurrentDirectory(), "auth.json");            
            if (!File.Exists(configFilePath))
            {
                _messenger.WriteLine("File used to authenticate with NHL.TV not found (" + configFilePath + "). By default it is auth.json in current directory, or a custom path for auth file can be specified with '-a' parameter.\n\nExample content of auth.json:\n{ \"email\": \"myemail@somemail.com\", \"password\": \"myaccountpassword\" }");
                return null;
            }

            var configJson = File.ReadAllText(configFilePath);
            using (var webClient = new WebClient())
            {
                webClient.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);
                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                webClient.Headers.Add(HttpRequestHeader.Accept, "application/json, text/plain, */*");
                webClient.Headers.Add("Origin", "https://nhltv.nhl.com");

                var json = webClient.UploadString(loginUrl, configJson);
                var model = JsonSerializer.Deserialize<Models.Json.Login.Rootobject>(json, serializationOptions);
                loginCookie = $"token={model.token}";
            }            

            var checkAccessUrl = $"https://nhltv.nhl.com/api/v3/contents/{feed.MediaId}/check-access";
            _messenger.WriteLine($"Trying to check access from {checkAccessUrl}", MessageCategory.Verbose);
            string authCode = null;
            using (var webClient = new WebClient())
            {
                webClient.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);
                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json;charset=UTF-8");
                webClient.Headers.Add(HttpRequestHeader.Cookie, loginCookie);

                webClient.Headers.Add("Origin", "https://nhltv.nhl.com");

                var json = webClient.UploadString(checkAccessUrl, "{\"type\":\"nhl\"}");                
                var model = JsonSerializer.Deserialize<Models.Json.CheckAccess.Rootobject>(json, serializationOptions);
                authCode = model.data;
            }

            var playerSettingsUrl = $"https://nhltv.nhl.com/api/v3/contents/{feed.MediaId}/player-settings";
            _messenger.WriteLine($"Trying to get player settings from {playerSettingsUrl}", MessageCategory.Verbose);
            string accessUrl = null;
            using (var webClient = new WebClient())
            {
                webClient.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);
                webClient.Headers.Add(HttpRequestHeader.Cookie, loginCookie);

                webClient.Headers.Add("Origin", "https://nhltv.nhl.com");

                var json = webClient.DownloadString(playerSettingsUrl);                
                var model = JsonSerializer.Deserialize<Models.Json.Player.Rootobject>(json, serializationOptions);
                accessUrl = !model.streamAccess.Contains('?') ? $"{model.streamAccess}?authorization_code={authCode}" :
                    $"{model.streamAccess}&authorization_code={authCode}";
            }

            _messenger.WriteLine($"Getting the feed...");
            string streamUrl = null;
            _messenger.WriteLine($"Trying to get stream URL from {accessUrl}", MessageCategory.Verbose);
            using (var webClient = new WebClient())
            {
                webClient.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);
                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json;charset=UTF-8");
                webClient.Headers.Add("Origin", "https://nhltv.nhl.com");

                var json = webClient.UploadString(accessUrl, "");                
                var model = JsonSerializer.Deserialize<Models.Json.StreamUrl.Rootobject>(json, serializationOptions);
                streamUrl = model.data.stream;
            }            

            return streamUrl;
        }


        private Rootobject GetSchedule(DateTime start, DateTime end)
        {
            string startDate, endDate;

            startDate = start.ToString("yyyy-MM-dd");
            endDate = end.ToString("yyyy-MM-dd");

            string schedulerUrl = "https://nhltv.nhl.com/api/v2/events?date_time_from=" + startDate + "T00:00:00-04:00&date_time_to=" + endDate
                    + "T00:00:00-04:00&sort_direction=asc";
            _messenger.WriteLine($"Trying to get NHL schedule data from {schedulerUrl}", MessageCategory.Verbose);
            Rootobject model;

            using (var webClient = new WebClient())
            {
                webClient.Headers.Add("User-Agent", UserAgent);
                var json = webClient.DownloadString(schedulerUrl);

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };

                model = JsonSerializer.Deserialize<Rootobject>(json, options);
            }
            return model;
        }

        public IEnumerable<Feed> GetFeeds(DateTime start, DateTime end)
        {
            var schedule = GetSchedule(start, end);
            var broadcasts = GetBroadcasters(start, end);
            var feeds = new List<Feed>();
            int idCounter = 1;

            foreach (var game in schedule.data)
            {
                foreach (var content in game.content)
                {
                    if (content.status.id == 4)
                    {
                        if (content.clientContentMetadata.Length == 0 || !content.clientContentMetadata[0].name.Equals("home", StringComparison.OrdinalIgnoreCase) &&
                                !content.clientContentMetadata[0].name.Equals("away", StringComparison.OrdinalIgnoreCase) &&
                                !content.clientContentMetadata[0].name.Equals("national", StringComparison.OrdinalIgnoreCase) &&
                                !content.clientContentMetadata[0].name.Equals("french", StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        var allBroadcasters = broadcasts.Where(b => b.HomeTeam.Equals(game.homeCompetitor.name, StringComparison.OrdinalIgnoreCase)
                                && b.AwayTeam.Equals(game.awayCompetitor.name, StringComparison.OrdinalIgnoreCase));

                        if (content.clientContentMetadata[0].name.Equals("french", StringComparison.OrdinalIgnoreCase))
                        {
                            allBroadcasters = allBroadcasters.Where(b => b.Language.Equals("fr", StringComparison.OrdinalIgnoreCase));
                        }
                        else
                        {
                            allBroadcasters = allBroadcasters.Where(b => b.Language.Equals("en", StringComparison.OrdinalIgnoreCase) && 
                                b.Type.Equals(content.clientContentMetadata[0].name, StringComparison.OrdinalIgnoreCase));
                        }                         
                        
                        var broadcaster = allBroadcasters.Any() ? allBroadcasters.First() : null;
                        var broadcasterString = allBroadcasters.Any() ? string.Join('/', allBroadcasters.Select(b => b.Name).ToArray()) : null;

                        var feed = new Feed()
                        {
                            Home = game.homeCompetitor.shortName,
                            Away = game.awayCompetitor.shortName,                            
                            Date = game.startTime.ToShortDateString(),
                            Id = idCounter,
                            MediaId = content.id.ToString(),
                            Type = broadcaster != null ? broadcaster.Type : content.clientContentMetadata[0].name.ToLower(),
                            Broadcaster = broadcasterString,
                            IsFrench = content.clientContentMetadata[0].name.Equals("french", StringComparison.OrdinalIgnoreCase)
                        };

                        feeds.Add(feed);
                        idCounter++;
                    }
                }
            }
            
            return feeds;
        }

        private List<Broadcast> GetBroadcasters(DateTime start, DateTime end)
        {
            string startDate, endDate;
            var broadcasts = new List<Broadcast>();

            startDate = start.ToString("yyyy-MM-dd");
            endDate = end.ToString("yyyy-MM-dd");

            try
            {
                string schedulerUrl = "https://statsapi.web.nhl.com/api/v1/schedule?startDate=" + startDate + "&endDate=" + endDate
                    + "&expand=schedule.teams,schedule.linescore,schedule.broadcasts.all";
                _messenger.WriteLine($"Trying to get NHL schedule data from {schedulerUrl}", MessageCategory.Verbose);
                Models.Json.ScheduleBroadcast.RootObject model;

                using (var webClient = new System.Net.WebClient())
                {
                    var json = webClient.DownloadString(schedulerUrl);

                    var options = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        WriteIndented = true
                    };

                    model = JsonSerializer.Deserialize<Models.Json.ScheduleBroadcast.RootObject>(json, options);
                }

                foreach (var date in model.Dates)
                {
                    foreach (var game in date.Games)
                    {
                        if (game.Broadcasts != null)
                        {
                            foreach (var broadcastItem in game.Broadcasts)
                            {
                                var broadcast = new Broadcast()
                                {
                                    HomeTeam = game.Teams.Home.Team.Name,
                                    AwayTeam = game.Teams.Away.Team.Name,
                                    Type = broadcastItem.Type,
                                    Name = broadcastItem.Name,
                                    Date = DateTime.Parse(date.Date),
                                    Language = broadcastItem.Language
                                };
                                broadcasts.Add(broadcast);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _messenger.WriteLine($"Getting broadcasts failed: {ex.Message}", MessageCategory.Verbose);
            }

            return broadcasts;
        }
    }
}
