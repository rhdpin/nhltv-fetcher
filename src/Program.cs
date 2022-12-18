using CommandLine;
using System;
using System.Reflection;
using LightInject;

namespace NhlTvFetcher
{
    class Program
    {                
        private static int _exitCode = 0;

        public static ServiceContainer IocContainer { get; private set; }

        static void Main(string[] args)
        {
            Console.CancelKeyPress += (s, ev) =>
            {
                IocContainer?.Dispose();                
            };

            using (IocContainer = new ServiceContainer())
            {
                Parser.Default.ParseArguments<Options>(args)
                    .WithParsed<Options>(options =>
                    {   
                        if (!options.OnlyUrl)
                        {
                            var version = Assembly.GetExecutingAssembly().GetName().Version;
                            Console.WriteLine($"{Assembly.GetExecutingAssembly().GetName().Name} {version.Major}.{version.Minor}.{version.Build}\n");                            
                        }
                        if (options.Days < 0)
                        {
                            Console.WriteLine("Value for parameter '-d' or '--days' can't be negative.");
                            _exitCode = -1;
                        }
                        DateTime date;
                        if (options.Date != null && !DateTime.TryParse(options.Date, out date))
                        {
                            Console.WriteLine($"Please check the format of given date string ('{options.Date}'). The date format should be in format yyyy-MM-dd");
                            _exitCode = -1;
                        }                        
                    })
                    .WithNotParsed<Options>((errors) =>
                    {
                        _exitCode = -1;
                        return;
                    });

                if (_exitCode == -1)
                    return;                

                IocContainer.Register<Downloader, Downloader>();                
                IocContainer.Register<FeedFetcher, FeedFetcher>();       

                Parser.Default.ParseArguments<Options>(args)
                    .WithParsed<Options>(options =>
                    {
                        IocContainer.Register<Options>(factory => options);
                        IocContainer.Register<Messenger, Messenger>();
                        IocContainer.Register<FeedManager, FeedManager>();
                        
                        var feedManager = IocContainer.GetInstance<FeedManager>();

                        if (options.Team != null)
                        {
                            feedManager.GetLatest(options.Team, options.TargetPath, options.OnlyUrl);
                        }
                        else if (options.Choose)
                        {
                            feedManager.ChooseFeed(options.TargetPath, options.OnlyUrl);
                        }
                        else
                        {
                            Console.WriteLine($"Help: {Assembly.GetExecutingAssembly().GetName().Name} --help");
                        }
                    });
            }
        }              
    }
}
