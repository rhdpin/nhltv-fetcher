using System;

namespace NhlTvFetcher.Data
{
    public class Feed
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Home { get; set; }
        public string Away { get; set; }
        public string MediaId { get; set; }
        public string Type { get; set; }
        public string Broadcaster { get; set; }
        public bool IsFrench { get; set; }

        public override string ToString()
        {
            return $"{Date} {Home}@{Away} ({Type}{(IsFrench? ", FR" : "")}{(Broadcaster != null ? ", " + Broadcaster : "")})";
        }
    }
}
