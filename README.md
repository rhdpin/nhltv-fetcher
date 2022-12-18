# nhltv-fetcher
Helps NHL.TV subscribers to download game feeds or view them with VLC. Downloading the games allows watching games offline and/or with external media player applications.

Main problem with official NHL.TV player user interfaces (web, mobile, TV) is that they are not good when watching games afterwards and trying to not get spoiled. NHL.TV apps allow to hide scores, but the timeline is still shown, especially when skipping intermissions and ads. It is most problematic during the playoffs, when timeline reveals if game went to OT and how long it lasted. 

External media player applications (like Kodi) can be customized so that game end time is not shown.

## Features
* Made with .NET 6.0 / C#
* Command-line interface
* Choose a game or get latest game of a team
* Download game as MP4 file
* View game stream with VLC without saving to file
* Publish game feed as HTTP-stream for devices not capable accessing games in other means
* Supports Windows, MacOS and Linux

## Requirements
* Active NHL.TV subscription is needed
* Same geo-blocking restrictions affect use of this application as using NHL.TV in general

## Installation
### Method 1: Download released binaries and install dependencies
Download the compiled binary files from [Releases](https://github.com/rhdpin/nhltv-fetcher/releases). Then you can install the [Streamlink](https://github.com/streamlink/streamlink) according to its installation instructions. nhltv-fetcher assumes that `streamlink` executable can be found from PATH. 

Create `auth.json` in same directory where nhltv-fetcher executable is. Content should define NHL.TV account information (email and password).

`{ "email": "my_nhltv_account_emailaddress", "password": "my_nhltv_account_password" }`

### Method 2: Docker
[Docker](https://www.docker.com/) container contains full setup, so no installation of Streamlink is needed. 

1. Install Docker if not already installed
2. Create `auth.json` in suitable directory (e.g. `/home/user/.nhltv-fetcher`). Content should define NHL.TV account information (email and password)

`{ "email": "my_nhltv_account_emailaddress", "password": "my_nhltv_account_password" }`

3. Run the container with command like: 
`docker run -it --rm -v /mnt/download:/app/download -v /home/user/.nhltv-fetcher:/app/config --network=host rhdpin/nhltv-fetcher:linux-x64 -c -p /app/download -a /app/config`

First `-v` parameter binds download folder from host to container. In the example host folder is `/mnt/download` and it's mapped to `/app/download` in container. Second `-v` parameter binds config folder from host to container. In the example command host folder is `/home/user/.nhltv-fetcher` and it's mapped to `/app/config` in container to read the authentication file. 

`rhdpin/nhltv-fetcher:linux-x64` is the image name to be used. Replace `linux-x64` with `linux-arm32v7` to get a image for ARMv7 device like Raspberry Pi. Rest of the parameters are for the nhltv-fetcher itself.

## Usage
```
$ ./nhltv-fetcher --help
nhltv-fetcher 1.0.0
Copyright (C) 2022 rhdpin

  -a, --auth-file-path    (Default: auth.json in current directory) Set full path of auth file

  -b, --bitrate           (Default: best) Specify bitrate of stream to be downloaded. Use verbose mode to see available bitrates.                          

  -c, --choose            Choose the feed from list of found feeds.

  -d, --days              (Default: 2) Specify how many days back to search games

  -e, --date              (Default: current date) Specify date to search games from (in format yyyy-MM-dd, e.g. 2019-12-22                          

  -h, --hide-progress     Hide download progress information

  -l, --play              Play the feed instead of writing to file (need to have VLC installed and defined in PATH env variable)                          

  -o, --overwrite         Overwrite file if it already exists (instead of skipping the download).

  -p, --path              (Default: current directory) Set target download path.

  -s, --stream            Create a stream of the feed to network. Connect to stream using URL shown in output.

  -t, --team              Get latest game for team (three letter abbreviation. E.g. WPG).

  -u, --url               Get only URL (for Streamlink) of the stream but don't download.

  -v, --verbose           Use verbose mode to get more detailed output.

  --help                  Display this help screen.

  --version               Display version information.
```

Choose the feed from list of found feeds and download it in `/mnt/download`
```
$ ./nhltv-fetcher -c -p /mnt/download
nhltv-fetcher 1.0.0

 1: 12/16/2022 LAK@BOS (HOME)
 2: 12/16/2022 LAK@BOS (AWAY)
 3: 12/16/2022 SEA@CAR (AWAY)
 4: 12/16/2022 PIT@FLA (HOME)
 5: 12/16/2022 PIT@FLA (AWAY)
 6: 12/16/2022 ANA@MTL (AWAY)
 7: 12/16/2022 PHI@NJD (HOME)
 8: 12/16/2022 PHI@NJD (AWAY)
 9: 12/16/2022 TOR@NYR (AWAY)
10: 12/16/2022 CBJ@TBL (HOME)
11: 12/16/2022 CBJ@TBL (AWAY)
12: 12/16/2022 DAL@WSH (HOME)
13: 12/16/2022 DAL@WSH (AWAY)
14: 12/16/2022 NSH@WPG (HOME)
15: 12/16/2022 NSH@WPG (AWAY)
16: 12/16/2022 VGK@CHI (HOME)
17: 12/16/2022 VGK@CHI (AWAY)
18: 12/16/2022 BUF@COL (AWAY)
19: 12/16/2022 STL@EDM (AWAY)
20: 12/17/2022 CHI@MIN (NATIONAL)
21: 12/17/2022 STL@CGY (NATIONAL)
22: 12/17/2022 STL@CGY (AWAY)
23: 12/17/2022 NYI@ARI (HOME)
24: 12/17/2022 NYI@ARI (AWAY)

Choose feed (q to quit): 9
Logging in...
Getting the feed...

Downloading feed: 12/16/2022 TOR@NYR (AWAY)
Writing stream to file: 364 MB (10.4 MB/s)
```
Get latest game of your favorite team. It tries to get feed of chosen team (away/home) if available, otherwise it uses first feed found. It's useful also when making scheduled calls (e.g. daily) to application to automatically load latest game. 
```
$ ./nhltv-fetcher -t NYR -p /mnt/download
nhltv-fetcher 1.0.0

Fetching latest feed for 'NYR'...
Logging in...
Getting the feed...
Feed found: 12/16/2022 TOR@NYR (AWAY)
Writing stream to file: 524 MB (11.6 MB/s)
```
Using parameter `-l` or `--play` to open the feed directly in VLC assumes that VLC is installed and the executable can be found from PATH environment variable.
## Releases
All release packages contain all needed files, so installation of .NET runtime is not needed. 

After extracting the files on Linux, run `chmod +x nhltv-fetcher` to make the application executable.

## Credits
Kodi NHL.TV plugin ([eracknaphobia/plugin.video.nhlgcl](https://github.com/eracknaphobia/plugin.video.nhlgcl))) for the authentication approach.