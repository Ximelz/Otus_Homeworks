-- Метод GetAllByUserId()
SELECT "Id", "UserId", "ListId", "Name", "CreatedAt", "State", "StateChangedAt", "DeadLine"
	FROM public."ToDoItem";

-- Метод GetActiveByUserId()
SELECT "Id", "UserId", "ListId", "Name", "CreatedAt", "State", "StateChangedAt", "DeadLine"
	FROM public."ToDoItem"
	WHERE "State" = 0;

-- Метод Add()
DO $$
DECLARE
	"RandomUserId" uuid;
	"RandomListId" uuid;
	"RandomStateChangedAt" date;
	"RandomDeadLine" date;
BEGIN
  	SELECT "UserId" INTO "RandomUserId" FROM "ToDoUser" ORDER BY random() LIMIT 1;
  	SELECT "Id" INTO "RandomListId" FROM "ToDoList" ORDER BY random() LIMIT 1;
	SELECT ('2025-09-01 10:00:00'::timestamp + (('2025-09-30 12:00:00'::timestamp - '2025-09-01 10:00:00'::timestamp)* random())) INTO "RandomStateChangedAt";
	SELECT ('2025-10-01 10:00:00'::timestamp + (('2025-10-31 12:00:00'::timestamp - '2025-10-01 10:00:00'::timestamp)* random())) INTO "RandomDeadLine";

	--Логика добавления задачи в таблицу. Многие значения получены с помощью рандома.
	INSERT INTO public."ToDoItem" ("Id", "UserId", "ListId", "Name", "CreatedAt", "State", "StateChangedAt", "DeadLine")
    VALUES (gen_random_uuid()::uuid,
			"RandomUserId",
			"RandomListId",
			'Task_' || random(150, 200)::varchar(100),
			default,
			0,
			"RandomStateChangedAt",
			"RandomDeadLine");
END $$

-- Метод Update()
UPDATE public."ToDoItem"
	SET "State" = 1
	WHERE "Id" IN (SELECT "Id"
		 	   	   FROM public."ToDoItem"
			   	   ORDER BY random()
			   	   LIMIT 1);

-- Метод Delete()
DELETE FROM public."ToDoItem"
	WHERE "Id" IN (SELECT "Id"
		 	   	   FROM public."ToDoItem"
			   	   ORDER BY random()
			   	   LIMIT 1);


-- Метод ExistsByName()
SELECT EXISTS(
		SELECT 1
		FROM public."ToDoItem"
		WHERE "Name" = 'Task_96'
		AND "UserId" IN (SELECT "UserId"
		 	   	   	 FROM public."ToDoUser"
			   	   	 ORDER BY random()
			   	   	 LIMIT 1));

-- Метод CountActive()
SELECT COUNT(*)
	FROM public."ToDoItem"
	WHERE "State" = 0;

-- Метод Find()
SELECT "Id", "UserId", "ListId", "Name", "CreatedAt", "State", "StateChangedAt", "DeadLine"
	FROM public."ToDoItem"
	WHERE "Name" LIKE 'Task_1%';
