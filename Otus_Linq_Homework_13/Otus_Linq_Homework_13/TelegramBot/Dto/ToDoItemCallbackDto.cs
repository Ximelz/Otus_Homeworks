using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus_Linq_Homework_13
{
    public class ToDoItemCallbackDto : CallbackDto
    {
        public ToDoItemCallbackDto(string Action, Guid? ToDoItemId) : base(Action)
        {
            this.ToDoItemId = ToDoItemId;
        }
        public Guid? ToDoItemId { get; private set; }
        public static new ToDoItemCallbackDto FromString(string input)
        {
            string[] inputArray = input.Split('|');
            Guid? Id;
            if (inputArray.Length == 1)
                Id = null;
            else if (inputArray[1] == "")
                Id = null;
            else
                Id = Guid.Parse(inputArray[1]);
            return new ToDoItemCallbackDto(inputArray[0], Id);
        }
        public override string ToString()
        {
            return $"{base.ToString()}|{ToDoItemId}";
        }
    }
}
