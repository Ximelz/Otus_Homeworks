using Otus_Notification_Homework_17;

namespace Otus_Notification_Homework_17
{
    public class Notification
    {
        public Guid Id { set; get; }                    //Id нотификации.
        public ToDoUser User { set; get; }                     //Пользователь нотификации.
        public string Type { set; get; }                       //Тип нотификации. Например: DeadLine_{ToDoItem.Id}, Today_{DateOnly.FromDateTime(DateTime.UtcNow)}.
        public string Text { set; get; }                       //Текст, который будет отправлен.
        public DateTime ScheduledAt { set; get; }              //Запланированная дата отправки.
        public bool IsNotified { set; get; }                   //Флаг отправки.
        public DateTime? NotifiedAt { set; get; }              //Фактическая дата отправки.
    }
}
