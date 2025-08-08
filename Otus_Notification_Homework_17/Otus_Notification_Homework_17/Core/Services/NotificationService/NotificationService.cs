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
        private IDataContextFactory<ToDoDataContext> factory;
        public async Task<bool> ScheduleNotification(Guid userId, string type, string text, DateTime scheduledAt, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            using (var dbConn = factory.CreateDataContext())
            {
                bool presentInDb = dbConn.NotificationTable
                                         .Where(x => x.UserId == userId)
                                         .Where(x => x.Type == type)
                                         .Any();
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

                dbConn.Insert(notification);
                return true;
            }
        }

        public async Task<IReadOnlyList<Notification>> GetScheduledNotification(DateTime scheduledBefore, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            using (var dbConn = factory.CreateDataContext())
            {
                return dbConn.NotificationTable
                             .LoadWith(u => u.User)
                             .Where(x => x.ScheduledAt <= scheduledBefore)
                             .Where(x => x.IsNotified == false)
                             .ToList()
                             .MapListNotifications()
                             .ToList();
            }
        }

        public async Task MarkNotified(Guid notificationId, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            using (var dbConn = factory.CreateDataContext())
            {
                dbConn.NotificationTable.Where(x => x.Id == notificationId).Set(x => x.IsNotified, true).Update();
            }
        }
    }
}
