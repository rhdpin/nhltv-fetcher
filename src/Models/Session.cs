using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using NhlTvFetcher.Data;

namespace NhlTvFetcher.Models {
    [SuppressMessage("ReSharper", "ConvertToUsingDeclaration")]
    public class Session {
        private readonly Options _options;
        private readonly Messenger _messenger;

        public static class NhlTv {
            public const string CheckAccessUri =
                "https://nhltv.nhl.com/api/v3/contents/%MEDIA_ID%/check-access";

            public const string LoginUri = "https://nhltv.nhl.com/api/v3/sso/nhl/login";
            public const string LogoutUri = "https://nhltv.nhl.com/api/v3/sso/nhl/logout";
            public const string OriginUri = "https://nhltv.nhl.com";

            public const string PlayerSettingsUri =
                "https://nhltv.nhl.com/api/v3/contents/%MEDIA_ID%/player-settings";

            public const string SchedulerUri =
                "https://nhltv.nhl.com/api/v2/events?date_time_from=%START_DATE%T00:00:00-04:00&date_time_to=%END_DATE%T23:59:59-07:00&sort_direction=asc";

            public const string UserAgent =
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.69 (KHTML, like Gecko) Chrome/129.0.0.0 Safari/537.69";
        }

        public static int PlayerScreenPosition { get; set; } = 0;
        public static IEnumerable<Feed> FeedsListCache { get; set; } = new List<Feed>();
        public static string FeedsListOutputCache { get; set; }
        public static string NhlTvAccountInfo { get; set; }
        public static string LogLevel { get; set; } = nameof(LogMessageCategory.Normal);
        public string SessionToken { get; set; }

        /// <summary>
        /// Creates a session helper class with defined values for use in accessing the NHLTV API.
        /// </summary>
        /// <param name="messenger"></param>
        /// <param name="options"></param>
        public Session(Messenger messenger, Options options) {
            _messenger = messenger;
            _options = options;

            SetLogLevel();
            SetNhlTvAccountInfo();
        }

        /// <summary>
        /// Gets a defined screen position for the current stream.
        /// </summary>
        /// <returns>A screen position for the current stream, e.g. Top-Left, Top-Right, Bottom-Left, Bottom-Right, Center.</returns>
        public static string GetScreenPosition(List<string> playerScreenPositionParameters) {
            if (playerScreenPositionParameters == null) {
                return "";
            }

            int curPosition = PlayerScreenPosition++;
            PlayerScreenPosition %= playerScreenPositionParameters.Count;

            //ScreenPosition %= TiledScreenParameterValues.Length;

            return playerScreenPositionParameters[curPosition];
        }

        /// <summary>
        /// Sets the log level for the session.
        /// </summary>
        private void SetLogLevel() {
            LogLevel = _options.VerboseMode
                ? nameof(LogMessageCategory.Verbose)
                : nameof(LogMessageCategory.Normal);
        }

        /// <summary>
        /// Sets the NHLTV account information for the session.
        /// </summary>
        /// <exception cref="FileNotFoundException"></exception>
        private void SetNhlTvAccountInfo() {
            var authFile = _options.AuthFilePath ??
                           Path.Combine(Directory.GetCurrentDirectory(), "auth.json");
            if (!File.Exists(authFile)) {
                throw new FileNotFoundException(
                    "File used to authenticate with NHL.TV not found (" + NhlTvAccountInfo +
                    "). By default it is auth.json in current directory, or a custom path for auth file can be specified with '-a' parameter.\n\nExample content of auth.json:\n{ \"email\": \"myemail@somemail.com\", \"password\": \"myaccountpassword\" }");
            }

            NhlTvAccountInfo = File.ReadAllText(authFile);
        }

        /// <summary>
        /// Gets the NHLTV API's "Check Access" URL with the given Media ID.
        /// </summary>
        /// <param name="mediaId">A valid NHL API Media ID.</param>
        /// <returns>A URL for checking access to the NHL API.</returns>
        public static string GetCheckAccessUrl(string mediaId) {
            return NhlTv.CheckAccessUri.Replace("%MEDIA_ID", mediaId);
        }

        /// <summary>
        /// Gets the NHLTV API's "Player Settings" URL with the given Media ID.
        /// </summary>
        /// <param name="mediaId">A valid NHL API Media ID.</param>
        /// <returns>A URL for getting the player settings.</returns>
        public static string GetPlayerSettingsUrl(string mediaId) {
            return NhlTv.PlayerSettingsUri.Replace("%MEDIA_ID", mediaId);
        }

        /// <summary>
        /// Gets the NHLTV API's "Scheduler" URL with the given Media ID.
        /// </summary>
        /// <param name="mediaId">A valid NHL API Media ID.</param>
        /// <returns>A URL for getting the current schedule.</returns>
        public static string GetSchedulerUrl(string mediaId) {
            return NhlTv.SchedulerUri.Replace("%MEDIA_ID", mediaId);
        }

        /// <summary>
        /// Logs in to the NHL API and retrieves the necessary token for use in getting feeds.
        /// </summary>
        public void LogIn() {
            _messenger.WriteLine("Logging in...", LogMessageCategory.Verbose);
            try {
                using (var webClient = new WebClient()) {
                    webClient.Headers.Add(HttpRequestHeader.UserAgent, NhlTv.UserAgent);
                    webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    webClient.Headers.Add(HttpRequestHeader.Accept,
                        "application/json, text/plain, */*");
                    webClient.Headers.Add("Origin", NhlTv.OriginUri);

                    var jsonResult = webClient.UploadString(NhlTv.LoginUri, NhlTvAccountInfo);
                    SessionToken = "token=" +
                                   jsonResult.Deserialize<Models.Json.Login.Rootobject>().token;
                }
            }
            catch (Exception e) {
                _messenger.WriteLine(e, LogMessageCategory.Verbose);
                throw;
            }
        }

        /// <summary>
        /// Log out of NHL.TV to avoid session buildup, causing eventual refusal of service.
        /// </summary>
        public void LogOut() {
            _messenger.WriteLine("Logging out...", LogMessageCategory.Verbose);

            try {
                using (WebClient webClient = new WebClient()) {
                    webClient.Headers.Add(HttpRequestHeader.UserAgent, NhlTv.UserAgent);
                    webClient.Headers.Add(HttpRequestHeader.ContentType,
                        "application/json;charset=UTF-8");
                    webClient.Headers.Add(HttpRequestHeader.Cookie, SessionToken);
                    webClient.Headers.Add(HttpRequestHeader.Accept,
                        "application/json, text/plain, */*");
                    webClient.Headers.Add("Origin", NhlTv.OriginUri);

                    var json = webClient.DownloadString(NhlTv.LogoutUri);
                }
            }
            catch (Exception e) {
                _messenger.WriteLine(e, LogMessageCategory.Verbose);
                throw;
            }
        }
    }
}
