CREATE TABLE [HistoryBoards](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BoardId] [int] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[CreatorId] [nvarchar](max) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[ModificationDate] [datetime] NULL,
	[Name] [nvarchar](max) NOT NULL,
	[IsPrivate] [bit] NOT NULL,
	[AcceptanceType] [int] NOT NULL,
	[Operation] [nchar](1) NOT NULL,
	[OperationDate] [datetime] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TRIGGER [Trg_Upd_HistoryBoards]
ON [Boards]
AFTER UPDATE
AS  
BEGIN
	INSERT INTO HistoryBoards
			   (BoardId		,CreationDate			,CreatorId			,Description			,ModificationDate			,Name			,IsPrivate			,AcceptanceType,		Operation,  OperationDate)
		 SELECT deleted.Id,	deleted.CreationDate,	deleted.CreatorId,	deleted.Description,	deleted.ModificationDate,	deleted.Name,	deleted.IsPrivate,	deleted.AcceptanceType,	'U',        GETUTCDATE()     
		 FROM deleted
END
GO

CREATE TRIGGER [Trg_Del_HistoryBoards]
ON [Boards]
AFTER DELETE
AS  
BEGIN
	INSERT INTO HistoryBoards
			   (BoardId		,CreationDate			,CreatorId			,Description			,ModificationDate	        ,Name			,IsPrivate			,AcceptanceType,		Operation,  OperationDate)
		 SELECT deleted.Id,	deleted.CreationDate,	deleted.CreatorId,	deleted.Description,	deleted.ModificationDate,	deleted.Name,	deleted.IsPrivate,	deleted.AcceptanceType,	'D',        GETUTCDATE()
		 FROM deleted
END
GO

ALTER TABLE [Boards] ENABLE TRIGGER [Trg_Del_HistoryBoards]
GO
