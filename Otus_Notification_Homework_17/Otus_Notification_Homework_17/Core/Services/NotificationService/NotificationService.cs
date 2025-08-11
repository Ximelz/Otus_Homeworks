using LinqToDB;
using Otus_Notification_Homework_17;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Otus_Notification_Homework_17
{
    public class NotificationService : INotificationService
    {
        public NotificationService(IDataContextFactory<ToDoDataContext> factory)
        {
            this.factory = factory;
        }
        private readonly IDataContextFactory<ToDoDataContext> factory;
        public async Task<bool> ScheduleNotification(Guid userId, string type, string text, DateTime scheduledAt, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            await using (var dbConn = factory.CreateDataContext())
            {
                bool presentInDb = await dbConn.Notifications
                                               .Where(x => x.UserId == userId)
                                               .Where(x => x.Type == type)
                                               .AnyAsync(ct);
                if (presentInDb) 
                    return false;

                NotificationModel notification = new NotificationModel()
                {
                    Id = Guid.NewGuid(),
                    Type = type,
                    Text = text,
                    ScheduledAt = scheduledAt,
                    UserId = userId
                };

                await dbConn.InsertAsync(notification, token: ct);
                return true;
            }
        }

        public async Task<IReadOnlyList<Notification>> GetScheduledNotification(DateTime scheduledBefore, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            await using (var dbConn = factory.CreateDataContext())
            {
                var notifications = await dbConn.Notifications
                                                .LoadWith(u => u.User)
                                                .Where(x => x.ScheduledAt <= scheduledBefore)
                                                .Where(x => x.IsNotified == false)
                                                .ToListAsync(ct);
                
                return notifications.MapListNotifications();
            }
        }

        public async Task MarkNotified(Guid notificationId, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            await using (var dbConn = factory.CreateDataContext())
            {
                await dbConn.Notifications.Where(x => x.Id == notificationId).Set(x => x.IsNotified, true).UpdateAsync(ct);
            }
        }
    }
}
