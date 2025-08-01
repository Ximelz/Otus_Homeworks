CREATE TABLE "ToDoUser"
(
	"UserId" uuid PRIMARY KEY,
	"TelegramUserId" bigint NOT NULL,
	"TelegramUserName" varchar(100) NOT NULL,
	"RegisteredAt" date DEFAULT CURRENT_DATE
);
CREATE INDEX idx_users_id ON public."ToDoUser" USING btree ("UserId");
CREATE INDEX idx_telegram_users_id ON public."ToDoUser" USING btree ("TelegramUserId");


CREATE TABLE "ToDoList"
(
	"Id" uuid PRIMARY KEY,
	"UserId" uuid NOT NULL,
	"Name" varchar(100) NOT NULL,
	"CreatedAt" date DEFAULT CURRENT_DATE,
	
	CONSTRAINT "List_UserId"
        FOREIGN KEY ("UserId") 
        REFERENCES "ToDoUser"("UserId")
        ON DELETE CASCADE
);
CREATE INDEX idx_list_id ON public."ToDoList" USING btree ("Id");

CREATE TABLE "ToDoItem"
(
	"Id" uuid PRIMARY KEY,
	"UserId" uuid NOT NULL,
	"ListId" uuid NOT NULL,
	"Name" varchar(100) NOT NULL,
	"CreatedAt" date DEFAULT CURRENT_DATE,
	"State" int NOT NULL,
	"StateChangedAt" date NULL,
	"DeadLine" date NULL,

	
	CONSTRAINT "ToDoItem_UserId"
        FOREIGN KEY ("UserId") 
        REFERENCES "ToDoUser"("UserId")
        ON DELETE CASCADE,
		
	CONSTRAINT "ToDoItem_ListId"
        FOREIGN KEY ("ListId") 
        REFERENCES "ToDoList"("Id")
        ON DELETE CASCADE	
);
CREATE INDEX idx_item_id ON public."ToDoItem" USING btree ("Id");