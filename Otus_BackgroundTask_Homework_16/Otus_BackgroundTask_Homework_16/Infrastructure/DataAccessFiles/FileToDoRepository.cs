using System.Data;
using System.Text.Json;

namespace Otus_BackgroundTask_Homework_16
{
    public class FileToDoRepository : IToDoRepository
    {
        private readonly string _path;
        public FileToDoRepository(string path)
        {
            _path = path;

            if (!Directory.Exists(_path))
                Directory.CreateDirectory(this._path);

            if (!File.Exists($"{_path}\\Index.json"))
                File.Create($"{_path}\\Index.json");
        }

        public Task Add(ToDoItem item, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            var json = JsonSerializer.Serialize(item);
            string userPath = $"{_path}\\{item.User.UserId}\\";

            if (!Directory.Exists(userPath))
                Directory.CreateDirectory(userPath);

            File.WriteAllText($"{userPath}\\{item.Id}.json", json);

            Dictionary<Guid, List<Guid>> items = ReadIndexFile();

            if (items.ContainsKey(item.User.UserId))
                items[item.User.UserId].Add(item.Id);
            else
                items.Add(item.User.UserId, new List<Guid> { item.Id });

            SaveIndexFile(items);

            return Task.CompletedTask;
        }

        public Task<int> CountActive(Guid userId, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            return Task.FromResult(GetActiveByUserId(userId, ct).Result.Count);
        }

        public Task Delete(Guid id, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            Dictionary<Guid, List<Guid>> items = ReadIndexFile();
            Guid userId = Guid.Empty;

            foreach (var item in items)
            {
                if (item.Value.Contains(id))
                {
                    userId = item.Key;
                    break;
                }
            }
            if (userId == Guid.Empty)
                throw new ArgumentException($"У пользователя {userId} еще нет задач!");

            if (!items[userId].Remove(id))
                throw new ArgumentException($"Задача \'{id}\' не найдена!");

            SaveIndexFile(items);

            File.Delete($"{_path}\\{userId}\\{id}.json");

            return Task.CompletedTask;
        }

        public Task<bool> ExistsByName(Guid userId, string name, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            ToDoItem? item = GetTasksByUserId(userId).Where(x => x.Name == name).FirstOrDefault();

            return Task.FromResult(item != null);
        }

        public Task<IReadOnlyList<ToDoItem>> Find(Guid userId, Func<ToDoItem, bool> predicate, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            return Task.FromResult((IReadOnlyList<ToDoItem>)GetTasksByUserId(userId).Where(predicate).ToList());
        }

        public Task<IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            return Task.FromResult((IReadOnlyList<ToDoItem>)GetTasksByUserId(userId).Where(x => x.State == ToDoItemState.Active).ToList());
        }

        public Task<IReadOnlyList<ToDoItem>> GetAllByUserId(Guid userId, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            return Task.FromResult((IReadOnlyList<ToDoItem>)GetTasksByUserId(userId));
        }

        public Task Update(ToDoItem item, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            var json = JsonSerializer.Serialize(item);
            File.WriteAllText($"{_path}\\{item.User.UserId}\\{item.Id}.json", json);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Метод получения списка всех задач всех пользователей из файла индекса.
        /// </summary>
        /// <param name="path">Путь к файлу индексу.</param>
        /// <returns>Список всех задач в виде словаря, где ключ это id пользователя, а value это список id его задач.</returns>
        private Dictionary<Guid, List<Guid>> ReadIndexFile()
        {
            string indexPath = $"{_path}\\Index.json";
            Dictionary<Guid, List<Guid>> toDoItems = new Dictionary<Guid, List<Guid>>();

            if (!File.Exists(indexPath))
                return toDoItems;

            string[] indexArray = File.ReadAllLines(indexPath);

            foreach (string item in indexArray)
            {
                string[] toDoItem = item.Split(';');

                if (!Guid.TryParse(toDoItem[0], out Guid todoId))
                    continue;

                if (!Guid.TryParse(toDoItem[1], out Guid userId))
                    continue;

                if (toDoItems.ContainsKey(userId))
                    toDoItems[userId].Add(todoId);
                else
                    toDoItems.Add(userId, new List<Guid> { todoId });
            }

            return toDoItems;
        }

        /// <summary>
        /// Метод сохранения всех задач всех пользователей в файл индекс.
        /// </summary>
        /// <param name="path">Путь к файлу индекса.</param>
        /// <param name="toDoItems">Список всех задач.</param>
        private void SaveIndexFile(Dictionary<Guid, List<Guid>> toDoItems)
        {
            string indexPath = $"{_path}\\Index.json";

            List<string> indexList = new List<string>();

            foreach (KeyValuePair<Guid, List<Guid>> pair in toDoItems)
                foreach (Guid toDoItem in pair.Value)
                    indexList.Add($"{toDoItem};{pair.Key}");

            File.WriteAllLines(indexPath, indexList);
        }


        /// <summary>
        /// Метод получения списка всех задач пользователя.
        /// </summary>
        /// <param name="userId">Id пользователя.</param>
        /// <returns>Список задач пользователя.</returns>
        private List<ToDoItem> GetTasksByUserId(Guid userId)
        {
            Dictionary<Guid, List<Guid>> items = ReadIndexFile();

            List<ToDoItem> activeItems = new List<ToDoItem>();

            if (!items.ContainsKey(userId))
                return activeItems;

            foreach (Guid itemId in items[userId])
            {
                string itemPath = $"{_path}\\{userId}\\{itemId}.json";
                ToDoItem? item = JsonSerializer.Deserialize<ToDoItem>(File.ReadAllText(itemPath));

                if (item != null)
                    activeItems.Add(item);
            }

            return activeItems;
        }

        public async Task<ToDoItem?> Get(Guid toDoItemId, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            string indexPath = $"{_path}\\Index.json";

            if (!File.Exists(indexPath))
                return null;

            string[] indexArray = File.ReadAllLines(indexPath);

            foreach (string item in indexArray)
            {
                string[] toDoItem = item.Split(';');

                if (!Guid.TryParse(toDoItem[0], out Guid todoId))
                    continue;

                if (todoId == toDoItemId)
                {
                    string itemPath = $"{_path}\\{toDoItem[1]}\\{toDoItem[0]}.json";
                    return JsonSerializer.Deserialize<ToDoItem>(File.ReadAllText(itemPath));
                }
            }

            return null;
        }
    }
}
