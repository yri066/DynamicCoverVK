# Динамическая обложка для сообществ и профилей ВКонтакте

[![codebeat badge](https://codebeat.co/badges/83571851-5ec2-40ed-82b2-9e30c4bd2c71)](https://codebeat.co/projects/github-com-yri066-dynamiccovervk-main)


Dynamic Cover VK позволяет создать из Web-страницы изображение и установить в качестве обложки с переодичностью в 1 минуту.


Для работы необходимо установить: **ASP.NET Core 6** и один из следующих браузеров: **`Google Chrome`**, **`Chromium`** или **`Microsoft Edge`**.

В файле Wallpaper/WallSettings.json указывается длина и ширина изображения, задержка перед созданием изображения, тип страницы (group/client), ID группы или пользователя, ключ доступа, ссылка на web-страницу и другие параметрый:

**StartBrowser** - необходимость запустить браузер в режиме удаленной отладки: true - запустить браузер, false - не заускать (если уже есть запущенный экземпляр).

**Program** - запускаемый браузер на основе Chromium:
1. **Windows**

	1.1. Google Chrome `C:\Program Files\Google\Chrome\Application\chrome.exe`
	
	1.2 Microsoft Edge `C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe`
2. **macOS**

	1.1. Google Chrome `/Applications/Google Chrome.app/Contents/MacOS/Google Chrome`
3. **Linux**

	1.1. Chromium `chromium` или `chromium-browser`



Пример параметров для изменения обложки в **профиле ользователя**:

```
{
	"Browser": {
		"Arguments": "--headless --disable-gpu",
		"StartBrowser": true, 
		"Program": "chromium-browser",
		"Port": 9222,
		"Delay": 10000
	},
	"Width": 1920,
	"Height": 768,
	"Type": "client",
	"VK_ID": "82169748",
	"VK_ACCESS_TOKEN": "vk1.a.ef033c224f574ba43ef033c224f574ba43",
	"WEB_PAGE_URL": "https://example.com"
}
```
