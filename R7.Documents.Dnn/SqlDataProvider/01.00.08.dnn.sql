-- NOTE: To manually execute this script you must 
-- replace {databaseOwner} and {objectQualifier} with real values. 
-- Defaults is "dbo." for database owner and "" for object qualifier 

-- Alter tables

ALTER TABLE {databaseOwner}[{objectQualifier}Documents_Documents]
	ALTER COLUMN [Title] nvarchar(255) NULL
GO

ALTER TABLE {databaseOwner}[{objectQualifier}Documents_Documents]
	ALTER COLUMN [Description] nvarchar(max) NULL
GO
