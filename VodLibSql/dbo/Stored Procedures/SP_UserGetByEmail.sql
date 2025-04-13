CREATE PROCEDURE [dbo].[SP_UserGetByEmail]
	@Email nvarchar(50)
AS
BEGIN
	SELECT 
		TOP 1  *
	FROM
		Users_T
	WHERE
		Users_T.Email = @Email
END