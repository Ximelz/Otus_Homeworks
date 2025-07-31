using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus_Linq_Homework_13
{
    /// <summary>
    /// Класс для взаимодействия с задачами.
    /// </summary>
    public class ToDoService : IToDoService
    {
        public ToDoService(int maxTasks, int maxLengthNameTask, IToDoRepository toDoRep)
        {
            this.toDoRep = toDoRep;
            this.maxTasks = maxTasks;
            this.maxLengthNameTask = maxLengthNameTask;
        }

        private readonly int maxTasks;
        private readonly int maxLengthNameTask;
        private readonly IToDoRepository toDoRep;

        public async Task<IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            return await toDoRep.GetActiveByUserId(userId, ct);
        }

        public async Task<IReadOnlyList<ToDoItem>> GetAllByUserId(Guid userId, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            return await toDoRep.GetAllByUserId(userId, ct);
        }

        public async Task<ToDoItem> Add(ToDoUser user, string name, DateTime deadLine, ToDoList? list, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            if (await toDoRep.CountActive(user.UserId, ct) >= maxTasks)
                throw new TaskCountLimitException(maxTasks);

            if (name.Length > maxLengthNameTask)
                throw new TaskLengthLimitException(name.Length, maxLengthNameTask);

            if (await toDoRep.ExistsByName(user.UserId, name, ct))
                throw new DuplicateTaskException(name);

            ToDoItem newItem = new ToDoItem(name, user);
            newItem.DeadLine = deadLine;
            newItem.List = list;
            await toDoRep.Add(newItem, ct);

            return newItem; 
        }

        public async Task MarkCompleted(Guid id, ToDoUser user, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            ToDoItem task = (await GetAllByUserId(user.UserId, ct)).Single(x => x.Id == id);

            if (task.State == ToDoItemState.Completed)
                throw new CompleteTaskException(id);

            task.State = ToDoItemState.Completed;
            task.StateChangedAt = DateTime.Now;
            await toDoRep.Update(task, ct);
        }

        public async Task Delete(Guid id, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            await toDoRep.Delete(id, ct);
        }

        public async Task<IReadOnlyList<ToDoItem>> Find(ToDoUser user, string namePrefix, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            return await toDoRep.Find(user.UserId, x => x.Name.StartsWith(namePrefix), ct);
        }

        public async Task<IReadOnlyList<ToDoItem>> GetByUserIdAndList(Guid userId, Guid? listId, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            IReadOnlyList<ToDoItem> activeTasks = await toDoRep.GetActiveByUserId(userId, ct);
            List<ToDoItem> resultList;

            if (listId == null)
                resultList = activeTasks.Where(x => x.List == null).ToList();
            else
                resultList = activeTasks
                             .Where(x => x.List != null)
                             .Where(y => y.List.Id == (Guid)listId)
                             .ToList();

            return resultList;
        }

        public async Task<ToDoItem>? Get(Guid toDoItemId, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            return await toDoRep.Get(toDoItemId, ct);
        }
    }
}
