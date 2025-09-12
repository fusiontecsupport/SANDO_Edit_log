
CREATE PROCEDURE [dbo].[pr_IRN_Bond_Transaction_Update_Assgn] @PTranMID int, @PIRNNO varchar(100), @PACKNO varchar(50),
@PACKDT DATETIME
AS
BEGIN

	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Update BONDTRANSACTIONMASTER Set
	IRNNO = @PIRNNO,
	ACKNO = @PACKNO,
	ACKDT = @PACKDT
	Where TRANMID = @PTranMID


END
