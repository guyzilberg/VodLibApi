CREATE PROCEDURE [dbo].[SP_UserGetByUsername]
	@Username nvarchar(50)
AS
BEGIN
	SELECT 
		TOP 1  *
	FROM
		Users_T
	WHERE
		Users_T.Username = @Username
END
