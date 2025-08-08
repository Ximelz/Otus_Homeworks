using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus_Notification_Homework_17
{
    public class TodayBackgroundTask : BackgroundTask
    {
        public TodayBackgroundTask(INotificationService notificationService, IUserRepository userRepository, IToDoRepository toDoRepository) : base(TimeSpan.FromDays(1), nameof(TodayBackgroundTask))
        {
            this.notificationService = notificationService;
            this.userRepository = userRepository;
            this.toDoRepository = toDoRepository;
        }

        private INotificationService notificationService;
        private IUserRepository userRepository;
        private IToDoRepository toDoRepository;

        protected async override Task Execute(CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            var users = await userRepository.GetUsers(ct);

            foreach (var user in users)
            {
                var tasks = await toDoRepository.GetActiveWithDeadline(user.UserId, DateTime.UtcNow.Date, DateTime.UtcNow.Date.AddDays(1).Date, ct);
                await notificationService.ScheduleNotification(user.UserId,
                                                               $"Today_{DateOnly.FromDateTime(DateTime.UtcNow.Date)}",
                                                               $"Ваши активная задачи {string.Join("\r\n", tasks.Select(x => x.Name))}.",
                                                               DateTime.UtcNow.Date, ct);
            }
        }
    }
}
