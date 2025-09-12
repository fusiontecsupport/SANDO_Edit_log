CREATE PROCEDURE [dbo].[pr_CreditNote_IRN_Transaction_Update_Assgn] @PTranMID int, @PIRNNO varchar(100), @PACKNO varchar(50),
@PACKDT DATETIME
AS
BEGIN

	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Update CREDITNOTE_TRANSACTIONMASTER Set
	IRNNO = @PIRNNO,
	ACKNO = @PACKNO,
	ACKDT = @PACKDT,
	TALLYSTAT = 1
	Where TRANMID = @PTranMID


END



