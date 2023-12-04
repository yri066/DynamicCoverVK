using System;

namespace Wallpaper.Model
{
    internal class CoverSettings
    {
        public const string Selector = "Settings";

        public Browser Browser { get; set; } = new Browser();
        public int Width { get; set; } = 1920;
        public int Height { get; set; } = 768;
        public string Type { get; set; } = "group";
        public string VK_ID { get; set; } = String.Empty;
        public string VK_ACCESS_TOKEN {  get; set; } = String.Empty;
        public string WEB_PAGE_URL { get; set; } = String.Empty;
    }

    internal class Browser
    {
        public const string Selector = "Settings:Browser";

        public string Arguments { get; set; } = String.Empty;
        public bool StartBrowser { get; set; } = false;
        public string Program { get; set; } = String.Empty;
        public bool SaveIdProcess { get; set; } = false;
        public int Port { get; set; } = 9222;
        public int Delay { get; set; } = 10000;

    }
}
