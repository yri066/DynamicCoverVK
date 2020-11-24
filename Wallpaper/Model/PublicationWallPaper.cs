using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Wallpaper.Model
{
    public class PublicationWallPaper
    {
        private const string VK_GROUP_ID = "YOUR_GROUP_ID";
        private const string VK_GROUP_KEY = "YOUR_GROUP_KEY_API";

        private const string VkUrl = @"https://api.vk.com/method/photos.getOwnerCoverPhotoUploadServer?group_id=" + VK_GROUP_ID + "&crop_x=00&crop_y=0&crop_x2=1590&crop_y2=400&&access_token=" + VK_GROUP_KEY + "&v=5.124";
        
        public async Task CreateImageAsync()
        {
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;

            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                p.StartInfo.FileName = Environment.CurrentDirectory + @"\HtmlRender\HtmlToImage.exe";
            
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                p.StartInfo.FileName = Environment.CurrentDirectory + @"/HtmlRender/HtmlToImage";
            
            p.StartInfo.Arguments = "YOUR_WEB_PAGE_URL";
            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            var bytes = new MemoryStream(Convert.FromBase64String(output)).ToArray();


            string SendUrl = await PostToUrl(VkUrl);

            SendUrl = SendUrl.Substring(27);
            SendUrl = SendUrl.Substring(0, SendUrl.Length - 3);
            SendUrl = SendUrl.Replace(@"\", "");

            string result = PostImage(SendUrl, bytes);

            string hash = result.Substring(result.IndexOf("h\":\"") + 4, result.IndexOf("photo") - 3 - (result.IndexOf("h\":\"") + 4));
            string photo = result.Substring(result.IndexOf("photo") + 8, result.IndexOf("}") - 1 - (result.IndexOf("photo") + 8));

            string SendPhoto = "https://api.vk.com/method/photos.saveOwnerCoverPhoto?hash=" + hash + "&photo=" + photo + "&access_token=" + VK_GROUP_KEY + "&v=5.124";
            await PostToUrl(SendPhoto);

            GC.Collect(3);
        }


        private string PostImage(string url, byte[] imgByte)//загрузить изображение по ссылке
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


        private async Task<string> PostToUrl(string url)//отправить запрос
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
    }
}
