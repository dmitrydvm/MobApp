# MobApp

Сервис предназначен для получения информации о последней версии андроид-приложения и скачивания файла `apk`.
Нужен для обновления клиентов в обход Google Play Market, если приложение предназначено для внутреннего использования и не публикуется в сторах.

Доступ закрыт аутентификацией OIDC, доступен только клиентам с валидным токеном и при наличии необходимого скоупа.

Должна быть следующая структура каталогов со скачиваемым приложением:
`/Content/{AppName}/{Platform}/{version}/{version.apk}`
Рядом с apk-файлом может опционально находиться файл `ReleaseNotes.txt`.
Содержимое ReleaseNotes.txt построчно выдается в ответе на запрос `/latestVersion`  в виде массива строк.

Документация по API после запуска приложения доступна здесь: 
https://localhost:5000/swagger

# Get starting
- Клонировать репо
- Переименовать контроллер в нужное название приложения 
- Если не нужна аутентификация, то удалить атрибут `[Authorize]` с контроллера
- Если аутентификация нужна, то прописать валидные данные в `Startup.cs` и `AppSettings.json`
- Разместить файлы для скачивания и `ReleaseNotes.txt` в соответствии со структурой каталогов `/Content/{AppName}/{Platform}/{version}/{version.apk}`

# References
- Asp.Net Core 2.2
- IdentitServer4
- NLog
- NSwag
