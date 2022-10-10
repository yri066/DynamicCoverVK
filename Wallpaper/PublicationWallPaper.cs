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
		public IConfiguration AppConfiguration { get; set; }
		private string VK_ID { get; }
		private string VK_ACCESS_TOKEN { get; }
		private string VkUrl { get; }
		private string ArgumentsStartApp { get; }

		public PublicationWallPaper(IConfiguration AppConfiguration)
		{
			this.AppConfiguration = AppConfiguration;
			VK_ID = AppConfiguration["VK_ID"];
			VK_ACCESS_TOKEN = AppConfiguration["VK_ACCESS_TOKEN"];

			string width = AppConfiguration["width"].ToString();
			string height = AppConfiguration["height"].ToString();

			if (AppConfiguration["type"] == "group")
			{
				VkUrl = $"https://api.vk.com/method/photos.getOwnerCoverPhotoUploadServer?group_id={VK_ID}&crop_x=00&crop_y=0&crop_x2={width}&crop_y2={height}&&access_token={VK_ACCESS_TOKEN}&v=5.131";
			}
			else
			{
				VkUrl = $"https://api.vk.com/method/photos.getOwnerCoverPhotoUploadServer?client_id={VK_ID}&crop_x=&crop_y=&crop_height={height}&crop_width={width}&access_token={VK_ACCESS_TOKEN}&v=5.191";
			}

			ArgumentsStartApp = $"{Path.Combine(Environment.CurrentDirectory, "HtmlRender", "HtmlToImage.dll")} {AppConfiguration["WEB_PAGE_URL"]} {width} {height}";
		}

		public async Task SetImage()
		{
			if (VK_ID == "" || VK_ACCESS_TOKEN == "" || AppConfiguration["WEB_PAGE_URL"] == "")
				return;

			string output = RunProgram();

			var bytes = new MemoryStream(Convert.FromBase64String(output)).ToArray();

			string SendUrlJson = await PostToUrl(VkUrl);

			string SendUrl = JsonSerializer.Deserialize<PostUrl>(SendUrlJson).response.upload_url;

			string SendPhotoJson = PostImage(SendUrl, bytes);
			var SendPhoto = JsonSerializer.Deserialize<SetPhoto>(SendPhotoJson);

			string hash = SendPhoto.hash;
			string photo = SendPhoto.photo;

			string SendPhotoUrl = $"https://api.vk.com/method/photos.saveOwnerCoverPhoto?hash={hash}&photo={photo}&access_token={VK_ACCESS_TOKEN}&v=5.131";
			await PostToUrl(SendPhotoUrl);
		}


		string PostImage(string url, byte[] imgByte)//отправить изображение по ссылке
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
			using var client = new HttpClient();

			return await client.GetStringAsync(url);
		}

		string RunProgram()
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
