using Otus_Notification_Homework_17;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Otus_Notification_Homework_17
{
    public class NotificationBackgroundTask : BackgroundTask
    {
        public NotificationBackgroundTask(INotificationService notificationService, ITelegramBotClient botClient) : base(TimeSpan.FromMinutes(1), nameof(NotificationBackgroundTask))
        {
            this.botClient = botClient;
            this.notificationService = notificationService;
        }

        private readonly INotificationService notificationService;
        private readonly ITelegramBotClient botClient;
        protected override async Task Execute(CancellationToken ct)
        {
            var notifications = await notificationService.GetScheduledNotification(DateTime.UtcNow, ct);
            foreach (var notification in notifications)
            {
                await botClient.SendMessage(chatId: notification.User.TelegramUserId, notification.Text, cancellationToken: ct);
                await notificationService.MarkNotified(notification.Id, ct);
            }
        }
    }
}
