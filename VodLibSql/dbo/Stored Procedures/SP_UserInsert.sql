-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SP_UserInsert]
	
	@Username NVARCHAR(50),
	@Password NVARCHAR(512),
	@PasswordSalt NVARCHAR(512),
	@UserRole INT = 1,
	@Email NVARCHAR(512)
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO Users_T(Username, [password],PasswordSalt, UserRoles, Email)
	VALUES (@Username, @Password,@PasswordSalt, @UserRole, @Email)

	SELECT SCOPE_IDENTITY();
END
