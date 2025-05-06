namespace PushNotification.Options
{
    public class PushOptions
    {
        public string ProxyUrl { get; set; } = "";         // e.g. http://proxy.example.com:8080
        public bool BypassProxyOnLocal { get; set; } = true;
        public string[] BypassList { get; set; } = new string[0];
    }
}