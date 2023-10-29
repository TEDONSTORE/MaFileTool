using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Pop3;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using maFileTool.Model;
using maFileTool.Services.Api;
using System.Collections.Specialized;
using maFileTool.Services;
using maFileTool.Services.SteamAuth;
using SteamKit2.Authentication;
using SteamKit2;
using SteamKit2.Internal;

namespace maFileTool.Core
{
    public class Worker
    {
        public static Settings settings = JsonConvert.DeserializeObject<Settings>(System.IO.File.ReadAllText(String.Format("{0}\\Settings.json", Environment.CurrentDirectory)));

        private readonly string _login;
        private readonly string _password;
        private readonly string _emailLogin;
        private readonly string _emailPassword;

        private string _activationId = String.Empty;
        private string _phoneNumber = String.Empty;
        private string _revocationCode = String.Empty;

        private int priorityCounter = 0;
        private bool emailVerify = true;

        public static Worker Instance;

        public Worker(string login, string password, string emailLogin, string emailPassword)
        {
            Instance = this;

            _login = login;
            _password = password;
            _emailLogin = emailLogin;
            _emailPassword = emailPassword;
        }

        public void DoWork()
        {
            #region Checks

            if (priorityCounter >= settings.Priority.Count()) 
            {
                Log("It looks like the available sms services are over. Try again later.");
                Program.quit = true;
                return;
            }

            string SmsService = settings.Priority.ElementAt(priorityCounter);

            if (String.IsNullOrEmpty(SmsService) || String.IsNullOrWhiteSpace(SmsService))
            {
                Log("It looks like no SMS service is set in the priorities.");
                Program.quit = true;
                return;
            }

            emailVerify = true;

            SmsService smsService = new SmsService("");

            switch (SmsService)
            {
                case "GetSms":
                    smsService = new SmsService(settings.GetSmsApiKey);
                    
                    if (String.IsNullOrEmpty(settings.GetSmsApiKey) || String.IsNullOrWhiteSpace(settings.GetSmsApiKey)) 
                    {
                        Log("GetSms apikey is not set! Specify the apikey in Settings.json");
                        priorityCounter++;
                        DoWork();
                        return;
                    }
                    smsService.BaseUrl = settings.GetSmsBaseUrl; //А вдруг
                    smsService.Country = settings.GetSmsCountry;
                    if (String.IsNullOrEmpty(smsService.BaseUrl) || String.IsNullOrWhiteSpace(smsService.BaseUrl)) smsService.BaseUrl = "getsms.online";
                    if (String.IsNullOrEmpty(smsService.Country) || String.IsNullOrWhiteSpace(smsService.Country)) smsService.Country = "or";
                    break;
                case "SmsActivationService":
                    smsService = new SmsService(settings.SmsActivationServiceApiKey);

                    if (String.IsNullOrEmpty(settings.SmsActivationServiceApiKey) || String.IsNullOrWhiteSpace(settings.SmsActivationServiceApiKey))
                    {
                        Log("SmsActivationService apikey is not set! Specify the apikey in Settings.json");
                        priorityCounter++;
                        DoWork();
                        return;
                    }
                    smsService.BaseUrl = settings.SmsActivationServiceBaseUrl; //А вдруг
                    smsService.Country = settings.SmsActivationServiceCountry;
                    if (String.IsNullOrEmpty(smsService.BaseUrl) || String.IsNullOrWhiteSpace(smsService.BaseUrl)) smsService.BaseUrl = "sms-activation-service.com";
                    if (String.IsNullOrEmpty(smsService.Country) || String.IsNullOrWhiteSpace(smsService.Country)) smsService.Country = "0";
                    break;
                case "FiveSim":
                    smsService = new SmsService(settings.FiveSimApiKey);

                    if (String.IsNullOrEmpty(settings.FiveSimApiKey) || String.IsNullOrWhiteSpace(settings.FiveSimApiKey))
                    {
                        Log("FiveSim apikey is not set! Specify the apikey in Settings.json");
                        priorityCounter++;
                        DoWork();
                        return;
                    }
                    smsService.BaseUrl = settings.FiveSimBaseUrl; //А вдруг
                    smsService.Country = settings.FiveSimCountry;
                    if (String.IsNullOrEmpty(smsService.BaseUrl) || String.IsNullOrWhiteSpace(smsService.BaseUrl)) smsService.BaseUrl = "5sim.biz";
                    if (String.IsNullOrEmpty(smsService.Country) || String.IsNullOrWhiteSpace(smsService.Country)) smsService.Country = "0";
                    break;
                case "GiveSms":
                    smsService = new SmsService(settings.GiveSmsApiKey);
                    if (String.IsNullOrEmpty(settings.GiveSmsApiKey) || String.IsNullOrWhiteSpace(settings.GiveSmsApiKey))
                    {
                        Log("GiveSms apikey is not set! Specify the apikey in Settings.json");
                        priorityCounter++;
                        DoWork();
                        return;
                    }
                    smsService.BaseUrl = settings.GiveSmsBaseUrl;
                    smsService.Country = settings.GiveSmsCountry;
                    if (String.IsNullOrEmpty(smsService.BaseUrl) || String.IsNullOrWhiteSpace(smsService.BaseUrl)) smsService.BaseUrl = "give-sms.com";
                    if (String.IsNullOrEmpty(smsService.Country) || String.IsNullOrWhiteSpace(smsService.Country)) smsService.Country = "0";
                    break;
                case "OnlineSim":
                    smsService = new SmsService(settings.OnlineSimApiKey);
                    if (String.IsNullOrEmpty(settings.OnlineSimApiKey) || String.IsNullOrWhiteSpace(settings.OnlineSimApiKey))
                    {
                        Log("OnlineSim apikey is not set! Specify the apikey in Settings.json");
                        priorityCounter++;
                        DoWork();
                        return;
                    }
                    smsService.BaseUrl = settings.OnlineSimBaseUrl;
                    smsService.Country = settings.OnlineSimCountry;
                    if (String.IsNullOrEmpty(smsService.BaseUrl) || String.IsNullOrWhiteSpace(smsService.BaseUrl)) smsService.BaseUrl = "onlinesim.io";
                    if (String.IsNullOrEmpty(smsService.Country) || String.IsNullOrWhiteSpace(smsService.Country)) smsService.Country = "0";
                    break;
                case "SmsActivate":
                    smsService = new SmsService(settings.SmsActivateApiKey);
                    if (String.IsNullOrEmpty(settings.SmsActivateApiKey) || String.IsNullOrWhiteSpace(settings.SmsActivateApiKey))
                    {
                        Log("SmsActivate apikey is not set! Specify the apikey in Settings.json");
                        priorityCounter++;
                        DoWork();
                        return;
                    }
                    smsService.BaseUrl = settings.SmsActivateBaseUrl;
                    smsService.Country = settings.SmsActivateCountry;
                    if (String.IsNullOrEmpty(smsService.BaseUrl) || String.IsNullOrWhiteSpace(smsService.BaseUrl)) smsService.BaseUrl = "sms-activate.org";
                    if (String.IsNullOrEmpty(smsService.Country) || String.IsNullOrWhiteSpace(smsService.Country)) smsService.Country = "0";
                    break;
                case "VakSms":
                    smsService = new SmsService(settings.VakSmsApiKey);
                    if (String.IsNullOrEmpty(settings.VakSmsApiKey) || String.IsNullOrWhiteSpace(settings.VakSmsApiKey))
                    {
                        Log("VakSms apikey is not set! Specify the apikey in Settings.json");
                        priorityCounter++;
                        DoWork();
                        return;
                    }
                    smsService.BaseUrl = settings.VakSmsBaseUrl;
                    smsService.Country = settings.VakSmsCountry;
                    if (String.IsNullOrEmpty(smsService.BaseUrl) || String.IsNullOrWhiteSpace(smsService.BaseUrl)) smsService.BaseUrl = "vak-sms.com";
                    if (String.IsNullOrEmpty(smsService.Country) || String.IsNullOrWhiteSpace(smsService.Country)) smsService.Country = "0";
                    break;
            }

            #endregion 

            try
            {
                Log(String.Format("Balance {0} - {1}", smsService.BaseUrl, smsService.Balance().Result));

                #region Authorization

                string username = _login;
                string password = _password;

                // Start a new SteamClient instance
                SteamClient steamClient = new SteamClient();

                // Connect to Steam
                steamClient.Connect();

                // Really basic way to wait until Steam is connected
                while (!steamClient.IsConnected)
                    Thread.Sleep(500);

                // Create a new auth session
                CredentialsAuthSession authSession;

                try
                {
                    AuthSessionDetails authSessionDetails = new AuthSessionDetails();
                    authSessionDetails.Username = username;
                    authSessionDetails.Password = password;
                    authSessionDetails.IsPersistentSession = false;
                    authSessionDetails.PlatformType = EAuthTokenPlatformType.k_EAuthTokenPlatformType_MobileApp;
                    authSessionDetails.ClientOSType = EOSType.Android9;
                    authSessionDetails.Authenticator = new UserFormAuthenticator(new SteamGuardAccount());

                    authSession = AsyncHelpers.RunSync<CredentialsAuthSession>(() => steamClient.Authentication.BeginAuthSessionViaCredentialsAsync(authSessionDetails));
                }
                catch (Exception ex)
                {
                    Log("Steam Login Error");
                    return;
                }

                // Starting polling Steam for authentication response
                AuthPollResult pollResponse;
                try
                {
                    pollResponse = AsyncHelpers.RunSync<AuthPollResult>(() => authSession.PollingWaitForResultAsync());
                }
                catch (Exception ex)
                {
                    Log("Steam Login Error 2");
                    return;
                }

                // Build a SessionData object
                SessionData sessionData = new SessionData()
                {
                    SteamID = authSession.SteamID.ConvertToUInt64(),
                    AccessToken = pollResponse.AccessToken,
                    RefreshToken = pollResponse.RefreshToken,
                };

                sessionData.SessionID = sessionData.GetCookies().GetCookies(new Uri("http://steamcommunity.com")).Cast<Cookie>().First(c => c.Name == "sessionid").Value;

                //Login succeeded
                //this.Session = sessionData;

                #endregion

                Log("Steam account login succeeded.");

                #region Getting Number
                
                while (true)
                {
                    string result = smsService.GetNumber(smsService.Country).Result;

                    if (result == "NO_MEANS" || result == "NO_BALANCE")
                    {
                        if (settings.Priority.Last() == SmsService) Program.quit = true;
                        Log("The balance of the SMS service has ended.");
                        Log("Sleep 1 min before switching to the next service.");
                        System.Threading.Thread.Sleep(60 * 1000);
                        priorityCounter++;
                        DoWork();
                        return;
                    }
                    else if (result == "NO_NUMBER" || result == "NO_NUMBERS")
                    {
                        if (settings.Priority.Last() == SmsService) Program.quit = true;
                        Log("The SMS service numbers out of stock.");
                        Log("Sleep 1 min before switching to the next service.");
                        System.Threading.Thread.Sleep(60 * 1000);
                        priorityCounter++;
                        DoWork();
                        return;
                    }
                    else if (string.IsNullOrEmpty(result) || string.IsNullOrWhiteSpace(result))
                    {
                        if (settings.Priority.Last() == SmsService) Program.quit = true;
                        Log("Unknown error.");
                        Log("Sleep 1 min before switching to the next service.");
                        System.Threading.Thread.Sleep(60 * 1000);
                        priorityCounter++;
                        DoWork();
                        return;
                    }
                    else
                    {
                        _activationId = result.Split(':')[1];
                        _phoneNumber = result.Split(':')[2];
                        Log(String.Format("Got a number {0}, ActivationId {1}", _phoneNumber, _activationId));
                    }

                    string is_valid = PhoneValidate(sessionData);

                    if (is_valid == "valid") break;
                    else if (is_valid == "invalid")
                    {
                        Log(String.Format("Bad number {0}", _phoneNumber));
                        smsService.SetStatus(_activationId, "10"); //Уведомление, что номер уже занят
                        continue;
                    }
                    else
                    {
                        Program.quit = true;
                        break;
                    }
                }

                if (Program.quit) return;

                #endregion

                // Begin linking mobile authenticator
                AuthenticatorLinker linker = new AuthenticatorLinker(sessionData);

                AuthenticatorLinker.LinkResult linkResponse = AuthenticatorLinker.LinkResult.GeneralFailure;
                while (linkResponse != AuthenticatorLinker.LinkResult.AwaitingFinalization)
                {
                    try
                    {
                        linkResponse = AsyncHelpers.RunSync<AuthenticatorLinker.LinkResult>(() => linker.AddAuthenticator());
                    }
                    catch (Exception ex)
                    {
                        Log($"Error adding your authenticator: {ex.Message}");
                        return;
                    }

                    switch (linkResponse)
                    {
                        case AuthenticatorLinker.LinkResult.MustProvidePhoneNumber:
                            _phoneNumber = FilterPhoneNumber(_phoneNumber);
                            if (!PhoneNumberOkay(_phoneNumber)) return;
                            linker.PhoneNumber = _phoneNumber;
                            break;

                        case AuthenticatorLinker.LinkResult.AuthenticatorPresent:
                            Log("This account already has an authenticator linked. You must remove that authenticator to add SDA as your authenticator.");
                            return;

                        case AuthenticatorLinker.LinkResult.FailureAddingPhone:
                            Log("Failed to add your phone number. Please try again or use a different phone number.");
                            linker.PhoneNumber = null;
                            break;

                        case AuthenticatorLinker.LinkResult.MustRemovePhoneNumber:
                            linker.PhoneNumber = null;
                            break;

                        case AuthenticatorLinker.LinkResult.MustConfirmEmail:
                            Log(String.Format("Number {0} accepted, waiting email from steam.", _phoneNumber));
                            ConfirmEmailForAdd(settings.MailServer, Int32.Parse(settings.MailPort));
                            if (!emailVerify) return;
                            break;

                        case AuthenticatorLinker.LinkResult.GeneralFailure:
                            Log("Error adding your authenticator.");
                            SaveAccountData();
                            return;
                    }
                } // End while loop checking for AwaitingFinalization

                smsService.SetStatus(_activationId, "1"); //Уведомление, что SMS отправлена

                var finalizeResponse = AuthenticatorLinker.FinalizeResult.GeneralFailure;
                while (finalizeResponse != AuthenticatorLinker.FinalizeResult.Success)
                {
                    Dictionary<string, int> waitOptions = new Dictionary<string, int>();
                    waitOptions.Add("timeout", Int32.Parse(settings.SMSTimeout) * 60);
                    waitOptions.Add("pollingInterval", 5);
                    var smsCode = smsService.WaitForResult(_activationId, _login, waitOptions).Result;

                    if (string.IsNullOrEmpty(smsCode))
                    {
                        Log("SMS not received");
                        smsService.SetStatus(_activationId, "-1"); //Отмена активации
                        SaveAccountData();
                        return;
                    }

                    Log(String.Format("Received SMS code {0}", smsCode));
                    finalizeResponse = AsyncHelpers.RunSync<AuthenticatorLinker.FinalizeResult>(() => linker.FinalizeAddAuthenticator(smsCode));

                    switch (finalizeResponse)
                    {
                        case AuthenticatorLinker.FinalizeResult.BadSMSCode:
                            Log("SMS code incorrect");
                            return;

                        case AuthenticatorLinker.FinalizeResult.UnableToGenerateCorrectCodes:
                            Log("Unable to generate the proper codes to finalize this authenticator. The authenticator should not have been linked. In the off-chance it was, please write down your revocation code, as this is the last chance to see it: " + linker.LinkedAccount.RevocationCode);
                            return;

                        case AuthenticatorLinker.FinalizeResult.GeneralFailure:
                            Log("Steam GeneralFailture :(");
                            SaveAccountData();
                            return;
                    }
                }

                _revocationCode = linker.LinkedAccount.RevocationCode;

                SaveAccount(linker.LinkedAccount);
                Log($"{_login}:{_password}:{_emailLogin}:{_emailPassword}:{_phoneNumber}:{linker.LinkedAccount.RevocationCode}");
                LogToFile($"{_login}:{_password}:{_emailLogin}:{_emailPassword}:{_phoneNumber}:{linker.LinkedAccount.RevocationCode}");

                smsService.SetStatus(_activationId, "6"); //Код верный, завершение активации
                SaveAccountData(_phoneNumber, _revocationCode);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public bool PhoneNumberOkay(string phoneNumber)
        {
            if (phoneNumber == null || phoneNumber.Length == 0) return false;
            if (phoneNumber[0] != '+') return false;
            return true;
        }

        private string PhoneValidate(SessionData session)
        {
            string url = "https://store.steampowered.com/phone/validate";

            NameValueCollection data = new NameValueCollection();
            data.Add("sessionID", session.SessionID);

            if (_phoneNumber.Contains("+"))
                data.Add("phoneNumber", _phoneNumber);
            else
                data.Add("phoneNumber", String.Format("+{0}", _phoneNumber));

            NameValueCollection headers = new NameValueCollection();
            headers.Add("Accept-Language", "en-US,en;q=0.7");
            headers.Add("Accept-Encoding", "gzip, deflate, br");
            headers.Add("X-Requested-With", "XMLHttpRequest");
            headers.Add("Origin", "https://store.steampowered.com");
            headers.Add("Upgrade-Insecure-Requests", "1");

            CookieContainer cookies = new CookieContainer();
            cookies.Add(new Cookie("sessionid", session.SessionID, "/", ".steampowered.com"));
            cookies.Add(new Cookie("steamLoginSecure", session.GetSteamLoginSecure(), "/", ".steampowered.com"));

            try
            {
                string response = AsyncHelpers.RunSync<string>(() => SteamWeb.POSTRequest(url, cookies, data));
                if (response.Contains("DOCTYPE"))
                {
                    response = AsyncHelpers.RunSync<string>(() => SteamWeb.POSTRequest(url, cookies, data));
                }

                JObject obj = JObject.Parse(response);
                string success = obj["success"].ToString();
                string is_valid = obj["is_valid"].ToString();

                if (success == "True")
                {
                    if (is_valid == "True")
                    {
                        return "valid";
                    }
                    else
                    {
                        return "invalid";
                    }
                }
                else
                {
                    return "error";
                }
            }
            catch (Exception e)
            {
                return "error";
            }
        }

        public void SaveToExcel(string phone, string revocationCode)
        {
            Account ac = Program.accounts.First(t => t.Login == _login);
            ac.Phone = phone;
            ac.RevocationCode = revocationCode;
            Excel excel = new Excel();
            bool showed = false;
            int ext = 0;
            while (true)
            {
                try
                {
                    excel.WriteRowToExcel(Program.steam, ac, Int32.Parse(ac.Id));
                    break;
                }
                catch (InvalidOperationException)
                {
                    if (!showed)
                    {
                        Log("Couldn't access excel file. Please close excel.");
                        Log("Please close all excel processes for further work!");
                        showed = true;
                    }
                    ext++;
                }
                if (ext >= 60) 
                {
                    Log("The error occurred because excel blocked the file.");
                    Program.quit = true;
                    break;
                }
                Thread.Sleep(1 * 1000);
            }
        }

        public string GetLoginCodeFromEmail(string host, int port)
        {
            Thread.Sleep(10000);

            var loginCode = string.Empty;

            if (settings.MailProtocol.ToLower() == "imap")
            {
                using (var client = new ImapClient())
                {
                    client.CheckCertificateRevocation = false;
                    try
                    {
                        client.Connect(host, port, Convert.ToBoolean(settings.UseSSL.ToLower()));
                        client.Authenticate(_emailLogin, _emailPassword);
                    }
                    catch (MailKit.Security.AuthenticationException ex)
                    {
                        Log($"Email error => {ex.Message.ToString()}");
                        emailVerify = false;
                        return loginCode;
                    }
                    catch (MailKit.Security.SslHandshakeException ex)
                    {
                        Log($"Email error => {ex.Message.ToString()}");
                        emailVerify = false;
                        return loginCode;
                    }

                    var inbox = client.Inbox;
                    inbox.Open(FolderAccess.ReadOnly);

                    for (var i = inbox.Count - 1; i >= 0; i--)
                    {
                        var message = inbox.GetMessage(i);

                        var code = Regex.Match(message.HtmlBody, "class=([\"])title-48 c-blue1 fw-b a-center([^>]+)([>])([^<]+)").Groups[4].Value;
                        if (string.IsNullOrEmpty(code)) continue;

                        loginCode = code.Trim();
                        Log($"Login code: {loginCode}");
                        break;
                    }
                }
            }
            else if (settings.MailProtocol.ToLower() == "pop3")
            {
                //Not tested
                using (var client = new Pop3Client()) 
                {
                    client.CheckCertificateRevocation = false;
                    try
                    {
                        client.Connect(host, port, Convert.ToBoolean(settings.UseSSL.ToLower()));
                        client.Authenticate(_emailLogin, _emailPassword);
                    }
                    catch (MailKit.Security.AuthenticationException ex)
                    {
                        Log($"Email error => {ex.Message.ToString()}");
                        emailVerify = false;
                        return loginCode;
                    }
                    catch (MailKit.Security.SslHandshakeException ex)
                    {
                        Log($"Email error => {ex.Message.ToString()}");
                        emailVerify = false;
                        return loginCode;
                    }

                    int count = client.GetMessageCount();
                    for (int i = count - 1; i >= 0; i--) 
                    {
                        var message = client.GetMessage(i);
                        if (message.Subject.Contains("store.steampowered.com")) 
                        {
                            var code = Regex.Match(message.HtmlBody, "class=([\"])title-48 c-blue1 fw-b a-center([^>]+)([>])([^<]+)").Groups[4].Value;
                            if (string.IsNullOrEmpty(code)) continue;

                            loginCode = code.Trim();
                            Log($"Login code: {loginCode}");
                            client.Disconnect(true);
                            break;
                        }
                    }
                }
            }

            return loginCode;
        }

        private void ConfirmEmailForAdd(string host, int port)
        {
            Thread.Sleep(10000);

            if (settings.MailProtocol.ToLower() == "imap")
            {
                using (var client = new ImapClient())
                {
                    client.CheckCertificateRevocation = false;
                    try
                    {
                        client.Connect(host, port, Convert.ToBoolean(settings.UseSSL.ToLower()));
                        client.Authenticate(_emailLogin, _emailPassword);
                    }
                    catch (MailKit.Security.AuthenticationException ex)
                    {
                        Log($"Email error => {ex.Message.ToString()}");
                        emailVerify = false;
                        return;
                    }
                    catch (MailKit.Security.SslHandshakeException ex) 
                    {
                        Log($"Email error => {ex.Message.ToString()}");
                        emailVerify = false;
                        return;
                    }

                    var inbox = client.Inbox;
                    inbox.Open(FolderAccess.ReadOnly);

                    for (var i = inbox.Count - 1; i >= 0; i--)
                    {
                        var message = inbox.GetMessage(i);
                        var link = Regex.Match(message.HtmlBody, "store([.])steampowered([.])com([\\/])phone([\\/])ConfirmEmailForAdd([?])stoken=([^\"]+)").Groups[0].Value;
                        if (string.IsNullOrEmpty(link)) continue;

                        WebClient webClient = new WebClient();
                        string s = webClient.DownloadString("https://" + link);
                        
                        //Unable to add phone
                        //There have been too many attempts to add a phone number to this account. Please only add phones that you own and try again in 1 week.

                        if (s.Contains("1 week")) 
                        {
                            emailVerify = false;
                            string time = DateTime.Now.AddDays(7).ToString("dd.MM.yy HH:mm");
                            SaveAccountData(string.Empty, string.Empty, time);
                            Log("Too many attempts to add a phone number to this account. Please try again in 1 week.");
                            break;
                        }

                        Log("Email confirmed.");
                        break;
                    }

                    client.Disconnect(true);
                }
            }
            else if (settings.MailProtocol.ToLower() == "pop3") 
            {
                //Not tested
                using (var client = new Pop3Client())
                {
                    client.CheckCertificateRevocation = false;
                    try
                    {
                        client.Connect(host, port, Convert.ToBoolean(settings.UseSSL.ToLower()));
                        client.Authenticate(_emailLogin, _emailPassword);
                    }
                    catch (MailKit.Security.AuthenticationException ex)
                    {
                        Log($"Email error => {ex.Message.ToString()}");
                        emailVerify = false;
                        return;
                    }
                    catch (MailKit.Security.SslHandshakeException ex)
                    {
                        Log($"Email error => {ex.Message.ToString()}");
                        emailVerify = false;
                        return;
                    }

                    int count = client.GetMessageCount();
                    for (int i = count - 1; i >= 0; i--)
                    {
                        var message = client.GetMessage(i);
                        if (message.Subject.Contains("store.steampowered.com"))
                        {
                            var link = Regex.Match(message.HtmlBody, "store([.])steampowered([.])com([\\/])phone([\\/])ConfirmEmailForAdd([?])stoken=([^\"]+)").Groups[0].Value;
                            if (string.IsNullOrEmpty(link)) continue;

                            new WebClient().DownloadString("https://" + link);
                            Log("Email confirmed.");
                            client.Disconnect(true);
                            break;
                        }
                    }
                }
            }

            Thread.Sleep(2000);
        }

        private void SaveAccountData(string phoneNumber = "", string revocationCode = "", string week = "")
        {
            string time = DateTime.Now.AddMinutes(15).ToString("dd.MM.yy HH:mm");
            if (!string.IsNullOrEmpty(week)) time = week;

            string mode = settings.Mode.ToLower();
            switch (mode)
            {
                case "txt":
                    List<string> list = File.ReadAllLines(Program.steamtxt).ToList();
                    string line = list.FirstOrDefault(t => t.Contains($"{_login}:"));
                    int number = (list.IndexOf(line) + 1);
                    if (string.IsNullOrEmpty(phoneNumber) && string.IsNullOrEmpty(revocationCode))
                    {
                        line = $"{line.Split(':')[0]}:{line.Split(':')[1]}:{line.Split(':')[2]}:{line.Split(':')[3]}:{time}";
                        LineChanger(line, Program.steamtxt, number);
                    }
                    else 
                    {
                        line = $"{line.Split(':')[0]}:{line.Split(':')[1]}:{line.Split(':')[2]}:{line.Split(':')[3]}:{phoneNumber}:{revocationCode}";
                        LineChanger(line, Program.steamtxt, number);
                    }  
                    break;
                case "excel":
                    if(string.IsNullOrEmpty(phoneNumber) && string.IsNullOrEmpty(revocationCode))
                        SaveToExcel(time, time);
                    else
                        SaveToExcel(phoneNumber, revocationCode);
                    break;
            }
        }

        private static string FilterPhoneNumber(string phoneNumber)
        {
            phoneNumber = phoneNumber.Replace("-", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty);
            if (!phoneNumber.Contains("+") || !phoneNumber.Contains(Uri.EscapeDataString("+")))
                phoneNumber = String.Format("+{0}", phoneNumber);
            return phoneNumber;
        }
        private static void SaveAccount(SteamGuardAccount account)
        {
            var filename = account.Session.SteamID.ToString() + ".maFile";
            var jsonAccount = JsonConvert.SerializeObject(account);
            string path = String.Format("{0}\\maFiles", Environment.CurrentDirectory);
            if (!Directory.Exists(path)) Directory.CreateDirectory("maFiles");
            File.WriteAllText(String.Format("{0}\\{1}", path, filename), jsonAccount);
        }

        public void Log(string message) 
        {
            string time = DateTime.Now.ToString("HH:mm");
            int index = Program.accounts.FindIndex(t => t.Login == _login);
            Console.WriteLine($"[{time}][{_login}][{(index + 1)}/{Program.accounts.Count}] - {message}"); 
        }
        private static void LogToFile(string message) => File.AppendAllText("result.log", message + "\n");

        private void LineChanger(string newText, string fileName, int lineToEdit)
        {
            string[] array = File.ReadAllLines(fileName);
            array[lineToEdit - 1] = newText;
            File.WriteAllLines(fileName, array);
        }
    }
}