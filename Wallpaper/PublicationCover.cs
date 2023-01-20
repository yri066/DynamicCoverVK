using Microsoft.Extensions.Configuration;
using System;
using System.IO;
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
        private IConfiguration _configuration { get; set; }
        private string VK_ID { get; }
        private string VK_ACCESS_TOKEN { get; }
        private string VkUrl { get; }
        private string PageUrl { get; }
        private string Width { get; }
        private string Height { get; }
        private int Delay { get; }

        /// <summary>
        /// Инициализирует новый экземпляр класса PublicationCover.
        /// </summary>
        /// <param name="AppConfiguration"></param>
        public PublicationCover(IConfiguration AppConfiguration)
        {
            this._configuration = AppConfiguration;
            VK_ID = _configuration["VK_ID"];
            VK_ACCESS_TOKEN = _configuration["VK_ACCESS_TOKEN"];
            PageUrl = _configuration["WEB_PAGE_URL"];

            Width = _configuration["Width"];
            Height = _configuration["Height"];
            Delay = int.Parse(_configuration["Browser:Delay"]);


            if (_configuration["type"] == "group")
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
        public async void SetImage()
        {
            if (string.IsNullOrEmpty(VK_ID) || string.IsNullOrEmpty(VK_ACCESS_TOKEN) || string.IsNullOrEmpty(PageUrl))
                return;

            var output = await GetImage(PageUrl, Width, Height, Delay);
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
        private async Task<string> GetImage(string url, string width, string height, int delay)
        {
            var port = int.Parse(_configuration["Browser:Port"]);

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
    }
}
