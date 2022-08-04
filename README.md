# massNickChangerBot
Меняет никнейм всех пользователей на сервере. 


# **Команды**
> /никнейм-изменить-всем НИКНЕЙМ
- Меняет всем пользователям на сервере никнейм на указанный.
-- Не может имзенить никнейм, если: у бота нет прав, бот находится по иерархии ниже или равен той, что у пользователя, которому меняют

> /никнейм-менять-всегда ДА / НЕТ
- Если включить, то при любом изменении пользователя на сервере (никнейма, ролей, аватара) - бот будет проверять, является ли никнейм этого пользователя тем, что был введен командой ранее, если это не так - он его изменит. По сути, уибрает возможность на изменение никнеймов.

> /остановить-изменение
- Отключает массовое изменение никнеймов (функция менять всегда останется в прежнем положении)


# Как добавить бота:
1. Переходим в https://discord.com/developers/applications (при необходимости авторизуемся через наш дискорд аккаунт)
2. Жмем на New Application
3. Пишем любое имя для будущего бота, жмем Create
4. Переходим на вкладку Bot и жмем на Add Bot
5. Жмем Reset Token и копируем полученный токен
6. Листаем ниже и включаем SERVER MEMBERS INTENT
7. Скачиваем https://github.com/Paladic/massNickChangerBot/releases/download/realese/massNickChangerBot.rar
8. Разахивируем в удобное нам место
9. Открываем файлик appsettings.json
10. Вместо YOUR_TOKEN вставляем скопированный ранее токен
11. Возвращаемся на сайт где ранее создавали бота
12. Переходим в OAuth2 > URL Generator
13. Ставим галочки > **SCOPES:** bot, applications.commands | **BOT PERMISSIONS:** Administrator
14. Копируем ссылку ниже и вставляем в браузер
15. Выбираем нужный нам сервер и добавляем бота туда
16. Запускаем NicknameChangerBot.exe
17. Ждем от 1 минуты до часу пока Discord добавит команды (обычно это происходит за 2-3 минуты, но иногда бывают задержки)
18. Пользуемся!
