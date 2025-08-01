DO $$
DECLARE
	"RandomUserId" uuid;
	"RandomListId" uuid;
	"RandomRegisteredAt" date;
	"RandomStateChangedAt" date;
	"RandomDeadLine" date;
BEGIN
  FOR i IN 1..5 LOOP
	SELECT ('2025-07-01 10:00:00'::timestamp + (('2025-07-31 12:00:00'::timestamp - '2025-07-01 10:00:00'::timestamp)* random())) INTO "RandomRegisteredAt";
	
    INSERT INTO public."ToDoUser" ("UserId", "TelegramUserId", "TelegramUserName", "RegisteredAt")
    VALUES (gen_random_uuid()::uuid, random(1, 1000000000)::bigint, 'User_' || i::varchar(100), "RandomRegisteredAt");
	
  END LOOP;
  FOR i IN 1..10 LOOP
  	SELECT "UserId" INTO "RandomUserId" FROM "ToDoUser" ORDER BY random() LIMIT 1;
	SELECT ('2025-07-01 10:00:00'::timestamp + (('2025-07-31 12:00:00'::timestamp - '2025-07-01 10:00:00'::timestamp)* random())) INTO "RandomRegisteredAt";
	
    INSERT INTO public."ToDoList" ("Id", "UserId", "Name", "CreatedAt")
    VALUES (gen_random_uuid()::uuid, "RandomUserId", 'List_' || i::varchar(100), "RandomRegisteredAt");
	
  END LOOP;
  FOR i IN 1..100 LOOP
  	SELECT "UserId" INTO "RandomUserId" FROM "ToDoUser" ORDER BY random() LIMIT 1;
  	SELECT "Id" INTO "RandomListId" FROM "ToDoList" ORDER BY random() LIMIT 1;
	  
	SELECT ('2025-07-01 10:00:00'::timestamp + (('2025-07-31 12:00:00'::timestamp - '2025-07-01 10:00:00'::timestamp)* random())) INTO "RandomRegisteredAt";
	SELECT ('2025-09-01 10:00:00'::timestamp + (('2025-09-30 12:00:00'::timestamp - '2025-09-01 10:00:00'::timestamp)* random())) INTO "RandomStateChangedAt";
	SELECT ('2025-10-01 10:00:00'::timestamp + (('2025-10-31 12:00:00'::timestamp - '2025-10-01 10:00:00'::timestamp)* random())) INTO "RandomDeadLine";
	
    INSERT INTO public."ToDoItem" ("Id", "UserId", "ListId", "Name", "CreatedAt", "State", "StateChangedAt", "DeadLine")
    VALUES (gen_random_uuid()::uuid, "RandomUserId", "RandomListId", 'Task_' || i::varchar(100), "RandomRegisteredAt", random()::int, "RandomStateChangedAt", "RandomDeadLine");
	
  END LOOP;
END
$$