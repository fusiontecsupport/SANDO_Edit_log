/*

DECLARE @TotalRowsCount INT,
        @FilteredRowsCount INT

EXEC [dbo].[pr_SearchExportCartingOrder] NULL, 1, 'ASC', 1, 10, @TotalRowsCount OUTPUT,@FilteredRowsCount OUTPUT,
'2021-07-01', '2021-08-29', 32

SELECT @TotalRowsCount, @FilteredRowsCount
*/
alter PROCEDURE [dbo].[pr_SearchExportCartingOrder]  /* CHANGE */
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
      SBMDATE SMALLDATETIME
      ,SBMTIME DATETIME
      , SBMNO int
      , EXPRTNAME  VARCHAR(100)
      , CHANAME  VARCHAR(100)
	   ,DISPSTATUS smallint,SBMDNO VARCHAR(100)
	  , SBMID int 
      , RowNum INT
	  , SAFlg int
	  ,ESBMID int
    )

    INSERT INTO @TableMaster(
	  SBMDATE
	  ,SBMTIME
      , SBMNO 
      , EXPRTNAME  ,CHANAME
	
	
	   ,DISPSTATUS,SBMDNO 
	  , SBMID
        , RowNum
		--,SAFlg
		--,ESBMID
		)
    SELECT  SBMDATE
      , SBMTIME
      , SBMNO 
      , EXPRTNAME  ,CHANAME
	    ,SHIPPINGBILLMASTER.DISPSTATUS,SBMDNO 
	  , SHIPPINGBILLDETAIL.SBMID
            , Row_Number() OVER (
            ORDER BY
            
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
           	      WHEN 2 then SBMNO
			  	  WHEN 3 then EXPRTNAME
				   
			  	 
                END             
            END ASC,
            
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
           	      WHEN 2 then SBMNO
			  	  WHEN 3 then EXPRTNAME
				 
			  	
                END
            END DESC,
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
                  WHEN 0 THEN SBMDATE
                  WHEN 1 THEN SBMTIME
                END             
            END ASC,

            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 0 THEN SBMDATE
				  when 1 then SBMTIME
                END
            END DESC,
            
       
			  CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex  
                
				   WHEN 4 then CHANAME
				   WHEN 6 THEN SBMDNO 
                END             
            END ASC,

            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
				
				    WHEN 4 then CHANAME
					WHEN 6 THEN SBMDNO
                END
            END DESC                             

            ) AS RowNum 
			--,1 -- 0 for allow to modify SA and 1 for record not available to modify SA
			--, SHIPPINGBILLDETAIL.ESBMID
   FROM         dbo.SHIPPINGBILLMASTER join SHIPPINGBILLDETAIL on SHIPPINGBILLMASTER.SBMID  = SHIPPINGBILLDETAIL.SBMID /* CHANGE  TABLE NAME */
    WHERE (COMPYID=@LCOMPYID) and (SBMDATE BETWEEN @LSDate AND @LEDate) AND (@FilterTerm IS NULL 
              OR SBMDATE LIKE @FilterTerm
              OR SBMTIME LIKE @FilterTerm
              OR SBMNO LIKE @FilterTerm
              OR  EXPRTNAME  LIKE @FilterTerm
			    OR  CHANAME  LIKE @FilterTerm
				OR SBMDNO  LIKE @FilterTerm
           )

	--update @TableMaster
	--set SAFlg = 0
	--from @tablemaster as A, EXPORTSHIPPINGBILLDETAIL as b
	--where a.ESBMID = b.ESBMID

    SELECT SBMDATE
      , SBMTIME
      , SBMNO 
      , EXPRTNAME ,CHANAME 
	    ,case DISPSTATUS when 1 then 'C' end as  DISPSTATUS,SBMDNO 
	  , SBMID
	 -- , SAFlg
    FROM    @TableMaster
    WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum
	ORDER BY SBMDNO  DESC
    
 SELECT @TotalRowsCount = COUNT(distinct SHIPPINGBILLMASTER.sbmid)
   FROM         dbo.SHIPPINGBILLMASTER join SHIPPINGBILLDETAIL on SHIPPINGBILLMASTER.SBMID  = SHIPPINGBILLDETAIL.SBMID /* CHANGE  TABLE NAME */
    WHERE (COMPYID=@LCOMPYID) and (SBMDATE BETWEEN @LSDate AND @LEDate)
	
    
    SELECT @FilteredRowsCount = COUNT(*)
    FROM   @TableMaster
        
END

