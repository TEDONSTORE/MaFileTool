using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Threading;

namespace maFileTool.Services.Api
{
    public class SmsService
    {
        public string BaseUrl { get; set; }

        public string ApiKey { get; set; }

        public string Country { get; set; }

        public int DefaultTimeout { get; set; } = 120;

        public int PollingInterval { get; set; } = 10;

        //Network client
        private ApiClient apiClient;

        //Api constructor
        public SmsService(string key)
        {
            ApiKey = key;
            apiClient = new ApiClient();
        }

        #region BaseLogic
        private long CurrentTime()
        {
            return Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds);
        }

        private string BuildQuery(Dictionary<string, string> parameters)
        {
            string query = "";

            foreach (KeyValuePair<string, string> p in parameters)
            {
                if (query.Length > 0)
                {
                    query += "&";
                }

                query += p.Key + "=" + Uri.EscapeDataString(p.Value);
            }

            return query;
        }

        private async Task<string> Res(string action)
        {
            var parameters = new Dictionary<string, string>();
            parameters["action"] = action;
            return await Res(parameters);
        }

        private async Task<string> Res(Dictionary<string, string> parameters)
        {
            parameters["api_key"] = ApiKey;
            string url = string.Empty;
            if (BaseUrl.Contains("onlinesim"))
                url = String.Format("http://api-conserver.{0}/stubs/handler_api.php?{1}", BaseUrl, BuildQuery(parameters));
            else if (BaseUrl.Contains("getsms") || BaseUrl.Contains("sms-activate"))
                url = String.Format("http://api.{0}/stubs/handler_api.php?{1}", BaseUrl, BuildQuery(parameters));
            else if(BaseUrl.Contains("5sim"))
                url = String.Format("http://api1.{0}/stubs/handler_api.php?{1}", BaseUrl, BuildQuery(parameters));
            else
                url = String.Format("http://{0}/stubs/handler_api.php?{1}", BaseUrl, BuildQuery(parameters));

            return await apiClient.Get(url);
        }

        #endregion

        public async Task<string> Balance()
        {
            string response = await Res("getBalance");
            return response;
        }

        public async Task<string> GetNumbersStatus()
        {
            var parameters = new Dictionary<string, string>();
            parameters["action"] = "getNumbersStatus";

            string response = await Res(parameters);

            var array = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);

            //steam - mt
            string numbers = array.First(t => t.Key.StartsWith("mt")).Value;

