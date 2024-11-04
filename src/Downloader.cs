using NhlTvFetcher.Data;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NhlTvFetcher.Models;

namespace NhlTvFetcher {
    public class Downloader {
        private readonly Messenger _messenger;
        private readonly Options _options;

        public Downloader(Messenger messenger, Options options) {
            _messenger = messenger;
            _options = options;
        }

        public void Download(DownloadRequest request) {
            // This dirty solution requires that StreamLink can be found from current dir or PATH
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                request.Streamer.AppName += ".exe";
            }

            var targetDirectory = Path.GetDirectoryName(request.TargetFilePath);
            if (request.TargetFilePath != null && !targetDirectory.Equals(string.Empty) &&
                !Directory.Exists(targetDirectory)) {
                _messenger.WriteLine(
                    "Target path was not found. Do you want to create it? (y/n): ");
                var cki = Console.ReadKey();
                if (cki.Key == ConsoleKey.Y) {
                    Directory.CreateDirectory(targetDirectory);
                }
            }

            if (_options.VerboseMode) {
                request.Streamer.Parameters["-v"] = "";
                request.Streamer.Parameters["-l"] = "debug";
            }

            if (_options.Play) {
                request.Streamer.Parameters["--player-passthrough"] = "http"; //hls or http
            }
            else if (_options.Stream) {
                request.Streamer.Parameters["--player-external-http"] = "";
            }
            else {
                request.Streamer.Parameters["-f"] = "";
                request.Streamer.Parameters["-o"] = request.TargetFilePath;
            }

            string args = request.Streamer.ArgsToCommandLineString(true);
            _messenger.WriteLine(
                $"\nStarting download with command: \n'{request.Streamer.AppName} {args}'",
                LogMessageCategory.Verbose);

            _ = LaunchProcess(request.Streamer.AppName, args, request.Name, request.TargetFilePath);
        }

        async Task LaunchProcess(string processName, string arguments, string processTitle,
            string targetFilePath) {
            using (var process = new Process() {
                       StartInfo = new ProcessStartInfo(processName) {
                           RedirectStandardOutput = true,
                           Arguments = arguments,
                           UseShellExecute = false,
                       }
                   }) {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(Environment.NewLine + "STARTING FEED: " + processTitle);

                process.OutputDataReceived += (s, e) => sb.AppendLine("\t" + e.Data);
                process.ErrorDataReceived += (s, e) => sb.AppendLine("\t" + e.Data);
                process.Start();
                process.BeginOutputReadLine();
                process.Exited += (s, e) => { };

                var timeoutSignal = new CancellationTokenSource();
                try {
                    await process.WaitForExitAsync(timeoutSignal.Token);
                    if (_options.VerboseMode) {
                        sb.AppendLine("FEED HAS EXITED: " + processTitle);
                        _messenger.WriteLine(sb.ToString() + Environment.NewLine);
                    }
                }
                catch (OperationCanceledException) {
                    process.Kill();
                    sb.AppendLine("FEED TERMINATED UNEXPECTEDLY: " + processTitle);
                    Console.WriteLine(sb.ToString() + Environment.NewLine);
                }
            }
        }
    }
}
