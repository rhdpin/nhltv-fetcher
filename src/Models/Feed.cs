using System;

namespace NhlTvFetcher.Data {
    public class Feed {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Home { get; set; }
        public string Away { get; set; }
        public string MediaId { get; set; }
        public string Type { get; set; }
        public string Broadcaster { get; set; }
        public bool IsFrench { get; set; }
        public bool IsReplay { get; set; }

        public override string ToString() {
            return $"{Date.ToString("ddd yyyy-MM-dd hh:mmtt")} " +
                   $"{Away}@{Home} " +
                   $"({Type})" +
                   $"{(Broadcaster != null ? $" ({Broadcaster})" : "")}" +
                   $"{(IsFrench ? " (FRENCH)" : "")}";
        }

        /// <summary>
        /// String format for OS filesystems, e.g. <c>2024-10-12_0700PM_PIT_at_TOR_Away_SNPIT</c>.
        /// </summary>
        /// <returns></returns>
        public string ToFileNameString() {
            return $"{Date:yyyy-MM-dd_hhmmtt}_" +
                   $"{Away}_at_{Home}_" +
                   $"{Type}_" +
                   $"{Broadcaster}" +
                   $"{(IsFrench ? "_FR" : "")}";
        }
    }
}