using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PushNotification
{
    public class TelegramPushService : INotificationSender
    {
        private readonly string _botToken;
        private readonly string _chatId;

        public TelegramPushService(string botToken, string chatId)
        {
            _botToken = botToken;
            _chatId = chatId;
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

                var levelTag = $"[{level.ToString().ToUpper()}]";
                var formattedMessage = $"{levelTag} {message}";

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

                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.PostAsync(url, payload);
                    return response.IsSuccessStatusCode;
                    //if (!response.IsSuccessStatusCode)
                    //{
                    //    var content = await response.Content.ReadAsStringAsync();
                    //    throw new Exception($"發送訊息失敗: {response.StatusCode}, {content}");
                    //}
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        // 避免訊息中有特殊字元造成 JSON 格式錯誤
        private static string EscapeJson(string value)
        {
            return value.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n");
        }
    }
}
