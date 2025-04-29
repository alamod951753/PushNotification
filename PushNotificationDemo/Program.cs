using System;
using System.Net.Http;
using System.Threading.Tasks;
using PushNotification;

class Program
{
    static async Task Main()
    {
        // 傳送到 Telegram
        var pushService = new TelegramPushService("7675005108:AAFS4aWdacAy7oBm8cV2hfm96tOVS9hmGvo", "-4610441882");
        bool isSuccess = await pushService.SendMessageAsync(
            message: "test alert",
            format: MessageFormat.Html,
            level: LogLevel.Info
        );

        Console.WriteLine(isSuccess ? "發送成功" : "發送失敗");
    }
}