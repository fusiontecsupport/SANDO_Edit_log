/*

DECLARE @total INT,
        @filtered INT

EXEC [dbo].[pr_SearchExportTruckOut] NULL, 1, 'ASC', 1, 10, @TotalRowsCount= @total OUTPUT,
		@FilteredRowsCount= @filtered OUTPUT, @PSDATE = '2021-08-01', @PEDATE = '2021-08-31', @PCOMPYID = 32

SELECT @total, @filtered
*/

alter PROCEDURE [dbo].[pr_Search_Import_TruckOut]  /* CHANGE */
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
	  ,VHLNO VARCHAR(100)
	  ,CHASNAME VARCHAR(100)
	  ,GDRVNAME VARCHAR(100),GIDID int
	  , GODID int 
      , RowNum INT
    )

	update GATEOUTDETAIL
	set  GODATE = convert(varchar(10),GODATE,120)

    INSERT INTO @TableMaster(GODATE,
GOTIME ,GONO,GODNO,VHLNO,CHASNAME ,GDRVNAME,GIDID,GODID, RowNum)
    SELECT  GODATE
            , GOTIME
			,GONO
			,GODNO
			
			,isnull(GATEOUTDETAIL.VHLNO,'')
			,isnull(CHASNAME ,'')
			,isnull(GDRVNAME,''),GATEOUTDETAIL.GIDID
            , GODID
            , Row_Number() OVER (
            ORDER BY
            
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
                  WHEN 2 THEN GONO
                  WHEN 3 THEN GODNO
				  
                  WHEN 4 THEN GATEOUTDETAIL.VHLNO
				    WHEN 5 THEN CHASNAME
                  WHEN 6 THEN GDRVNAME
	 
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 2 THEN GONO
                  WHEN 3 THEN GODNO
				    WHEN 4 THEN GATEOUTDETAIL.VHLNO
				    WHEN 5 THEN CHASNAME
                  WHEN 6 THEN GDRVNAME
	 			
               
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
            
FROM    dbo.GATEOUTDETAIL          
WHERE (GATEOUTDETAIL.COMPYID= @LCOMPYID) AND (GODATE BETWEEN @LSDATE AND @LEDATE) AND (GATEOUTDETAIL.SDPTID = 1) AND (REGSTRID = 3) AND  (@FilterTerm IS NULL 
              OR GODATE LIKE @FilterTerm
              OR GOTIME LIKE @FilterTerm
			  OR GONO LIKE @FilterTerm
              OR GODNO LIKE @FilterTerm
			
			  OR GATEOUTDETAIL.VHLNO LIKE @FilterTerm
              OR CHASNAME LIKE @FilterTerm
			  OR GDRVNAME LIKE @FilterTerm
            )

    SELECT GODATE
            , GOTIME
			,GONO
			,GODNO
			,VHLNO
			,CHASNAME 
			,GDRVNAME,GIDID
            , GODID
    FROM    @TableMaster
    WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum
    ORDER BY GODNO DESC
    --SELECT @TotalRowsCount = COUNT(*)
      --FROM GATEOUTDETAIL INNER JOIN GATEINDETAIL ON GATEOUTDETAIL.GIDID=GATEINDETAIL.GIDID LEFT OUTER JOIN CONTAINERSIZEMASTER ON GATEINDETAIL.CONTNRSID=CONTAINERSIZEMASTER.CONTNRSID   /* CHANGE  TABLE NAME */
      --WHERE (GATEINDETAIL.COMPYID=@LCOMPYID) AND (GODATE BETWEEN @PSDATE AND @PEDATE)
      
SELECT @TotalRowsCount = COUNT(*)
FROM   dbo.GATEOUTDETAIL           
WHERE (GATEOUTDETAIL.COMPYID= @LCOMPYID) AND (GODATE BETWEEN @LSDATE AND @LEDATE) AND (GATEOUTDETAIL.SDPTID = 1) AND (REGSTRID = 3)
      
    SELECT @FilteredRowsCount = COUNT(*)
    FROM   @TableMaster
        
END



