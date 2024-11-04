# nhltv-fetcher
Helps NHL.TV subscribers to download game feeds or view them with VLC. Downloading the games allows watching games offline and/or with external media player applications.

Main problem with official NHL.TV player user interfaces (web, mobile, TV) is that they are not good when watching games afterwards and trying to not get spoiled. NHL.TV apps allow to hide scores, but the timeline is still shown, especially when skipping intermissions and ads. It is most problematic during the playoffs, when timeline reveals if game went to OT and how long it lasted. 

External media player applications (like [Kodi](https://kodi.tv/)) can be customized so that game end time is not shown.

## Features
* Made with .NET 8.0 / C#
* Command-line interface
* Choose a game or get latest game of a team
* Download game as MP4 file
* View game stream with MPV or VLC without saving to file
* Publish game feed as HTTP-stream for devices not capable of accessing games in other means

## Requirements
* Active NHL.TV subscription is needed
* Same geo-blocking restrictions affect use of this application as using NHL.TV in general
* Windows, MacOS or Linux operating system

## Installation
Installation can be done using three methods: building from sources, using the released binaries or by using the Docker container.

### Method 1: Build the sources and install dependencies
1. [Install .NET Framework SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
2. [Install Streamlink](https://github.com/streamlink/streamlink). `nhltv-fetcher` assumes that `streamlink` executable can be found from PATH. 
3. Clone the repo: `git clone https://github.com/rhdpin/nhltv-fetcher`
4. Build (in `src/` directory): `dotnet build`
5. Create [authentication file](#authentication-file) (`src\bin\Debug\net8.0\auth.json`)

Executable is located in `src\bin\Debug\net8.0\`.

### Method 2: Download released binaries and install dependencies
1. Download and extract the compiled binary files from [Releases](https://github.com/rhdpin/nhltv-fetcher/releases)
2. [Install Streamlink](https://github.com/streamlink/streamlink). nhltv-fetcher assumes that `streamlink` executable can be found from PATH. 
3. Create [authentication file](#authentication-file) in same directory where `nhltv-fetcher` executable is. 

MacOS: Newer MacOS operating systems complain that Apple can't check if the binary files are malicious. The following command can be used (in directory where binaries were extracted) to make Mac treat the files as safe:
`xattr -d com.apple.quarantine *`
### Method 3: Docker
[Docker image](https://hub.docker.com/repository/docker/rhdpin/nhltv-fetcher) contains full setup, so no installation of Streamlink is needed. 

1. [Install Docker](https://docs.docker.com/engine/install/) if not already installed
2. Create [authentication file](#authentication-file) in suitable directory (e.g. `/home/user/.nhltv-fetcher/auth.json`)
3. Run the container with command like: 
`docker run -it --rm -v /mnt/download:/app/download -v /home/user/.nhltv-fetcher:/app/config --network=host rhdpin/nhltv-fetcher:linux-x64 -c -p /app/download -a /app/config/auth.json`

First `-v` parameter binds download folder from host to container. In the example host folder is `/mnt/download` and it's mapped to `/app/download` in container. Second `-v` parameter binds config folder from host to container. In the example command host folder is `/home/user/.nhltv-fetcher` and it's mapped to `/app/config` in container to read the authentication file. 

`rhdpin/nhltv-fetcher:linux-x64` is the image name to be used. Replace `linux-x64` with `linux-arm32v7` to get a image for ARMv7 device like Raspberry Pi. Rest of the parameters are for `nhltv-fetcher` itself.

### Authentication file
Authentication file (`auth.json`) must contain valid NHL.TV account email and password. Application can not be used without it.

`{ "email": "my_nhltv_account_emailaddress", "password": "my_nhltv_account_password" }`

## Usage
```
$ ./nhltv-fetcher --help
nhltv-fetcher 1.1.1
Copyright (C) 2024 rhdpin

  -a, --auth-file-path         (Default: auth.json in current directory) Set full path of the JSON authorization file
                               containing your NHLTV email and password.

  -b, --bitrate                (Default: best) Specify the bitrate of the stream to be downloaded. Use verbose mode
                               ('-v') to see available bitrates.

  -c, --choose                 (Default: false) Choose the feed from a list of found feeds.

  -d, --days                   (Default: 2) Specify how many days back to search for games.

  -e, --date                   (Default: current date) Specify date to search games from (in format yyyy-MM-dd, e.g.
                               2019-12-22.

  -f, --french                 (Default: false) Prefer french feeds when getting the latest game feed for the selected
                               team (use with '-t').

  -h, --hide-progress          (Default: false) Hide download progress information.

  -l, --play                   (Default: false) Play the feed instead of downloading it (need to have VLC/MPV installed
                               and defined in PATH env variable).

  -m, --player                 (Default: mpv) If playing the feed ('-l'), you can choose your media player by using this
                               parameter option. Built-in player parameters exist for MPV and VLC but any player can be
                               used along with a custom set of parameters ('--player-parameters').

  --player-parameters          (Default: none) If playing the feed ('-l'), pass any additional parameters for use when
                               invoking the media player.
                                e.g. --player-parameters="--ontop-level=system --taskbar-progress=no"

  -o, --overwrite              (Default: false) Overwrite file if it already exists (instead of skipping the download).

  -p, --path                   (Default: current directory) Set target download path.

  -s, --stream                 (Default: false) Create a stream of the feed to network. Connect to stream using URL
                               shown in output.

  -t, --team                   (Default: none) Get latest game for team (three letter abbreviation. E.g. WPG).

  -u, --url                    (Default: false) Return the URL (for Streamlink) of the stream but don't download.

  -v, --verbose                (Default: false) Use verbose mode to get more detailed output.

  --tiled-player-parameters    If playing multiple feeds ('-y'), pass these parameters to set tiled positions for each
                               player instance.
                                Note: Default layouts exist for both MPV and VLC media players, but can be overridden
                                with these values.
                                e.g. --tiled-player-parameters="50%+0+0 50%+100%+0 50%+0+100% 50%+100%+100% 25%+50%+50%"

  -x, --stream-position        (Default: ask) If playing the stream ('-l'), will start at this position:
                                1        Start
                                2        2nd Period (approx)
                                3        3rd Period (approx)
                                4        Live
                                xx       Custom time (in minutes)

  -y, --multiple-feeds         (Default: false) If playing the stream ('-l'), will continuously ask for another feed
                               number. Used to play multiple streams without invoking the app multiple times.

  --help                       Display this help screen.

  --version                    Display version information.
```

Choose the feed from list of found feeds and download it in `/mnt/download`
```
$ ./nhltv-fetcher -c -p /mnt/download
nhltv-fetcher 1.0.9

 1: Sat 2024-11-02 12:00PM DAL@FLA (HOME) (SCRIPPS)
 2: Sat 2024-11-02 12:00PM DAL@FLA (AWAY) (VICTORY+)
 3: Sat 2024-11-02 01:00PM BOS@PHI (HOME) (NBCSP)
 4: Sat 2024-11-02 01:00PM BOS@PHI (AWAY) (NESN)
 5: Sat 2024-11-02 04:00PM CHI@LAK (AWAY) (CHSN)
 6: Sat 2024-11-02 04:00PM CHI@LAK (HOME) (FDSNW)
 7: Sat 2024-11-02 05:00PM CBJ@WSH (HOME) (MNMT)
 8: Sat 2024-11-02 05:00PM CBJ@WSH (AWAY) (FDSNOH)
 9: Sat 2024-11-02 07:00PM SEA@OTT (AWAY) (KHN)
10: Sat 2024-11-02 07:00PM SEA@OTT (NATIONAL) (SN1)
11: Sat 2024-11-02 07:00PM BUF@DET (HOME) (FDSNDET)
12: Sat 2024-11-02 07:00PM BUF@DET (AWAY) (MSG-B)
13: Sat 2024-11-02 07:00PM MTL@PIT (HOME) (SN-PIT)
14: Sat 2024-11-02 07:00PM MTL@PIT (NATIONAL) (TVAS) (FRENCH)
15: Sat 2024-11-02 07:00PM MTL@PIT (NATIONAL) (SNE)
16: Sat 2024-11-02 07:00PM TOR@STL (NATIONAL) (CBC)
17: Sat 2024-11-02 07:00PM TOR@STL (HOME) (FDSNMW)
18: Sat 2024-11-02 08:00PM COL@NSH (AWAY) (ALT2)
19: Sat 2024-11-02 08:00PM COL@NSH (HOME) (FDSNSO)
20: Sat 2024-11-02 10:00PM UTA@VGK (HOME) (SCRIPPS)
21: Sat 2024-11-02 10:00PM UTA@VGK (AWAY) (UTAH16)
22: Sat 2024-11-02 10:00PM VAN@SJS (HOME) (NBCSCA)
23: Sat 2024-11-02 10:00PM VAN@SJS (NATIONAL) (CBC)

24: Sun 2024-11-03 01:00PM NYI@NYR (HOME) (MSG)
25: Sun 2024-11-03 01:00PM NYI@NYR (AWAY) (MSGSN)
26: Sun 2024-11-03 03:00PM TBL@WPG (HOME) (TSN3)
27: Sun 2024-11-03 03:00PM TBL@WPG (AWAY) (FDSNSUN)
28: Sun 2024-11-03 05:00PM SEA@BOS (HOME) (NESN)
29: Sun 2024-11-03 05:00PM SEA@BOS (AWAY) (KHN)
30: Sun 2024-11-03 05:00PM WSH@CAR (AWAY) (MNMT)
31: Sun 2024-11-03 05:00PM WSH@CAR (HOME) (FDSNSO)
32: Sun 2024-11-03 06:00PM TOR@MIN (HOME) (FDSNNO)
33: Sun 2024-11-03 06:00PM TOR@MIN (AWAY) (SNO)
34: Sun 2024-11-03 08:00PM EDM@CGY (NATIONAL) (SN)
35: Sun 2024-11-03 08:00PM CHI@ANA (HOME) (VICTORY+)
36: Sun 2024-11-03 08:00PM CHI@ANA (AWAY) (CHSN)

Choose feed number (q to quit): 23
Logging in...
Getting the feed...

Downloading feed: Sat 2024-11-02 10:00PM VAN@SJS (NATIONAL) (CBC)
```
Download latest game of your favorite team. It tries to get feed of chosen team (away/home) if available, otherwise it uses first feed found. It's useful also when making scheduled calls (e.g. daily) to application to automatically load latest game. 
```
$ ./nhltv-fetcher -t DAL -p /mnt/download
nhltv-fetcher 1.0.9

Fetching latest feed for 'DAL'...
Logging in...
Getting the feed...
Feed found: Sat 2024-11-02 10:00PM VAN@SJS (NATIONAL) (CBC)
Downloading feed: Sat 2024-11-02 10:00PM VAN@SJS (NATIONAL) (CBC)
```
Using parameter `-l` or `--play` to open the feed directly in MPV or VLC (assuming that either player is installed and the executable can be found from PATH environment variable).
## Releases
All release packages contain all needed files, so installation of .NET runtime is not needed. 

After extracting the files on Linux, run `chmod +x nhltv-fetcher` to make the application executable.

## Credits
Kodi NHL.TV plugin ([eracknaphobia/plugin.video.nhlgcl](https://github.com/eracknaphobia/plugin.video.nhlgcl))) for the authentication approach.
