CREATE PROCEDURE [dbo].[SP_FileUpdateByID]
  @FileID INT 
 ,@FilePath NVARCHAR(400)
 ,@FileExtension NVARCHAR(100)
 ,@UserID INT
 ,@FilePublicTitle NVARCHAR(400)
 ,@FileSafeTitle NVARCHAR(400)
 ,@SavedFileName NVARCHAR(300)
 ,@FileType INT
 ,@FileStatus INT
AS
BEGIN
	UPDATE Files_T
	SET
		[FilePath] = @FilePath
        ,[FileExtension] = @FileExtension
        ,[UserID] = @UserID
        ,[FilePublicTitle] = @FilePublicTitle
        ,[FileSafeTitle] = @FileSafeTitle
        ,[SavedFileName] = @SavedFileName
        ,[FileType] = @FileType
        ,[FileStatus] = @FileStatus
    WHERE
        FileID = @FileID
END
