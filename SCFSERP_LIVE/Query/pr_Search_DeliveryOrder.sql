CREATE PROCEDURE [dbo].[pr_Search_DeliveryOrder]  /* CHANGE */
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
      DODATE SMALLDATETIME
    , DONO VARCHAR(15)
    , DODREFNAME  VARCHAR(15)
    , CONTNRSDESC  VARCHAR(5)	
	, DOREFNAME VARCHAR(100)
    , DODREFNO VARCHAR(25)
	, DODID int 
	, DOMID int
	, DISPSTATUS int
	,DODNO VARCHAR(50)
	,IGMNO VARCHAR(50)
	,GPLNO VARCHAR(50)
    , RowNum INT
    )
    INSERT INTO @TableMaster(DODATE, DONO, DODREFNAME, CONTNRSDESC, DOREFNAME, DODREFNO, DODID, DOMID, DISPSTATUS,DODNO,IGMNO,GPLNO, RowNum)
  
    SELECT   dbo.DELIVERYORDERMASTER.DODATE, dbo.DELIVERYORDERMASTER.DONO, DODREFNAME, 
                      dbo.CONTAINERSIZEMASTER.CONTNRSDESC, DOREFNAME,DODREFNO,  dbo.DELIVERYORDERDETAIL.DODID, DELIVERYORDERMASTER.DOMID, 
                      dbo.DELIVERYORDERMASTER.DISPSTATUS,DODNO,IGMNO,GPLNO


            , Row_Number() OVER (
            ORDER BY
            
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/
            
            CASE @SortDirection
                 WHEN 'ASC'  THEN
                      CASE @SortIndex
           				  WHEN 1 then DONO
			  			  WHEN 2 then DODREFNAME
					      WHEN 3 then CONTNRSDESC
			  			  WHEN 4 then DOREFNAME
						  WHEN 5 then DODREFNO
						
						  WHEN 9 THEN DODNO
						  WHEN 10 THEN IGMNO
						   WHEN 11 THEN GPLNO
					   END             
				 END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                   CASE @SortIndex
           			    WHEN 1 then DONO
			  		    WHEN 2 then DODREFNAME
					    WHEN 3 then CONTNRSDESC
			  			WHEN 4 then DOREFNAME
						WHEN 5 then DODREFNO
						
						 WHEN 9 THEN DODNO
						  WHEN 10 THEN IGMNO
						   WHEN 11 THEN GPLNO
                    END
               END DESC,
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                   CASE @SortIndex
                     WHEN 0 THEN DODATE
                   END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 0 THEN DODATE
                END
            END DESC
            
            ) AS RowNum
            
FROM         dbo.DELIVERYORDERMASTER INNER JOIN
             dbo.DELIVERYORDERDETAIL ON DELIVERYORDERDETAIL.DOMID=DELIVERYORDERMASTER.DOMID INNER JOIN
			 dbo.BILLENTRYDETAIL ON DELIVERYORDERDETAIL.BILLEDID=BILLENTRYDETAIL.BILLEDID INNER JOIN
                      dbo.GATEINDETAIL ON dbo.BILLENTRYDETAIL.GIDID = dbo.GATEINDETAIL.GIDID INNER JOIN
					   dbo.CONTAINERSIZEMASTER ON dbo.CONTAINERSIZEMASTER.CONTNRSID = dbo.GATEINDETAIL.CONTNRSID 

WHERE   (dbo.DELIVERYORDERMASTER.COMPYID = @LCompyId) and
(DODATE BETWEEN @PSDate AND @PEDate) AND (DELIVERYORDERMASTER.SDPTID=1) AND (@FilterTerm IS NULL 
             OR DODATE LIKE @FilterTerm
              OR DONO LIKE @FilterTerm
              OR DODREFNAME  LIKE @FilterTerm
              OR CONTNRSDESC    LIKE @FilterTerm
              oR DOREFNAME LIKE @FilterTerm
              OR DODREFNO    LIKE @FilterTerm
             
			  OR DODNO    LIKE @FilterTerm
			  OR IGMNO    LIKE @FilterTerm
			  OR GPLNO    LIKE @FilterTerm)
			   
ORDER BY dbo.DELIVERYORDERMASTER.DONO
            
    SELECT DODATE, DONO, DODREFNAME, CONTNRSDESC, DOREFNAME, DODREFNO,  DODID, DOMID, 
	case DISPSTATUS when 1 then 'C' else '' end as DISPSTATUS,DODNO,IGMNO,GPLNO
    FROM    @TableMaster
    WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum
    ORDER BY DODNO DESC

SELECT    @TotalRowsCount = COUNT(*)
FROM         dbo.DELIVERYORDERMASTER INNER JOIN
             dbo.DELIVERYORDERDETAIL ON DELIVERYORDERDETAIL.DOMID=DELIVERYORDERMASTER.DOMID INNER JOIN
			 dbo.BILLENTRYDETAIL ON DELIVERYORDERDETAIL.BILLEDID=BILLENTRYDETAIL.BILLEDID INNER JOIN
                      dbo.GATEINDETAIL ON dbo.BILLENTRYDETAIL.GIDID = dbo.GATEINDETAIL.GIDID INNER JOIN
					   dbo.CONTAINERSIZEMASTER ON dbo.CONTAINERSIZEMASTER.CONTNRSID = dbo.GATEINDETAIL.CONTNRSID 
WHERE dbo.DELIVERYORDERMASTER.SDPTID = 1 AND  dbo.DELIVERYORDERMASTER.COMPYID = @LCompyId AND DODATE BETWEEN @LSDate AND @LEDate

    
    SELECT @FilteredRowsCount = COUNT(*)
    FROM   @TableMaster
        
END

