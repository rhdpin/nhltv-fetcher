using System.Collections.Generic;
using CommandLine;

namespace NhlTvFetcher {
    public class Options {
        [Option('a', "auth-file-path", Required = false,
            HelpText =
                "(Default: auth.json in current directory) Set full path of the JSON authorization file containing your NHLTV email and password.")]
        public string AuthFilePath { get; set; }

        [Option('b', "bitrate", Required = false, Default = "best",
            HelpText =
                "Specify the bitrate of the stream to be downloaded. Use verbose mode ('-v') to see available bitrates.")]
        public string Bitrate { get; set; }

        [Option('c', "choose", SetName = "choose", Required = false, Default = false,
            HelpText = "Choose the feed from a list of found feeds.")]
        public bool Choose { get; set; }

        [Option('d', "days", Required = false, Default = 2,
            HelpText = "Specify how many days back to search for games.")]
        public int Days { get; set; }

        [Option('e', "date", Required = false,
            HelpText =
                "(Default: current date) Specify date to search games from (in format yyyy-MM-dd, e.g. 2019-12-22.")]
        public string Date { get; set; }

        [Option('f', "french", Required = false, Default = false,
            HelpText =
                "Prefer french feeds when getting the latest game feed for the selected team (use with '-t').")]
        public bool French { get; set; }

        [Option('h', "hide-progress", Required = false, Default = false,
            HelpText = "Hide download progress information.")]
        public bool HideProgress { get; set; }

        [Option('l', "play", Required = false, Default = false,
            HelpText =
                "Play the feed instead of downloading it (need to have VLC/MPV installed and defined in PATH env variable).")]
        public bool Play { get; set; }

        [Option('m', "player", Required = false, Default = "mpv",
            HelpText =
                "If playing the feed ('-l'), you can choose your media player by using this parameter option. Built-in player " +
                "parameters exist for MPV and VLC but any player can be used along with a custom set of parameters ('--player-parameters').")]
        public string Player { get; set; }

        [Option("player-parameters", Required = false, Separator = ' ',
            HelpText =
                "(Default: none) If playing the feed ('-l'), pass any additional parameters for use when invoking the media player. " +
                "\n\te.g. --player-parameters=\"--ontop-level=system --taskbar-progress=no\"")]
        public IEnumerable<string> PlayerParameters { get; set; }

        [Option('o', "overwrite", Required = false, Default = false,
            HelpText = "Overwrite file if it already exists (instead of skipping the download).")]
        public bool OverwriteExistingFile { get; set; }

        [Option('p', "path", Required = false,
            HelpText = "(Default: current directory) Set target download path.")]
        public string TargetPath { get; set; }

        [Option('s', "stream", Required = false, Default = false,
            HelpText =
                "Create a stream of the feed to network. Connect to stream using URL shown in output.")]
        public bool Stream { get; set; }

        [Option('t', "team", SetName = "team", Required = false,
            HelpText =
                "(Default: none) Get latest game for team (three letter abbreviation. E.g. WPG).")]
        public string Team { get; set; }

        [Option('u', "url", Required = false, Default = false,
            HelpText = "Return the URL (for Streamlink) of the stream but don't download.")]
        public bool OnlyUrl { get; set; }

        [Option('v', "verbose", Required = false, Default = false,
            HelpText = "Use verbose mode to get more detailed output.")]
        public bool VerboseMode { get; set; }

        [Option("tiled-player-parameters", Required = false, Separator = ' ',
            HelpText =
                "If playing multiple feeds ('-y'), pass these parameters to set tiled positions for each player instance." +
                "\n\tNote: Default layouts exist for both MPV and VLC media players, but can be overridden with these values." +
                "\n\te.g. --tiled-player-parameters=\"50%+0+0 50%+100%+0 50%+0+100% 50%+100%+100% 25%+50%+50%\"")]
        public IEnumerable<string> TiledPlayerParameters { get; set; }

        [Option('x', "stream-position", Required = false,
            HelpText =
                "(Default: ask) If playing the stream ('-l'), will start at this position: " +
                "\n\t1\t Start\n\t2\t 2nd Period (approx)\n\t3\t 3rd Period (approx)\n\t4\t Live\n\txx\t Custom time (in minutes)")]
        public int PlayPosition { get; set; }

        [Option('y', "multiple-feeds", Required = false, Default = false,
            HelpText =
                "If playing the stream ('-l'), will continuously ask for another feed number. Used to play multiple streams without invoking the app multiple times.")]
        public bool MultipleFeeds { get; set; }
    }
}
