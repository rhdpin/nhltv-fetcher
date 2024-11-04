using NhlTvFetcher.Models;
using System;

namespace NhlTvFetcher {
    public class Messenger {
        private readonly Options _options;

        public Messenger(Options options) {
            _options = options;
        }

        public void Write(string message) {
            Console.Write(message);
        }

        public void WriteLine(string line) {
            Console.WriteLine(line);
        }

        public void WriteLine(string line, LogMessageCategory category) {
            if (category == LogMessageCategory.Verbose && !_options.VerboseMode) {
                return;
            }

            WriteLine(line);
        }

        public void OverwriteLine(string line,
            LogMessageCategory category = LogMessageCategory.Normal) {
            if (category == LogMessageCategory.Verbose && !_options.VerboseMode) {
                return;
            }

            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(line);
        }

        internal void WriteLine(Exception e, LogMessageCategory category) {
            string exceptionMessage = $"{e.Message}\n\n{e.InnerException}\n\n{e.StackTrace}";
            WriteLine(exceptionMessage, category);
        }
    }
}
