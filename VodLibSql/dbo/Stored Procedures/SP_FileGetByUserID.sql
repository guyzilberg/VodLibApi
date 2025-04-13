CREATE PROCEDURE [dbo].[SP_FileGetByUserID]
	@FileID INT
AS
BEGIN
	SELECT 
		*
	FROM
		Files_T
	WHERE
		FileID = @FileID
END
