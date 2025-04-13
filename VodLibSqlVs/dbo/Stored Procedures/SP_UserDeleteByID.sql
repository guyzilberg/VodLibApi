-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SP_UserDeleteByID]
	@UserID INT
AS
BEGIN
	SET NOCOUNT ON;

	DELETE Users_T
	WHERE 
		UserID = @UserID
END