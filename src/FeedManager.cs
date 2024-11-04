using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NhlTvFetcher.Data;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NhlTvFetcher.Models;

namespace NhlTvFetcher {
    public class FeedManager {
        private readonly Messenger _messenger;
        private readonly FeedFetcher _feedFetcher;
        private readonly Options _options;
        private readonly Downloader _downloader;
        private readonly Session _session;

        private readonly DateTime _startDate;
        private readonly DateTime _endDate;

        /// <summary>
        /// Keeps values of *approximate* time offsets for skipping to 1st/2nd/3rd period of stream.
        /// NOTE: do NOT include overtime values as it will spoil the game.
        /// </summary>
        enum StartTimeOffset {
            FirstPeriod = 0,
            SecondPeriod = 65,
            ThirdPeriod = 120
        }

        public FeedManager(Messenger messenger, FeedFetcher feedFetcher, Options options,
            Downloader downloader,
            Session session) {
            _messenger = messenger;
            _feedFetcher = feedFetcher;
            _options = options;
            _downloader = downloader;
            _session = session;

            _startDate = _endDate = (options.Date != null)
                ? DateTime.Parse(options.Date)
                : DateTime.Now.Date;
            _endDate = _endDate.Date.AddDays(1).AddSeconds(-1);
            _startDate = _startDate.Subtract(new TimeSpan(24 * options.Days, 0, 0)).Date;

            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, errors) => { return true; };
        }

        public int ChooseFeed(string targetPath, bool getOnlyUrl) {
            if (!Session.FeedsListCache.Any()) {
                Session.FeedsListCache = _feedFetcher.GetFeeds(_startDate, _endDate);

                if (!Session.FeedsListCache.Any()) {
                    _messenger.WriteLine("No feeds were found.");
                    return 1;
                }

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("");
                string curDay = Session.FeedsListCache.First().Date.Date.ToString();
                string newDay;
                foreach (var feed in Session.FeedsListCache) {
                    newDay = feed.Date.Date.ToString();
                    if (curDay != newDay) {
                        sb.AppendLine("");
                        curDay = newDay;
                    }

                    sb.AppendLine($"{feed.Id,2}: {feed.ToString()}");
                }

                Session.FeedsListOutputCache = sb.ToString();
            }

            string input = "";
            while (!int.TryParse(input, out _)
                   || int.Parse(input) <= 0
                   || int.Parse(input) > Session.FeedsListCache.Count()) {
                _messenger.WriteLine(Session.FeedsListOutputCache);
                _messenger.Write("Choose feed number (q to quit): ");

                input = Console.ReadLine();

                if (input.Equals(ConsoleKey.Q.ToString(), StringComparison.OrdinalIgnoreCase)) {
                    return 0;
                }
            }

            var chosenFeed = Session.FeedsListCache.FirstOrDefault(f => f.Id == int.Parse(input));

            string streamUrl = null;
            try {
                streamUrl = _feedFetcher.GetStreamUrl(chosenFeed);
            }
            catch (Exception e) {
                _messenger.WriteLine($"Problem when getting stream URL: {e.Message}");
                return 1;
            }

            if (getOnlyUrl) {
                _messenger.WriteLine(streamUrl);
                return 1;
            }

            if (streamUrl != null) {
                Dictionary<string, string> timeOffsetParams = null;
                if (_options.Play) {
                    timeOffsetParams =
                        CalculateUserStartTimeOffset(chosenFeed.Date, chosenFeed.IsReplay);
                }

                _messenger.WriteLine($"\nDownloading feed: {chosenFeed.ToString()}");
                Download(streamUrl, chosenFeed, targetPath, timeOffsetParams);
            }

            _session.LogOut();

            return 1;
        }

        public void GetLatest(string teamName, string targetPath, bool getOnlyUrl) {
            if (!getOnlyUrl) {
                _messenger.WriteLine($"Fetching latest feed for '{teamName}'...");
            }

            var feed = _feedFetcher.GetLatestFeedForTeam(teamName, _startDate, _endDate);
            if (feed == null) {
                return;
            }

            string streamUrl;
            try {
                streamUrl = _feedFetcher.GetStreamUrl(feed);
            }
            catch (Exception e) {
                _messenger.WriteLine($"Problem when getting stream URL: {e.Message}");
                return;
            }

            if (getOnlyUrl) {
                _messenger.WriteLine(streamUrl);
                return;
            }

            if (streamUrl != null) {
                Dictionary<string, string> timeOffsetParams = null;
                if (_options.Play) {
                    CalculateUserStartTimeOffset(feed.Date, feed.IsReplay);
                }

                _messenger.WriteLine($"Feed found: {feed.ToString()}");
                Download(streamUrl, feed, targetPath, timeOffsetParams);
            }

            _session.LogOut();
        }

