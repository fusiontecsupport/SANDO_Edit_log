/*

DECLARE @total INT
        @filtered INT

EXEC [dbo].[pr_SearchPerson] NULL, 1, 'ASC', 1, 10, @TotalRowsCount= @total OUTPUT,@FilteredRowsCount= @filtered OUTPUT

SELECT @total, @filtered
*/

alter PROCEDURE [dbo].[pr_Search_ESeal_GateOut]  /* CHANGE */
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
	  ,GDRVNAME VARCHAR(100),GIDID int
	  , GODID int 
	  , Edchk varchar(15)
      , RowNum INT
    )

    INSERT INTO @TableMaster(GODATE,
GOTIME ,GONO,GODNO,CONTNRNO ,CONTNRSCODE,VHLNO,CHANAME ,GDRVNAME,GIDID,GODID, RowNum)
    SELECT  GODATE
            , GOTIME
			,GONO
			,GODNO
			,CONTNRNO 
			,CONTNRSCODE
			,GATEOUTDETAIL.VHLNO
			,CHANAME 
			,GDRVNAME,GATEOUTDETAIL.GIDID
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
            
			FROM GATEOUTDETAIL  (nolock) Inner Join  
			dbo.VEHICLETICKETDETAIL (nolock) on dbo.GATEOUTDETAIL.GIDID = dbo.VEHICLETICKETDETAIL.GIDID   inner join  
			dbo.GATEINDETAIL (nolock) on  dbo.GATEINDETAIL.GIDID = dbo.VEHICLETICKETDETAIL.GIDID  inner join 
			dbo.CONTAINERSIZEMASTER (nolock) on  dbo.GATEINDETAIL.CONTNRSID = dbo.CONTAINERSIZEMASTER.CONTNRSID  



WHERE (GATEOUTDETAIL.COMPYID= @LCOMPYID) AND (GODATE BETWEEN @LSDATE AND @LEDATE) AND (GATEINDETAIL.SDPTID = 11) AND (REGSTRID = 1) AND  (@FilterTerm IS NULL 
              OR GODATE LIKE @FilterTerm
              OR GOTIME LIKE @FilterTerm
			  OR GONO LIKE @FilterTerm
              OR GODNO LIKE @FilterTerm
			  OR CONTNRNO LIKE @FilterTerm
              OR CONTNRSCODE LIKE @FilterTerm
			  OR GATEOUTDETAIL.VHLNO LIKE @FilterTerm
   OR CHANAME LIKE @FilterTerm
			  OR GDRVNAME LIKE @FilterTerm
            )

	update a
	set Edchk = case when tm.TRANMID is null then 'Not-Exists' else 'Exists' end
	from @TableMaster a 
	left join TRANSACTIONDETAIL t(nolock) on  a.GIDID = t.TRANDREFID 
	join TRANSACTIONMASTER tm(nolock) on t.TRANMID = tm.TRANMID and  tm.SDPTID = 11

	update @TableMaster
	set Edchk = 'Not-Exists' 	
	where Edchk  is null

    SELECT GODATE
            , GOTIME
			,GONO
			,GODNO
			,CONTNRNO 
			,CONTNRSCODE
			,VHLNO
			,CHANAME 
			,GDRVNAME,GIDID
            , GODID, Edchk
    FROM    @TableMaster
    WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum
    ORDER BY GODNO DESC
    --SELECT @TotalRowsCount = COUNT(*)
      --FROM GATEOUTDETAIL INNER JOIN GATEINDETAIL ON GATEOUTDETAIL.GIDID=GATEINDETAIL.GIDID LEFT OUTER JOIN CONTAINERSIZEMASTER ON GATEINDETAIL.CONTNRSID=CONTAINERSIZEMASTER.CONTNRSID   /* CHANGE  TABLE NAME */
      --WHERE (GATEINDETAIL.COMPYID=@LCOMPYID) AND (GODATE BETWEEN @PSDATE AND @PEDATE)
      
SELECT @TotalRowsCount = isnull(COUNT(*)  ,0)
FROM GATEOUTDETAIL  (nolock) Inner Join  
			dbo.VEHICLETICKETDETAIL (nolock) on dbo.GATEOUTDETAIL.GIDID = dbo.VEHICLETICKETDETAIL.GIDID   inner join  
			dbo.GATEINDETAIL (nolock) on  dbo.GATEINDETAIL.GIDID = dbo.VEHICLETICKETDETAIL.GIDID  inner join 
			dbo.CONTAINERSIZEMASTER (nolock) on  dbo.GATEINDETAIL.CONTNRSID = dbo.CONTAINERSIZEMASTER.CONTNRSID  
       
WHERE (GATEOUTDETAIL.COMPYID= @LCOMPYID) AND (GODATE BETWEEN @LSDATE AND @LEDATE) AND (GATEINDETAIL.SDPTID = 11) AND (REGSTRID = 1)
      
    SELECT @FilteredRowsCount = isnull(COUNT(*),0)
    FROM   @TableMaster
        
END


