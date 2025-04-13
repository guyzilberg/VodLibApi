CREATE PROCEDURE [dbo].[SP_FileDeleteByID]
	@FileID INT
AS
BEGIN
/*maybe better just to change status to deleted?*/
	DELETE
	FROM Files_T
	WHERE FileID = @FileID
END
