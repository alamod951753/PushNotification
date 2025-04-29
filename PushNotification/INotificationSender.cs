using System.Threading.Tasks;

namespace PushNotification
{
    public interface INotificationSender
    {
        Task<bool> SendMessageAsync(string message, MessageFormat format = MessageFormat.PlainText, LogLevel level = LogLevel.Debug);
    }
}