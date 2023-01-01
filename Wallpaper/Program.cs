using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Wallpaper
{
    internal class Program
    {
        public static IConfiguration AppConfiguration { get; set; }

        static void initSettings()
        {
            var builder = new ConfigurationBuilder().AddJsonFile("WallSettings.json", optional: false, reloadOnChange: true);
            AppConfiguration = builder.Build();
        }

        static async Task Main(string[] args)
        {
            initSettings();

            while (true)
            {
                if (DateTime.Now.Second == 0)
                {
                    try
                    {
                        await Task.Run(() => new PublicationWallPaper(AppConfiguration).SetImage());
                        GC.Collect(3);
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
