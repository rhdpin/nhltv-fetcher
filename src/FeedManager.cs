using System;
using System.IO;
using System.Linq;
using NhlTvFetcher.Data;
using System.Net;
using NhlTvFetcher.Models.Json.ScheduleBroadcast;

namespace NhlTvFetcher
{
    public class FeedManager
    {
        private readonly Messenger _messenger;
        private readonly FeedFetcher _feedFetcher;
        private readonly Options _options;        
        private readonly Downloader _downloader;        

        private readonly DateTime _startDate;
        private readonly DateTime _endDate;

        public FeedManager(Messenger messenger, FeedFetcher feedFetcher, Options options, Downloader downloader)
        {
            _messenger = messenger;
            _feedFetcher = feedFetcher;
            _options = options;            
            _downloader = downloader;

            _startDate = _endDate = (options.Date != null) ? DateTime.Parse(options.Date) : DateTime.Now;
            _startDate = _startDate.Subtract(new TimeSpan(24 * options.Days, 0, 0));

            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) =>
            {
                return true;
            };
        }
        public void ChooseFeed(string targetPath, bool getOnlyUrl)
        {            
            var feeds = _feedFetcher.GetFeeds(_startDate, _endDate);

            foreach (var feed in feeds)
            {
                Console.WriteLine($"{feed.Id.ToString().PadLeft(2)}: {GetFeedDisplayName(feed)}");
            }

            if (!feeds.Any())
            {
                _messenger.WriteLine("No feeds were found.");
                return;
            }
            
            Console.Write("\nChoose feed (q to quit): ");
            var input = Console.ReadLine();

            if (input.Equals("q", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            
            var chosenFeed = feeds.FirstOrDefault(f => f.Id == int.Parse(input));

            string streamUrl = null;
            try
            {
                streamUrl = _feedFetcher.GetStreamUrl(chosenFeed);
            }
            catch (Exception e)
            {
                _messenger.WriteLine($"Problem when getting stream URL: {e.Message}");
                return;
            }

            if (getOnlyUrl)
            {
                Console.WriteLine(streamUrl);
                return;
            }
            if (streamUrl != null)
            {
                _messenger.WriteLine($"\nDownloading feed: {GetFeedDisplayName(chosenFeed)}");
                Download(streamUrl, chosenFeed, targetPath);
            }
        }

        public void GetLatest(string teamName, string targetPath, bool getOnlyUrl)
        {
            if (!getOnlyUrl)
            {
                _messenger.WriteLine($"Fetching latest feed for '{teamName}'...");
            }                

            var feed = _feedFetcher.GetLatestFeedForTeam(teamName, _startDate, _endDate);            
            if (feed == null)
            {
                return;
            }

            string streamUrl;
            try
            {
                streamUrl = _feedFetcher.GetStreamUrl(feed);
            }
            catch (Exception e)
            {
                _messenger.WriteLine($"Problem when getting stream URL: {e.Message}");
                return;
            }

            if (getOnlyUrl)
            {
                Console.WriteLine(streamUrl);
                return;
            }

            if (streamUrl != null)
            {
                _messenger.WriteLine($"Feed found: {GetFeedDisplayName(feed)}");
                Download(streamUrl, feed, targetPath);
            }            
        }               

        private void Download(string streamUrl, Feed feed, string targetPath)
        {
            var fileName = GetTargetFileName(feed, targetPath);            

            if (File.Exists(fileName) && !_options.OverwriteExistingFile && !_options.Play && !_options.Stream)
            {
                _messenger.WriteLine("Skipping download because file already exists.");
                return;
            }

            var downloadRequest = new DownloadRequest() { StreamUrl = streamUrl, TargetFileName = fileName };
            _downloader.Download(downloadRequest);            
        }

        private static string GetTargetFileName(Feed feed, string directoryPath)
        {            
            var formattedDate = feed.Date.Replace("/", "-");
            var formattedBroadcaster = feed.Broadcaster?.Replace("/", "_");
            return Path.Combine(directoryPath ?? "", $"{formattedDate}-{feed.Away}@{feed.Home}-{feed.Type}{(feed.IsFrench ? "-French" : "")}{(formattedBroadcaster != null ? "-" + formattedBroadcaster : "")}.mp4");
        }

        private static string GetFeedDisplayName(Feed feed)
        {
            return $"{feed.Date} {feed.Away}@{feed.Home} ({feed.Type}{(feed.IsFrench ? ", French" : "")}{(feed.Broadcaster != null ? ", " + feed.Broadcaster : "")})";
        }
    }
}
