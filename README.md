# Динамическая обложка для сообществ и профилей ВКонтакте

<a href="https://codebeat.co/projects/github-com-yri066-dynamiccovervk-main"><img alt="codebeat badge" src="https://codebeat.co/badges/018fffdf-e62e-4226-a658-884715fd1d2a" /></a>

Для работы необходим установленный: **ASP.NET Core 6**

Сборка: 
вначале собрать проект Wallpaper, в собранном проекте создать папку HtmlRender и в нее разместить собранный проект HtmlToImage

В файле Wallpaper/WallSettings.json указывается длина и ширина изображения, тип страницы (`group`/`client`), ID группы или пользователя, ключ доступа, и ссылка на web-страницу.

Проект не поддерживает воспроизведение видео.

Для работы проекта на Linux, необходимо установить пакет `libreadline6-dev`

**Поддрежка сообществ**

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

**Поддрежка личных страниц**

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
Способ получения **токена пользователя** доступен по ссылке - [Как получить токен пользователя для ВКонтакте](https://dvmn.org/encyclopedia/qna/63/kak-poluchit-token-polzovatelja-dlja-vkontakte/)

**Токен привязывается к ip адресу, с которого он был получен.**
Идентификатор пользователя можно получить: url адрес страницы или в настройках профиля (изменить никнейм).
