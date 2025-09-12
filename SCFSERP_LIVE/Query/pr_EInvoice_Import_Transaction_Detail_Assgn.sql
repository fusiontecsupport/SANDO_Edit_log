create PROCEDURE pr_EInvoice_Import_Transaction_Detail_Assgn @PTranMID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DECLARE @TableMaster TABLE
    ( 
	   PrdDesc  varchar(50),
	   HsnCd varchar(15),
	   Qty int,
	   UnitCode varchar(15),
	   UnitPrice numeric(18,2),
	   TotAmt  numeric(18,2),
	   AssAmt numeric(18,2),
	   GstRt numeric(18,2),
	   IgstAmt numeric(18,2),
	   CgstAmt numeric(18,2),
	   SgstAmt numeric(18,2),
	   TotItemVal numeric(18,2)	


    )

	Insert into @TableMaster(PrdDesc, HsnCd, Qty, UnitCode, UnitPrice, TotAmt, AssAmt, GstRt, IgstAmt, CgstAmt, SgstAmt, TotItemVal)
	Select 'Storage Charges',STRG_HSNCODE, 1, 'Nos', STRG_TAXABLE_AMT, STRG_TAXABLE_AMT, STRG_TAXABLE_AMT, 18, STRG_IGST_AMT, STRG_CGST_AMT,
	STRG_SGST_AMT, STRG_TAXABLE_AMT + STRG_IGST_AMT + STRG_CGST_AMT + STRG_SGST_AMT  
	from Z_IMPORT_EINVOICE_DETAILS Where STRG_TAXABLE_AMT > 0 And TRANMID = @PTranMID

	Insert into @TableMaster(PrdDesc, HsnCd, Qty, UnitCode, UnitPrice, TotAmt, AssAmt, GstRt, IgstAmt, CgstAmt, SgstAmt, TotItemVal)
	Select 'Handling Charges',HANDL_HSNCODE, 1, 'Nos', HANDL_TAXABLE_AMT, HANDL_TAXABLE_AMT, HANDL_TAXABLE_AMT, 18, HANDL_IGST_AMT, HANDL_CGST_AMT,
	HANDL_SGST_AMT, HANDL_TAXABLE_AMT + HANDL_IGST_AMT + HANDL_SGST_AMT + HANDL_SGST_AMT  
	from Z_IMPORT_EINVOICE_DETAILS Where HANDL_TAXABLE_AMT > 0 And TRANMID = @PTranMID

	Select * From @TableMaster

END

