using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Text.Json;
using Wallpaper.Class;

namespace Wallpaper.Model
{
    public class PublicationWallPaper
    {
        public IConfiguration AppConfiguration { get; set; }
        private string VK_GROUP_ID { get; }
        private string VK_GROUP_API_KEY { get; }
        private string VkUrl { get; }
        private string ArgumentsStartApp { get; }

        public PublicationWallPaper(IConfiguration AppConfiguration)
        {
            this.AppConfiguration = AppConfiguration;
            VK_GROUP_ID = AppConfiguration["VK_GROUP_ID"];
            VK_GROUP_API_KEY = AppConfiguration["VK_GROUP_API_KEY"];
            

            string width = AppConfiguration["width"].ToString();
            string height = AppConfiguration["height"].ToString();
            VkUrl = "https://api.vk.com/method/photos.getOwnerCoverPhotoUploadServer?group_id=" + VK_GROUP_ID + "&crop_x=00&crop_y=0&crop_x2=" + width + "&crop_y2=" + height + "&&access_token=" + VK_GROUP_API_KEY + "&v=5.124";

            ArgumentsStartApp = AppConfiguration["WEB_PAGE_URL"] + " " + width + " " + height;
        }
        public async Task SetImage()
        {
            if (VK_GROUP_ID == "" || VK_GROUP_API_KEY == "" || AppConfiguration["WEB_PAGE_URL"] == "")
                return;

            string output = RunProgram();

            var bytes = new MemoryStream(Convert.FromBase64String(output)).ToArray();

            string SendUrlJson = await PostToUrl(VkUrl);
            string SendUrl = JsonSerializer.Deserialize<PostUrl>(SendUrlJson).response.upload_url;

            string SendPhotoJson = PostImage(SendUrl, bytes);
            var SendPhoto = JsonSerializer.Deserialize<SetPhoto>(SendPhotoJson);

            string hash = SendPhoto.hash;
            string photo = SendPhoto.photo;

            string SendPhotoUrl = "https://api.vk.com/method/photos.saveOwnerCoverPhoto?hash=" + hash + "&photo=" + photo + "&access_token=" + VK_GROUP_API_KEY + "&v=5.124";
            await PostToUrl(SendPhotoUrl);
        }


        string PostImage(string url, byte[] imgByte)//загрузить изображение по ссылке
        {
            HttpClient httpClient = new HttpClient();
            MultipartFormDataContent form = new MultipartFormDataContent();

            byte[] imagebytearraystring = imgByte;
            form.Add(new ByteArrayContent(imagebytearraystring, 0, imagebytearraystring.Count()), "photo", "image.png");
            HttpResponseMessage response = httpClient.PostAsync(url, form).Result;

            httpClient.Dispose();
            string rez = response.Content.ReadAsStringAsync().Result;
            return rez;
        }


        async Task<string> PostToUrl(string url)//отправить запрос
        {
            string rez = null;
            WebRequest request = WebRequest.Create(url);
            WebResponse response2 = await request.GetResponseAsync();
            using (Stream stream = response2.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    rez = reader.ReadToEnd();
                }
            }
            response2.Close();
            return rez;
        }

        string RunProgram()
        {
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                p.StartInfo.FileName = Environment.CurrentDirectory + @"\HtmlRender\HtmlToImage.exe";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                p.StartInfo.FileName = Environment.CurrentDirectory + @"/HtmlRender/HtmlToImage";

            p.StartInfo.Arguments = ArgumentsStartApp;
            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            return output;
        }
    }
}
