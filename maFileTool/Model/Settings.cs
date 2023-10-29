namespace maFileTool.Model
{
    public class Settings
    {
        public string Mode { get; set; } = "TXT";
        public string BindingTimeout { get; set; } = "1";
        public string SMSTimeout { get; set; } = "1";
        public string GetSmsApiKey { get; set; } = string.Empty;
        public string SmsActivationServiceApiKey { get; set; } = string.Empty;
        public string FiveSimApiKey { get; set; } = string.Empty;
        public string GiveSmsApiKey { get; set; } = string.Empty;
        public string OnlineSimApiKey {  get; set; } = string.Empty;
        public string SmsActivateApiKey { get; set; } = string.Empty;
        public string VakSmsApiKey { get; set; } = string.Empty;
        public string GetSmsBaseUrl { get; set; } = "getsms.online";
        public string SmsActivationServiceBaseUrl { get; set; } = "sms-activation-service.com";
        public string FiveSimBaseUrl { get; set; } = "sms-activation-service.com";
        public string GiveSmsBaseUrl { get; set; } = "give-sms.com";
        public string OnlineSimBaseUrl { get; set; } = "onlinesim.io";
        public string SmsActivateBaseUrl { get; set; } = "sms-activate.org";
        public string VakSmsBaseUrl { get; set; } = "vak-sms.com";
        public string[] Priority { get; set; } = new string[] { "GetSms", "SmsActivationService", "FiveSim", "GiveSms", "OnlineSim", "SmsActivate", "VakSms" };
        public string GetSmsCountry { get; set; } = "or";
        public string SmsActivationServiceCountry { get; set; } = "0";
        public string FiveSimCountry { get; set; } = "0";
        public string GiveSmsCountry { get; set; } = "0";
        public string OnlineSimCountry { get; set; } = "0";
        public string SmsActivateCountry { get; set; } = "0";
        public string VakSmsCountry { get; set; } = "0";
        public string MailServer { get; set; } = string.Empty;
        public string MailPort { get; set; } = "993";
        public string MailProtocol { get; set; } = "IMAP";
        public string UseSSL { get; set; } = "true";
    }
}