using System;
using System.Collections.Generic;

namespace NhlTvFetcher.Models;

public static class StreamerFactory {
    public enum Streamer {
        Custom,
        Streamlink
    }

    public static IStreamer CreateStreamer() {
        return CreateStreamer(Streamer.Custom);
    }

    public static IStreamer CreateStreamer(string streamerType) {
        return CreateStreamer(streamerType, null);
    }

    public static IStreamer CreateStreamer(string streamerType,
        Dictionary<string, string> streamerParameters) {
        if (Enum.TryParse<Streamer>(streamerType, out Streamer streamer)) {
            return CreateStreamer(streamer, streamerParameters);
        }

        return new CustomStreamer() {
            AppName = streamerType,
            Parameters = streamerParameters
        };
    }

    public static IStreamer CreateStreamer(Streamer streamerType) {
        return CreateStreamer(streamerType, null, null);
    }

    public static IStreamer CreateStreamer(Streamer streamerType,
        Dictionary<string, string> streamerParameters) {
        return CreateStreamer(streamerType, streamerParameters, null);
    }

    public static IStreamer CreateStreamer(Streamer streamerType, string customStreamerName) {
        return CreateStreamer(streamerType, null, customStreamerName);
    }

    public static IStreamer CreateStreamer(Streamer streamerType,
        Dictionary<string, string> streamerParameters,
        string customStreamerName) {
        IStreamer streamer;

        switch (streamerType) {
            case Streamer.Streamlink:
                streamer = new StreamlinkStreamer();
                break;

            default:
                streamer = new CustomStreamer() {
                    AppName = customStreamerName
                };
                break;
        }

        if (streamerParameters != null) {
            foreach (var param in streamerParameters) {
                streamer.Parameters[param.Key] = param.Value;
            }
        }

        return streamer;
    }
}
