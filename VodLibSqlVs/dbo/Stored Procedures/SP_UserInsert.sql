-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SP_UserInsert]
	@UserRole INT,
	@Username NVARCHAR(50),
	@Password NVARCHAR(512),
	@PasswordSalt NVARCHAR(512)
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO Users_T(Username, [password], UserRoles, PasswordSalt)
	VALUES (@Username, @Password,@UserRole, PasswordSalt)
END