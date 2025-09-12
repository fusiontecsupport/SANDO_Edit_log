/*
EXEC [dbo].[pr_Search_Import_GateOut] @FilterTerm = NULL , @SortIndex = 1 
        , @SortDirection  = 'ASC'
        , @StartRowNum  = 1 --the first row to return
        , @EndRowNum  = 10 --the last row to return
        , @TotalRowsCount = 10
        , @FilteredRowsCount = 1
		,@PSDATE ='2021-08-01'
		,@PEDATE = '2021-09-30'
		,@PCOMPYID = 32

*/
alter PROCEDURE [dbo].[pr_Search_Import_GateOut]  /* CHANGE */
 @FilterTerm nvarchar(250) = NULL --parameter to search all columns by
        , @SortIndex INT = 1 -- a one based index to indicate which column to order by
        , @SortDirection CHAR(4) = 'ASC' --the direction to sort in, either ASC or DESC
        , @StartRowNum INT = 1 --the first row to return
        , @EndRowNum INT = 10 --the last row to return
        , @TotalRowsCount INT OUTPUT
        , @FilteredRowsCount INT OUTPUT
		,@PSDATE SMALLDATETIME
		,@PEDATE SMALLDATETIME
		,@PCOMPYID INT
 
AS BEGIN
    --Wrap filter term with % to search for values that contain @FilterTerm
    SET @FilterTerm = '%' + @FilterTerm + '%'
	DECLARE @LSDATE SMALLDATETIME,@LEDATE SMALLDATETIME,@LCOMPYID INT
	SET @LSDATE=@PSDATE
	SET @LEDATE=@PEDATE
	SET @LCOMPYID=@PCOMPYID

    DECLARE @TableMaster TABLE
    ( 
	GODATE SMALLDATETIME
      ,GOTIME DATETIME
	  ,GONO INT
	  ,GODNO VARCHAR(15)
	  ,CONTNRNO VARCHAR(100)
	  ,CONTNRSCODE VARCHAR(100)
	  ,VHLNO VARCHAR(100)
	  ,CHANAME VARCHAR(100)
	  ,GDRVNAME VARCHAR(100)
	  ,IGMNO VARCHAR(100)
	  ,GPLNO VARCHAR(100)
	  ,VSLNAME VARCHAR(100)
	  ,STMRNAME VARCHAR(100)
	  ,PRDTDESC VARCHAR(100)
	  ,BOENO VARCHAR(100)
	  , ASLMDNO VARCHAR(25)
	  ,VTTYPE VARCHAR(50) 
	  ,GIDID int
	  , GODID int 
      , RowNum INT
    )

    INSERT INTO @TableMaster(GODATE,
GOTIME ,GONO,GODNO,CONTNRNO ,CONTNRSCODE,VHLNO,CHANAME ,GDRVNAME,IGMNO,GPLNO,VSLNAME,STMRNAME,PRDTDESC,
BOENO,ASLMDNO,VTTYPE,

GIDID,GODID, RowNum)
    SELECT  GODATE
            , GOTIME
			,GONO
			,GODNO
			,CONTNRNO 
			,CONTNRSCODE
			,GATEOUTDETAIL.VHLNO
			,isnull(CHANAME ,'')
			,GDRVNAME,IGMNO,GPLNO,VSLNAME,STMRNAME,PRDTDESC
			,isnull(GATEINDETAIL.BOENO,'')
			,dbo.AUTHORIZATIONSLIPMASTER.ASLMDNO
			,case when cast(dbo.AUTHORIZATIONSLIPMASTER.ASLMTYPE as varchar(10))= '1' then 'Load'
			      when cast(dbo.AUTHORIZATIONSLIPMASTER.ASLMTYPE as varchar(10)) = '2' then 'DeStuff' Else cast(dbo.AUTHORIZATIONSLIPMASTER.ASLMTYPE as varchar(10)) end			
			,GATEOUTDETAIL.GIDID
			
            , GODID
            , Row_Number() OVER (
            ORDER BY
            
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
                  WHEN 2 THEN GONO
                  WHEN 3 THEN GODNO
				    WHEN 4 THEN CONTNRNO
                  WHEN 5 THEN GATEOUTDETAIL.VHLNO
				    WHEN 6 THEN CHANAME
                  WHEN 7 THEN GDRVNAME
	               WHEN 8 THEN IGMNO
                  WHEN 9 THEN GPLNO
				    WHEN 10 THEN VSLNAME
                  WHEN 11 THEN STMRNAME
				   WHEN 12 THEN PRDTDESC
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 2 THEN GONO
                  WHEN 3 THEN GODNO
				    WHEN 4 THEN CONTNRNO
                  WHEN 5 THEN GATEOUTDETAIL.VHLNO
				    WHEN 6 THEN CHANAME
                  WHEN 7 THEN GDRVNAME
	 			WHEN 8 THEN IGMNO
                  WHEN 9 THEN GPLNO
				    WHEN 10 THEN VSLNAME
                  WHEN 11 THEN STMRNAME
				   WHEN 12 THEN PRDTDESC
               
                END
            END DESC ,
			 CASE @SortDirection    /*DATETIME ORDER BY*/
              WHEN 'ASC'  THEN
                CASE @SortIndex
                  WHEN 0 THEN GODATE
                  WHEN 1 THEN GOTIME
	 
                END             
            END ASC,
     CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 0 THEN GODATE
                  WHEN 1 THEN GOTIME				
               
                END
            END DESC            

            
            ) AS RowNum
            
