﻿-- NOTE: To manually execute this script you must 
-- replace {databaseOwner} and {objectQualifier} with real values. 
-- Defaults is "dbo." for database owner and "" for object qualifier 

IF EXISTS (select * from sys.procedures where name = N'{objectQualifier}Documents_GetDocument')
    DROP PROCEDURE {databaseOwner}[{objectQualifier}Documents_GetDocument]
GO

CREATE PROCEDURE {databaseOwner}[{objectQualifier}Documents_GetDocument]
    @ItemId   int,
    @ModuleId int
AS
BEGIN
    SELECT D.*,
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

DELETE FROM {databaseOwner}[{objectQualifier}PackageDependencies]
	WHERE PackageID = (select PackageID from {databaseOwner}[{objectQualifier}Packages] where Name = N'R7.Documents')
	AND PackageName = N'R7.DotNetNuke.Extensions'
GO