CREATE PROCEDURE [dbo].[SP_FileInsert]

 @FilePath NVARCHAR(400)
 ,@FileExtension NVARCHAR(100)
 ,@UserID INT
 ,@FilePublicTitle NVARCHAR(400)
 ,@FileSafeTitle NVARCHAR(400)
 ,@SavedFileName NVARCHAR(300)
 ,@FileType INT
 ,@FileStatus INT
AS
BEGIN
	INSERT INTO Files_T
        ([FilePath]
        ,[FileExtension]
        ,[UserID]
        ,[FilePublicTitle]
        ,[FileSafeTitle]
        ,[SavedFileName]
        ,[FileType]
        ,[FileStatus])
    VALUES
        (@FilePath
        ,@FileExtension
        ,@UserID
        ,@FilePublicTitle
        ,@FileSafeTitle
        ,@SavedFileName
        ,@FileType
        ,@FileStatus)

    SELECT SCOPE_IDENTITY();
END
