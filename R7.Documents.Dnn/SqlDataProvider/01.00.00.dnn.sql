-- NOTE: To manually execute this script you must 
-- replace {databaseOwner} and {objectQualifier} with real values. 
-- Defaults is "dbo." for database owner and "" for object qualifier 

-- Create tables

IF NOT EXISTS (select * from dbo.sysobjects where id = object_id(N'{databaseOwner}[{objectQualifier}Documents_Documents]') and OBJECTPROPERTY(id, N'IsTable') = 1)
	BEGIN
		CREATE TABLE {databaseOwner}[{objectQualifier}Documents_Documents]
		(
			ItemID int IDENTITY(1, 1) NOT NULL,
			ModuleID int NOT NULL,
			CreatedDate datetime NULL,
			URL nvarchar (250) NULL,
			Title nvarchar (150) NULL,
			Category nvarchar (50) NULL,
			CreatedByUserID INT NULL, 
			OwnedByUserID INT NULL, 
			ModifiedByUserID INT NULL, 
			ModifiedDate datetime NULL, 
			SortOrderIndex INT NULL, 
			Description nvarchar (255) NULL,
			ForceDownload BIT,
			IsPublished BIT NOT NULL
            CONSTRAINT [PK_{objectQualifier}Documents_Documents] PRIMARY KEY CLUSTERED (ModuleID)
            CONSTRAINT [FK_{objectQualifier}Documents_Documents_Modules] FOREIGN KEY (ModuleID)
                REFERENCES {databaseOwner}[{objectQualifier}Modules] (ModuleID) ON DELETE CASCADE
		)

        CREATE NONCLUSTERED INDEX [IX_{objectQualifier}Documents_Documents]
            ON {databaseOwner}[{objectQualifier}Documents_Documents] (ModuleID)
	END
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
	@ItemId   INT,
	@ModuleId INT
AS
SELECT D.Itemid,
       D.Moduleid,
       D.Title,
       D.Url,
       D.Category,
       CreatedByUser.DisplayName AS "CreatedByUser",
       OwnedByUser.DisplayName AS "OwnedByUser",
       ModifiedByUser.DisplayName AS "ModifiedByUser",       
       F.Size,
       F.ContentType,
       UT.TrackClicks,
       UT.Clicks,
       UT.NewWindow,
       D.OwnedByUserID, 
       D.ModifiedByUserID, 
       D.ModifiedDate,
       D.CreatedByUserID, 
       D.CreatedDate, 
       D.SortOrderIndex,
       D.Description,
	   D.ForceDownload,
	   D.IsPublished
FROM {databaseOwner}{objectQualifier}Documents_Documents AS D
LEFT OUTER JOIN {databaseOwner}{objectQualifier}Users AS CreatedByUser on D.CreatedByUserID = CreatedByUser.UserId 
LEFT OUTER JOIN {databaseOwner}{objectQualifier}Users AS OwnedByUser on D.OwnedByUserID = OwnedByUser.UserId
LEFT OUTER JOIN {databaseOwner}{objectQualifier}Users AS ModifiedByUser on D.ModifiedByUserID = ModifiedByUser.UserId
LEFT OUTER JOIN {databaseOwner}{objectQualifier}UrlTracking AS UT on D.URL = UT.Url and UT.ModuleId = @ModuleID
LEFT OUTER JOIN {databaseOwner}{objectQualifier}Files AS F on D.URL = 'fileid=' + convert(varchar,F.FileID)
WHERE D.ItemId = @ItemId and D.ModuleId = @ModuleId

GO

CREATE PROCEDURE {databaseOwner}{objectQualifier}Documents_GetDocuments
	@ModuleId INT,
	@PortalId INT
AS
SELECT D.ItemId,
       D.Moduleid,
       D.Title,
       D.Url,
       CreatedByUser.DisplayName AS "CreatedByUser",
       OwnedByUser.DisplayName AS "OwnedByUser",
       ModifiedByUser.DisplayName AS "ModifiedByUser",       
       D.Category,
       F.Size,
       UT.TrackClicks,
       UT.Clicks,
       UT.NewWindow,
       D.OwnedByUserID, 
       D.ModifiedByUserID, 
       D.ModifiedDate,
       D.CreatedByUserID, 
       D.CreatedDate, 
       D.SortOrderIndex,
       D.Description,
	   D.ForceDownload,
	   D.IsPublished
FROM {databaseOwner}{objectQualifier}Documents_Documents AS D
LEFT OUTER JOIN {databaseOwner}{objectQualifier}Users AS CreatedByUser on D.CreatedByUserID = CreatedByUser.UserId 
LEFT OUTER JOIN {databaseOwner}{objectQualifier}Users AS OwnedByUser on D.OwnedByUserID = OwnedByUser.UserId
LEFT OUTER JOIN {databaseOwner}{objectQualifier}Users AS ModifiedByUser on D.ModifiedByUserID = ModifiedByUser.UserId
LEFT OUTER JOIN {databaseOwner}{objectQualifier}Files AS F on D.URL = 'fileid=' + convert(varchar, F.FileID)
LEFT OUTER JOIN {databaseOwner}{objectQualifier}UrlTracking AS UT on D.URL = UT.Url and UT.ModuleId = @ModuleID
WHERE D.ModuleId = @ModuleId 
ORDER by D.Title
GO
