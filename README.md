# MobApp
Service for downloading applications and receiving their release notes. Provides a response in the json like https://github.com/javiersantos/AppUpdater/wiki/UpdateFrom.JSON

Сервис предназначен для получения информации о последней версии андроид-приложения и скачивания файла apk.
Нужен для обновления клиентов в обход Google Play Market, если приложение предназначено для внутреннего использования и не публикуется в сторах.

Доступ закрыт аутентификацией OIDC, доступен только клиентам с валидным токеном и при наличии необходимого скоупа.

Документация по API после запуска приложения доступна здесь: 
https://localhost:5000/swagger

Комментарии в коде на русском по причине корпоративных стандартов.
