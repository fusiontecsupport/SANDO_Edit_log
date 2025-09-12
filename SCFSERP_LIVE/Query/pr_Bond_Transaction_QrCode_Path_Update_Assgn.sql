
CREATE PROCEDURE [dbo].[pr_Bond_Transaction_QrCode_Path_Update_Assgn] @PTranMID int, @PPath varchar(max)
AS
BEGIN

	SET NOCOUNT ON;

	Update BONDTRANSACTIONMASTER Set QRCODEPATH = @PPath Where TRANMID = @PTranMID

END