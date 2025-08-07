using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus_BackgroundTask_Homework_16
{
    public static class ExtensionsMapper
    {
        public static List<ToDoItem> MapListItems(this List<ToDoItemModel> toDoItemModels)
        {
            List<ToDoItem> items = new List<ToDoItem>();
            foreach (var item in toDoItemModels)
            {
                items.Add(ModelMapper.MapFromModel(item));
            }
            return items;
        }

        public static List<ToDoList> MapListLists(this List<ToDoListModel> toDoItemModels)
        {
            List<ToDoList> items = new List<ToDoList>();
            foreach (var item in toDoItemModels)
            {
                items.Add(ModelMapper.MapFromModel(item));
            }
            return items;
        }

        public static List<ToDoUser> MapListUsers(this List<ToDoUserModel> toDoItemModels)
        {
            List<ToDoUser> items = new List<ToDoUser>();
            foreach (var item in toDoItemModels)
            {
                items.Add(ModelMapper.MapFromModel(item));
            }
            return items;
        }
    }
}
