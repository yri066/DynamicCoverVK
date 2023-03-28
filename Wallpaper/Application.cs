using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.IO;

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
        public string Program { get; private set; }

        /// <summary>
        /// Порт для подключения.
        /// </summary>
        public int Port { get; private set; } = 9222;

        /// <summary>
        /// Сохранять идентификатор процесса в файл.
        /// </summary>
        public bool SaveID { get; private set; } = false;

        /// <summary>
        /// Название файла для сохранения идентификатора процесса.
        /// </summary>
        public readonly string ProcessIdFile = "process.pid";

        /// <summary>
        /// Аргументы запуска.
        /// </summary>
        public string Arguments { get { return $"--remote-debugging-port={Port} {_configuration["Browser:Arguments"]}"; } }

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
            _configuration = configuration;
        }

        /// <summary>
        /// Запускает приложение.
        /// </summary>
        public void Start()
        {
            var configuration = _configuration.GetSection("Browser");

            CheckApplicationData(configuration);
            Program = configuration["Program"];
            Port = int.Parse(configuration["Port"]);
            SaveID = bool.Parse(configuration["SaveIdProcess"]);

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
        /// Проверяет данные конфигурации.
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

            if (configuration["SaveIdProcess"] == null)
            {
                throw new ArgumentNullException("SaveIdProcess", "Значения не задано.");
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