        private void Download(string streamUrl, Feed feed, string targetPath,
            Dictionary<string, string> streamerParams) {
            var filePath = GetTargetFilePath(feed, targetPath);

            var suspectFilePath = filePath.Replace(Path.GetFileNameWithoutExtension(filePath),
                Path.GetFileNameWithoutExtension(filePath) + "-suspect");
            if ((File.Exists(filePath) || File.Exists(suspectFilePath)) &&
                !_options.OverwriteExistingFile &&
                !_options.Play && !_options.Stream) {
                _messenger.WriteLine("Skipping download because file already exists.");
                return;
            }

            var downloadRequest = new DownloadRequest() {
                Name = feed.ToString(),
                TargetFilePath = filePath,
                Streamer = new StreamlinkStreamer() {
                    StreamUrl = streamUrl,
                    StreamNameValue = _options.Bitrate,
                    MediaPlayer = new MpvMediaPlayer() {
                        TitleParameterValue = feed.ToString()
                    }
                }
            };

            if (streamerParams != null && streamerParams.Count > 0) {
                foreach (var kv in streamerParams) {
                    downloadRequest.Streamer.Parameters[kv.Key] = kv.Value;
                }
            }

            foreach (var kvString in _options.PlayerParameters) {
                string[] kvPair = kvString.Split("=");

                downloadRequest.Streamer.MediaPlayer.Parameters[kvPair[0]] =
                    kvPair.Length > 1 ? kvPair[1] : "";
            }

            if (_options.TiledPlayerParameters.Any()) {
                downloadRequest.Streamer.MediaPlayer.TiledScreenParameterValues =
                    _options.TiledPlayerParameters.ToList();
            }

            _downloader.Download(downloadRequest);
        }

        private static string GetTargetFilePath(Feed feed, string directoryPath) {
            return Path.Combine(directoryPath ?? "", feed.ToFileNameString() + $".mp4");
        }

        /// <summary>
        /// Calculates an approximate offset in minutes for the user to be able to start playing from a
        /// certain location within the stream. If the game is a Replay, then add the offset from the
        /// start; otherwise subtract from the current stream position.
        /// </summary>
        /// <param name="feedStartTime">The starting time of the chosen feed.</param>
        /// <param name="streamIsReplay">Needed to determine if the stream offset must be calculated
        /// from the beginning or current place of the stream.</param>
        /// <returns>An offset value, if chosen, for the user to start playing the stream.</returns>
        public Dictionary<string, string> CalculateUserStartTimeOffset(DateTime feedStartTime,
            bool streamIsReplay) {
            Dictionary<string, string> offsetParams = new Dictionary<string, string>();

            var timeDiff = (int)DateTime.Now.Subtract(feedStartTime).TotalMinutes;

            int userChoice = _options.PlayPosition;
            if (userChoice <= 0 || userChoice > 4) {
                int temp = 0;
                do {
                    var ask = timeDiff switch {
                        > (int)StartTimeOffset.ThirdPeriod =>
                            "1: Start\n" + "2: 2nd Period\n" + "3: 3rd Period\n" +
                            (streamIsReplay ? "" : "4: Live\n"),

                        > (int)StartTimeOffset.SecondPeriod =>
                            "1: Start\n" + "2: 2nd Period\n" + "4: Live\n",

                        > (int)StartTimeOffset.FirstPeriod =>
                            "1: Start\n" + "4: Live\n",

                        _ => throw new ArgumentOutOfRangeException(
                            $"parameter time is before feed time: {nameof(timeDiff)}")
                    };

                    _messenger.Write("\n" + ask + "\nChoose stream starting point: ");
                    int.TryParse(Console.ReadLine(), out temp);
                } while (temp is < 1 or > 4);

                userChoice = temp;
            }

            switch (userChoice) {
                case 1:
                    offsetParams.Add("--hls-live-restart", "");
                    break;

                case 2:
                    offsetParams.Add("--hls-start-offset",
                        streamIsReplay
                            ? $"{(int)StartTimeOffset.SecondPeriod}m"
                            : $"{timeDiff - (int)StartTimeOffset.SecondPeriod}m");
                    break;

                case 3:
                    offsetParams.Add("--hls-start-offset",
                        streamIsReplay
                            ? $"{(int)StartTimeOffset.ThirdPeriod}m"
                            : $"{timeDiff - (int)StartTimeOffset.ThirdPeriod}m");
                    break;

                case 4:
                default: break;
            }

            return offsetParams;
        }
    }
}
