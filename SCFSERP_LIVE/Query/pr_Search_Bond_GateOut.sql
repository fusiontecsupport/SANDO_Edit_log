-- select * from BONDBONDGATEOUTDETAIL

ALTER PROCEDURE [dbo].[pr_Search_Bond_GateOut]  /* CHANGE */
 @FilterTerm nvarchar(250) = NULL --parameter to search all columns by
        , @SortIndex INT = 1 -- a one based index to indicate which column to order by
        , @SortDirection CHAR(4) = 'ASC' --the direction to sort in, either ASC or DESC
        , @StartRowNum INT = 1 --the first row to return
        , @EndRowNum INT = 10 --the last row to return
        , @TotalRowsCount INT OUTPUT
        , @FilteredRowsCount INT OUTPUT
        , @PCompyId INT
        , @PSDate Smalldatetime
        , @PEDate Smalldatetime
 
AS BEGIN

Declare @LCompyId int,@LSDate Smalldatetime, @LEDate Smalldatetime
set @LCompyId = @PCompyId
set @LSDate = @PSDate
set @LEDate = @PEDate

    --Wrap filter term with % to search for values that contain @FilterTerm
    SET @FilterTerm = '%' + @FilterTerm + '%'


    DECLARE @TableMaster TABLE
    (
      GODATE SMALLDATETIME
    , GODNO VARCHAR(25)
    , CONTNRNO  VARCHAR(20)	
	, CONTNRSDESC  VARCHAR(20)	
	, CONTNRSCODE  VARCHAR(20)	
	, PRDTDESC  VARCHAR(150)	
    , STMRNAME VARCHAR(150)
	, VHLNO  VARCHAR(25)	
	, IMPRTRNAME VARCHAR(150)		
	, GIDID int
	, GODID INT
	, DISPSTATUS smallint
    , RowNum INT
    )
    INSERT INTO @TableMaster(GODATE, GODNO, CONTNRNO, CONTNRSDESC,CONTNRSCODE, PRDTDESC, STMRNAME,  VHLNO, GIDID, GODID, IMPRTRNAME, 
	 DISPSTATUS, RowNum)
  
    SELECT   dbo.BONDGATEOUTDETAIL.GODATE, dbo.BONDGATEOUTDETAIL.GODNO,  BONDGATEINDETAIL.CONTNRNO,	CONTAINERSIZEMASTER.CONTNRSDESC,
     dbo.CONTAINERSIZEMASTER.CONTNRSCODE, dbo.BONDGATEINDETAIL.PRDTDESC,dbo.BONDGATEINDETAIL.STMRNAME,  
	 dbo.BONDGATEOUTDETAIL.VHLNO, dbo.BONDGATEINDETAIL.GIDID, dbo.BONDGATEOUTDETAIL.GODID, 
	 IMPRTNAME,	dbo.BONDGATEOUTDETAIL.DISPSTATUS
            , Row_Number() OVER (
            ORDER BY
            
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/
            
            CASE @SortDirection
                 WHEN 'ASC'  THEN
                      CASE @SortIndex
           				  WHEN 1 then GODNO
			  			  WHEN 2 then CONTNRNO
					      WHEN 3 then CONTNRSDESC
						  WHEN 4 then PRDTDESC
			  			  WHEN 5 then STMRNAME
					   END             
				 END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                   CASE @SortIndex
           			    WHEN 1 then GODNO
			  			  WHEN 2 then CONTNRNO
					      WHEN 3 then CONTNRSDESC
						  WHEN 4 then PRDTDESC
			  			  WHEN 5 then STMRNAME
                    END
               END DESC,
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                   CASE @SortIndex
                     WHEN 0 THEN GODATE
                   END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 0 THEN GODATE
                END
            END DESC
            
            ) AS RowNum


FROM    dbo.BONDGATEOUTDETAIL (nolock)  
		JOIN dbo.BONDGATEINDETAIL(nolock) ON dbo.BONDGATEOUTDETAIL.GIDID = dbo.BONDGATEINDETAIL.GIDID				
		JOIN dbo.CONTAINERSIZEMASTER(nolock) ON dbo.BONDGATEINDETAIL.CONTNRSID = dbo.CONTAINERSIZEMASTER.CONTNRSID 
		
WHERE (dbo.BONDGATEOUTDETAIL.COMPYID = @LCompyId) 
and (GODATE BETWEEN @PSDate AND @PEDate) AND (BONDGATEOUTDETAIL.SDPTID=10) 
AND (@FilterTerm IS NULL 
             OR GODATE LIKE @FilterTerm
              OR GODNO LIKE @FilterTerm
              OR CONTNRNO  LIKE @FilterTerm
              OR CONTNRSDESC    LIKE @FilterTerm
			  OR PRDTDESC    LIKE @FilterTerm
              oR STMRNAME LIKE @FilterTerm)			   
ORDER BY GODNO 
            
    SELECT GODATE, GODNO, CONTNRNO, CONTNRSDESC,CONTNRSCODE, PRDTDESC, STMRNAME, VHLNO, IMPRTRNAME,  GIDID, GODID, 
	(CASE WHEN DISPSTATUS = 1 THEN 'CANCELLED' ELSE 'IN BOOKS' END)  AS DISPSTATUS
    FROM    @TableMaster
    WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum
	ORDER BY GODNO  DESC
    

	SELECT    @TotalRowsCount = COUNT(*)
	FROM BONDGATEOUTDETAIL
	WHERE  dbo.BONDGATEOUTDETAIL.SDPTID =10 
	AND  dbo.BONDGATEOUTDETAIL.COMPYID = @LCompyId
	AND GODATE BETWEEN @LSDate AND @LEDate

    
    SELECT @FilteredRowsCount = COUNT(*)
    FROM   @TableMaster
        
END


