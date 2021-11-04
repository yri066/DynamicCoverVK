using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Wallpaper.Model
{
    public class BackgroundTimePublication : BackgroundService
    {
        public IConfiguration AppConfiguration { get; set; }
        public BackgroundTimePublication()
        {
            var builder = new ConfigurationBuilder().AddJsonFile("WallSettings.json");
            AppConfiguration = builder.Build();
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
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
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
