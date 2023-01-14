using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Wallpaper
{
    internal class Program
    {
        public static IConfiguration AppConfiguration { get; set; }
        public static Application App { get; set; }

        static void initSettings()
        {
            var builder = new ConfigurationBuilder().AddJsonFile("WallSettings.json", optional: false, reloadOnChange: true);
            AppConfiguration = builder.Build();
        }

        static async Task Main(string[] args)
        {
            initSettings();
            App = new Application(AppConfiguration);
            var start = bool.Parse(AppConfiguration["Browser:StartBrowser"]);

            if (start)
            {
                App.Start();
                await Task.Delay(2000);
            }

            while (true)
            {
                if (DateTime.Now.Second == 0)
                {
                    try
                    {
                        new PublicationCover(AppConfiguration).SetImage();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }

                await Task.Delay(1000);
            }
        }
    }
}
