using Otus_Notification_Homework_17;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus_Notification_Homework_17
{
    public class DeadlineBackgroundTask : BackgroundTask
    {
        public DeadlineBackgroundTask(INotificationService notificationService, IUserRepository userRepository, IToDoRepository toDoRepository) : base(TimeSpan.FromHours(1), nameof(DeadlineBackgroundTask))
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
                var tasks = await toDoRepository.GetActiveWithDeadline(user.UserId, DateTime.UtcNow.AddDays(-1).Date, DateTime.UtcNow.Date, ct);
                foreach (var task in tasks)
                    await notificationService.ScheduleNotification(user.UserId, $"DeaLine {task.Id}", $"Ой! Вы пропустили дедлайн по задаче {task.Name}", DateTime.UtcNow, ct);
            }
        }
    }
}
