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
            int width = Int32.Parse(args[1]);
            int height = Int32.Parse(args[2]);

            using (var re = new GcHtmlRenderer(uri))
            {
                re.VirtualTimeBudget = 15000; //Задержка перед созданием изображения

                PngSettings pngSettings = new PngSettings //Параметры изображения
                {
                    DefaultBackgroundColor = Color.White,
                    WindowSize = new Size(width, height)
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
