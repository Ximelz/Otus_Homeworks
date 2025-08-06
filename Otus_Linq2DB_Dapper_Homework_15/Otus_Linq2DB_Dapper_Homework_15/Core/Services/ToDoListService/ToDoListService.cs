namespace Otus_Linq2DB_Dapper_Homework_15
{
    public class ToDoListService : IToDoListService
    {
        private readonly IToDoListRepository toDoListRepository;
        public ToDoListService(IToDoListRepository toDoListRepository)
        {
            this.toDoListRepository = toDoListRepository;
        }
        public async Task<ToDoList> Add(ToDoUser user, string name, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            if (name.Length > 10)
                throw new ArgumentException("Имя списка не может быть больше 10 символов!");

            if (await toDoListRepository.ExistsByName(user.UserId, name, ct))
                throw new ArgumentException($"Список с именем {name} уже есть у текущего пользователя!");

            ToDoList list = new ToDoList(name, user);
            await toDoListRepository.Add(list, ct);

            return list;
        }

        public async Task<ToDoList?> Get(Guid id, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            return await toDoListRepository.Get(id, ct);
        }

        public Task Delete(Guid id, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            toDoListRepository.Delete(id, ct);
            return Task.CompletedTask;
        }

        public async Task<IReadOnlyList<ToDoList>> GetUserLists(Guid userId, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            return await toDoListRepository.GetByUserId(userId, ct);
        }
    }
}
