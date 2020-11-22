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
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (DateTime.Now.Second != 0)
                {
                    await Task.Delay(1000, stoppingToken);
                }
                else
                {
                    try
                    {
                        await new PublicationWallPaper().CreateImageAsync();
                        GC.Collect(3);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(DateTime.Now.ToString() + " " + "Error");
                        Console.WriteLine(e);
                    }
                    finally
                    {
                        await Task.Delay(1000, stoppingToken);
                    }
                }
            }
        }
    }
}
