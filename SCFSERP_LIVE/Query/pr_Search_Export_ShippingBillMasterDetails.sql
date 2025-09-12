-- =============================================
-- Author:		<Yamuna J>
-- Create date: <26/07/2021>
-- Description:	<Export Shipping Bill Master Detail>
-- declare @TotalRowsCount int , @FilteredRowsCount int exec [pr_Search_Export_ShippingBillMasterDetails] null, 1, 'asc',1,10, @TotalRowsCount output, @FilteredRowsCount output,'2021-07-01', '2021-11-30',32 select  @TotalRowsCount ,@FilteredRowsCount
-- =============================================
alter PROCEDURE [dbo].[pr_Search_Export_ShippingBillMasterDetails]
  (
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
   )
 
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
      ESBMDATE SMALLDATETIME
    
      , ESBMDNO VARCHAR(100)
      , EXPRTNAME  VARCHAR(100)
      , CHANAME  VARCHAR(100)
	   ,VHLNO VARCHAR(25)
	  ,ESBMNOP numeric(18,2)
	   ,ESBMQTY numeric(18,4)
	 
	  , ESBMID int 
	 
      , RowNum INT
	  , SAFlg int
	  ,GIDID int
    )

    INSERT INTO @TableMaster(
	  ESBMDATE
	 
      , ESBMDNO 
      , EXPRTNAME  ,CHANAME
	   ,VHLNO 
	 ,ESBMNOP  ,ESBMQTY
	
	 
	  , ESBMID
        , RowNum
		,SAFlg,GIDID)
    SELECT  ESBMDATE
      
      , ESBMDNO 
      , EXPRTNAME  ,EXPORTSHIPPINGBILLMASTER.CHANAME, isnull(GATEINDETAIL.VHLNO,'')
	    ,ESBMNOP  ,ESBMQTY
	  ,EXPORTSHIPPINGBILLMASTER.ESBMID
            , Row_Number() OVER (
            ORDER BY
            
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
           	      WHEN 1 then ESBMDNO
			  	  WHEN 2 then EXPRTNAME
				     WHEN 3 then EXPORTSHIPPINGBILLMASTER.CHANAME
			  	 
                END             
            END ASC,
            
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
           	      WHEN 1 then ESBMDNO
			  	  WHEN 2 then EXPRTNAME
				   WHEN 3 then EXPORTSHIPPINGBILLMASTER.CHANAME
			  	
                END
            END DESC,
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
                  WHEN 0 THEN ESBMDATE
                
                END             
            END ASC,

            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 0 THEN ESBMDATE
				
                END
            END DESC,
            
       
			  CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex  
                
				   WHEN 4 then ESBMNOP
				      WHEN 5 then ESBMQTY
                END             
            END ASC,

            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
				
				    WHEN 4 then ESBMNOP
					 WHEN 5 then ESBMQTY
                END
            END DESC                             

            ) AS RowNum
			,1 -- 0 for allow to modify SA and 1 for record not available to modify SA
			, GATEINDETAIL.GIDID
   FROM         dbo.EXPORTSHIPPINGBILLMASTER   /* CHANGE  TABLE NAME */
		LEFT JOIN GATEINDETAIL ON EXPORTSHIPPINGBILLMASTER.ESBMID = GATEINDETAIL.ESBMID
    WHERE (EXPORTSHIPPINGBILLMASTER.COMPYID=@LCOMPYID) and (ESBMDATE BETWEEN @LSDate AND @LEDate) AND (@FilterTerm IS NULL 
              OR ESBMDATE LIKE @FilterTerm
              OR VHLNO LIKE @FilterTerm
              OR ESBMDNO LIKE @FilterTerm
              OR  EXPRTNAME  LIKE @FilterTerm
			    OR  EXPORTSHIPPINGBILLMASTER.CHANAME  LIKE @FilterTerm
				  OR  ESBMNOP  LIKE @FilterTerm
			    OR  ESBMQTY  LIKE @FilterTerm
           )

	
	update @TableMaster
	set SAFlg = case when b.ESBMID IS not NULL then 0 else 1 end
	from @tablemaster as A LEFT JOIN SHIPPINGBILLDETAIL as b
	ON a.ESBMID = b.ESBMID  and A.GIDID = b.GIDID
--	WHERE b.ESBMID IS not NULL

    SELECT ESBMDATE
      
      , ESBMDNO 
      , EXPRTNAME ,CHANAME , VHLNO
	 ,ESBMNOP  ,ESBMQTY
	  , ESBMID, SAFlg, GIDID
    FROM    @TableMaster
    WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum
    
    SELECT @TotalRowsCount = COUNT(*)
   FROM        dbo.EXPORTSHIPPINGBILLMASTER  /* CHANGE  TABLE NAME */
    WHERE  (COMPYID=@LCOMPYID) and (ESBMDATE BETWEEN @LSDate AND @LEDate)
	
    
    SELECT @FilteredRowsCount = COUNT(*)
    FROM   @TableMaster
        
END