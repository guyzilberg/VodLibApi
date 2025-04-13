-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SP_UserGetByID]
	@UserID INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT 
		*
	FROM
		Users_T
	WHERE 
		UserID = @UserID
END
