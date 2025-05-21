using CommandLine;
using System.Runtime.InteropServices;

namespace NhlTvFetcher
{
    public class Options
    {
        [Option('a', "auth-file-path", Required = false, HelpText = "(Default: auth.json in current directory) Set full path of auth file")]
        public string AuthFilePath { get; set; }

        [Option('b', "bitrate", Required = false, Default = "best", HelpText = "Specify bitrate of stream to be downloaded. Use verbose mode to see available bitrates.")]
        public string Bitrate { get; set; }

        [Option('c', "choose", SetName = "choose", Required = false, HelpText = "Choose the feed from list of found feeds.")]
        public bool Choose { get; set; }

        [Option('d', "days", Required = false, Default = 2, HelpText = "Specify how many days back to search games")]
        public int Days { get; set; }

        [Option('e', "date", Required = false, HelpText = "(Default: current date) Specify date to search games from (in format yyyy-MM-dd, e.g. 2019-12-22")]
        public string Date { get; set; }
        
        [Option('f', "french", Required = false, HelpText = "Prefer French feeds when getting latest game feed for the selected team (use with -t)")]
        public bool French { get; set; }

        [Option('h', "hide-progress", Required = false, HelpText = "Hide download progress information")]
        public bool HideProgress { get; set; }

        [Option('l', "play", Required = false, HelpText = "Play the feed instead of writing to file (need to have VLC installed and defined in PATH env variable)")]
        public bool Play { get; set; }

        [Option('o', "overwrite", Required = false, HelpText = "Overwrite file if it already exists (instead of skipping the download).")]
        public bool OverwriteExistingFile { get; set; }
        
        [Option('p', "path", Required = false, HelpText = "(Default: current directory) Set target download path.")]
        public string TargetPath { get; set; }

        [Option('r', "broadcaster", Required = false, HelpText = "(Default: none) Set preferred national broadcaster feed(s) if no suitable home/away feed is available. Used with -t parameter. Partial strings can be used too (e.g. ABC,TNT,ES)")]
        public string Broadcasters { get; set; }

        [Option('s', "stream", Required = false, HelpText = "Create a stream of the feed to network. Connect to stream using URL shown in output.")]
        public bool Stream { get; set; }

        [Option('t', "team", SetName = "team", Required = false, HelpText = "Get latest game for team (three letter abbreviation. E.g. WPG).")]
        public string Team { get; set; }

        [Option('u', "url", Required = false, HelpText = "Get only URL (for Streamlink) of the stream but don't download.")]
        public bool OnlyUrl { get; set; }

        [Option('v', "verbose", Required = false, HelpText = "Use verbose mode to get more detailed output.")]
        public bool VerboseMode { get; set; }
        
    }
}
