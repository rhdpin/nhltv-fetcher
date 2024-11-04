using System;
using System.Collections.Generic;

namespace NhlTvFetcher.Models;

public static class MediaPlayerFactory {
    public enum MediaPlayer {
        Custom,
        Mpv,
        Vlc
    }

    public static IMediaPlayer CreateMediaPlayer() {
        return new CustomMediaPlayer();
    }

    public static IMediaPlayer CreateMediaPlayer(string playerType) {
        return CreateMediaPlayer(playerType, null);
    }

    public static IMediaPlayer CreateMediaPlayer(string playerType,
        Dictionary<string, string> playerParameters) {
        if (Enum.TryParse<MediaPlayer>(playerType, out MediaPlayer player)) {
            return CreateMediaPlayer(player, playerParameters);
        }

        return new CustomMediaPlayer() {
            AppName = playerType,
            Parameters = playerParameters
        };
    }

    public static IMediaPlayer CreateMediaPlayer(MediaPlayer playerType) {
        return CreateMediaPlayer(playerType, null, null);
    }

    public static IMediaPlayer CreateMediaPlayer(MediaPlayer playerType,
        Dictionary<string, string> playerParameters) {
        return CreateMediaPlayer(playerType, playerParameters, null);
    }

    public static IMediaPlayer CreateMediaPlayer(MediaPlayer playerType, string customPlayerName) {
        return CreateMediaPlayer(playerType, null, customPlayerName);
    }

    public static IMediaPlayer CreateMediaPlayer(MediaPlayer playerType,
        Dictionary<string, string> playerParameters,
        string customPlayerName) {
        IMediaPlayer player;

        switch (playerType) {
            case MediaPlayer.Mpv:
                player = new MpvMediaPlayer();
                break;

            case MediaPlayer.Vlc:
                player = new VlcMediaPlayer();
                break;

            default:
                player = new CustomMediaPlayer() {
                    AppName = customPlayerName
                };
                break;
        }

        if (playerParameters != null) {
            foreach (var param in playerParameters) {
                player.Parameters[param.Key] = param.Value;
            }
        }

        return player;
    }
}
