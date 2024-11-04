using CommandLine;
using System;
using System.Reflection;
using LightInject;
using NhlTvFetcher.Models;

namespace NhlTvFetcher {
    class Program {
        private static int _exitCode = 0;

        public static ServiceContainer IocContainer { get; private set; }

        static void Main(string[] args) {
            Console.CancelKeyPress += (s, ev) => { IocContainer?.Dispose(); };

            if (args == null || args.Length == 0) {
                args = new string[] { "--help" };
            }

            using (IocContainer = new ServiceContainer()) {
                Parser.Default.ParseArguments<Options>(args)
                    .WithParsed<Options>(options => {
                        if (!options.OnlyUrl) {
                            var version = Assembly.GetExecutingAssembly().GetName().Version;
                            Console.WriteLine(
                                $"{Assembly.GetExecutingAssembly().GetName().Name} {version.Major}.{version.Minor}.{version.Build}\n");
                        }

                        if (options.Days < 0) {
                            Console.WriteLine(
                                "Value for parameter '-d' or '--days' can't be negative.");
                            _exitCode = -1;
                        }

                        DateTime date;
                        if (options.Date != null && !DateTime.TryParse(options.Date, out date)) {
                            Console.WriteLine(
                                $"Please check the format of given date string ('{options.Date}'). The date format should be in format yyyy-MM-dd");
                            _exitCode = -1;
                        }

                        if (options.Team == null && options.French) {
                            Console.WriteLine(
                                $"French feeds can only be preferred when getting latest game for a team");
                            _exitCode = -1;
                        }
                    })
                    .WithNotParsed<Options>((errors) => {
                        _exitCode = -1;
                        return;
                    });

                if (_exitCode == -1)
                    return;

                //IocContainer.Register<Downloader, Downloader>();
                //IocContainer.Register<FeedFetcher, FeedFetcher>();

                Parser.Default.ParseArguments<Options>(args)
                    .WithParsed<Options>(async options => {
                        if (options.Team == null && options.Choose == false) {
                            Console.WriteLine(
                                $"Help: {Assembly.GetExecutingAssembly().GetName().Name} --help");
                            return;
                        }

                        IocContainer.Register<Options>(factory => options, "Options",
                            new PerContainerLifetime());

                        IocContainer.Register<Messenger>(new PerContainerLifetime());

                        IocContainer.Register<Session>(new PerContainerLifetime());

                        IocContainer.Register<Downloader>(new PerContainerLifetime());

                        IocContainer.Register<FeedFetcher>(new PerContainerLifetime());

                        IocContainer.Register<FeedManager>(new PerContainerLifetime());
                        var feedManager = IocContainer.GetInstance<FeedManager>();

                        if (options.Team != null) {
                            feedManager.GetLatest(options.Team, options.TargetPath,
                                options.OnlyUrl);
                        }
                        else if (options.Choose) {
                            int result = 0;
                            do {
                                result = feedManager.ChooseFeed(options.TargetPath,
                                    options.OnlyUrl);
                                System.Threading.Thread.Sleep(2000);
                            } while (options.MultipleFeeds && result > 0);
                        }
                    });
            }
        }
    }
}
