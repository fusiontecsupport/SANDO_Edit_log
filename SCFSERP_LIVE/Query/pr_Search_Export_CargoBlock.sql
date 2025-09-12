/*

DECLARE @TotalRowsCount INT
        @FilteredRowsCount INT

EXEC [dbo].[pr_Search_Export_CargoBlock] NULL, 1, 'ASC', 1, 10, @TotalRowsCount OUTPUT,@FilteredRowsCount OUTPUT,
'2021-07-01', '2021-07-29', 32

SELECT @TotalRowsCount, @FilteredRowsCount
*/
CREATE PROCEDURE [dbo].pr_Search_Export_CargoBlock  /* CHANGE */
 @FilterTerm nvarchar(250) = NULL --parameter to search all columns by
        , @SortIndex INT = 1 -- a one based index to indicate which column to order by
        , @SortDirection CHAR(4) = 'ASC' --the direction to sort in, either ASC or DESC
        , @StartRowNum INT = 1 --the first row to return
        , @EndRowNum INT = 10 --the last row to return
        , @TotalRowsCount INT OUTPUT
        , @FilteredRowsCount INT OUTPUT
        , @PSDate Smalldatetime
        , @PEDate Smalldatetime
		,@PCOMPYID INT
 
AS BEGIN
    --Wrap filter term with % to search for values that contain @FilterTerm
    SET @FilterTerm = '%' + @FilterTerm + '%'
	declare @LSDate Smalldatetime
        , @LEDate Smalldatetime,@LCOMPYID INT
		set @LSDate=@PSDate
		set @LEDate=@PEDate
		set @LCOMPYID=@PCOMPYID
    DECLARE @TableMaster TABLE
    (
		GBDATE SMALLDATETIME
      , GBNO INT
      , SBMDATE SMALLDATETIME
      , SBMTIME DATETIME
      , SBMNO int
      , EXPRTNAME  VARCHAR(100)
      , CHANAME  VARCHAR(100)
	  , VHLNO NVARCHAR(50)
	  , DISPSTATUS smallint
	  , SBMDNO VARCHAR(100)
	  , GBDNO VARCHAR(100)
	  , SBMID int 
	  , GBDID int 
	  ,GBTYPEDESC VARCHAR(15)
      , RowNum INT
    )

    INSERT INTO @TableMaster(GBDATE, GBNO, SBMDATE,SBMTIME, SBMNO,EXPRTNAME,CHANAME,VHLNO,DISPSTATUS,SBMDNO,GBDNO,SBMID,GBDID,GBTYPEDESC, RowNum)
    SELECT DISTINCT TOP(100)   GBDATE, GBNO, SBMDATE,SBMTIME, SBMNO,EXPRTNAME,SHIPPINGBILLMASTER.CHANAME,GATEINDETAIL.VHLNO,EXPORT_CARGO_BLOCK_DETAILS.DISPSTATUS,SBMDNO,GBDNO,
			SHIPPINGBILLMASTER.SBMID,EXPORT_CARGO_BLOCK_DETAILS.GBDID, CASE WHEN ISNULL(GBTYPE,0) = 0 THEN 'BLOCK' ELSE 'UNBLOCK' END
            , Row_Number() OVER (
            ORDER BY
            
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
           	      WHEN 2 then GBNO
				  WHEN 3 then SBMNO
			  	  WHEN 4 then EXPRTNAME
				  WHEN 5 then SHIPPINGBILLMASTER.CHANAME
				  WHEN 6 then GATEINDETAIL.VHLNO
			  	 
                END             
            END ASC,
            
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
           	      WHEN 2 then GBNO
				  WHEN 3 then SBMNO
			  	  WHEN 4 then EXPRTNAME
				  WHEN 5 then SHIPPINGBILLMASTER.CHANAME
				  WHEN 6 then GATEINDETAIL.VHLNO
			  	
                END
            END DESC,
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
                  WHEN 0 THEN GBDATE
				  WHEN 1 THEN SBMDATE				  
                  WHEN 2 THEN SBMTIME
                END             
            END ASC,

            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 0 THEN GBDATE
				  WHEN 1 THEN SBMDATE				  
                  WHEN 2 THEN SBMTIME
                END
            END DESC,
            
       
			  CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex  
                				   
				   WHEN 6 THEN GBDNO 
                END             
            END ASC,

            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex			
				    
					WHEN 6 THEN GBDNO
                END
            END DESC                             

            ) AS RowNum
   FROM [EXPORT_CARGO_BLOCK_DETAILS] 
		join  dbo.SHIPPINGBILLMASTER  (NOLOCK) on EXPORT_CARGO_BLOCK_DETAILS.sbmid = SHIPPINGBILLMASTER.SBMID
          Inner Join dbo.SHIPPINGBILLDETAIL (NOLOCK) ON SHIPPINGBILLDETAIL.SBMID = SHIPPINGBILLMASTER.SBMID  
          Inner Join GATEINDETAIL  (NOLOCK) ON SHIPPINGBILLDETAIL.GIDID = GATEINDETAIL.GIDID       /* CHANGE  TABLE NAME */
    WHERE GATEINDETAIL.SDPTID = 2 --AND (SHIPPINGBILLMASTER.COMPYID=@LCOMPYID) 
	       and (GBDATE BETWEEN @LSDate AND @LEDate) AND (@FilterTerm IS NULL 		   
              OR GBDATE LIKE @FilterTerm
			  OR SBMDATE LIKE @FilterTerm
              OR SBMTIME LIKE @FilterTerm
              OR SBMNO LIKE @FilterTerm
             OR SHIPPINGBILLMASTER.EXPRTNAME  LIKE @FilterTerm
			  OR SHIPPINGBILLMASTER.CHANAME  LIKE @FilterTerm
			  OR GATEINDETAIL.VHLNO  LIKE @FilterTerm
			  OR SBMDNO  LIKE @FilterTerm
           )

    SELECT DISTINCT GBDATE, GBNO,SBMDATE
      , SBMTIME
      , SBMNO 
      , EXPRTNAME ,CHANAME ,VHLNO
	  ,case DISPSTATUS when 1 then 'C' when 0 then 'In book' end as  DISPSTATUS,GBDNO
	  , GBDID, SBMID, SBMDNO , GBTYPEDESC
    FROM    @TableMaster
    WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum
	ORDER BY GBDATE DESC, SBMDATE  DESC
    
 SELECT @TotalRowsCount = COUNT(*)
   FROM [EXPORT_CARGO_BLOCK_DETAILS] 
		join  dbo.SHIPPINGBILLMASTER  (NOLOCK) on EXPORT_CARGO_BLOCK_DETAILS.sbmid = SHIPPINGBILLMASTER.SBMID
          Inner Join dbo.SHIPPINGBILLDETAIL (NOLOCK) ON SHIPPINGBILLDETAIL.SBMID = SHIPPINGBILLMASTER.SBMID  
          Inner Join GATEINDETAIL  (NOLOCK) ON SHIPPINGBILLDETAIL.GIDID = GATEINDETAIL.GIDID       /* CHANGE  TABLE NAME */
    WHERE GATEINDETAIL.SDPTID = 2 --AND (SHIPPINGBILLMASTER.COMPYID=@LCOMPYID) 
	       and (GBDATE BETWEEN @LSDate AND @LEDate)
    
    SELECT @FilteredRowsCount = COUNT(*)
    FROM   @TableMaster
        
END

