using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus_Notification_Homework_17
{
    public class ToDoListCallbackDto : CallbackDto
    {
        public ToDoListCallbackDto(string Action, Guid? ToDoListId) : base(Action)
        {
            this.ToDoListId = ToDoListId;
        }
        public Guid? ToDoListId { get; private set; }
        public static new ToDoListCallbackDto FromString(string input)
        {
            string[] inputArray = input.Split('|');
            Guid? Id;
            if (inputArray.Length == 1)
                Id = null;
            else if (inputArray[1] == "")
                Id = null;
            else
                Id = Guid.Parse(inputArray[1]);
            return new ToDoListCallbackDto(inputArray[0], Id);
        }
        public override string ToString()
        {
            return $"{base.ToString()}|{ToDoListId}";
        }
    }
}
