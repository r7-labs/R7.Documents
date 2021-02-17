﻿-- NOTE: To manually execute this script you must 
-- replace {databaseOwner} and {objectQualifier} with real values. 
-- Defaults is "dbo." for database owner and "" for object qualifier 

IF EXISTS (select * from {databaseOwner}[{objectQualifier}ModuleDefinitions] where DefinitionName = N'R7.Documents')
BEGIN
    -- Remove definition added by new install
    DELETE FROM {databaseOwner}[{objectQualifier}ModuleDefinitions]
        WHERE DefinitionName = N'R7_Documents'

    -- Rename old definition
    UPDATE {databaseOwner}[{objectQualifier}ModuleDefinitions]
        SET DefinitionName = N'R7_Documents' WHERE DefinitionName = N'R7.Documents'
END
GO

IF NOT EXISTS (select * from sys.columns where object_id = object_id(N'{databaseOwner}[{objectQualifier}Documents_Documents]') and name = N'StartDate')
    ALTER TABLE {databaseOwner}[{objectQualifier}Documents_Documents]
        ADD StartDate datetime NULL,
            EndDate datetime NULL
GO

IF EXISTS (select * from sys.columns where object_id = object_id(N'{databaseOwner}[{objectQualifier}Documents_Documents]') and name = N'IsPublished')
BEGIN
    UPDATE {databaseOwner}[{objectQualifier}Documents_Documents]
        SET EndDate = GETDATE()
        WHERE IsPublished = 0

    ALTER TABLE {databaseOwner}[{objectQualifier}Documents_Documents]
        DROP COLUMN IsPublished
END
GO

-- Fix indices for pre-01.09.00 installs

ALTER TABLE {databaseOwner}[{objectQualifier}Documents_Documents]
    DROP CONSTRAINT [PK_{objectQualifier}Documents_Documents]

DROP INDEX {databaseOwner}[{objectQualifier}Documents_Documents].[IX_{objectQualifier}Documents_Documents]

ALTER TABLE {databaseOwner}[{objectQualifier}Documents_Documents]
    ADD CONSTRAINT [PK_{objectQualifier}Documents_Documents] PRIMARY KEY CLUSTERED (ItemID)

CREATE NONCLUSTERED INDEX [IX_{objectQualifier}Documents_Documents]
    ON {databaseOwner}[{objectQualifier}Documents_Documents] (ModuleID)

-- Updade SPs

IF EXISTS (select * from sys.procedures where name = N'{objectQualifier}Documents_GetDocument')
    DROP PROCEDURE {databaseOwner}[{objectQualifier}Documents_GetDocument]
GO

CREATE PROCEDURE {databaseOwner}[{objectQualifier}Documents_GetDocument]
    @ItemId   int,
    @ModuleId int
AS
BEGIN
    SELECT D.*,
       {databaseOwner}[{objectQualifier}UserDisplayName] (D.CreatedByUserID) AS CreatedByUser,
       {databaseOwner}[{objectQualifier}UserDisplayName] (D.ModifiedByUserID) AS ModifiedByUser,
       {databaseOwner}[{objectQualifier}UserDisplayName] (D.OwnedByUserID) AS OwnedByUser,
       F.Size,
       UT.TrackClicks,
       UT.Clicks,
       UT.NewWindow
    FROM {databaseOwner}[{objectQualifier}Documents_Documents] D
        LEFT JOIN {databaseOwner}[{objectQualifier}UrlTracking] UT
            ON D.URL = UT.Url AND D.ModuleID = UT.ModuleID
        LEFT JOIN {databaseOwner}[{objectQualifier}Files] F
            ON D.URL = 'fileid=' + CONVERT (varchar, F.FileID)
    WHERE D.ItemID = @ItemId AND D.ModuleID = @ModuleId
END
GO

IF EXISTS (select * from sys.procedures where name = N'{objectQualifier}Documents_GetDocuments')
    DROP PROCEDURE {databaseOwner}[{objectQualifier}Documents_GetDocuments]
GO

CREATE PROCEDURE {databaseOwner}[{objectQualifier}Documents_GetDocuments]
    @ModuleId int,
    @PortalId int -- ignored
AS
BEGIN
    SELECT D.*,
           {databaseOwner}[{objectQualifier}UserDisplayName] (D.CreatedByUserID) AS CreatedByUser,
           {databaseOwner}[{objectQualifier}UserDisplayName] (D.ModifiedByUserID) AS ModifiedByUser,
           {databaseOwner}[{objectQualifier}UserDisplayName] (D.OwnedByUserID) AS OwnedByUser,
           F.Size,
           UT.TrackClicks,
           UT.Clicks,
           UT.NewWindow
        FROM {databaseOwner}[{objectQualifier}Documents_Documents] D
            LEFT JOIN {databaseOwner}[{objectQualifier}UrlTracking] UT
                ON D.URL = UT.Url AND D.ModuleID = UT.ModuleID
            LEFT JOIN {databaseOwner}[{objectQualifier}Files] F
                ON D.URL = 'fileid=' + CONVERT (varchar, F.FileID)
    WHERE D.ModuleId = @ModuleId
    ORDER BY D.Title
END
GO
