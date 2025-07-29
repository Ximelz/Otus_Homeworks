using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Security.Cryptography.X509Certificates;
using Telegram.Bot.Types;

namespace Otus_Linq_Homework_13
{
    public class FileToDoRepository : IToDoRepository
    {
        private readonly string _path;
        public FileToDoRepository(string path)
        {
            this._path = path;

            if (!Directory.Exists(_path))
                Directory.CreateDirectory(this._path);

            if (!File.Exists($"{_path}\\Index.json"))
                File.Create($"{_path}\\Index.json");
        }

        public Task Add(ToDoItem item, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
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
                items.Add(item.User.UserId, new List<Guid>{item.Id});

            SaveIndexFile(items);

            return Task.CompletedTask;
        }

        public Task<int> CountActive(Guid userId, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            return Task.FromResult(GetActiveByUserId(userId, ct).Result.Count);
        }

        public Task Delete(Guid id, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
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
                throw new ArgumentException($"� ������������ {userId} ��� ��� �����!");

            if (!items[userId].Remove(id))
                throw new ArgumentException($"������ \'{id}\' �� �������!");

            SaveIndexFile(items);

            File.Delete($"{_path}\\{userId}\\{id}.json");

            return Task.CompletedTask;
        }

        public Task<bool> ExistsByName(Guid userId, string name, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            ToDoItem? item = GetTasksByUserId(userId).Where(x => x.Name == name).FirstOrDefault();

            return Task.FromResult(item != null);
        }

        public Task<IReadOnlyList<ToDoItem>> Find(Guid userId, Func<ToDoItem, bool> predicate, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            return Task.FromResult((IReadOnlyList<ToDoItem>)GetTasksByUserId(userId).Where(predicate).ToList());
        }

        public Task<IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            return Task.FromResult((IReadOnlyList<ToDoItem>)GetTasksByUserId(userId).Where(x => x.State == ToDoItemState.Active).ToList());
        }

        public Task<IReadOnlyList<ToDoItem>> GetAllByUserId(Guid userId, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            return Task.FromResult((IReadOnlyList<ToDoItem>)GetTasksByUserId(userId));
        }

        public Task Update(ToDoItem item, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            var json = JsonSerializer.Serialize(item);
            File.WriteAllText($"{_path}\\{item.User.UserId}\\{item.Id}.json", json);

            return Task.CompletedTask;
        }

        /// <summary>
        /// ����� ��������� ������ ���� ����� ���� ������������� �� ����� �������.
        /// </summary>
        /// <param name="path">���� � ����� �������.</param>
        /// <returns>������ ���� ����� � ���� �������, ��� ���� ��� id ������������, � value ��� ������ id ��� �����.</returns>
        private Dictionary<Guid,List<Guid>> ReadIndexFile()
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
                    toDoItems.Add(userId, new List<Guid>{ todoId });
            }

            return toDoItems;
        }

        /// <summary>
        /// ����� ���������� ���� ����� ���� ������������� � ���� ������.
        /// </summary>
        /// <param name="path">���� � ����� �������.</param>
        /// <param name="toDoItems">������ ���� �����.</param>
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
        /// ����� ��������� ������ ���� ����� ������������.
        /// </summary>
        /// <param name="userId">Id ������������.</param>
        /// <returns>������ ����� ������������.</returns>
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
    }
}
