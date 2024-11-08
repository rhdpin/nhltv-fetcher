using System;
using System.Collections.Generic;
using System.Linq;

namespace NhlTvFetcher.Models {
    public interface IMediaPlayer {
        /// <summary>The command line alias of the media player, e.g. <c>mpv</c> or <c>vlc</c>.</summary>
        string AppName { get; set; }

        /// <summary>A dictionary of additional parameters to use with the player, e.g. <c>{"--demuxer-max-bytes", "512MiB"}</c></summary>
        Dictionary<string, string> Parameters { get; set; }

        /// <summary>The name of the parameter used to set the title of the player's window.</summary>
        public string TitleParameterName { get; set; }

        /// <summary>The value used to set the title of the player's window.</summary>
        public string TitleParameterValue { get; set; }

        /// <summary>The player's screen position parameter option name for tiling windows, e.g. <c>--geometry</c>.</summary>
        public string TiledScreenParameterName { get; set; }

        /// <summary>A list of tiling positions for each new player instance.</summary>
        public List<string> TiledScreenParameterValues { get; set; }

        /// <summary>
        /// Join all <see cref="Parameters"/> for command-line use. Any parameter values
        /// containing a space will be enclosed in single quotations for safe usage. Also
        /// joins screen position parameters based on the <see cref="ScreenPosition"/>
        /// setting.
        /// </summary>
        /// <returns>A command-line ready string of the player's parameters.</returns>
        public string ParamsToString() {
            string paramList = string.Join(" ",
                Parameters.Select(x =>
                    x.Key +
                    FormatParameterValue(x.Value)
                )
            );

            if (TiledScreenParameterName != null &&
                TiledScreenParameterValues != null) {
                paramList +=
                    $" {TiledScreenParameterName}" +
                    $"{FormatParameterValue(Session.GetScreenPosition(TiledScreenParameterValues))}";
            }

            if (TitleParameterName != null
                && TitleParameterValue != null) {
                paramList +=
                    $" {TitleParameterName}" +
                    $"{FormatParameterValue(TitleParameterValue)}";
            }

            return paramList;
        }

        /// <summary>
        /// Formats a parameter value for use in command line execution.
        /// </summary>
        /// <param name="value">The string value of any given parameter name.</param>
        /// <returns>The value, enclosed in single quotes if contains spaces; otherwise blank string.</returns>
        private string FormatParameterValue(string value) {
            return (!string.IsNullOrWhiteSpace(value)
                    ? $"={(value.Contains(' ')
                        ? $"'{value}'"
                        : $"{value}")}"
                    : ""
                );
        }
    }

    public class CustomMediaPlayer : IMediaPlayer {
        public string AppName { get; set; }
        public string TitleParameterName { get; set; }
        public string TitleParameterValue { get; set; }
        public Dictionary<string, string> Parameters { get; set; }

        public string TiledScreenParameterName { get; set; }
        public List<string> TiledScreenParameterValues { get; set; }
    }

    public class MpvMediaPlayer : IMediaPlayer {
        public string AppName { get; set; } = "mpv";
        public string TitleParameterName { get; set; } = "--force-media-title";
        public string TitleParameterValue { get; set; } = "";

        public Dictionary<string, string> Parameters { get; set; } =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                { "--cache", "yes" },
                { "--force-seekable", "yes" },
                { "--hr-seek", "yes" },
                { "--hr-seek-framedrop", "yes" },
                { "--keep-open", "no" },
                { "--no-border", "" },
                { "--keepaspect", "yes" },
                { "--keepaspect-window", "yes" },
                { "--window-maximized", "no" },
                { "--fs", "no" }
            };

        public string TiledScreenParameterName { get; set; } = "--geometry";

        public List<string> TiledScreenParameterValues { get; set; } = [
            "50%+0+0", // top-left (50%)
            "50%+100%+0", // top-right (50%)
            "50%+0+100%", // bottom-left (50%)
            "50%+100%+100%", // bottom-right (50%)
            "25%+50%+50%" // center (25%)
        ];
    }

    public class VlcMediaPlayer : IMediaPlayer {
        public string AppName { get; set; } = "vlc";
        public string TitleParameterName { get; set; } = "--meta-title";
        public string TitleParameterValue { get; set; } = "";

        public Dictionary<string, string> Parameters { get; set; } =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                { "--no-one-instance", "" },
                { "--play-and-exit", "" },
                { "--qt-minimal-view", "" }
            };

        public string TiledScreenParameterName { get; set; } = "--align";

        public List<string> TiledScreenParameterValues { get; set; } = [
            "5", // top-left
            "6", // top-right
            "9", // bottom-left
            "10", // bottom-right
            "0" // center
        ];
    }
}
