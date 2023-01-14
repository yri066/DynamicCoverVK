using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;

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
        /// Конфигурация приложения.
        /// </summary>
        private IConfiguration _configuration { get; set; }

        /// <summary>
        /// Программа для запуска.
        /// </summary>
        public string Programm { get; private set; }

        /// <summary>
        /// Порт для подключения.
        /// </summary>
        public int Port { get; private set; } = 9222;

        /// <summary>
        /// Аргументы запуска.
        /// </summary>
        public string Arguments { get { return $"--headless --remote-debugging-port={Port} --disable-gpu"; } }

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
        public delegate void ServerClocedEventHandler();

        /// <summary>
        /// Cобытие завершения работы приложения.
        /// </summary>
        public event ServerClocedEventHandler ServerCloced;

        public Application(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Запускает приложение.
        /// </summary>
        public void Start()
        {
            CheckApplicationData(_configuration.GetSection("Browser"));
            Programm = _configuration["Browser:Program"];
            Port = int.Parse(_configuration["Browser:Port"]);

            if (State != Status.Off)
            {
                return;
            }

            StartServer(new EventHandler((sender, e) =>
            {
                ProcessClosed();
            })
            );

            State = Status.Run;
        }

        /// <summary>
        /// Запускает приложение.
        /// </summary>
        protected void StartServer(EventHandler @event = null)
        {
            _process = new Process();
            _process.StartInfo.FileName = Programm;
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

            ServerCloced?.Invoke();
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
        /// Проверяет данные.
        /// </summary>
        /// <param name="configuration">Конфигурация приложения.</param>
        private void CheckApplicationData(IConfiguration configuration)
        {
            if (string.IsNullOrEmpty(configuration["Program"]))
            {
                throw new ArgumentNullException("Program", "Программа для запуска не задана");
            }

            if (configuration["Port"] == null)
            {
                throw new ArgumentNullException("Port", "Значения не задано.");
            }

            var port = int.Parse(configuration["Port"]);
            if (port < 0 || port > 65535)
            {
                throw new ArgumentOutOfRangeException("Port", "Значения порта задано вне допустимого диапазона 0 - 65535");
            }

            if (configuration["Delay"] == null)
            {
                throw new ArgumentNullException("Delay", "Значения не задано.");
            }

            var delay = int.Parse(configuration["Delay"]);
            if (delay < 0 || delay > 125000)
            {
                throw new ArgumentOutOfRangeException("Delay", "Значения ожидания загрузки страницы задано вне допустимого диапазона 0 - 125000");
            }
        }
    }
}
