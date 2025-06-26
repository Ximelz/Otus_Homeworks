using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus_Linq_Homework_13
{
    public class CallbackDto
    {
        public CallbackDto(string Action)
        {
            this.Action = Action;
        }
        public string Action { private set; get; }
        public static CallbackDto FromString(string input)
        {
            string[] inputArray = input.Split('|');
            return new CallbackDto(inputArray[0]);
        }
        public override string ToString()
        {
            return Action;
        }
    }
}
