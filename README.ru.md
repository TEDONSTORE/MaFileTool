# MaFileTool
[English Version](README.md) | [Русская Версия](README.ru.md)

Зачем это нужно:
- Данный софт отлично подойдёт для автоматической привязки Steam Guard к Вашим аккаунтам и создания мафайлов.

Возможности:
- <div>Автоматическое решение капчи через сервис rucaptcha/2capthca -  <a href="https://rucaptcha.com/?from=947328" target="_blank">регистрация</a></div>
- Использование 5 смс сервисов:
- GetSms - <a href="https://getsms.online/ru/reg.html" target="_blank">регистрация</a></div>
- GiveSms - <a href="https://give-sms.com/?ref=14040" target="_blank">регистрация</a></div>
- OnlineSim - <a href="https://onlinesim.io/?ref=40882" target="_blank">регистрация</a></div>
- SmsActivate - <a href="https://sms-activate.org/?ref=431207" target="_blank">регистрация</a></div>
- VakSms - <a href="https://vak-sms.com/accounts/registration/" target="_blank">регистрация</a></div>
- Автоматический переход на другой сервис если закончились номера или баланс
- Задание приоритетов
- Возможность изменить домен смс сервиса при блокировке РКН

Настройка:

Для настройки необходимо указать Ваши данные в файле Settings.json

Все настройки интуитивно понятны, но на всякий случай я расписал все ниже.
- Mode - Режим работы. Принимает одно из двух значений TXT или EXCEL
  EXCEL: В папке с программой лежит Steam.xlsx. Просто заполните свои данные (login, password, email, emailpassword) в соответствии с имеющимся форматированием.
  Номер телефона и код восстановления будут записываться в соответствующие ячейки.
  TXT: В папке с программой лежит Steam.txt. Формат данных - login:password:email:emailpassword. Для корректной работы пароли не должны содержать символ двоеточия ':'.
  После успешной привязки данные от аккаунта будут сохранены в result.log
  Данные в result.log сохраняются внезависимости от выбранного режима!
- BindingTimeout - Задержка между привязкой аккаунтов в минутах (по умолчанию 1 мин.)
- SMSTimeout - Время ожадания СМС кода в минутах (по умолчанию 1 мин.)
- CaptchaApiKey - api ключ сервиса <a href="https://rucaptcha.com/?from=947328" target="_blank">rucaptcha/2captcha</a></div>
- GetSmsApiKey - api ключ сервиса <a href="https://getsms.online/ru/reg.html" target="_blank">GetSms</a></div>
- GiveSmsApiKey - api ключ сервиса <a href="https://give-sms.com/?ref=14040" target="_blank">GiveSms</a></div>
- OnlineSimApiKey - api ключ сервиса <a href="https://onlinesim.io/?ref=40882" target="_blank">OnlineSim</a></div>
- SmsActivateApiKey - api ключ сервиса <a href="https://sms-activate.org/?ref=431207" target="_blank">SmsActivate</a></div>
- VakSmsApiKey - api ключ сервиса <a href="https://vak-sms.com/accounts/registration/" target="_blank">VakSms</a></div>
- GetSmsBaseUrl - домен GetSms. Меняйте только если не работает.
- GiveSmsBaseUrl - домен GiveSms. Меняйте только если не работает.
- OnlineSimBaseUrl - домен OnlineSim. Меняйте только если не работает.
- SmsActivateBaseUrl - домен SmsActivate. Меняйте только если не работает.
- VakSmsBaseUrl - домен VakSms. Меняйте только если не работает.
- Priority - система задания очередности использования смс сервисов.

Например по умолчанию установлено ["GetSms", "GiveSms", "OnlineSim", "SmsActivate", "VakSms"] - это значит, что первым будет использоваться GetSms, вторым GiveSms и т.д.
Можно поставить например ["OnlineSim", "SmsActivate", "VakSms"] - первым будет использоваться OnlineSim, вторым SmsActivate и т.д. Убранные сервисы использоваться не будут.
Переход на следующий сервис происходит если закончились номера или баланс.
Если вы не планируете пользваться каким либо сервисом - можно не указывать его api ключ.

- MailServer - Сервер входящей почты. Например box.steamail.pro
- MailPort - Порт сервера.
- MailProtocol - Протокол IMAP или POP3. Использование с POP3 не тестировалось.

Страны:

Теперь вы можете указать страну при заказе номера. Для этого достаточно ввести ID страны в поле соответствующего SMS-сервиса. Например, идентификатор России - 0, Украины - 1, США - 187. Перед добавлением ID убедитесь, что выбранный СМС сервис поддерживает работу с выбранной страной. Список ID всех стран для каждого отдельного SMS сервиса прилагается ниже:
- GetSmsCountry - https://drive.google.com/file/d/1KeCSBXhrN4agG-tWZCoI9OVqeCowugX_/view?usp=sharing
- GiveSmsCountry - https://give-sms.com/api.html#countrylist
- OnlineSimCountry - ID from => https://drive.google.com/file/d/14Lj0h-Uo41ykhhFUgRhv9kCeOeHzagM1/view?usp=sharing
- SmsActivateCountry - ID from => https://drive.google.com/file/d/14Lj0h-Uo41ykhhFUgRhv9kCeOeHzagM1/view?usp=sharing
- VakSmsCountry - ID from => https://drive.google.com/file/d/14Lj0h-Uo41ykhhFUgRhv9kCeOeHzagM1/view?usp=sharing

Лимиты:
- Steam дает 4 попытки в неделю на привязку номера Steam Guard
- Вы можете привязать один номер телефона ко многим аккаунтам, но тогда между привязками нужно будет подождать 15 минут.

Ошибки/баги:
- IP Banned - Вы сделали слишком много неудачных/удачных попыток входа. Смените ip или в качестве альтернативы выставить значение BindingTimeout на 4-5 мин.
- Steam GeneralFailure :( - Пожалуй самая бесячая ошибка. Примерно 5% аккаунтов будут с такой ошибкой.
  Возникает когда сервера стима не отвечают/стим возвращает неверный статус код/стиму не нравится аккаунт.
  Если возникла на этапе авторизации - смените ip адрес.
  Иногда возникает после ввода капчи, даже если ввести правильно. Причины неизвестны.
  Если возникает после принятия email - лучше всего отложить этот аккаунт на неделю.
  Мы не знаем из за чего это происходит - тоже самое будет если привязывать через обычный SDA и даже через официальное приложение Steam.
  Возникает после 4 неудачных попыток привязки номера телефона. После чего нужно ждать неделю чтобы попробовать снова.
