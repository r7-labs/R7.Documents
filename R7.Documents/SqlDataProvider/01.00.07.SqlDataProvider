-- NOTE: To manually execute this script you must 
-- replace {databaseOwner} and {objectQualifier} with real values. 
-- Defaults is "dbo." for database owner and "" for object qualifier 

-- Alter tables

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = object_id(N'{databaseOwner}[{objectQualifier}Documents_Documents]') AND name = N'LinkAttributes')
	ALTER TABLE {databaseOwner}[{objectQualifier}Documents_Documents]
		ADD [LinkAttributes] nvarchar(255) NULL
GO

-- Drop stored procedures 

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'{databaseOwner}{objectQualifier}Documents_GetDocument') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE {databaseOwner}{objectQualifier}Documents_GetDocument
GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'{databaseOwner}{objectQualifier}Documents_GetDocuments') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE {databaseOwner}{objectQualifier}Documents_GetDocuments
GO

-- Create stored procedures

CREATE PROCEDURE {databaseOwner}{objectQualifier}Documents_GetDocument
	@ItemId   int,
	@ModuleId int
AS
SELECT D.*,
       CreatedByUser.DisplayName AS "CreatedByUser",
       OwnedByUser.DisplayName AS "OwnedByUser",
       ModifiedByUser.DisplayName AS "ModifiedByUser",       
       F.Size,
       F.ContentType,
       UT.TrackClicks,
       UT.Clicks,
       UT.NewWindow
FROM {databaseOwner}{objectQualifier}Documents_Documents AS D
LEFT OUTER JOIN {databaseOwner}{objectQualifier}Users AS CreatedByUser on D.CreatedByUserID = CreatedByUser.UserId 
LEFT OUTER JOIN {databaseOwner}{objectQualifier}Users AS OwnedByUser on D.OwnedByUserID = OwnedByUser.UserId
LEFT OUTER JOIN {databaseOwner}{objectQualifier}Users AS ModifiedByUser on D.ModifiedByUserID = ModifiedByUser.UserId
LEFT OUTER JOIN {databaseOwner}{objectQualifier}UrlTracking AS UT on D.URL = UT.Url and UT.ModuleId = @ModuleID
LEFT OUTER JOIN {databaseOwner}{objectQualifier}Files AS F on D.URL = 'fileid=' + convert(varchar,F.FileID)
WHERE D.ItemId = @ItemId and D.ModuleId = @ModuleId
GO

CREATE PROCEDURE {databaseOwner}{objectQualifier}Documents_GetDocuments
	@ModuleId int,
	@PortalId int
AS
SELECT D.*,
       CreatedByUser.DisplayName AS "CreatedByUser",
       OwnedByUser.DisplayName AS "OwnedByUser",
       ModifiedByUser.DisplayName AS "ModifiedByUser",
       F.Size,
       UT.TrackClicks,
       UT.Clicks,
       UT.NewWindow
FROM {databaseOwner}{objectQualifier}Documents_Documents AS D
LEFT OUTER JOIN {databaseOwner}{objectQualifier}Users AS CreatedByUser on D.CreatedByUserID = CreatedByUser.UserId 
LEFT OUTER JOIN {databaseOwner}{objectQualifier}Users AS OwnedByUser on D.OwnedByUserID = OwnedByUser.UserId
LEFT OUTER JOIN {databaseOwner}{objectQualifier}Users AS ModifiedByUser on D.ModifiedByUserID = ModifiedByUser.UserId
LEFT OUTER JOIN {databaseOwner}{objectQualifier}Files AS F on D.URL = 'fileid=' + convert(varchar, F.FileID)
LEFT OUTER JOIN {databaseOwner}{objectQualifier}UrlTracking AS UT on D.URL = UT.Url and UT.ModuleId = @ModuleID
WHERE D.ModuleId = @ModuleId 
ORDER by D.Title
GO
