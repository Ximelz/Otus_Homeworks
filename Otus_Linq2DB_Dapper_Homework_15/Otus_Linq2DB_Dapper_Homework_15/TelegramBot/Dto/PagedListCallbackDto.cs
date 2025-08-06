using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus_Linq2DB_Dapper_Homework_15
{
    public class PagedListCallbackDto : ToDoListCallbackDto
    {
        public PagedListCallbackDto(string Action, Guid? ToDoListId, int Page) : base(Action, ToDoListId)
        {
            this.Page = Page;
        }
        public int Page { get; private set; }
        public static new PagedListCallbackDto FromString(string input)
        {
            string[] inputArray = input.Split('|');
            Guid? Id;
            int currentPage = 0;
            if (inputArray.Length == 1)
                Id = null;
            else if (inputArray[1] == "")
                Id = null;
            else
                Id = Guid.Parse(inputArray[1]);

            if (inputArray.Length > 2)
                int.TryParse(inputArray[2], out currentPage);

            return new PagedListCallbackDto(inputArray[0], Id, currentPage);
        }
        public override string ToString()
        {
            return $"{base.ToString()}|{Page}";
        }
    }
}
