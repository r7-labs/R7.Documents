-- NOTE: To manually execute this script you must
-- replace {databaseOwner} and {objectQualifier} with real values.
-- Defaults is "dbo." for database owner and "" for object qualifier

IF NOT EXISTS (select * from sys.columns where object_id = object_id(N'{databaseOwner}[{objectQualifier}Documents_Documents]') and name = N'ParentDocumentID')
ALTER TABLE {databaseOwner}[{objectQualifier}Documents_Documents]
    ADD [ParentDocumentID] int NULL
GO
