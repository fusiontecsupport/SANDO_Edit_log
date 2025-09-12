-- =============================================
-- Author:		<Yamuna J>
-- Create date: <14/08/2021>
-- Description:	<Get Shipping Admission>
-- declare @TotalRowsCount int , @FilteredRowsCount int exec [pr_Search_Export_Shipping_Admission] null, 1, 'asc',1,10, @TotalRowsCount output, @FilteredRowsCount output select  @TotalRowsCount ,@FilteredRowsCount
-- =============================================
alter PROCEDURE [dbo].[pr_Search_Export_Shipping_Admission](
  @FilterTerm nvarchar(250) = NULL --parameter to search all columns by
        , @SortIndex INT = 1 -- a one based index to indicate which column to order by
        , @SortDirection CHAR(4) = 'ASC' --the direction to sort in, either ASC or DESC
        , @StartRowNum INT = 1 --the first row to return
        , @EndRowNum INT = 10 --the last row to return
        , @TotalRowsCount INT OUTPUT
        , @FilteredRowsCount INT OUTPUT
)
AS
BEGIN
	
	--Wrap filter term with % to search for values that contain @FilterTerm
    SET @FilterTerm = '%' + @FilterTerm + '%'
	SET NOCOUNT ON;

	Declare  @tblshippingaddmission Table 
	(
	    GIDATE varchar(10)     
      , GIDNO VARCHAR(25)
      , ESBMIDATE  varchar(10)  
      , ESBMDNO   VARCHAR(25)
	  , SBMDATE varchar(10)  
	  , SBMDNO VARCHAR(25)
      , PRDTDESC VARCHAR(150)
	  , VHLNO VARCHAR(150)
	  , VSLNAME VARCHAR(100)
	  , SBDQTY numeric(18,2) 
	  , DISPSTATUS SMALLINT
	  , GIDID int 
	  , ESBMID int 
	  , SBMID int 	 
	  , SBDID int 	 
      , RowNum INT
	)

	INSERT INTO @tblshippingaddmission (GIDATE , GIDNO , ESBMIDATE , ESBMDNO , SBMDATE , SBMDNO , PRDTDESC, VHLNO , 
	                                    VSLNAME , SBDQTY , DISPSTATUS  , GIDID , ESBMID  , SBMID  , SBDID , RowNum ) 

	SELECT  TOP 100 PERCENT isnull(Convert(varchar(10),GATEINDETAIL.GIDATE, 103),'') as GIDATE, ISNULL(GATEINDETAIL.GIDNO,'') as GIDNO,
	                       isnull(Convert(varchar(10),EXPORTSHIPPINGBILLMASTER.ESBMIDATE, 103),'') as ESBMIDATE,
	                       
	                       --ISNULL(EXPORTSHIPPINGBILLMASTER.ESBMIDATE,'') as ESBMIDATE, 
						   ISNULL(EXPORTSHIPPINGBILLMASTER.ESBMDNO,'') as ESBMDNO, 
						   --ISNULL(SHIPPINGBILLMASTER.SBMDATE,'') as SBMDATE, 
						    isnull(Convert(varchar(10),SHIPPINGBILLMASTER.SBMDATE, 103),'') as SBMDATE,
						   ISNULL(SHIPPINGBILLMASTER.SBMDNO,'') as SBMDNO, 
						   ISNULL(GATEINDETAIL.PRDTDESC,'') as PRDTDESC,ISNULL(GATEINDETAIL.VHLNO,'') as VHLNO,
	                       ISNULL(GATEINDETAIL.VSLNAME,'') as VSLNAME,ISNULL(SHIPPINGBILLDETAIL.UNLOADEDNOP,0) as SBDQTY,
						   ISNULL(SHIPPINGBILLMASTER.DISPSTATUS,0) as DISPSTATUS,ISNULL(SHIPPINGBILLDETAIL.GIDID,0) as GIDID,
						   ISNULL(EXPORTSHIPPINGBILLMASTER.ESBMID,0) as ESBMID, ISNULL(SHIPPINGBILLMASTER.SBMID,0) as SBMID,
						   ISNULL(SHIPPINGBILLDETAIL.SBDID,0) as SBDID,
						   Row_Number() OVER (  ORDER BY
							       /*VARCHAR, NVARCHAR, CHAR ORDER BY*/   
		          CASE @SortDirection
				    WHEN 'ASC'  THEN
					CASE @SortIndex
				
           	      WHEN 2 then GATEINDETAIL.GIDNO
			  	  WHEN 3 then EXPORTSHIPPINGBILLMASTER.ESBMDNO
			  	  WHEN 5 then SHIPPINGBILLMASTER.SBMDNO
			  	  WHEN 6 then GATEINDETAIL.PRDTDESC
				  WHEN 7 then GATEINDETAIL.VHLNO
				  WHEN 8 then GATEINDETAIL.VSLNAME			  	  
                END             
            END ASC,
			CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex

           	    WHEN 2 then GATEINDETAIL.GIDNO
			  	  WHEN 3 then EXPORTSHIPPINGBILLMASTER.ESBMDNO
			  	  WHEN 5 then SHIPPINGBILLMASTER.SBMDNO
			  	  WHEN 6 then GATEINDETAIL.PRDTDESC
				  WHEN 7 then GATEINDETAIL.VHLNO
				  WHEN 8 then GATEINDETAIL.VSLNAME	
                END
            END DESC,
            
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
					WHEN 1 then SHIPPINGBILLMASTER.SBMNO
                  
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
				  WHEN 1 then SHIPPINGBILLMASTER.SBMNO                 
                END
            END DESC                            

            ) AS RowNum

	FROM   SHIPPINGBILLDETAIL (nolock) Inner Join 
	       SHIPPINGBILLMASTER (nolock) ON SHIPPINGBILLMASTER.SBMID = SHIPPINGBILLDETAIL.SBMID INNER Join 
		   EXPORTSHIPPINGBILLMASTER (nolock) ON SHIPPINGBILLDETAIL.ESBMID = EXPORTSHIPPINGBILLMASTER.ESBMID  Left join 
		   EXPORTSHIPPINGBILLDETAIL (nolock) ON EXPORTSHIPPINGBILLMASTER.ESBMID = EXPORTSHIPPINGBILLDETAIL.ESBMID  
		   and SHIPPINGBILLDETAIL.GIDID = EXPORTSHIPPINGBILLDETAIL.GIDID  Left join
		   GATEINDETAIL (nolock) ON SHIPPINGBILLDETAIL.GIDID = GATEINDETAIL.GIDID    
   WHERE   GATEINDETAIL.CONTNRID=1 
   --AND GIDATE>= '2022-04-01'
   and		(isnull(EXPORTSHIPPINGBILLDETAIL.ESBMID,0) = 0 OR ISNULL(SHIPPINGBILLDETAIL.GIDID,0) =0 or isnull(EXPORTSHIPPINGBILLDETAIL.GIDID,0) = 0)
   and		ISNULL(SHIPPINGBILLDETAIL.UNLOADEDNOP,0) > 0 
   AND (@FilterTerm is null OR
		 GATEINDETAIL.GIDNO like @FilterTerm or 
		 EXPORTSHIPPINGBILLMASTER.ESBMDNO like @FilterTerm or 
		 SHIPPINGBILLMASTER.SBMDNO  like @FilterTerm or 
		 GATEINDETAIL.PRDTDESC  like @FilterTerm or 
		 GATEINDETAIL.VHLNO  like @FilterTerm or 
		 GATEINDETAIL.VSLNAME	 like @FilterTerm )

