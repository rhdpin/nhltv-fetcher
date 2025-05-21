﻿using NhlTvFetcher.Data;
using NhlTvFetcher.Models;
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
        public const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:124.0) Gecko/20100101 Firefox/124.0";        
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
                .Where(f => (!f.IsFrench || _options.French) && f.Away.Equals(teamName, StringComparison.OrdinalIgnoreCase) ||
                f.Home.Equals(teamName, StringComparison.OrdinalIgnoreCase)).ToList();
            if (feedsWithSelectedTeam.Count == 0)
            {
                _messenger.WriteLine($"No game feed for '{teamName}' was found ({startDate.ToShortDateString()}-{endDate.ToShortDateString()})");
                return null;
            }

            var latestDate = feedsWithSelectedTeam[0].Date;
            var isHomeTeam = feedsWithSelectedTeam[0].Home.Equals(teamName, StringComparison.OrdinalIgnoreCase);
            var latestStreams = feedsWithSelectedTeam.Where(f => f.Date.Equals(latestDate));

            Feed chosenFeed = null;

            if (_options.French)
            {
                if (isHomeTeam)
                    chosenFeed = latestStreams.FirstOrDefault(f => f.Type.Equals("home", StringComparison.OrdinalIgnoreCase) && f.IsFrench);
                else
                    chosenFeed = latestStreams.FirstOrDefault(f => f.Type.Equals("away", StringComparison.OrdinalIgnoreCase) && f.IsFrench);

                chosenFeed ??= latestStreams.FirstOrDefault(f => f.IsFrench);
            }

            if (chosenFeed == null)
            {
                if (isHomeTeam)
                    chosenFeed = latestStreams.FirstOrDefault(f => f.Type.Equals("home", StringComparison.OrdinalIgnoreCase) && !f.IsFrench);
                else
                    chosenFeed = latestStreams.FirstOrDefault(f => f.Type.Equals("away", StringComparison.OrdinalIgnoreCase) && !f.IsFrench);
            }

            chosenFeed ??= latestStreams.FirstOrDefault(f => f.Type == "national");
            chosenFeed ??= latestStreams.FirstOrDefault(f => !f.IsFrench);

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
                        if (content.clientContentMetadata.Length == 0 || 
                                !content.clientContentMetadata[0].name.Equals("home", StringComparison.OrdinalIgnoreCase) &&
                                !content.clientContentMetadata[0].name.Equals("away", StringComparison.OrdinalIgnoreCase) &&
                                !content.clientContentMetadata[0].name.Equals("national", StringComparison.OrdinalIgnoreCase) &&
                                !content.clientContentMetadata[0].name.Equals("sportsnet", StringComparison.OrdinalIgnoreCase) &&
                                !content.clientContentMetadata[0].name.Equals("tnt", StringComparison.OrdinalIgnoreCase) &&
                                !content.clientContentMetadata[0].name.Equals("cbc", StringComparison.OrdinalIgnoreCase) &&
                                !content.clientContentMetadata[0].name.Equals("espn", StringComparison.OrdinalIgnoreCase) &&
                                !content.clientContentMetadata[0].name.Equals("abc", StringComparison.OrdinalIgnoreCase) &&
                                !content.clientContentMetadata[0].name.Equals("french", StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        var broadcasterType = content.clientContentMetadata[0].name;

                        if (content.clientContentMetadata[0].name.Equals("sportsnet", StringComparison.OrdinalIgnoreCase) ||
                                content.clientContentMetadata[0].name.Equals("tnt", StringComparison.OrdinalIgnoreCase) ||
                                content.clientContentMetadata[0].name.Equals("espn", StringComparison.OrdinalIgnoreCase) ||
                                content.clientContentMetadata[0].name.Equals("abc", StringComparison.OrdinalIgnoreCase) ||
                                content.clientContentMetadata[0].name.Equals("cbc", StringComparison.OrdinalIgnoreCase))
                        {
                            broadcasterType = "national";
                        }

                        var allBroadcasters = broadcasts.Where(b => b.HomeTeam.Equals(game.homeCompetitor.shortName, StringComparison.OrdinalIgnoreCase)
                                && b.AwayTeam.Equals(game.awayCompetitor.shortName, StringComparison.OrdinalIgnoreCase));

                        if (broadcasterType.Equals("french", StringComparison.OrdinalIgnoreCase))
                        {
                            allBroadcasters = allBroadcasters.Where(b => b.Language == "fr");
                        }
                        else
                        {      
                            allBroadcasters = allBroadcasters.Where(b => b.Language == "en" && 
                                GetStreamType(b.Type).Equals(broadcasterType, StringComparison.OrdinalIgnoreCase));
                        }

                        Broadcast broadcaster = null;
                        string broadcasterString = null;

                        if  (allBroadcasters.Any())
                        {
                            broadcaster = allBroadcasters.FirstOrDefault(b => b.Name.Equals(content.clientContentMetadata[0].name,
                                StringComparison.InvariantCultureIgnoreCase));

                            if (broadcaster != null)
                            {
                                broadcasterString = broadcaster.Name;                                
                            }
                            else
                            {
                                broadcaster = allBroadcasters.First();
                                if (content.clientContentMetadata[0].name.Equals("home", StringComparison.OrdinalIgnoreCase) ||
                                    content.clientContentMetadata[0].name.Equals("away", StringComparison.OrdinalIgnoreCase))
                                {
                                    broadcasterString = broadcaster.Name;
                                }
                                else
                                {
                                    broadcasterString = content.clientContentMetadata[0].name;
                                }
                            }                        
                        }                        

                        var feed = new Feed()
                        {
                            Home = game.homeCompetitor.shortName,
                            Away = game.awayCompetitor.shortName,                            
                            Date = broadcaster != null ? broadcaster.Date : game.startTime,
                            Id = idCounter,
                            MediaId = content.id.ToString(),
                            Type = broadcaster != null ? GetStreamType(broadcaster.Type): content.clientContentMetadata[0].name.ToLower(),
                            Broadcaster = broadcasterString,
                            IsFrench = content.clientContentMetadata[0].name.Equals("french", StringComparison.OrdinalIgnoreCase)
                        };

                        feeds.Add(feed);
                        idCounter++;
                    }
                }
            }
            
            return feeds;

            string GetStreamType(string broadcastType)
            {
                if (broadcastType == "H")
                {
                    return "home";
                }
                else if (broadcastType == "A")
                {
                    return "away";
                }
                else if (broadcastType == "N")
                {
                    return "national";
                }
                else
                {
                    return broadcastType;
                }
            }
        }

        private List<Broadcast> GetBroadcasters(DateTime start, DateTime end)
        {
            string startDate, endDate;
            var broadcasts = new List<Broadcast>();

            startDate = start.ToString("yyyy-MM-dd");
            endDate = end.ToString("yyyy-MM-dd");

            try
            {
                string schedulerUrl = "https://api-web.nhle.com/v1/schedule/" + startDate;
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

                foreach (var date in model.GameWeek)
                {                    
                    foreach (var game in date.Games)
                    {
                        if (game.TvBroadcasts != null)
                        {
                            foreach (var broadcastItem in game.TvBroadcasts)
                            {
                                var broadcast = new Broadcast()
                                {
                                    HomeTeam = game.HomeTeam.Abbrev,
                                    AwayTeam = game.AwayTeam.Abbrev,
                                    Type = broadcastItem.Market,
                                    Name = broadcastItem.Network,
                                    Date = DateTime.Parse(date.Date)
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
