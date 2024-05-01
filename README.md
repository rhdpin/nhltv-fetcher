# nhltv-fetcher
Helps NHL.TV subscribers to download game feeds or view them with VLC. Downloading the games allows watching games offline and/or with external media player applications.

Main problem with official NHL.TV player user interfaces (web, mobile, TV) is that they are not good when watching games afterwards and trying to not get spoiled. NHL.TV apps allow to hide scores, but the timeline is still shown, especially when skipping intermissions and ads. It is most problematic during the playoffs, when timeline reveals if game went to OT and how long it lasted. 

External media player applications (like [Kodi](https://kodi.tv/)) can be customized so that game end time is not shown.

## Features
* Made with .NET 6.0 / C#
* Command-line interface
* Choose a game or get latest game of a team
* Download game as MP4 file
* View game stream with VLC without saving to file
* Publish game feed as HTTP-stream for devices not capable accessing games in other means

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
5. Create [authentication file](#authentication-file) (`src\bin\Debug\net6.0\auth.json`)

Executable is located in `src\bin\Debug\net6.0\`.

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
nhltv-fetcher 1.0.8
Copyright (C) 2023 rhdpin

  -a, --auth-file-path    (Default: auth.json in current directory) Set full path of auth file

  -b, --bitrate           (Default: best) Specify bitrate of stream to be downloaded. Use verbose mode to see available bitrates.                          

  -c, --choose            Choose the feed from list of found feeds.

  -d, --days              (Default: 2) Specify how many days back to search games

  -e, --date              (Default: current date) Specify date to search games from (in format yyyy-MM-dd, e.g. 2019-12-22                          

  -f, --french            Prefer French feeds when getting latest game feed for the selected team (use with -t)

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
nhltv-fetcher 1.0.8

 1: Tue 22-12-27 CHI@CAR (home, BSSO)
 2: Tue 22-12-27 CHI@CAR (away, NBCSCH)
 3: Tue 22-12-27 BOS@OTT (home, TSN5)
 4: Tue 22-12-27 BOS@OTT (away, NESN)
 5: Tue 22-12-27 BOS@OTT (home, French, RDS)
 6: Tue 22-12-27 WSH@NYR (away, NBCSWA+)
 7: Tue 22-12-27 PIT@NYI (home, MSGSN)
 8: Tue 22-12-27 PIT@NYI (away, ATTSN-PT)
 9: Tue 22-12-27 MIN@WPG (home, TSN3)
10: Tue 22-12-27 MIN@WPG (away, BSN/BSWI)
11: Tue 22-12-27 TOR@STL (home, BSMW)
12: Tue 22-12-27 DAL@NSH (home, BSSO)
13: Tue 22-12-27 DAL@NSH (away, BSSWX)
14: Tue 22-12-27 COL@ARI (home, BSAZX)
15: Tue 22-12-27 COL@ARI (away, ALT2)
16: Tue 22-12-27 EDM@CGY (national, SNE/SNW)
17: Tue 22-12-27 SJS@VAN (home, SNP)
18: Tue 22-12-27 SJS@VAN (away, NBCSCA)
19: Tue 22-12-27 VGK@LAK (home, BSW)
20: Tue 22-12-27 VGK@LAK (away, ATTSN-RM)
21: Wed 22-12-28 MTL@TBL (home, BSSUNX)
22: Wed 22-12-28 MTL@TBL (national, SN)
23: Wed 22-12-28 MTL@TBL (away, French, RDS)
24: Wed 22-12-28 DET@PIT (home, ATTSN-PT)
25: Wed 22-12-28 DET@PIT (away, BSDET)
26: Wed 22-12-28 BOS@NJD (national, TNT)
27: Wed 22-12-28 CGY@SEA (national, TNT)
28: Wed 22-12-28 CGY@SEA (away, SNW)
29: Wed 22-12-28 VGK@ANA (home, BSSC)
30: Wed 22-12-28 VGK@ANA (away, ATTSN-RM)

Choose feed (q to quit): 20
Logging in...
Getting the feed...

Downloading feed: Tue 22-12-27 VGK@LAK (away, ATTSN-RM)
Writing stream to file: 364 MB (10.4 MB/s)
```
Get latest game of your favorite team. It tries to get feed of chosen team (away/home) if available, otherwise it uses first feed found. It's useful also when making scheduled calls (e.g. daily) to application to automatically load latest game. 
```
$ ./nhltv-fetcher -t DAL -p /mnt/download
nhltv-fetcher 1.0.8

Fetching latest feed for 'DAL'...
Logging in...
Getting the feed...
Feed found: Tue 22-12-27 DAL@NSH (away, BSSWX)
Writing stream to file: 524 MB (11.6 MB/s)
```
Using parameter `-l` or `--play` to open the feed directly in VLC assumes that VLC is installed and the executable can be found from PATH environment variable.
## Releases
All release packages contain all needed files, so installation of .NET runtime is not needed. 

After extracting the files on Linux, run `chmod +x nhltv-fetcher` to make the application executable.

## Credits
Kodi NHL.TV plugin ([eracknaphobia/plugin.video.nhlgcl](https://github.com/eracknaphobia/plugin.video.nhlgcl))) for the authentication approach.