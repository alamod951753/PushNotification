using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using PushNotification.Options;

namespace PushNotification
{
    public class TelegramPushService : INotificationSender
    {
        private readonly string _botToken;
        private readonly string _chatId;
        private readonly HttpClient _httpClient;

        public TelegramPushService(string botToken, string chatId, PushOptions options = null)
        {
            _botToken = botToken;
            _chatId = chatId;

            var handler = new HttpClientHandler();

            // 如果有指定 Proxy，就使用
            if (!string.IsNullOrEmpty(options?.ProxyUrl))
            {
                handler.Proxy = new WebProxy(options.ProxyUrl, false)
                {
                    BypassProxyOnLocal = options.BypassProxyOnLocal,
                    BypassList = options.BypassList ?? Array.Empty<string>()
                };
                handler.UseProxy = true;
            }

            _httpClient = new HttpClient(handler);
        }

        /// <summary>
        /// 發送訊息到 Telegram
        /// </summary>
        /// <param name="message">訊息內容</param>
        /// <param name="format">訊息格式：PlainText、Markdown、Html</param>
        /// <param name="level">訊息等級：Debug、Info、Warn、Error、Fatal</param>
        public async Task<bool> SendMessageAsync(string message, MessageFormat format = MessageFormat.PlainText, LogLevel level = LogLevel.Debug)
        {
            try
            {
                var url = $"https://api.telegram.org/bot{_botToken}/sendMessage";

                var levelTag = $"[ #{level.ToString().ToUpper()} ]";
                var formattedMessage = $"{levelTag}\n{message}";

                string parseMode = format == MessageFormat.Html ? "HTML" :
                                format == MessageFormat.Markdown ? "Markdown" : null;

                var json = new StringBuilder();
                json.Append("{");
                json.AppendFormat("\"chat_id\":\"{0}\",", _chatId);
                json.AppendFormat("\"text\":\"{0}\"", EscapeJson(formattedMessage));

                if (!string.IsNullOrEmpty(parseMode))
                    json.AppendFormat(",\"parse_mode\":\"{0}\"", parseMode);

                json.Append("}");

                var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, payload);
                if (!response.IsSuccessStatusCode)
                {
                    LogError($"Telegram 發送失敗: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                    return false;
                }
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                LogError($"Telegram 發送異常: {ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }

        // 避免訊息中有特殊字元造成 JSON 格式錯誤
        private static string EscapeJson(string value)
        {
            return value.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n");
        }

        private void LogError(string message)
        {
            string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TelegramPushService.log");
            string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}\n";
            File.AppendAllText(logPath, logEntry);
        }
    }
}
