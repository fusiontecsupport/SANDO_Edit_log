/*

DECLARE @total INT
        @filtered INT

EXEC [dbo].[pr_SearchPerson] NULL, 1, 'ASC', 1, 10, @TotalRowsCount= @total OUTPUT,@FilteredRowsCount= @filtered OUTPUT

SELECT @total, @filtered
*/

ALTER PROCEDURE [dbo].[pr_Search_Export_GateOut]  /* CHANGE */
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
            
	--		FROM dbo.STUFFINGDETAIL (nolock) Inner Join  
  
 --dbo.STUFFINGMASTER (nolock) on dbo.STUFFINGDETAIL.STFMID = dbo.STUFFINGMASTER.STFMID inner Join 
 -- dbo.AUTHORIZATIONSLIPDETAIL (nolock) on  dbo.STUFFINGDETAIL.STFDID = dbo.AUTHORIZATIONSLIPDETAIL.STFDID inner join 
 --dbo.VEHICLETICKETDETAIL (nolock) on dbo.STUFFINGDETAIL.STFDID = dbo.VEHICLETICKETDETAIL.STFDID  inner join  
 --dbo.GATEINDETAIL (nolock) on  dbo.GATEINDETAIL.GIDID = dbo.VEHICLETICKETDETAIL.GIDID  inner join 
 --dbo.CONTAINERSIZEMASTER (nolock) on  dbo.GATEINDETAIL.CONTNRSID = dbo.CONTAINERSIZEMASTER.CONTNRSID  inner join 
 --dbo.GATEOUTDETAIL (nolock) on  dbo.VEHICLETICKETDETAIL.GIDID =  dbo.GATEOUTDETAIL.GIDID 
 FROM         dbo.STUFFINGMASTER (nolock)  JOIN
                      dbo.VEHICLETICKETDETAIL (nolock) INNER JOIN
                      dbo.GATEINDETAIL (nolock) INNER JOIN
                      dbo.CONTAINERSIZEMASTER (nolock) ON dbo.GATEINDETAIL.CONTNRSID = dbo.CONTAINERSIZEMASTER.CONTNRSID ON 
                      dbo.VEHICLETICKETDETAIL.GIDID = dbo.GATEINDETAIL.GIDID JOIN
                      dbo.STUFFINGDETAIL (nolock) ON dbo.GATEINDETAIL.GIDID = dbo.STUFFINGDETAIL.GIDID LEFT OUTER JOIN
                      dbo.GATEOUTDETAIL  (nolock) ON dbo.GATEINDETAIL.GIDID = dbo.GATEOUTDETAIL.GIDID ON dbo.STUFFINGMASTER.STFMID = dbo.STUFFINGDETAIL.STFMID