FROM   dbo.GATEOUTDETAIL  INNER JOIN 
dbo.GATEINDETAIL ON dbo.GATEINDETAIL.GIDID = dbo.GATEOUTDETAIL.GIDID    INNER JOIN 
dbo.CONTAINERSIZEMASTER ON dbo.GATEINDETAIL.CONTNRSID = dbo.CONTAINERSIZEMASTER.CONTNRSID  INNER JOIN 
dbo.VEHICLETICKETDETAIL ON dbo.VEHICLETICKETDETAIL.GIDID = dbo.GATEOUTDETAIL.GIDID and VEHICLETICKETDETAIL.VTSTYPE = 1 INNER JOIN 
dbo.AUTHORIZATIONSLIPDETAIL ON  dbo.VEHICLETICKETDETAIL.ASLDID = dbo.AUTHORIZATIONSLIPDETAIL.ASLDID INNER JOIN 
dbo.AUTHORIZATIONSLIPMASTER ON  dbo.AUTHORIZATIONSLIPDETAIL.ASLMID = dbo.AUTHORIZATIONSLIPMASTER.ASLMID  
WHERE (GATEOUTDETAIL.COMPYID= @LCOMPYID) AND (GODATE BETWEEN @LSDATE AND @LEDATE) AND (GATEINDETAIL.SDPTID = 1) AND (REGSTRID = 1) AND  (@FilterTerm IS NULL 
              OR GODATE LIKE @FilterTerm
              OR GOTIME LIKE @FilterTerm
			  OR GONO LIKE @FilterTerm
              OR GODNO LIKE @FilterTerm
			  OR CONTNRNO LIKE @FilterTerm
              OR CONTNRSCODE LIKE @FilterTerm
			  OR GATEOUTDETAIL.VHLNO LIKE @FilterTerm
              OR CHANAME LIKE @FilterTerm
			  OR GDRVNAME LIKE @FilterTerm
			    OR IGMNO LIKE @FilterTerm
              OR GPLNO LIKE @FilterTerm
			  OR VSLNAME LIKE @FilterTerm
              OR STMRNAME LIKE @FilterTerm
			  OR PRDTDESC LIKE @FilterTerm
            )

    SELECT GODATE
            , GOTIME
			,GONO
			,GODNO
			,CONTNRNO 
			,CONTNRSCODE
			,VHLNO
			,CHANAME 
			,GDRVNAME,IGMNO,GPLNO,VSLNAME,STMRNAME,PRDTDESC
			,BOENO,ASLMDNO,VTTYPE
			,GIDID
            , GODID
    FROM    @TableMaster
    WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum
    ORDER BY GODNO DESC
    
      
SELECT @TotalRowsCount = COUNT(*)
FROM   dbo.GATEOUTDETAIL  INNER JOIN 
dbo.GATEINDETAIL ON dbo.GATEINDETAIL.GIDID = dbo.GATEOUTDETAIL.GIDID    INNER JOIN 
dbo.CONTAINERSIZEMASTER ON dbo.GATEINDETAIL.CONTNRSID = dbo.CONTAINERSIZEMASTER.CONTNRSID  INNER JOIN 
dbo.VEHICLETICKETDETAIL ON dbo.VEHICLETICKETDETAIL.GIDID = dbo.GATEOUTDETAIL.GIDID and VEHICLETICKETDETAIL.VTSTYPE = 1 INNER JOIN 
dbo.AUTHORIZATIONSLIPDETAIL ON  dbo.VEHICLETICKETDETAIL.ASLDID = dbo.AUTHORIZATIONSLIPDETAIL.ASLDID INNER JOIN 
dbo.AUTHORIZATIONSLIPMASTER ON  dbo.AUTHORIZATIONSLIPDETAIL.ASLMID = dbo.AUTHORIZATIONSLIPMASTER.ASLMID      
WHERE (GATEOUTDETAIL.COMPYID= @LCOMPYID) AND (GODATE BETWEEN @LSDATE AND @LEDATE) AND (GATEINDETAIL.SDPTID = 1) AND (REGSTRID = 1)
      
    SELECT @FilteredRowsCount = COUNT(*)
    FROM   @TableMaster
        
END

