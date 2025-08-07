using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Otus_BackgroundTask_Homework_16
{
    public static class ModelMapper
    {
        public static ToDoUser MapFromModel(ToDoUserModel model) => new ToDoUser() {
                                                                    UserId = model.UserId,
                                                                    TelegramUserId = model.TelegramUserId,
                                                                    TelegramUserName = model.TelegramUserName,
                                                                    RegisteredAt = model.RegisteredAt };

        public static ToDoUserModel MapToModel(ToDoUser entity) => new ToDoUserModel() {
                                                                   UserId = entity.UserId,
                                                                   TelegramUserId = entity.TelegramUserId,
                                                                   TelegramUserName = entity.TelegramUserName,
                                                                   RegisteredAt = entity.RegisteredAt };

        public static ToDoItem MapFromModel(ToDoItemModel model) => new ToDoItem() {
                                                                    Id = model.Id,
                                                                    Name = model.Name,
                                                                    CreatedAt = model.CreatedAt,
                                                                    State = model.State,
                                                                    StateChangedAt = model.StateChangedAt,
                                                                    DeadLine = model.DeadLine,
                                                                    List = model.List != null ? MapFromModel(model.List) : null,
                                                                    User = MapFromModel(model.User) };

        public static ToDoItemModel MapToModel(ToDoItem entity) => new ToDoItemModel() {
                                                                   Id = entity.Id,
                                                                   Name = entity.Name,
                                                                   CreatedAt = entity.CreatedAt,
                                                                   State = entity.State,
                                                                   StateChangedAt = entity.StateChangedAt,
                                                                   DeadLine = entity.DeadLine,
                                                                   UserId = entity.User.UserId,
                                                                   ListId = entity.List != null ? entity.List.Id : null,
                                                                   List = entity.List != null ? MapToModel(entity.List) : null,
                                                                   User = MapToModel(entity.User) };
        public static ToDoList MapFromModel(ToDoListModel model) => new ToDoList() {
                                                                    Id = model.Id,
                                                                    Name = model.Name,
                                                                    CreatedAt = model.CreatedAt,
                                                                    User = MapFromModel(model.User) };
        public static ToDoListModel MapToModel(ToDoList entity) => new ToDoListModel() {
                                                                   Id = entity.Id,
                                                                   Name = entity.Name,
                                                                   CreatedAt = entity.CreatedAt,
                                                                   UserId = entity.User.UserId,
                                                                   User = MapToModel(entity.User) };
    }
}
