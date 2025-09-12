alter PROCEDURE [dbo].[pr_Report_Import_Invoice_Detail] 
        @PCompyId INT
		,@PTRANBTYPE int
		,@PREGSTRID int
        , @PSDate Smalldatetime
        , @PEDate Smalldatetime
 
AS BEGIN

Declare @LCompyId int,@LTRANBTYPE int,@LREGSTRID int,@LSDate Smalldatetime, @LEDate Smalldatetime
set @LCompyId = @PCompyId
set @LTRANBTYPE=@PTRANBTYPE
set @LREGSTRID=@PREGSTRID
set @LSDate = @PSDate
set @LEDate = @PEDate


    DECLARE @TableMaster TABLE
    (
		[Sl No.] int
    , [Bill no]  VARCHAR(25)
	, [Bill Date] datetime
--	, [CHA] VARCHAR(150)
	, [Billing CHA] VARCHAR(150)
	, [Importer Name] VARCHAR(150)
	, [Steamer Name] VARCHAR(150)    
	, [St Date] datetime
	, [Ed Date] datetime
	, [Days] int
	, [Storage GAmt] numeric(18,2)
	, [Storage Gst] numeric(18,2)
	, [Storage NetAmt] numeric(18,2)	
	, [Handling GAmt] numeric(18,2)
	, [Handling Gst] numeric(18,2)
	, [Handling NetAmt] numeric(18,2)
	, [BSBilling CHA] VARCHAR(150)
	, [BS Bill no]  VARCHAR(25)
	, [BS Bill Date] datetime
	, [BSStorage GAmt] numeric(18,2)
	, [BSStorage Gst] numeric(18,2)
	, [BSStorage NetAmt] numeric(18,2)
	, [BSHandling GAmt] numeric(18,2)
	, [BSHandling Gst] numeric(18,2)
	, [BSHandling NetAmt] numeric(18,2)
	, [Container No] varchar(25)
	, [Container Size] varchar(25)
	, [Type] varchar(15)
	, [Billed Category] varchar(25)
	, [BS Type] varchar(15)
	, [BS Category] varchar(25)
	, [Tally CHA] VARCHAR(150)
	, [Licence CHA] VARCHAR(150)
    )
				
    INSERT INTO @TableMaster( [Bill no] , [Bill Date],  [Billing CHA] , 	[Importer Name] ,  [Steamer Name] ,  
	[Container No], [Container Size], [St Date] , [Ed Date] , [Days], [Storage GAmt] ,
	[Storage Gst] , [Storage NetAmt] , [Handling GAmt] , [Handling Gst] , [Handling NetAmt] ,  [Type] , [Billed Category],
	[BSBilling CHA] ,  [BS Bill no]  , [BS Bill Date] , [BSStorage GAmt] ,  [BSStorage Gst] , [BSStorage NetAmt] ,
	[BSHandling GAmt] ,  [BSHandling Gst] , [BSHandling NetAmt], [BS Type] , [BS Category], [Tally CHA] , [Licence CHA], [Sl No.])
  
    SELECT   impbill.TRANDNO, impbill.TRANDATE, impbill.TRANREFNAME, impgidtl.IMPRTNAME, impgidtl.STMRNAME, 
	impgidtl.CONTNRNO, conts.CONTNRSDESC, impbilldtl.TRANSDATE, impbilldtl.TRANEDATE, datediff(D,impbilldtl.TRANSDATE,impbilldtl.TRANEDATE), impbill.STRG_TAXABLE_AMT,
	impbill.STRG_CGST_AMT+impbill.STRG_SGST_AMT+impbill.STRG_IGST_AMT,  impbill.STRG_TAXABLE_AMT+ impbill.STRG_CGST_AMT+impbill.STRG_SGST_AMT+impbill.STRG_IGST_AMT,
	impbill.HANDL_TAXABLE_AMT,impbill.HANDL_CGST_AMT+impbill.HANDL_SGST_AMT+impbill.HANDL_IGST_AMT,  impbill.HANDL_TAXABLE_AMT+ impbill.HANDL_CGST_AMT+impbill.HANDL_SGST_AMT+impbill.HANDL_IGST_AMT,
	impbill.TRANBTYPE, impbill.REGSTRID, impbs.TRANREFNAME, impbs.TRANDNO, impbs.TRANDATE, 
	impbs.STRG_TAXABLE_AMT, impbs.STRG_CGST_AMT+impbs.STRG_SGST_AMT+impbs.STRG_IGST_AMT,  impbs.STRG_TAXABLE_AMT+ impbs.STRG_CGST_AMT+impbs.STRG_SGST_AMT+impbs.STRG_IGST_AMT,
	impbs.HANDL_TAXABLE_AMT,impbs.HANDL_CGST_AMT+impbs.HANDL_SGST_AMT+impbs.HANDL_IGST_AMT,  impbs.HANDL_TAXABLE_AMT+ impbs.HANDL_CGST_AMT+impbs.HANDL_SGST_AMT+impbs.HANDL_IGST_AMT,
	impbs.TRANBTYPE, impbs.REGSTRID, opnshtm.OSMNAME, opnshtm.OSMLNAME
            , Row_Number() OVER (
            ORDER BY
			impbill.TRANDNO, impbill.TRANDATE, impbs.TRANREFNAME, impbs.TRANREFBNAME, impgidtl.IMPRTNAME, impgidtl.STMRNAME, impbs.TRANREFBNAME, impbs.TRANDNO, impbs.TRANDATE
            ) AS RowNum
            
