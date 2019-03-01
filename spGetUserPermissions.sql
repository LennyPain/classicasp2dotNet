SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE spGetUserPermissions
	@UserID	int	= 0
AS
	SELECT userPermissions FROM tbUserPermissions
	WHERE UserID = @UserID
	ORDER By userPermissions
GO