--FROM         dbo.STUFFINGDETAIL INNER JOIN
--                      dbo.STUFFINGMASTER ON dbo.STUFFINGDETAIL.STFMID = dbo.STUFFINGMASTER.STFMID INNER JOIN
--                      dbo.GATEINDETAIL INNER JOIN
--                      dbo.CONTAINERSIZEMASTER ON dbo.GATEINDETAIL.CONTNRSID = dbo.CONTAINERSIZEMASTER.CONTNRSID ON 
--                      dbo.STUFFINGDETAIL.GIDID = dbo.GATEINDETAIL.GIDID INNER JOIN
--                      dbo.AUTHORIZATIONSLIPDETAIL ON dbo.STUFFINGDETAIL.STFDID = dbo.AUTHORIZATIONSLIPDETAIL.STFDID INNER JOIN
--                      dbo.VEHICLETICKETDETAIL ON dbo.AUTHORIZATIONSLIPDETAIL.ASLDID = dbo.VEHICLETICKETDETAIL.ASLDID LEFT OUTER JOIN
--                      dbo.GATEOUTDETAIL ON dbo.GATEINDETAIL.GIDID = dbo.GATEOUTDETAIL.GIDID           
WHERE (GATEINDETAIL.COMPYID= @LCOMPYID) AND (GODATE BETWEEN @LSDATE AND @LEDATE) AND (GATEINDETAIL.SDPTID = 2) AND (REGSTRID = 1) AND  (@FilterTerm IS NULL 
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

    SELECT GODATE
            , GOTIME
			,GONO
			,GODNO
			,CONTNRNO 
			,CONTNRSCODE
			,VHLNO
			,CHANAME 
			,GDRVNAME,GIDID
            , GODID
    FROM    @TableMaster
    WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum
    ORDER BY GODNO DESC
    --SELECT @TotalRowsCount = COUNT(*)
      --FROM GATEOUTDETAIL INNER JOIN GATEINDETAIL ON GATEOUTDETAIL.GIDID=GATEINDETAIL.GIDID LEFT OUTER JOIN CONTAINERSIZEMASTER ON GATEINDETAIL.CONTNRSID=CONTAINERSIZEMASTER.CONTNRSID   /* CHANGE  TABLE NAME */
      --WHERE (GATEINDETAIL.COMPYID=@LCOMPYID) AND (GODATE BETWEEN @PSDATE AND @PEDATE)
      
SELECT @TotalRowsCount = COUNT(*)  
--FROM dbo.STUFFINGDETAIL (nolock) Inner Join  
  
-- dbo.STUFFINGMASTER (nolock) on dbo.STUFFINGDETAIL.STFMID = dbo.STUFFINGMASTER.STFMID inner Join 
--  dbo.AUTHORIZATIONSLIPDETAIL (nolock) on  dbo.STUFFINGDETAIL.STFDID = dbo.AUTHORIZATIONSLIPDETAIL.STFDID inner join 
-- dbo.VEHICLETICKETDETAIL (nolock) on dbo.STUFFINGDETAIL.STFDID = dbo.VEHICLETICKETDETAIL.STFDID  inner join  
-- dbo.GATEINDETAIL (nolock) on  dbo.GATEINDETAIL.GIDID = dbo.VEHICLETICKETDETAIL.GIDID  inner join 
-- dbo.CONTAINERSIZEMASTER (nolock) on  dbo.GATEINDETAIL.CONTNRSID = dbo.CONTAINERSIZEMASTER.CONTNRSID  inner join 
-- dbo.GATEOUTDETAIL (nolock) on  dbo.VEHICLETICKETDETAIL.GIDID =  dbo.GATEOUTDETAIL.GIDID 
FROM         dbo.STUFFINGMASTER (nolock)  JOIN
                      dbo.VEHICLETICKETDETAIL (nolock) INNER JOIN
                      dbo.GATEINDETAIL (nolock) INNER JOIN
                      dbo.CONTAINERSIZEMASTER (nolock) ON dbo.GATEINDETAIL.CONTNRSID = dbo.CONTAINERSIZEMASTER.CONTNRSID ON 
                      dbo.VEHICLETICKETDETAIL.GIDID = dbo.GATEINDETAIL.GIDID JOIN
                      dbo.STUFFINGDETAIL (nolock) ON dbo.GATEINDETAIL.GIDID = dbo.STUFFINGDETAIL.GIDID LEFT OUTER JOIN
                      dbo.GATEOUTDETAIL  (nolock) ON dbo.GATEINDETAIL.GIDID = dbo.GATEOUTDETAIL.GIDID ON dbo.STUFFINGMASTER.STFMID = dbo.STUFFINGDETAIL.STFMID
--FROM         dbo.STUFFINGDETAIL INNER JOIN
--                      dbo.STUFFINGMASTER ON dbo.STUFFINGDETAIL.STFMID = dbo.STUFFINGMASTER.STFMID INNER JOIN
--                      dbo.GATEINDETAIL INNER JOIN
--                      dbo.CONTAINERSIZEMASTER ON dbo.GATEINDETAIL.CONTNRSID = dbo.CONTAINERSIZEMASTER.CONTNRSID ON 
--                      dbo.STUFFINGDETAIL.GIDID = dbo.GATEINDETAIL.GIDID INNER JOIN
--                      dbo.AUTHORIZATIONSLIPDETAIL ON dbo.STUFFINGDETAIL.STFDID = dbo.AUTHORIZATIONSLIPDETAIL.STFDID INNER JOIN
--                      dbo.VEHICLETICKETDETAIL ON dbo.AUTHORIZATIONSLIPDETAIL.ASLDID = dbo.VEHICLETICKETDETAIL.ASLDID LEFT OUTER JOIN
--                      dbo.GATEOUTDETAIL ON dbo.GATEINDETAIL.GIDID = dbo.GATEOUTDETAIL.GIDID           
WHERE (GATEINDETAIL.COMPYID= @LCOMPYID) AND (GODATE BETWEEN @LSDATE AND @LEDATE) AND (GATEINDETAIL.SDPTID = 2) AND (REGSTRID = 1)
      
    SELECT @FilteredRowsCount = COUNT(*)
    FROM   @TableMaster
        
END



