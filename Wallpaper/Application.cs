using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.IO;
using Wallpaper.Model;

namespace Wallpaper
{
    /// <summary>
    /// Работа с приложением.
    /// </summary>
    public class Application
    {
        /// <summary>
        /// Состояния работы приложения.
        /// </summary>
        public enum Status
        {
            Run,
            Off
        }

        /// <summary>
        /// Конфигурация браузера.
        /// </summary>
        private Browser _browserOptions { get; set; }

        /// <summary>
        /// Программа для запуска.
        /// </summary>
        public string Program { get { return _browserOptions.Program; } }

        /// <summary>
        /// Порт для подключения.
        /// </summary>
        public int Port { get { return _browserOptions.Port; } }

        /// <summary>
        /// Сохранять идентификатор процесса в файл.
        /// </summary>
        public bool SaveID { get { return _browserOptions.SaveIdProcess; } }

        /// <summary>
        /// Задержка перед сознанием снимка.
        /// </summary>
        public int Delay { get { return _browserOptions.Delay; } }

        /// <summary>
        /// Название файла для сохранения идентификатора процесса.
        /// </summary>
        public readonly string ProcessIdFile = "process.pid";

        /// <summary>
        /// Аргументы запуска.
        /// </summary>
        public string Arguments { get { return $"--remote-debugging-port={Port} --remote-allow-origins=http://localhost:{Port} {_browserOptions.Arguments}"; } }

        /// <summary>
        /// Процесс, управляющий приложением.
        /// </summary>
        protected Process _process;

        /// <summary>
        /// Состояние работы приложения.
        /// </summary>
        public Status State { get; protected set; } = Status.Off;

        /// <summary>
        /// Делегат события завершения работы приложения.
        /// </summary>
        public delegate void ServerClosedEventHandler();

        /// <summary>
        /// Событие завершения работы приложения.
        /// </summary>
        public event ServerClosedEventHandler ServerClosed;

        public Application(IConfiguration configuration)
        {
            _browserOptions = configuration.GetSection(Browser.Selector).Get<Browser>();
            CheckBrowserData(_browserOptions);
        }

        /// <summary>
        /// Запускает приложение.
        /// </summary>
        public void Start()
        {
            if (State != Status.Off)
            {
                return;
            }

            StartServer(new EventHandler((sender, e) =>
            {
                ProcessClosed();
            })
            );

            if(SaveID)
            {
                File.WriteAllText(ProcessIdFile, _process.Id.ToString());
            }

            State = Status.Run;
        }

        /// <summary>
        /// Запускает приложение.
        /// </summary>
        protected void StartServer(EventHandler @event = null)
        {
            _process = new Process();
            _process.StartInfo.FileName = Program;
            _process.StartInfo.Arguments = Arguments;
            _process.StartInfo.UseShellExecute = false;
            _process.StartInfo.RedirectStandardInput = true;
            _process.StartInfo.RedirectStandardOutput = true;
            _process.EnableRaisingEvents = true;

            _process.Exited += @event;

            _process.Start();
            _process.BeginOutputReadLine();
        }

        /// <summary>
        /// Очищает ресурсы процесса после завершения работы.
        /// </summary>
        protected void ProcessClosed()
        {
            _process.Dispose();

            State = Status.Off;

            ServerClosed?.Invoke();
        }

        /// <summary>
        /// Отключает приложение.
        /// </summary>
        public void Close()
        {
            if (State == Status.Off)
            {
                return;
            }

            _process.Kill();
        }

        /// <summary>
        /// Проверяет данные конфигурации браузера.
        /// </summary>
        private void CheckBrowserData(Browser browser)
        {
            if (string.IsNullOrEmpty(browser.Program) && browser.StartBrowser)
            {
                throw new ArgumentNullException("Program", "Не указана программа для запуска.");
            }

            if (browser.Port < 1024 || browser.Port > 65535)
            {
                throw new ArgumentOutOfRangeException("Port", "Значение порта задано вне допустимого диапазона 1024 - 65535");
            }

            if (browser.Delay < 0 || browser.Delay > 125000)
            {
                throw new ArgumentOutOfRangeException("Delay", "Значение ожидания загрузки страницы задано вне допустимого диапазона 0 - 125000");
            }
        }
    }
}
