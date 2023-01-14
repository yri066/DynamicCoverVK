using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wallpaper
{
    /// <summary>
    /// Работа с браузером на основе Chromium.
    /// </summary>
    public class BrowserClient : IDisposable
    {
        private ClientWebSocket _webSocket;
        private int _port;

        /// <summary>
        /// Инициализирует новый экземпляр класса BrowserClient.
        /// </summary>
        public BrowserClient()
        {
            _webSocket = new ClientWebSocket();
        }

        /// <summary>
        /// Устанавливает подключение к браузеру.
        /// </summary>
        /// <param name="port">Номер порта.</param>
        public async Task ConnectAsync(int port)
        {
            _port = port;
            var result = PublicationCover.GetToUrl($"http://localhost:{port}/json");
            var WSurl = (string)JsonConvert.DeserializeObject<dynamic>(result)[0].webSocketDebuggerUrl;

            await _webSocket.ConnectAsync(new Uri(WSurl), CancellationToken.None);
        }

        /// <summary>
        /// Открывает новую вкладку.
        /// </summary>
        /// <param name="url">Адрес веб-страницы.</param>
        /// <param name="width">Ширина веб-страницы.</param>
        /// <param name="height">Высота веб-страницы.</param>
        /// <returns>Идентификатор вкладки.</returns>
        public async Task<string> CreateTab(string url, string width = "960", string height = "384")
        {
            var query = @$"{{""id"": 1, ""method"": ""Target.createTarget"", ""params"": {{""url"": ""{url}"", ""width"": {width}, ""height"": {height}}} }}";
            await Send(_webSocket, query);
            var result = await Receive(_webSocket);

            return (string)JsonConvert.DeserializeObject<dynamic>(result).result.targetId;
        }

        /// <summary>
        /// Создает скриншот веб-страницы.
        /// </summary>
        /// <param name="targetId">Идентификатор вкладки.</param>
        /// <param name="delay">Задержка перед созданием скриншота.</param>
        /// <returns>Скриншот веб-страницы в формате base64.</returns>
        public async Task<string> CaptureScreenshot(string targetId, int delay = 10000)
        {
            await Task.Delay(delay);

            using (var socket = new ClientWebSocket())
            {
                await socket.ConnectAsync(new Uri($"ws://localhost:{_port}/devtools/page/{targetId}"), CancellationToken.None);

                await Send(socket, @$"{{ ""id"": 2, ""method"": ""Page.captureScreenshot""}}");
                var result = await Receive(socket);

                await Send(socket, @$"{{ ""id"": 3, ""method"": ""Page.close""}}");
                await Receive(socket);

                return (string)JsonConvert.DeserializeObject<dynamic>(result).result.data;
            }
        }

        /// <summary>
        /// Отправляет сообщение.
        /// </summary>
        /// <param name="socket">Подключенный веб-сокет клиент.</param>
        /// <param name="data">Сообщение.</param>
        static async Task Send(ClientWebSocket socket, string data) =>
            await socket.SendAsync(Encoding.UTF8.GetBytes(data), WebSocketMessageType.Text, true, CancellationToken.None);

        /// <summary>
        /// Получает ответ.
        /// </summary>
        /// <param name="socket">Подключенный веб-сокет клиент.</param>
        /// <returns>Ответ.</returns>
        private async Task<string> Receive(ClientWebSocket socket)
        {
            var buffer = new ArraySegment<byte>(new byte[2048]);
            do
            {
                WebSocketReceiveResult result;
                using (var stream = new MemoryStream())
                {
                    do
                    {
                        result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                        stream.Write(buffer.Array, buffer.Offset, result.Count);
                    }
                    while (!result.EndOfMessage);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        return string.Empty;
                    }

                    stream.Seek(0, SeekOrigin.Begin);
                    using (var reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        return await reader.ReadToEndAsync();
                    }
                }
            }
            while (true);
        }

        public void Dispose()
        {
            _webSocket.Dispose();
        }
    }
}