--INSERT INTO @tblshippingaddmission (GIDATE , GIDNO , ESBMIDATE , ESBMDNO , SBMDATE , SBMDNO , PRDTDESC, VHLNO , 
--	                                    VSLNAME , SBDQTY , DISPSTATUS  , GIDID , ESBMID  , SBMID  , SBDID , RowNum ) 

--	SELECT  TOP 100 PERCENT isnull(Convert(varchar(10),GATEINDETAIL.GIDATE, 103),'') as GIDATE, ISNULL(GATEINDETAIL.GIDNO,'') as GIDNO,
--	                       isnull(Convert(varchar(10),EXPORTSHIPPINGBILLMASTER.ESBMIDATE, 103),'') as ESBMIDATE,
	                       
--	                       --ISNULL(EXPORTSHIPPINGBILLMASTER.ESBMIDATE,'') as ESBMIDATE, 
--						   ISNULL(EXPORTSHIPPINGBILLMASTER.ESBMDNO,'') as ESBMDNO, 
--						   --ISNULL(SHIPPINGBILLMASTER.SBMDATE,'') as SBMDATE, 
--						    isnull(Convert(varchar(10),SHIPPINGBILLMASTER.SBMDATE, 103),'') as SBMDATE,
--						   ISNULL(SHIPPINGBILLMASTER.SBMDNO,'') as SBMDNO, 
--						   ISNULL(GATEINDETAIL.PRDTDESC,'') as PRDTDESC,ISNULL(GATEINDETAIL.VHLNO,'') as VHLNO,
--	                       ISNULL(GATEINDETAIL.VSLNAME,'') as VSLNAME,ISNULL(SHIPPINGBILLDETAIL.UNLOADEDNOP,0) as SBDQTY,
--						   ISNULL(SHIPPINGBILLMASTER.DISPSTATUS,0) as DISPSTATUS,ISNULL(SHIPPINGBILLDETAIL.GIDID,0) as GIDID,
--						   ISNULL(EXPORTSHIPPINGBILLMASTER.ESBMID,0) as ESBMID, ISNULL(SHIPPINGBILLMASTER.SBMID,0) as SBMID,
--						   ISNULL(SHIPPINGBILLDETAIL.SBDID,0) as SBDID,
--						   Row_Number() OVER (  ORDER BY
--							       /*VARCHAR, NVARCHAR, CHAR ORDER BY*/   
--		          CASE @SortDirection
--				    WHEN 'ASC'  THEN
--					CASE @SortIndex
				
--           	      WHEN 2 then GATEINDETAIL.GIDNO
--			  	  WHEN 3 then EXPORTSHIPPINGBILLMASTER.ESBMDNO
--			  	  WHEN 5 then SHIPPINGBILLMASTER.SBMDNO
--			  	  WHEN 6 then GATEINDETAIL.PRDTDESC
--				  WHEN 7 then GATEINDETAIL.VHLNO
--				  WHEN 8 then GATEINDETAIL.VSLNAME			  	  
--                END             
--            END ASC,
--			CASE @SortDirection
--              WHEN 'DESC' THEN 
--                CASE @SortIndex

--           	    WHEN 2 then GATEINDETAIL.GIDNO
--			  	  WHEN 3 then EXPORTSHIPPINGBILLMASTER.ESBMDNO
--			  	  WHEN 5 then SHIPPINGBILLMASTER.SBMDNO
--			  	  WHEN 6 then GATEINDETAIL.PRDTDESC
--				  WHEN 7 then GATEINDETAIL.VHLNO
--				  WHEN 8 then GATEINDETAIL.VSLNAME	
--                END
--            END DESC,
            
            
--            CASE @SortDirection
--              WHEN 'ASC'  THEN
--                CASE @SortIndex
--					WHEN 1 then SHIPPINGBILLMASTER.SBMNO
                  
--                END             
--            END ASC,
--            CASE @SortDirection
--              WHEN 'DESC' THEN 
--                CASE @SortIndex
--				  WHEN 1 then SHIPPINGBILLMASTER.SBMNO                 
--                END
--            END DESC                            

--            ) AS RowNum

