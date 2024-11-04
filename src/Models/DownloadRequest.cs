using NhlTvFetcher.Models;

namespace NhlTvFetcher.Data {
    public class DownloadRequest {
        public string Name { get; set; }
        public string TargetFilePath { get; set; }
        public IStreamer Streamer { get; set; }
    }
}