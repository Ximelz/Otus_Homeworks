using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Otus_Linq_Homework_13
{
    public class FileToDoListRepository : IToDoListRepository
    {
        private string path;
        public FileToDoListRepository(string path)
        {
            this.path = path + "\\Lists\\";

            if (!Directory.Exists(this.path))
                Directory.CreateDirectory(this.path);
        }
        public Task<ToDoList?> Get(Guid id, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            string listFile = $"{path}{id}.json";

            if (!File.Exists(listFile))
                throw new FileNotFoundException($"Список \"{id}\" не найден!");

            ToDoList? list = JsonSerializer.Deserialize<ToDoList>(File.ReadAllText(listFile));

            if (list == null)
                throw new FileLoadException($"Ошибка с чтением файла хранения данных пользователя\"{id}\"! Файл расположен по следующему пути: {listFile}.");

            return Task.FromResult(list);
        }

        public Task<IReadOnlyList<ToDoList>> GetByUserId(Guid userId, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            string[] files = Directory.GetFiles(path);
            List<ToDoList> toDoLists = new List<ToDoList>();

            foreach (string file in files)
            {
                ToDoList list = JsonSerializer.Deserialize<ToDoList>(File.ReadAllText($"{file}"));
                toDoLists.Add(list);
            }

            return Task.FromResult((IReadOnlyList<ToDoList>)toDoLists.Where(x => x.User.UserId == userId).ToList());
        }

        public Task Add(ToDoList list, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            var json = JsonSerializer.Serialize(list);
            File.WriteAllText($"{path}{list.Id}.json", json);

            return Task.CompletedTask;
        }

        public Task Delete(Guid id, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            File.Delete(path + $"{id}.json");
            return Task.CompletedTask;
        }

        public Task<bool> ExistsByName(Guid userId, string name, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            return Task.FromResult(GetByUserId(userId, ct).Result.Where(x => x.Name == name).Any());
        }
    }
}