--	FROM   
--	       EXPORTSHIPPINGBILLMASTER (nolock) Left join 
--		   GATEINDETAIL (nolock) ON EXPORTSHIPPINGBILLMASTER.ESBMID = GATEINDETAIL.ESBMID  left join
--		   SHIPPINGBILLDETAIL (nolock) ON EXPORTSHIPPINGBILLMASTER.ESBMID  = SHIPPINGBILLDETAIL.ESBMID   left join 
--		   EXPORTSHIPPINGBILLDETAIL (nolock) ON EXPORTSHIPPINGBILLMASTER.ESBMID = EXPORTSHIPPINGBILLDETAIL.ESBMID  Left join
--		   SHIPPINGBILLMASTER (nolock) ON SHIPPINGBILLDETAIL.SBMID = SHIPPINGBILLMASTER.SBMID 		   
--   WHERE   GATEINDETAIL.CONTNRID=1 
--   --AND GIDATE>= '2022-04-01'
--   and		(ISNULL(SHIPPINGBILLDETAIL.GIDID,0) =0)
--   AND (@FilterTerm is null OR
--		 GATEINDETAIL.GIDNO like @FilterTerm or 
--		 EXPORTSHIPPINGBILLMASTER.ESBMDNO like @FilterTerm or 
--		 SHIPPINGBILLMASTER.SBMDNO  like @FilterTerm or 
--		 GATEINDETAIL.PRDTDESC  like @FilterTerm or 
--		 GATEINDETAIL.VHLNO  like @FilterTerm or 
--		 GATEINDETAIL.VSLNAME	 like @FilterTerm )
--	--group by 
--	--isnull(Convert(varchar(10),GATEINDETAIL.GIDATE, 103),''), ISNULL(GATEINDETAIL.GIDNO,''),
--	--                       isnull(Convert(varchar(10),EXPORTSHIPPINGBILLMASTER.ESBMIDATE, 103),''),
--	--					   ISNULL(EXPORTSHIPPINGBILLMASTER.ESBMDNO,'') ,
--	--					    isnull(Convert(varchar(10),SHIPPINGBILLMASTER.SBMDATE, 103),'') ,
--	--					   ISNULL(SHIPPINGBILLMASTER.SBMDNO,''), 
--	--					   ISNULL(GATEINDETAIL.PRDTDESC,'') ,ISNULL(GATEINDETAIL.VHLNO,'') ,
--	--                       ISNULL(GATEINDETAIL.VSLNAME,'') ,ISNULL(SHIPPINGBILLDETAIL.UNLOADEDNOP,0),
--	--					   ISNULL(SHIPPINGBILLMASTER.DISPSTATUS,0) ,ISNULL(SHIPPINGBILLDETAIL.GIDID,0),
--	--					   ISNULL(EXPORTSHIPPINGBILLMASTER.ESBMID,0) , ISNULL(SHIPPINGBILLMASTER.SBMID,0) ,
--	--					   ISNULL(SHIPPINGBILLDETAIL.SBDID,0)
		

   SELECT GIDATE , GIDNO , ESBMIDATE , ESBMDNO , SBMDATE , SBMDNO , PRDTDESC, VHLNO , 
	                                    VSLNAME , SBDQTY , DISPSTATUS  , GIDID , ESBMID  , SBMID -- , SBDID 
    FROM    @tblshippingaddmission
    WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum 
	GROUP BY GIDATE , GIDNO , ESBMIDATE , ESBMDNO , SBMDATE , SBMDNO , PRDTDESC, VHLNO , 
	                                    VSLNAME , SBDQTY , DISPSTATUS  , GIDID , ESBMID  , SBMID  --, SBDID 

   SELECT  @TotalRowsCount = COUNT(SHIPPINGBILLDETAIL.SBMID)
   FROM   SHIPPINGBILLDETAIL (nolock) Inner Join 
	       SHIPPINGBILLMASTER (nolock) ON SHIPPINGBILLMASTER.SBMID = SHIPPINGBILLDETAIL.SBMID INNER Join 
		   EXPORTSHIPPINGBILLMASTER (nolock) ON SHIPPINGBILLDETAIL.ESBMID = EXPORTSHIPPINGBILLMASTER.ESBMID  Left join 
		   EXPORTSHIPPINGBILLDETAIL (nolock) ON EXPORTSHIPPINGBILLMASTER.ESBMID = EXPORTSHIPPINGBILLDETAIL.ESBMID  
		   and SHIPPINGBILLDETAIL.GIDID = EXPORTSHIPPINGBILLDETAIL.GIDID  Left join
		   GATEINDETAIL (nolock) ON SHIPPINGBILLDETAIL.GIDID = GATEINDETAIL.GIDID    
    WHERE   GATEINDETAIL.CONTNRID=1 
	--AND GIDATE>= '2022-04-01'
   --and		isnull(EXPORTSHIPPINGBILLDETAIL.ESBMID,0) = 0
   and		(isnull(EXPORTSHIPPINGBILLDETAIL.ESBMID,0) = 0 OR ISNULL(SHIPPINGBILLDETAIL.GIDID,0) =0 or isnull(EXPORTSHIPPINGBILLDETAIL.GIDID,0) = 0)
   and		ISNULL(SHIPPINGBILLDETAIL.UNLOADEDNOP,0) > 0 

    SELECT @FilteredRowsCount = COUNT(*)
    FROM   @tblshippingaddmission 


END

--   EXEC [dbo].[pr_Search_Export_Shipping_Admission] @FilterTerm = '', @SortIndex = 1, @SortDirection = 'ASC', @StartRowNum = 1 , @EndRowNum = 100, @TotalRowsCount=100, @FilteredRowsCount=100  


--select isnull(Convert(varchar(10),GATEINDETAIL.GIDATE, 103),'') as GIDATE from GATEINDETAIL
