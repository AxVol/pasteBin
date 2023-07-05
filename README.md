# Paste bin 
___
#### Технологии
![C#](https://img.shields.io/badge/-Csharp_11-000278?style=for-the-badge&logo=c-sharp)
![.NET](https://img.shields.io/badge/-.NET_7-000278?style=for-the-badge&logo=.NET)
![NET](https://img.shields.io/badge/-ASP.NET_CORE-000278?style=for-the-badge&logo=.NET)
![REDIS](https://img.shields.io/badge/-Redis_Cache-000278?style=for-the-badge&logo=Redis)
![MSSQLSERVER](https://img.shields.io/badge/-MS_SQL_SERVER-000278?style=for-the-badge&logo=microsoft-sql-server)
![DOCKER](https://img.shields.io/badge/-DOCKER-000278?style=for-the-badge&logo=docker)
___
#### Установка и запуск
```CMD
git clone https://github.com/AxVol/pasteBin.git
cd {путь до папки с клонированным репозиторием}
docker-compose up --build
```
Если в системе отсутствует докер, придется поднять MS SQL Server и если используется Windows то WSL с Ubuntu и установить туда redis, изменить "connection string" в appsettings.json для подключения к БД и подключение в классе RedisService для редис сервера 
___
#### Описание проекта
Paste bin с системой пользователей и возможностью создавать посты, которые имеют короткую и уникальную ссылку, а так же дату их удаления из системы. Если её не установить, он автоматически удалиться через 30 дней, так же их комментировать и лайкать. Популярные же метаданные хранятся в редис кэше для быстрого доступа к ним обновляя его каждый день, для этого был написан сервис, который каждые 24 часа из кэша записывает данные в БД и обнавляет их в нем, так же этот сервис отвечает за защиту от накртуки просмотров запоминая что смотрел пользователей в течении 24-х часов, по истечению времени он очищает БД от просмотров чтобы её не засорять, для взаимодействия с базой данных использовался Entity Framework 