using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Wallpaper
{
    internal class Program
    {
        private static IConfiguration AppConfiguration { get; set; }
        private static Application App { get; set; }
        private static readonly TimeSpan _setInterval = TimeSpan.FromMinutes(1);
        private static Timer _timer;

        static void initSettings()
        {
            if(!File.Exists("CoverSettings.json"))
            {
                throw new FileNotFoundException("Отсутствует файл настроек: CoverSettings.json");
            }

            var builder = new ConfigurationBuilder().AddJsonFile("CoverSettings.json", optional: false, reloadOnChange: true);
            AppConfiguration = builder.Build();
        }

        static void Main(string[] args)
        {
            try
            {
                initSettings();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            App = new Application(AppConfiguration);
            var start = bool.Parse(AppConfiguration["Browser:StartBrowser"]);

            if (start)
            {
                try
                {
                    App.Start();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                Task.Delay(2000).Wait();

                if(App.State == Application.Status.Off)
                {
                    Console.WriteLine("Не удалось запустить браузер.");
                    return;
                }
            }

            Console.WriteLine("Started.");

            var delaySeconds = 60 - DateTime.UtcNow.Second;
            _timer = new Timer(SetImage, null, TimeSpan.FromSeconds(delaySeconds), _setInterval);

            Console.WriteLine("Для выхода нажмите Enter.");
            Console.ReadLine();

            _timer.Dispose();
        }

        /// <summary>
        /// Устанавливает обложку на страницу.
        /// </summary>
        private static void SetImage(object state)
        {
            try
            {
                new PublicationCover(AppConfiguration).SetImage();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
