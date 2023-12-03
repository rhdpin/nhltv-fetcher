using System;
using System.Collections.Generic;

namespace NhlTvFetcher.Models
{
    public class Broadcast
    {
        private static readonly List<string> FrenchBroadcasters = new() { "RDS", "TVAS", "TVAS2" };

        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public string Type { get; set; }        
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Language
        {
            get
            {
                return Name != null && FrenchBroadcasters.Contains(Name) ? "fr" : "en";
            }
        }

    }
}
