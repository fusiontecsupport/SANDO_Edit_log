CREATE PROCEDURE pr_Transaction_CreditNote_QrCode_Path_Update_Assgn @PTranMID int, @PPath varchar(max)
AS
BEGIN

	SET NOCOUNT ON;

	Update CREDITNOTE_TRANSACTIONMASTER Set QRCODEPATH = @PPath Where TRANMID = @PTranMID

END
