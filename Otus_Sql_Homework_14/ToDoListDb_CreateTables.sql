CREATE TABLE "ToDoUser"
(
	"UserId" uuid PRIMARY KEY,
	"TelegramUserId" bigint NOT NULL,
	"TelegramUserName" varchar(100) NOT NULL,
	"RegisteredAt" date DEFAULT CURRENT_DATE
);
CREATE INDEX idx_users_id ON public."ToDoUser" ("UserId");
CREATE UNIQUE INDEX idx_telegram_users_id ON public."ToDoUser" ("TelegramUserId");


CREATE TABLE "ToDoList"
(
	"Id" uuid PRIMARY KEY,
	"UserId" uuid NOT NULL,
	"Name" varchar(100) NOT NULL,
	"CreatedAt" date DEFAULT CURRENT_DATE,
	
	CONSTRAINT "List_UserId"
        FOREIGN KEY ("UserId") 
        REFERENCES "ToDoUser"("UserId")
);
CREATE INDEX idx_list_id ON public."ToDoList" ("Id");

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
        REFERENCES "ToDoUser"("UserId"),
		
	CONSTRAINT "ToDoItem_ListId"
        FOREIGN KEY ("ListId") 
        REFERENCES "ToDoList"("Id")
);
CREATE INDEX idx_item_id ON public."ToDoItem" ("Id");

CREATE TABLE "Notification"
(
	"Id" uuid PRIMARY KEY,
	"UserId" uuid NOT NULL,
	"Type" varchar(100) NOT NULL,
	"Text" varchar(1000) NOT NULL,
	"ScheduledAt" date NOT NULL DEFAULT CURRENT_DATE,
	"IsNotified" bool DEFAULT FALSE,
	"NotifiedAt" date NULL,

	
	CONSTRAINT "Notification_UserId"
        FOREIGN KEY ("UserId") 
        REFERENCES "ToDoUser"("UserId"),
);
CREATE INDEX idx_notification_id ON public."Notification" ("Id");
CREATE INDEX idx_userInNotification_id ON public."Notification" ("UserId");