# MaFileTool
[English Version](README.en.md) | [Русская Версия](README.ru.md)

What is it:
- This software is perfect for automatically linking Steam Guard to your accounts and creating mafiles.

Abilities:
- <div>Automatic captcha solution via the rucaptcha/2captcha service - <a href="https://rucaptcha.com/?from=947328" target="_blank">registration</a></div>
- Using 5 sms services:
- GetSms - <a href="https://getsms.online/en/reg.html" target="_blank">registration</a></div>
- GiveSms - <a href="https://give-sms.com/?ref=14040" target="_blank">registration</a></div>
- OnlineSim - <a href="https://onlinesim.io/?ref=40882" target="_blank">registration</a></div>
- SmsActivate - <a href="https://sms-activate.org/?ref=431207" target="_blank">registration</a></div>
- VakSms - <a href="https://vak-sms.com/accounts/registration/" target="_blank">registration</a></div>
- Automatic transfer to another service if you run out of numbers or balance
- Setting priorities
- The ability to change the domain of the SMS service when blocked

Installation:

To configure it, you need to specify your data in the Settings.json file.

All the settings are intuitive, but just in case, I have described everything below.

- Mode - Mode of operation. Accepts one of two values TXT or EXCEL
 
 EXCEL: In the folder with the program is Steam.xlsx . Just fill in your data (login, password, email, emailpassword) according to the available formatting.
 The phone number and recovery code will be recorded in the appropriate cells.
 
 TXT: In the folder with the program is Steam.txt . The data format is login:password:email:emailpassword. For correct operation, passwords must not contain the colon character ':'.
 After successful linking, the data from the account will be saved in result.log
 The data in result.log is saved regardless of the selected mode!
- BindingTimeout - The delay between linking accounts in minutes (by default 1 min.)
- SMSTimeout - The time of the SMS code in minutes (by default 1 min.)
- CaptchaApiKey - apikey of <a href="https://rucaptcha.com/?from=947328" target="_blank">rucaptcha/2captcha</a></div>
- GetSmsApiKey - apikey of <a href="https://getsms.online/ru/reg.html" target="_blank">GetSms</a></div>
- GiveSmsApiKey - apikey of <a href="https://give-sms.com/?ref=14040" target="_blank">GiveSms</a></div>
- OnlineSimApiKey - apikey of <a href="https://onlinesim.io/?ref=40882" target="_blank">OnlineSim</a></div>
- SmsActivateApiKey - apikey of <a href="https://sms-activate.org/?ref=431207" target="_blank">SmsActivate</a></div>
- VakSmsApiKey - apikey of <a href="https://vak-sms.com/accounts/registration/" target="_blank">VakSms</a></div>
- GetSmsBaseUrl - GetSms domain. Change it only if it doesn't work.
- GiveSmsBaseUrl - GiveSms domain. Change it only if it doesn't work.
- OnlineSimBaseUrl - OnlineSim domain. Change it only if it doesn't work.
- SmsActivateBaseUrl - SmsActivate domain. Change it only if it doesn't work.
- VakSmsBaseUrl - the VakSms domain. Change it only if it doesn't work.
- Priority - a system for prioritizing the use of SMS services.

For example, ["GetSms", "GiveSms", "OnlineSim", "SmsActivate", "VakSms"] is set by default - this means that GetSms will be used first, GiveSms second, etc.
You can put for example ["OnlineSim", "SmsActivate", "VakSms"] - OnlineSim will be used first, SmsActivate second, etc. The removed services will not be used.
The transition to the next service occurs if the numbers or balance have run out.
If you do not plan to use any service, you can not specify its api key.
- Mail Server - Incoming mail server. For example box.steamail.pro
- Mail Port - Server port.
- Mail Protocol - IMAP or POP3 protocol. Use with POP3 has not been tested.

Countries:

Now you can specify the country of the number when ordering.
To do this, it is enough to insert the country ID in the field of the corresponding SMS service.
For example ID of Russia - 0, Ukraine - 1, USA - 187. Before inserting the id, check the country support in the sms service.
A list of all countries for each individual service is attached below:

- GetSmsCountry - https://drive.google.com/file/d/1KeCSBXhrN4agG-tWZCoI9OVqeCowugX_/view?usp=sharing
- GiveSmsCountry - https://give-sms.com/api.html#countrylist
- OnlineSimCountry - ID from => https://drive.google.com/file/d/14Lj0h-Uo41ykhhFUgRhv9kCeOeHzagM1/view?usp=sharing
- SmsActivateCountry - ID from => https://drive.google.com/file/d/14Lj0h-Uo41ykhhFUgRhv9kCeOeHzagM1/view?usp=sharing
- VakSmsCountry - ID from => https://drive.google.com/file/d/14Lj0h-Uo41ykhhFUgRhv9kCeOeHzagM1/view?usp=sharing

Limits:
- Steam gives 4 attempts per week to link the Steam Guard number
- You can link one phone number to many accounts, but then you need to wait 15 minutes between linking.

Errors/bugs:
- IP Banned - You have made too many failed/successful login attempts. Change the ip or alternatively set the BindingTimeout value for 4-5 minutes.
- Steam GeneralFailure :( - Perhaps the most infuriating error. Approximately 5% of accounts will have this error.
 Occurs when the steam servers do not respond /the steam returns an incorrect status code / the steam does not like the account.
 If it occurred at the authorization stage, change the ip address.
 Sometimes occurs after entering a captcha, even if entered correctly. The reasons are unknown.
 If it occurs after accepting an email, it is best to postpone this account for a week.
 We don't know why this is happening - the same thing will happen if you link through a regular SDA and even through the official Steam application.
 Occurs after 4 unsuccessful attempts to link a phone number. After that, you need to wait a week to try again.
