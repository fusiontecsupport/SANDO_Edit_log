
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
-- =============================================
-- Author:		<Rajesh S>
-- Create date: <14/08/2021>
-- Description:	<Get Shipping Admission>
--  exec pr_Search_ExportShippingAdmission_Multiple '2021-05-01', '2021-09-30', 32, 27526,45714
-- =============================================
ALTER PROCEDURE [dbo].pr_Search_ExportShippingAdmission_Multiple
@PSDate Smalldatetime
, @PEDate Smalldatetime
, @PCompyId INT
, @PCHAID int
, @PEXPRTID int
AS
BEGIN

Declare @LCompyId int,@LSDate Smalldatetime, @LEDate Smalldatetime,@LCHAID int, @LEXPRTID int
set @LCompyId = @PCompyId
set @LCHAID = @PCHAID
set @LEXPRTID = @PEXPRTID
set @LSDate = @PSDate
set @LEDate = @PEDate
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
	  , ESBMWGHT int 	 
      , RowNum INT
	  --,ESBDID INT
	)

	DECLARE @SortDirection VARCHAR(5), @SortIndex INT
	set @SortDirection = 'DESC'
	SET @SortIndex =  2

	INSERT INTO @tblshippingaddmission (GIDATE , GIDNO , ESBMIDATE , ESBMDNO , SBMDATE , SBMDNO , PRDTDESC, VHLNO , 
	                                    VSLNAME , SBDQTY , DISPSTATUS  , GIDID , ESBMID  , SBMID  , SBDID ,ESBMWGHT, RowNum--, ESBDID 
										) 

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
						   ISNULL(SHIPPINGBILLDETAIL.SBDID,0) as SBDID,  ISNULL(EXPORTSHIPPINGBILLDETAIL.ESBMWGHT,0) as ESBMWGHT,
						   --, ISNULL(EXPORTSHIPPINGBILLDETAIL.ESBDID,0) AS ESBDID,
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
		   EXPORTSHIPPINGBILLDETAIL (nolock) ON EXPORTSHIPPINGBILLMASTER.ESBMID = EXPORTSHIPPINGBILLDETAIL.ESBMID  Left join
		   GATEINDETAIL (nolock) ON SHIPPINGBILLDETAIL.GIDID = GATEINDETAIL.GIDID    
   WHERE   GATEINDETAIL.CONTNRID=1 
   and	   GATEINDETAIL.COMPYID = @LCompyId
   and		isnull(EXPORTSHIPPINGBILLDETAIL.ESBMID,0) = 0
   and	   SHIPPINGBILLMASTER.EXPRTID = @LEXPRTID
   AND		SHIPPINGBILLMASTER.CHAID = @LCHAID
	--group by 
	--isnull(Convert(varchar(10),GATEINDETAIL.GIDATE, 103),''), ISNULL(GATEINDETAIL.GIDNO,''),
	--                       isnull(Convert(varchar(10),EXPORTSHIPPINGBILLMASTER.ESBMIDATE, 103),''),
	--					   ISNULL(EXPORTSHIPPINGBILLMASTER.ESBMDNO,'') ,
	--					    isnull(Convert(varchar(10),SHIPPINGBILLMASTER.SBMDATE, 103),'') ,
	--					   ISNULL(SHIPPINGBILLMASTER.SBMDNO,''), 
	--					   ISNULL(GATEINDETAIL.PRDTDESC,'') ,ISNULL(GATEINDETAIL.VHLNO,'') ,
	--                       ISNULL(GATEINDETAIL.VSLNAME,'') ,ISNULL(SHIPPINGBILLDETAIL.UNLOADEDNOP,0),
	--					   ISNULL(SHIPPINGBILLMASTER.DISPSTATUS,0) ,ISNULL(SHIPPINGBILLDETAIL.GIDID,0),
	--					   ISNULL(EXPORTSHIPPINGBILLMASTER.ESBMID,0) , ISNULL(SHIPPINGBILLMASTER.SBMID,0) ,
	--					   ISNULL(SHIPPINGBILLDETAIL.SBDID,0)
		

   SELECT GIDATE , GIDNO , ESBMIDATE , ESBMDNO , SBMDATE , SBMDNO , PRDTDESC, VHLNO , 
	                                    VSLNAME , SBDQTY , DISPSTATUS  , GIDID , ESBMID  , SBMID ,ESBMWGHT --, ESBDID 
    FROM    @tblshippingaddmission
	order BY GIDATE , GIDNO , ESBMIDATE , ESBMDNO , SBMDATE , SBMDNO , PRDTDESC, VHLNO , 
	                                    VSLNAME , SBDQTY , DISPSTATUS  , GIDID , ESBMID  , SBMID ,ESBMWGHT --, ESBDID 



END


