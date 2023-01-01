using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Wallpaper.Model;

namespace Wallpaper
{
    internal class PublicationWallPaper
    {
        private IConfiguration _configuration { get; set; }
        private string VK_ID { get; }
        private string VK_ACCESS_TOKEN { get; }
        private string VkUrl { get; }
        private string ArgumentsStartApp { get; }
        private string PathApp { get; }
        private string PageUrl { get; }

        public PublicationWallPaper(IConfiguration AppConfiguration)
        {
            this._configuration = AppConfiguration;
            VK_ID = _configuration["VK_ID"];
            VK_ACCESS_TOKEN = _configuration["VK_ACCESS_TOKEN"];
            PageUrl = _configuration["WEB_PAGE_URL"];

            string width = _configuration["width"].ToString();
            string height = _configuration["height"].ToString();

            PathApp = Path.Combine(Environment.CurrentDirectory, "HtmlRender", "HtmlToImage.dll");


            if (_configuration["type"] == "group")
            {
                VkUrl = $"https://api.vk.com/method/photos.getOwnerCoverPhotoUploadServer?group_id={VK_ID}&crop_x=00&crop_y=0&crop_x2={width}&crop_y2={height}&&access_token={VK_ACCESS_TOKEN}&v=5.131";
            }
            else
            {
                VkUrl = $"https://api.vk.com/method/photos.getOwnerCoverPhotoUploadServer?client_id={VK_ID}&crop_x=&crop_y=&crop_height={height}&crop_width={width}&access_token={VK_ACCESS_TOKEN}&v=5.191";
            }

            ArgumentsStartApp = $"{PathApp} {PageUrl} {width} {height}";
        }

        /// <summary>
        /// Устанавливает изображение на страницу через VK Api.
        /// </summary>
        public async Task SetImage()
        {
            if (string.IsNullOrEmpty(VK_ID) || string.IsNullOrEmpty(VK_ACCESS_TOKEN) || string.IsNullOrEmpty(PageUrl))
                return;

            if (!File.Exists(Path.Combine(PathApp)))
                return;

            string output = RunProgram();

            var bytes = new MemoryStream(Convert.FromBase64String(output)).ToArray();

            string SendUrlJson = await GetToUrl(VkUrl);

            string SendUrl = JsonSerializer.Deserialize<PostUrl>(SendUrlJson).response.upload_url;

            string SendPhotoJson = PostImage(SendUrl, bytes);
            var SendPhoto = JsonSerializer.Deserialize<SetPhoto>(SendPhotoJson);

            string hash = SendPhoto.hash;
            string photo = SendPhoto.photo;

            string SendPhotoUrl = $"https://api.vk.com/method/photos.saveOwnerCoverPhoto?hash={hash}&photo={photo}&access_token={VK_ACCESS_TOKEN}&v=5.131";
            await GetToUrl(SendPhotoUrl);
        }

        /// <summary>
        /// Отправляет изображение на указанный адрес.
        /// </summary>
        /// <param name="url">Адрес запроса.</param>
        /// <param name="imgByte">Изображение в виде массива byte.</param>
        /// <returns>Результат запроса Json.</returns>
        private string PostImage(string url, byte[] imgByte)//отправить изображение по ссылке
        {
            using var httpClient = new HttpClient();
            using var form = new MultipartFormDataContent();

            byte[] imagebytearraystring = imgByte;
            form.Add(new ByteArrayContent(imagebytearraystring, 0, imagebytearraystring.Count()), "photo", "image.png");
            using var response = httpClient.PostAsync(url, form).Result;

            return response.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// Выполняет GET запрос на указанный адрес.
        /// </summary>
        /// <param name="url">Адрес запроса.</param>
        /// <returns>Результат запроса Json.</returns>
        private async Task<string> GetToUrl(string url)//отправить запрос
        {
            using var client = new HttpClient();

            return await client.GetStringAsync(url);
        }

        /// <summary>
        /// Запускает приложение для создания скриншота.
        /// </summary>
        /// <returns>Изображение в формате base64.</returns>
        private string RunProgram()
        {
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "dotnet";
            p.StartInfo.Arguments = ArgumentsStartApp;
            p.Start();

            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            return output;
        }
    }
}
