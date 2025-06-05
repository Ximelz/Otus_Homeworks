using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Otus_Scenario_Homework_11
{
    public class FileUsersRepository : IUserRepository
    {
        public FileUsersRepository(string path)
        {
            _path = path + "\\Users\\";

            if (!Directory.Exists(_path))
                Directory.CreateDirectory(_path);
        }
        private readonly string _path;
        public Task<ToDoUser?> GetUser(Guid userId, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            string userFile = $"{_path}\\{userId}.json";

            if (!File.Exists(userFile))
                throw new FileNotFoundException($"Пользователь \"{userId}\" не найден!");

            ToDoUser? user = JsonSerializer.Deserialize<ToDoUser>(File.ReadAllText(userFile));

            if (user == null)
                throw new FileLoadException($"Ошибка с чтением файла хранения данных пользователя\"{userId}\"! Файл расположен по следующему пути {userFile}.");

            return Task.FromResult(user);
        }

        public Task<ToDoUser?> GetUserByTelegramUserId(long telegramUserId, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            DirectoryInfo userDirectory = new DirectoryInfo(_path);
            var files = userDirectory.EnumerateFiles();

            foreach (FileInfo userFile in files)
            {
                ToDoUser? user = JsonSerializer.Deserialize<ToDoUser>(File.ReadAllText(userFile.ToString()));
                if (user == null)
                    continue;

                if (user.TelegramUserId == telegramUserId)
                    return Task.FromResult(user);
            }

            return null;
        }

        public Task Add(ToDoUser user, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            var json = JsonSerializer.Serialize(user);
            File.WriteAllText($"{_path}\\{user.UserId}.json", json);

            return Task.CompletedTask;
        }
    }
}
