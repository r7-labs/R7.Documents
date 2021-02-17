-- NOTE: To manually execute this script you must 
-- replace {databaseOwner} and {objectQualifier} with real values. 
-- Defaults is "dbo." for database owner and "" for object qualifier 

-- Drop tables

ALTER TABLE {databaseOwner}[{objectQualifier}Documents_Documents] DROP CONSTRAINT [FK_{objectQualifier}Documents_Documents_Modules]
GO

DROP TABLE {databaseOwner}[{objectQualifier}Documents_Documents]
GO

-- Drop stored procedures

DROP PROCEDURE {databaseOwner}[{objectQualifier}Documents_GetDocument]
GO

DROP PROCEDURE {databaseOwner}[{objectQualifier}Documents_GetDocuments]
GO
