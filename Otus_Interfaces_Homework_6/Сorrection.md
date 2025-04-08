1. Перенес изменение свойства даты изменения статуса задачи в метод обработки команды /completetask
2. Удалены методы GetAllByUserID и GetTasksCount из IToDoService и ToDoService
3. Добавлено использование userid в GetActiveByUserID
4. Изменена логика работы команды /addtask. Теперь проводится проверка добавляемой задачи на длину, дублирование (DublicateCheck) и максимальное количество задач (MaxTasksCheck).
5. Удалены флаги removeCommandActive, completeCommandActive и startFlag
6. Добавлен readonly для consoleUsers в классе UserService
7. Добавлено в конструктор ConsoleUser присваение id и имя пользователя из telegram
8. Созданы исключения CommandException, AuthUserException, ArgsException и IndexOutRange. Все созданные исключения перенесены в отдельный файл.
9. iToDoService передается через конструктор.
10. Условия обработки команд перенесены из HandleUpdateAsync в HeandlerCommands
11. Преобразования строки в массив команды с аргументами теперь происходит в методе обработки аргументов StringArrayHandler. Теперь данный метод обрабатывает не только аргументы команды /addtask. Так же были добавлены методы удаления пустых ячеек списка DeleteNullItemsArray, проверки строки на пустоту ValidateString и объединения нескольких ячеек в 1 строку ConcatArgsInArray.
12. Добавлены команды из прошлого ДЗ /maxtasks и /maxnametask и связанный с ними метод ParseAndValidateInt. Для обработки этих команд добавлены методы SetMaxTasks и SetMaxLengthNameTasks.
13. Добавлен метод CheckAuthUser, проверяющий авторизацию текущего пользователя.
14. Изменена логика метода ShowTasks. Метод приведения IToDoService к ToDoService вынесен в метод ParseToDoService, логика преобразования элементов списка в строку вынесена в метод ToDoListInString с использованием StringBuilder. Оставлен только 1 цикл преобразования массива в строку для обоих команд /showtasks и /showalltasks.
15. Изменена логика работы методов RemoveTask и CompleteTask. В обоих методах используется метод приведения IToDoService к ToDoService, получение списка задач на основании 3 аргумента GetToDoItemsList и метод парсинга строки в число.
16. Логика формирования строки с активными командами выведена в отдельный метод BotsCommandString и была перенесена в finally. Данное решение было принято мной для того, чтобы в случае возникновения ошибки активные команды все равно заново отобразились.
17. Удалил файлы библиотеки Otus.ToDoList.ConsoleBot и подключил ее как отдельный проект.