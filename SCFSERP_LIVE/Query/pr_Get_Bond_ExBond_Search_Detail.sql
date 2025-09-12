use SCFS_ERP
go
-- exec pr_Get_Bond_ExBond_Search_Detail 'admin', 'admin',4, 2, '2022-10-01', '2022-10-31'
-- exec [pr_Get_Bond_ExBond_Search_Detail] 5698, 13582
-- exec [pr_Get_Bond_ExBond_Search_Detail] 5756, 0
-- select * From exbondmaster 
-- select * from bondmaster where bnddno = '2015456021'
-- select * From bondgateoutdetail
alter proc pr_Get_Bond_ExBond_Search_Detail
@bndid int,
@ebndid int=0,
@bndNO VARCHAR(100)='',
@ebndNO VARCHAR(100)=''
as
begin

	set nocount on
	if @ebndid>0
	begin
		SELECT @BNDID = BNDID, @ebndNO = EBNDDNO FROM EXBONDMASTER(NOLOCK) WHERE EBNDID = @ebndid
		SELECT @bndNO = BNDDNO FROM BONDMASTER(NOLOCK) WHERE BNDID = @BNDID 
	end

	if @BNDID>0
	begin		
		SELECT @bndNO = BNDDNO FROM BONDMASTER(NOLOCK) WHERE BNDID = @BNDID 
	end

	declare @BondDtlRpt 	table 	
    (
      [Bond Date] varchar(12)      
      , [Bond ID No] INT
	  , [Bond No] VARCHAR(50)
      , [Bond CIF Amount]   numeric(18,2)
	  ,[CHA] VARCHAR(200)
	  ,[Importer] VARCHAR(200)
	  ,[Status] VARCHAR(100)
    )

    INSERT INTO @BondDtlRpt([Bond Date] , [Bond No] , [Bond ID No] ,  [Bond CIF Amount]   , 
	[CHA],[Importer] ,[Status] 
	)
	
    SELECT  Convert(varchar(10),BNDDATE,103), BNDDNO, BNDNO , BNDCIFAMT  ,  B.CATENAME, C.CATENAME , 
	CASE a.DISPSTATUS WHEN 0 THEN 'In-Books'
		when 1 then 'Cancelled' END AS DISPSTATUS
	  FROM         dbo.BONDMASTER A (nolock)  
					LEFT JOIN CATEGORYMASTER B(NOLOCK) ON A.CHAID = B.CATEID AND B.CATETID = 4
					LEFT JOIN CATEGORYMASTER C(NOLOCK) ON A.IMPRTID = C.CATEID AND C.CATETID = 1
    where BNDID = @bndid
	AND BNDDNO = @bndNO

	if exists (select '*' from @BondDtlRpt)
		select 'Bond Information' 'Heading',* from @BondDtlRpt
		order by 1,2
	else
		select 'Bond Information' 'Heading', '' [Bond Date] , '' [Bond No] , ''[Bond ID No] ,  
		0 [Bond CIF Amount]   , '' 	[CHA], '' [Importer] , ''[Status]
		

	 DECLARE @BondContainerOUTDtlRpt TABLE
    (
      GO_Date varchar(12)
    , GO_NO VARCHAR(25)
    , Container_NO  VARCHAR(20)	
	, Container_Size  VARCHAR(20)		
	, Product  VARCHAR(150)	
    , Steamer VARCHAR(150)
	, Vehicle VARCHAR(25)	
	, Importer VARCHAR(150)	
    )
    INSERT INTO @BondContainerOUTDtlRpt(GO_Date, GO_NO, Container_NO, Container_Size, Product, 
	Steamer,  Vehicle, Importer
	)
  
    SELECT   Convert(varchar(10), dbo.BONDGATEOUTDETAIL.GODATE,103), dbo.BONDGATEOUTDETAIL.GODNO,  BONDGATEINDETAIL.CONTNRNO,	CONTAINERSIZEMASTER.CONTNRSDESC,
     dbo.BONDGATEINDETAIL.PRDTDESC,dbo.BONDGATEINDETAIL.STMRNAME,  
	 dbo.BONDGATEOUTDETAIL.VHLNO,  IMPRTNAME
	FROM    dbo.BONDGATEOUTDETAIL (nolock)  
		JOIN dbo.BONDGATEINDETAIL(nolock) ON dbo.BONDGATEOUTDETAIL.GIDID = dbo.BONDGATEINDETAIL.GIDID				
		JOIN DBO.BONDMASTER (NOLOCK) ON dbo.BONDGATEINDETAIL.BNDID = BONDMASTER.BNDID
		JOIN dbo.CONTAINERSIZEMASTER(nolock) ON dbo.BONDGATEINDETAIL.CONTNRSID = dbo.CONTAINERSIZEMASTER.CONTNRSID 		
	WHERE dbo.BONDGATEINDETAIL.BNDID = @bndid
	AND BNDDNO = @bndNO

	if exists (select '*' from @BondContainerOUTDtlRpt)
		select 'Container OUT Information' 'Heading',* from @BondContainerOUTDtlRpt
		order by 1,2
	else
		select  'Container OUT Information' 'Heading', '' GO_Date, ''GO_NO, ''Container_NO, ''Container_Size, ''Product, ''Steamer,  
		''Vehicle, '' Importer

	DECLARE @ExBondDtlRpt TABLE
    (
      [ExBond Date] varchar(12)            
	  , [ExBond No] VARCHAR(50)
	  ,[Bond Date] varchar(12)            
	  , [Bond No] VARCHAR(50)
      , [ExBond Ins.Amt]  numeric(18,2)
	  ,[CHA] VARCHAR(200)
	  ,[Importer] VARCHAR(200)
	  ,[Status] VARCHAR(100)
    )

	INSERT INTO @ExBondDtlRpt([ExBond Date] , [ExBond No] , [Bond No], [Bond Date], 
	[ExBond Ins.Amt] , [CHA],	[Importer] ,[Status])
  
    SELECT  Convert(varchar(10),EBNDDATE,103), EBNDDNO, b.BNDDNO , Convert(varchar,BNDDATE,103), 
	EBNDINSAMT, c.CATENAME, d.CATENAME , 
             CASE a.DISPSTATUS WHEN 0 THEN 'In-Books'
		when 1 then 'Cancelled' END AS DISPSTATUS
   FROM         dbo.ExBondMASTER A (nolock)  join BONDMASTER b (nolock) on a.BNDID = b.BNDID
					LEFT JOIN CATEGORYMASTER c(NOLOCK) ON A.CHAID = c.CATEID AND c.CATETID = 4
					LEFT JOIN CATEGORYMASTER d(NOLOCK) ON A.IMPRTID = d.CATEID AND d.CATETID = 1
    WHERE   b.BNDID = @bndid
	AND b.BNDDNO = @bndNO

	
	if exists (select '*' from @ExBondDtlRpt)
		select 'Ex-Bond Information' 'Heading',* from @ExBondDtlRpt
		order by 1,2
	else
		select 'Ex-Bond Information' 'Heading', '' [ExBond Date] , '' [ExBond No] , '' [Bond No], ''[Bond Date], 
	0[ExBond Ins.Amt] , ''[CHA],	''[Importer] ,''[Status]

	

	DECLARE @ExBondVTDtlRpt TABLE
    (
      VT_Date varchar(12)
    , VT_No VARCHAR(15)
	, Product  VARCHAR(150)	
    , ExBond_No VARCHAR(25)
	, Bond_No VARCHAR(100)
	, VT_NOP numeric(18,2)
	, VT_Space numeric(18,2)
	, Vehicle  VARCHAR(25)	
	, Driver_Name VARCHAR(150)	
    )
		INSERT INTO @ExBondVTDtlRpt(VT_Date, VT_No, Product, ExBond_No, Bond_No, VT_NOP, 
		VT_Space, Vehicle, Driver_Name)
  
		SELECT  Convert(varchar(10), dbo.BONDVEHICLETICKETDETAIL.VTDATE,103) , dbo.BONDVEHICLETICKETDETAIL.VTDNO,  
		dbo.BONDPRODUCTGROUPMASTER.PRDTGDESC, dbo.EXBONDMASTER.EBNDDNO, dbo.BONDMASTER.BNDDNO,  
		BONDVEHICLETICKETDETAIL.VTQTY, BONDVEHICLETICKETDETAIL.VTAREA, 
		dbo.BONDVEHICLETICKETDETAIL.VHLNO, 	BONDVEHICLETICKETDETAIL.DRVNAME
		FROM    dbo.BONDVEHICLETICKETDETAIL (nolock)  JOIN dbo.EXBONDMASTER (nolock)  ON dbo.BONDVEHICLETICKETDETAIL.EBNDID =dbo.EXBONDMASTER.EBNDID
				JOIN BONDMASTER (NOLOCK) ON EXBONDMASTER.BNDID = BONDMASTER.BNDID
				JOIN BONDPRODUCTGROUPMASTER (NOLOCK) ON EXBONDMASTER.PRDTGID = BONDPRODUCTGROUPMASTER.PRDTGID
		where dbo.BONDMASTER.BNDID = @bndid
		AND BONDMASTER.BNDDNO = @bndNO
			
	if exists (select '*' from @ExBondVTDtlRpt)
		select 'Ex-Bond VT Details' 'Heading',* from @ExBondVTDtlRpt
		order by 1,2	
	else
		select 'Ex-Bond VT Details' 'Heading', '' VT_Date, ''VT_No, ''Product, ''ExBond_No, ''Bond_No, 0 VT_NOP, 0 VT_Space, 
		''Vehicle, ''Driver_Name

	DECLARE @BondInvoiceDetailsRpt TABLE
    (
	 ExBond_No VARCHAR(25)
	, Bond_No VARCHAR(100)
    , Invoice_Date varchar(12)
    , Invoice_No  VARCHAR(50)
	, Invoice_Ref  VARCHAR(50)    
    , Billed_To VARCHAR(100)	
	, Net_Amount numeric(18,2)
	, GST_Amount numeric(18,2)
	, Invoice_Type varchar(50)
	
    )
    INSERT INTO @BondInvoiceDetailsRpt(ExBond_No , Bond_No , Invoice_Date, Invoice_No , 
	Invoice_Ref,Billed_To,Net_Amount,GST_Amount,Invoice_Type)
  
    SELECT   TRANDEBNDNO, TRANDBNDNO, Convert(varchar(10), TRANDATE,103) ,  TRANDNO, TRANBILLREFNO, TRANREFNAME, TRANNAMT, TRAN_CGST_AMT+TRAN_SGST_AMT+TRAN_IGST_AMT,
	
	(CASE WHEN TRANTID = 2 THEN 'Bond Invoice' 
		 WHEN TRANTID = 3 THEN 'ExBond VT Invoice'
		 WHEN TRANTID = 4 THEN 'Bond Manual Invoice'
		 Else '' END)            
	FROM       BONDTRANSACTIONMASTER a (NOLOCK) join BONDTRANSACTIONDETAIL b (nolock)on a.tranmid=b.TRANMID
	WHERE ((b.BNDID = @bndid AND B.TRANDBNDNO = @bndNO) or (b.EBNDID = @ebndid AND B.TRANDEBNDNO = @ebndNO))
	and TRANTID in (2,3,4)
	group by  TRANDEBNDNO, TRANDBNDNO, TRANDATE,  TRANDNO, TRANBILLREFNO, TRANREFNAME, TRANNAMT, TRAN_CGST_AMT+TRAN_SGST_AMT+TRAN_IGST_AMT,	
	(CASE WHEN TRANTID = 2 THEN 'Bond Invoice' 
		 WHEN TRANTID = 3 THEN 'ExBond VT Invoice'
		 WHEN TRANTID = 4 THEN 'Bond Manual Invoice'
		 Else '' END)            

	if exists (select '*' from @BondInvoiceDetailsRpt)
		select 'Invoice Details' 'Heading',* 
		from @BondInvoiceDetailsRpt
		order by 1,2	
	else
		select 'Invoice Details' 'Heading', '' ExBond_No , ''Bond_No , ''Invoice_Date, ''Invoice_No , ''Invoice_Ref,''Billed_To,
		0 Net_Amount,0 GST_Amount, ''Invoice_Type
end
