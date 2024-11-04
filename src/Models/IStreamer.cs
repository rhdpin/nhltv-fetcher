using System;
using System.Collections.Generic;
using System.Linq;

namespace NhlTvFetcher.Models {
    public interface IStreamer {
        /// <summary>The command line alias of the streamer app, e.g. <c>streamlink</c> or <c>livestreamer</c>.</summary>
        string AppName { get; set; }

        /// <summary>=Key name for the stream, e.g. <c>name</c>, <c>pixels</c>, or <c>bitrate</c>.</summary>
        string StreamNameKey { get; set; }

        /// <summary>Value name for the stream, e.g. <c>720</c> or <c>best</c>.</summary>
        string StreamNameValue { get; set; }

        /// <summary>The URL of the stream.</summary>
        string StreamUrl { get; set; }

        /// <summary>Full user agent string for use in http header.</summary>
        string UserAgent { get; set; }

        /// <summary>A dictionary of additional parameters to use with the streamer, e.g. <c>{--stream-segment-threads, 4}</c></summary>
        Dictionary<string, string> Parameters { get; set; }

        /// <summary>The media player application to use for the stream.</summary>
        public IMediaPlayer MediaPlayer { get; set; }

        const string StreamUrlHlsPrefix = "hlsvariant://";

        /// <summary>
        /// Join all <see cref="Parameters"/> for command-line use. Any parameter values
        /// containing a space will be enclosed in single quotations for safe usage.
        /// NOTE: Does not include IMediaPlayer parameters.
        /// </summary>
        public string ParamsToString() {
            return string.Join(" ",
                Parameters.Select(x =>
                    x.Key +
                    (!string.IsNullOrWhiteSpace(x.Value)
                        ? $"={(x.Value.Contains(' ')
                            ? $"'{x.Value}'"
                            : $"{x.Value}")}"
                        : ""
                    )
                )
            );
        }

        /// <summary>
        /// Formats the IStreamer object into a string ready for command line use.
        /// </summary>
        /// <returns>
        /// A command line formatted string of all the streamer and, if chosen, the
        /// associated media player values to use for initializing the stream.
        /// </returns>
        public string ArgsToCommandLineString(bool includeMediaPlayerArgs = false) {
            return $"\"{StreamUrlHlsPrefix}{StreamUrl} " +
                   $"name_key={StreamNameKey}\" {StreamNameValue} " +
                   $"--http-header \"User-Agent={UserAgent}\" " +
                   $"{ParamsToString()} " +
                   (includeMediaPlayerArgs
                       ? $"--player {MediaPlayer.AppName} --player-args \"{MediaPlayer.ParamsToString()}\" "
                       : "");
        }
    }

    internal class CustomStreamer : IStreamer {
        public string AppName { get; set; }
        public string StreamNameKey { get; set; }
        public string StreamNameValue { get; set; }
        public string StreamUrl { get; set; }
        public string UserAgent { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public IMediaPlayer MediaPlayer { get; set; }
    }

    public class StreamlinkStreamer : IStreamer {
        public string AppName { get; set; } = "streamlink";

        //public string LogLevel { get; set; } = nameof(LoggingLevel.error);
        //public string LogLevelSwitch { get; set; } = "-l";
        public string StreamNameKey { get; set; } = "bitrate";
        public string StreamNameValue { get; set; } = "best";
        public string StreamUrl { get; set; }
        public string UserAgent { get; set; } = Session.NhlTv.UserAgent;

        public Dictionary<string, string> Parameters { get; set; } =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                { "--stream-segment-threads", "4" },
                { "--stream-segment-attempts", "9" }
            };

        public IMediaPlayer MediaPlayer { get; set; } = new VlcMediaPlayer();

        public StreamlinkStreamer() { }

        public StreamlinkStreamer(string appName, string streamNameKey, string streamNameValue,
            string streamUrl, string userAgent, Dictionary<string, string> parameters,
            IMediaPlayer mediaPlayer) {
            AppName = appName;
            StreamNameKey = streamNameKey;
            StreamNameValue = streamNameValue;
            StreamUrl = streamUrl;
            UserAgent = userAgent;
            Parameters = parameters;
            MediaPlayer = mediaPlayer;
        }
    }
}
