# Динамическая обложка для сообществ и профилей ВКонтакте

[![codebeat badge](https://codebeat.co/badges/83571851-5ec2-40ed-82b2-9e30c4bd2c71)](https://codebeat.co/projects/github-com-yri066-dynamiccovervk-main)


Dynamic Cover VK позволяет создать из Web-страницы изображение и установить в качестве обложки с переодичностью в 1 минуту.


Для работы необходим установленный: **ASP.NET Core 6**

Сборка: 
вначале собрать проект Wallpaper, в собранном проекте создать папку HtmlRender и в нее разместить собранный проект HtmlToImage

В файле Wallpaper/WallSettings.json указывается длина и ширина изображения, тип страницы (`group`/`client`), ID группы или пользователя, ключ доступа, и ссылка на web-страницу.

Проект не поддерживает воспроизведение видео.

Для работы проекта на Linux, необходимо установить следующие пакеты:
```
sudo apt-get install libxss1 libappindicator1 libindicator7 libnss3-dev
```

**Поддрежка группы**

```
{
	"width": 1200,
	"height": 400,
	"type": "group",
	"VK_ID": "152760895",
	"VK_ACCESS_TOKEN": "ef033c224f574ba43ef033c224f574ba43",
	"WEB_PAGE_URL": "Web page Url"
}
```
Получить **токен сообщества** можно в: Настройки->Работа с API

**Поддрежка профиля**

```
{
	"width": 1920,
	"height": 640,
	"type": "client",
	"VK_ID": "82169748",
	"VK_ACCESS_TOKEN": "vk1.a.ef033c224f574ba43ef033c224f574ba43",
	"WEB_PAGE_URL": "Web page Url"
}
```
Способ получения **токена пользователя** доступен по ссылке - [Authorization Code Flow для получения ключа доступа пользователя](https://dev.vk.com/api/access-token/authcode-flow-user).
Идентификатор пользователя можно получить: url адрес страницы или в настройках профиля (изменить никнейм).