FROM       TRANSACTIONMASTER impbill(nolock) 
				join TRANSACTIONDETAIL impbilldtl (nolock) on impbill.TRANMID = impbilldtl.TRANMID
				join GATEINDETAIL impgidtl (nolock) on impbilldtl.TRANDREFID = impgidtl.GIDID
				join CONTAINERSIZEMASTER conts (nolock) on impgidtl.CONTNRSID = conts.CONTNRSID
				left join TRANSACTIONMASTER impbs (nolock) on impbill.tranmid = impbs.TRANLMID and impbs.REGSTRID = 65
					and (impbs.TRANBTYPE=@LTRANBTYPE or @LTRANBTYPE =0) and (impbs.TRANDATE BETWEEN @LSDate AND @LEDate) and impbill.COMPYID = impbs.COMPYID
					left join TRANSACTIONDETAIL impbsdtl (nolock) on impbs.TRANMID = impbsdtl.TRANMID and impbilldtl.TRANDREFID = impbsdtl.TRANDREFID
				left join OPENSHEETDETAIL opnshtd(nolock) on impgidtl.GIDID =opnshtd.GIDID
				left join OPENSHEETMASTER opnshtm(nolock) on opnshtd.OSMID =opnshtm.OSMID
WHERE (impbill.COMPYID = @LCompyId) 
and (impbill.REGSTRID=@LREGSTRID 
or (@PREGSTRID= 0 and (impbill.REGSTRID in(1,2,6)))
or (@PREGSTRID= 99 and (impbill.TRAN_PULSE_STRG_TYPE=1 and impbs.TRANLMID is null) )) 
	and (impbill.TRANBTYPE=@LTRANBTYPE or @LTRANBTYPE =0) 
	and (impbill.TRANDATE BETWEEN @LSDate AND @LEDate) 
	AND (impbill.SDPTID=1) 
			   
update @TableMaster
set Type = case when type = '1' then 'Load' 
				when type = '2' then 'De-Stuff' end
update @TableMaster
set [BS Type] = case when [BS Type] = '1' then 'Load' 
				when [BS Type] = '2' then 'De-Stuff' end

update @TableMaster
set [Billed Category] = REGSTRDESC
from @TableMaster join EXPORT_INVOICE_REGISTER b(nolock) on [Billed Category] = regstrid

update @TableMaster
set [BS Category] = REGSTRDESC
from @TableMaster join EXPORT_INVOICE_REGISTER b(nolock) on [BS Category] = regstrid

    SELECT [Sl No.],[Bill no] , [Bill Date],  [Billing CHA] , 	[Importer Name] ,  [Steamer Name] ,  
	[Container No], [Container Size], [St Date] , [Ed Date] , [Days], [Storage GAmt] ,
	[Storage Gst] , [Storage NetAmt] , [Handling GAmt] , [Handling Gst] , [Handling NetAmt] ,  [Type] , [Billed Category],
	[BSBilling CHA] ,  [BS Bill no]  , [BS Bill Date] , [BSStorage GAmt] ,  [BSStorage Gst] , [BSStorage NetAmt] ,
	[BSHandling GAmt] ,  [BSHandling Gst] , [BSHandling NetAmt], [BS Type] , [BS Category], [Tally CHA] , [Licence CHA]
    FROM    @TableMaster    
    ORDER BY [Sl No.]

END





