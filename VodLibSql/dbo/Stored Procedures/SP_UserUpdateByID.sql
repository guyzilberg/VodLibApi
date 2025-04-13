-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SP_UserUpdateByID]
	@UserID INT,
	@UserRole INT,
	@Username NVARCHAR(50),
	@Password NVARCHAR(512),
	@PasswordSalt NVARCHAR(512),
	@Email NVARCHAR(512)
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE Users_T
	SET
		Username = @Username,
		[password] = @Password,
		UserRoles = @UserRole,
		PasswordSalt = @PasswordSalt,
		Email = @Email
	WHERE 
		UserID = @UserID
END