            return numbers;
        }

        public async Task<string> GetNumber(string country = "", string forward = "")
        {
            var parameters = new Dictionary<string, string>();
            parameters["action"] = "getNumber";

            if (BaseUrl.Contains("getsms"))
                parameters["service"] = "sm"; //getsms - sm
            if (BaseUrl.Contains("sms-activation-service"))
            {
                parameters["service"] = "mt"; //sms-activation-service - mt
                parameters["lang"] = "ru";
            }
            if (BaseUrl.Contains("5sim"))
                parameters["service"] = "mt"; //give-sms - mt
            if (BaseUrl.Contains("give-sms"))
                parameters["service"] = "mt"; //give-sms - mt
            if (BaseUrl.Contains("onlinesim"))
                parameters["service"] = "mt"; //onlinesim - mt
            if (BaseUrl.Contains("sms-activate"))
                parameters["service"] = "mt"; //sms-activate - mt
            if (BaseUrl.Contains("vak-sms"))
                parameters["service"] = "mt"; //vak-sms - mt

            parameters["operator"] = "any";

            if (!String.IsNullOrEmpty(country) || !String.IsNullOrWhiteSpace(country))
                parameters["country"] = country;

            if (!String.IsNullOrEmpty(forward) || !String.IsNullOrWhiteSpace(forward))
                parameters["forward"] = forward;

            string response = await Res(parameters);
            return response;
        }

        public async Task<string> SetStatus(string id, string status, string forward = "")
        {
            var parameters = new Dictionary<string, string>();
            parameters["action"] = "setStatus";
            parameters["id"] = id;

            if (BaseUrl.Contains(Core.Worker.settings.SmsActivationServiceBaseUrl))
            {
                if (status == "-1") parameters["status"] = "8";
                else parameters["status"] = status;
                parameters["lang"] = "ru";
            }
            else if (BaseUrl.Contains(Core.Worker.settings.GiveSmsBaseUrl)) 
            {
                if (status == "-1") parameters["status"] = "8";
                else parameters["status"] = status;
            }
            else if (BaseUrl.Contains(Core.Worker.settings.OnlineSimBaseUrl))
            {
                if (status == "-1") parameters["status"] = "6";
                else parameters["status"] = status;
            }
            else parameters["status"] = status;

            if(!String.IsNullOrEmpty(forward) || !String.IsNullOrWhiteSpace(forward))
                parameters["forward"] = forward;

            string response = await Res(parameters);
            return response;
        }

        private string result = string.Empty;
        public async Task<string> GetStatus(string id)
        {
            var parameters = new Dictionary<string, string>();
            parameters["action"] = "getStatus";
            parameters["id"] = id;

            string response = await Res(parameters);
            result = response;
            return response;
        }

        public async Task<string> WaitForResult(string id, string login, Dictionary<string, int> waitOptions)
        {
            result = "STATUS_WAIT_CODE";

            long startedAt = CurrentTime();

            int timeout = waitOptions.TryGetValue("timeout", out timeout) ? timeout : DefaultTimeout;
            int pollingInterval = waitOptions.TryGetValue("pollingInterval", out pollingInterval) ? pollingInterval : PollingInterval;
            int counter = 0;
            bool b = false;

            while (true)
            {
                
                long now = CurrentTime();
                //Console.WriteLine("{0} {1}", now, startedAt);
                counter++;
                if (now - startedAt < timeout) await Task.Delay(pollingInterval * 1000).ConfigureAwait(false);
                else break;

                string time = DateTime.Now.ToString("HH:mm");
                //Logs
                if (b)
                {
                    int index = Program.accounts.FindIndex(t => t.Login == login);
                    Console.SetCursorPosition(String.Format("[{0}][{1}/{2}] - Waiting SMS code - {3} sec.", login, (index + 1), Program.accounts.Count, (pollingInterval * counter)).Length, Console.CursorTop - 1);
                    do { Console.Write("\b \b"); } while (Console.CursorLeft > 0);
                    Console.WriteLine("[{0}][{1}][{2}/{3}] - Waiting SMS code - {4}/{5} sec.", time, login, (index + 1), Program.accounts.Count, (pollingInterval * counter), timeout);
                }
                else
                {
                    b = true;
                    int index = Program.accounts.FindIndex(t => t.Login == login);
                    Console.WriteLine("[{0}][{1}][{2}/{3}] - Waiting SMS code - {4}/{5} sec.", time, login, (index + 1), Program.accounts.Count, (pollingInterval * counter), timeout);
                }

                try
                {
                    GetStatus(id);
                    if (result != "STATUS_WAIT_CODE" && !result.Contains("502 Bad Gateway"))
                    {
                        return result;
                    }
                }
                catch 
                {
                    // ignore network errors
                }
            }

            //throw new System.TimeoutException("Timeout " + timeout + " seconds reached");
            return string.Empty;
        }

        public class ApiClient
        {
            private readonly HttpClient client = new HttpClient();

            public virtual async Task<string> Get(string url)
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);

                return await Execute(request);
            }

            private async Task<string> Execute(HttpRequestMessage request)
            {
                var response = await client.SendAsync(request);

                string body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new WebException("Unexpected response: " + body);
                }

                if (body.StartsWith("BAD_KEY") || body.StartsWith("BAD_SERVICE"))
                {
                    throw new WebException(body);
                }

                if (body.StartsWith("ACCESS_BALANCE") || body.StartsWith("STATUS_OK"))
                {
                    return body.Split(':')[1].Trim();
                }

                return body;
            }
        }
    }
}
