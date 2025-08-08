using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus_Notification_Homework_17
{
    public interface INotificationService
    {
        /// <summary>
        /// Метод создания нотификации.
        /// </summary>
        /// <param name="userId">Id пользователя.</param>
        /// <param name="type">Тип нотификации</param>
        /// <param name="text">Текст уведомления.</param>
        /// <param name="scheduledAt">Запланированная дата отправки</param>
        /// <param name="ct">Токен отмены.</param>
        /// <returns>Если запись с userId и type уже есть, то вернуть false и не добавлять запись, иначе вернуть true.</returns>
        Task<bool> ScheduleNotification(Guid userId, string type, string text, DateTime scheduledAt, CancellationToken ct);

        /// <summary>
        /// Метод получения нотификации.
        /// </summary>
        /// <param name="scheduledBefore">Фактическая дата отправки.</param>
        /// <param name="ct">Токен отмены.</param>
        /// <returns>Возвращает нотификации, у которых IsNotified = false && ScheduledAt <= scheduledBefore.</returns>
        Task<IReadOnlyList<Notification>> GetScheduledNotification(DateTime scheduledBefore, CancellationToken ct);

        /// <summary>
        /// Пометка отправки нотификациии.
        /// </summary>
        /// <param name="notificationId">Id нотификации.</param>
        /// <param name="ct">Токен отмены.</param>
        Task MarkNotified(Guid notificationId, CancellationToken ct);
    }
}
