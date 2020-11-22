using GrapeCity.Documents.Html;
using System;
using System.Drawing;
using System.IO;

namespace HtmlToImage
{
    class Program
    {
        static void Main(string[] args)
        {
            var uri = new Uri(args[0]);

            using (var re = new GcHtmlRenderer(uri))
            {
                re.VirtualTimeBudget = 5000;//Задержка перед созданием изображения

                PngSettings pngSettings = new PngSettings//Параметры изображения
                {
                    DefaultBackgroundColor = Color.White,
                    WindowSize = new Size(1590, 400)
                };

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    re.RenderToPng(memoryStream, pngSettings);

                    Console.WriteLine(Convert.ToBase64String(memoryStream.ToArray()));
                }
            }
        }
    }
}
