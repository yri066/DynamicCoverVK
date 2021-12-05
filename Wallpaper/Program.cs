using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Wallpaper
{
    internal class Program
    {
        public static IConfiguration AppConfiguration { get; set; }
        static void initProgram()
        {
            var builder = new ConfigurationBuilder().AddJsonFile("WallSettings.json");
            AppConfiguration = builder.Build();
        }
        static async Task Main(string[] args)
        {
            initProgram();
            Console.WriteLine("Start");
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
                        Console.WriteLine(DateTime.Now.ToString() + " Error ");
                        Console.WriteLine(e);
                    }
                }
                else
                {
                    await Task.Delay(1000);
                }
            }
        }
    }
}
