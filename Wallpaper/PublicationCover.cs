using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Wallpaper.Model;

namespace Wallpaper
{
    /// <summary>
    /// Класс для установки обложки в сообществе или в профиле пользователя.
    /// </summary>
    internal class PublicationCover
    {
        private CoverSettings _coverOptions { get; set; }
        private string Type { get {  return _coverOptions.Type; } }
        private string VK_ID { get { return _coverOptions.VK_ID; } }
        private string VK_ACCESS_TOKEN { get { return _coverOptions.VK_ACCESS_TOKEN; } }
        private string VkUrl { get; }
        private string WebPageUrl { get { return _coverOptions.WEB_PAGE_URL; } }
        private int Width { get { return _coverOptions.Width; } }
        private int Height { get { return _coverOptions.Height; } }
        private int Delay { get { return _coverOptions.Browser.Delay; } }

        /// <summary>
        /// Инициализирует новый экземпляр класса PublicationCover.
        /// </summary>
        /// <param name="AppConfiguration"></param>
        public PublicationCover(IConfiguration AppConfiguration)
        {
            _coverOptions = AppConfiguration.GetSection(CoverSettings.Selector).Get<CoverSettings>();
            CheckCoverData(_coverOptions);

            if (Type == "group")
            {
                VkUrl = $"https://api.vk.com/method/photos.getOwnerCoverPhotoUploadServer?group_id={VK_ID}&crop_x=0&crop_y=0&crop_x2={Width}&crop_y2={Height}&access_token={VK_ACCESS_TOKEN}&v=5.131";
            }
            else
            {
                VkUrl = $"https://api.vk.com/method/photos.getOwnerCoverPhotoUploadServer?client_id={VK_ID}&crop_x=0&crop_y=0&crop_height={Height}&crop_width={Width}&access_token={VK_ACCESS_TOKEN}&v=5.191";
            }
        }

        /// <summary>
        /// Устанавливает обложку на страницу.
        /// </summary>
        public void SetImage()
        {
            var output = GetImage(WebPageUrl, Width, Height, Delay).Result;
            var bytes = Convert.FromBase64String(output);

            var SendUrlJson = GetToUrl(VkUrl);
            var SendUrl = JsonSerializer.Deserialize<PostUrl>(SendUrlJson).response.upload_url;

            var SendPhotoJson = PostImage(SendUrl, bytes);
            var SendPhoto = JsonSerializer.Deserialize<SetPhoto>(SendPhotoJson);

            var hash = SendPhoto.hash;
            var photo = SendPhoto.photo;

            var SendPhotoUrl = $"https://api.vk.com/method/photos.saveOwnerCoverPhoto?hash={hash}&photo={photo}&access_token={VK_ACCESS_TOKEN}&v=5.131";
            GetToUrl(SendPhotoUrl);
        }

        /// <summary>
        /// Получить изображение веб-страницы.
        /// </summary>
        /// <param name="url">Адрес веб-страницы.</param>
        /// <param name="width">Ширина веб-страницы.</param>
        /// <param name="height">Высота веб-страницы.</param>
        /// <param name="delay">Задержка перед созданием скриншота.</param>
        /// <returns>Изображение веб-страницы в формате base64.</returns>
        private async Task<string> GetImage(string url, int width, int height, int delay)
        {
            var port = _coverOptions.Browser.Port;

            using (var client = new BrowserClient())
            {
                await client.ConnectAsync(port);
                var targetId = await client.CreateTab(url, width, height);

                return await client.CaptureScreenshot(targetId, delay);
            }
        }

        /// <summary>
        /// Отправляет изображение на указанный адрес.
        /// </summary>
        /// <param name="url">Адрес запроса.</param>
        /// <param name="imageByte">Изображение в виде массива byte.</param>
        /// <returns>Результат запроса.</returns>
        private string PostImage(string url, byte[] imageByte)
        {
            using var httpClient = new HttpClient();
            using var form = new MultipartFormDataContent
            {
                { new ByteArrayContent(imageByte, 0, imageByte.Count()), "photo", "image.png" }
            };

            using var response = httpClient.PostAsync(url, form).Result;
            return response.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// Выполняет GET запрос на указанный адрес.
        /// </summary>
        /// <param name="url">Адрес запроса.</param>
        /// <returns>Результат запроса.</returns>
        public static string GetToUrl(string url)
        {
            using var client = new HttpClient();
            return client.GetStringAsync(url).Result;
        }

        /// <summary>
        /// Проверяет данные конфигурации.
        /// </summary>
        private void CheckCoverData(CoverSettings settings)
        {
            if (string.IsNullOrEmpty(settings.Type))
            {
                throw new ArgumentNullException("Type", "Не указан идентификатор.");
            }

            if (string.IsNullOrEmpty(settings.VK_ID))
            {
                throw new ArgumentNullException("VK_ID", "Не указан идентификатор.");
            }

            if (string.IsNullOrEmpty(settings.VK_ACCESS_TOKEN))
            {
                throw new ArgumentNullException("VK_ACCESS_TOKEN", "Не указан токен.");
            }

            if (string.IsNullOrEmpty(settings.WEB_PAGE_URL))
            {
                throw new ArgumentNullException("WEB_PAGE_URL", "Не указан адрес страницы.");
            }

            if (settings.Width < 1 || settings.Width > 65535)
            {
                throw new ArgumentOutOfRangeException("Width", "Значение задано вне допустимого диапазона 1 - 65535");
            }

            if (settings.Height < 1 || settings.Height > 65535)
            {
                throw new ArgumentOutOfRangeException("Height", "Значение задано вне допустимого диапазона 1 - 65535");
            }
        }
    }
}
