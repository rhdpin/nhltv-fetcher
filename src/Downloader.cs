﻿using NhlTvFetcher.Data;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NhlTvFetcher
{
    public class Downloader
    {
        private const string StreamLinkAppName = "streamlink";
        private readonly Process _process = null;
        private readonly Messenger _messenger;
        private readonly Options _options;        

        public Downloader(Messenger messenger, Options options)
        {
            _messenger = messenger;
            _options = options;            
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_process != null && !_process.HasExited)
                {
                    _process.Kill(true);
                }
            }
        }

        public void Download(DownloadRequest request)
        {
            // This dirty solution requires that StreamLink can be found from current dir or PATH
            var streamLinkAppName = "streamlink";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                streamLinkAppName += ".exe";
            }

            var targetDirectory = Path.GetDirectoryName(request.TargetFilePath);
            if (request.TargetFilePath != null && !targetDirectory.Equals(string.Empty) && !Directory.Exists(targetDirectory))
            {
                _messenger.WriteLine("Target path was not found. Do you want to create it? (y/n): ");
                var cki = Console.ReadKey();
                if (cki.Key == ConsoleKey.Y)
                {
                    Directory.CreateDirectory(targetDirectory);
                }
            }
                        

            var loggingString = string.Empty;
            if (_options.VerboseMode)
            {
                loggingString = $"-v -l debug";
            }

            string outputMode = $"-f -o {request.TargetFilePath}";
            if (_options.Play)
            {
                outputMode = "";
            }
            else if (_options.Stream)
            {
                outputMode = "--player-external-http";
            }

            var streamUrl = request.StreamUrl;
            var streamArgs = $"\"hlsvariant://{streamUrl} name_key=bitrate \" {_options.Bitrate} --http-header " +
                                $"\"User-Agent={FeedFetcher.UserAgent}\" --stream-segment-threads=4 {loggingString} {outputMode}";

            _messenger.WriteLine($"Starting download with command '{StreamLinkAppName} {streamArgs}", MessageCategory.Verbose);

            using (Process process = new Process())
            {
                process.StartInfo.FileName = streamLinkAppName;
                process.StartInfo.Arguments = streamArgs;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;

                StringBuilder output = new StringBuilder();

                using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
                {
                    process.OutputDataReceived += (sender, e) =>
                    {
                        if (e.Data == null)
                        {
                            outputWaitHandle.Set();
                        }
                        else
                        {
                            if (_options.Stream)
                            {
                                _messenger.WriteLine(e.Data);
                            }
                            output.AppendLine(e.Data);
                        }
                    };

                    try
                    {
                        process.Start();
                    }
                    catch (Exception e)
                    {
                        _messenger.WriteLine($"Could not start Streamlink '{StreamLinkAppName}': {e.Message}. Ensure that the Streamlink (https://streamlink.github.io/install.html) application is installed and located in current directory or $PATH.");
                        return;
                    }

                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    if (!_options.VerboseMode)
                    {
                        if (!_options.HideProgress && !_options.Play && !_options.Stream)
                        {
                            Task.Run(() =>
                            {
                                var counter = 1;
                                long previousSize = 0;
                                DateTime previousTime = DateTime.Now;
                                double downloadRate = 0;
                                string downloadRateString;

                                while (process != null && !process.HasExited)
                                {
                                    Thread.Sleep(500);
                                                                        
                                    if (File.Exists(request.TargetFilePath))
                                    {
                                        var currentSize = new FileInfo(request.TargetFilePath).Length;
                                        if (counter % 10 == 0)
                                        {
                                            var currentTime = DateTime.Now;
                                            var secondsElapsed = (currentTime - previousTime).TotalMilliseconds / 1000;
                                            var bytesDownloaded = currentSize - previousSize;
                                            downloadRate = bytesDownloaded / 1024 / 1024 / secondsElapsed;
                                            previousSize = currentSize;
                                            previousTime = currentTime;
                                        }
                                        downloadRateString = downloadRate > 0 ? downloadRate.ToString("0.0") : "---";

                                        _messenger.OverwriteLine($"Writing stream to file: {currentSize / 1024 / 1024} MB ({downloadRateString} MB/s)");
                                        counter++;
                                    }                                    
                                }
                            });
                        }                        

                        process.WaitForExit();
                        _messenger.WriteLine("");

                        var unexpectedOutput = false;

                        // Write Streamlink output to console (other than the regular messages)                        
                        using (var stringReader = new StringReader(output.ToString()))
                        {
                            string line = null;
                            do
                            {
                                line = stringReader.ReadLine();

                                if (line != null)
                                {
                                    if (!line.StartsWith("[cli][info]") && !line.Contains("[download]") &&
                                        !line.StartsWith(request.TargetFilePath))
                                    {
                                        _messenger.WriteLine($"{line}");
                                        unexpectedOutput = true;
                                    }
                                    else if (_options.VerboseMode)
                                    {
                                        _messenger.WriteLine($"{line}");
                                    }
                                }
                            } while (line != null);
                        }

                        if (unexpectedOutput)
                        {
                            _messenger.WriteLine("\nLooks like something happened when downloading. If the log indicates more than just warnings, " + 
                                "please check first that you can access NHL.TV from IP address you are currently using.");
                            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(request.TargetFilePath);

                            if (File.Exists(request.TargetFilePath))
                            {
                                var suspectFilePath = request.TargetFilePath.Replace(fileNameWithoutExtension, 
                                    fileNameWithoutExtension + "-suspect");

                                if (File.Exists(suspectFilePath))
                                {
                                    File.Delete(suspectFilePath);
                                }

                                File.Move(request.TargetFilePath, suspectFilePath);
                            }                            
                        }
                    }
                    else
                    {
                        _messenger.WriteLine("Full output from Streamlink is displayed after process has exited. Please wait.", MessageCategory.Verbose);

                        process.WaitForExit();

                        // Output all output from Streamlink after process has exited
                        _messenger.WriteLine(output.ToString());
                    }
                }
            }
        }
    }
}
