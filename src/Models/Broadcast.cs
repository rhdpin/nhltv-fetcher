using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NhlTvFetcher.Models
{
    public class Broadcast
    {
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public string Type { get; set; }        
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Language { get; set; }
    }
}
