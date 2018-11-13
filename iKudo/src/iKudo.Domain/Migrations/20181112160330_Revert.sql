IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'HistoryBoards'))
BEGIN
	DROP TABLE [HistoryBoards]
END

IF EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'Trg_Upd_HistoryBoards'))
BEGIN
	DROP TRIGGER [Trg_Upd_HistoryBoards]
END

IF EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'Trg_Del_HistoryBoards'))
BEGIN
	DROP TRIGGER [Trg_Del_HistoryBoards]
END
GO