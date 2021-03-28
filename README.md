# Динамическая обложка для группы ВКонтакте

<a href="https://codebeat.co/projects/github-com-yri066-dynamiccovervk-main"><img alt="codebeat badge" src="https://codebeat.co/badges/9692e3d7-e316-4b08-a7da-4bbe9c30fcb2" /></a>

Программа создает обложку из web-страницы и загружает в группу ВК

[Пример работы в группе](https://vk.com/st_petersburg_group), [импользуемая web-страница (случайные дома)](https://hacklife.badwolf.tech/wallpaperurl.php)

Сборка: 
вначале собрать проект Wallpaper, в собранном проекте создать папку HtmlRender и в нее разместить собранный проект HtmlToImage

В файле Wallpaper/Model/PublicationWallPaper.cs указывается ID группы, ключ доступа к группе и ссылка на web-страницу.

Проект не поддерживает воспроизведение видео.
