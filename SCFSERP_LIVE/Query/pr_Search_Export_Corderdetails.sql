
-- EXEC pr_Search_Export_Corderdetails @ESBMID =65020, @GIDID = 201755
-- select * From EXPORTSHIPPINGBILLMASTER where ESBMID =65020
-- select * From GATEINDETAIL where ESBMID =201755
-- =============================================
-- Author:		<Yamuna J>
-- Create date: <14/08/2021>
-- Description:	<Get Shipping Admission>
-- exec [pr_Search_Export_Corderdetails] 23294
-- exec pr_Search_Export_Corderdetails @ESBMID =22537
-- =============================================
--   EXEC [dbo].[pr_Search_Export_Corderdetails] @ESBMID = 2070
--select isnull(Convert(varchar(10),GATEINDETAIL.GIDATE, 103),'') as GIDATE from GATEINDETAIL

alter PROCEDURE [dbo].[pr_Search_Export_Corderdetails](
 @ESBMID int,
 @GIDID int
)
AS
BEGIN
	
	--Wrap filter term with % to search for values that contain @FilterTerm
   
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
      , EXPRTID int 
	  , EXPRTNAME nvarchar(100)
	  , DESTINATION nvarchar(250)
	)

	INSERT INTO @tblshippingaddmission (GIDATE , GIDNO , ESBMIDATE , ESBMDNO , SBMDATE , SBMDNO , PRDTDESC, VHLNO , 
	                                    VSLNAME , SBDQTY , DISPSTATUS  , GIDID , ESBMID  , SBMID  , SBDID ,EXPRTID, EXPRTNAME,DESTINATION) 

	SELECT TOP 100 PERCENT isnull(Convert(varchar(10),GATEINDETAIL.GIDATE, 103),'') as GIDATE, ISNULL(GATEINDETAIL.GIDNO,'') as GIDNO,
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
						   ISNULL(SHIPPINGBILLDETAIL.SBDID,0) as SBDID ,ISNULL(EXPORTSHIPPINGBILLMASTER.EXPRTID,0) as EXPRTID, 
						   ISNULL(EXPORTSHIPPINGBILLMASTER.EXPRTNAME,'') as EXPRTNAME,ISNULL(EXPORTSHIPPINGBILLMASTER.DESTINATION,'') as DESTINATION

	FROM    GATEINDETAIL (nolock) 				
				left Join SHIPPINGBILLDETAIL (nolock)  ON GATEINDETAIL.GIDID    = SHIPPINGBILLDETAIL.GIDID 
				Left join SHIPPINGBILLMASTER (nolock) ON SHIPPINGBILLDETAIL.SBMID= SHIPPINGBILLMASTER.SBMID  
				left join EXPORTSHIPPINGBILLMASTER (nolock) ON SHIPPINGBILLDETAIL.ESBMID = EXPORTSHIPPINGBILLMASTER.ESBMID and   EXPORTSHIPPINGBILLMASTER.ESBMID = @ESBMID
				   
		   --left Join EXPORTSHIPPINGBILLDETAIL (nolock)  ON SHIPPINGBILLDETAIL.GIDID = EXPORTSHIPPINGBILLDETAIL.GIDID  
   WHERE   GATEINDETAIL.CONTNRID=1  
    
   and (GATEINDETAIL.GIDID = @GIDID or @GIDID=0)

   SELECT GIDATE , GIDNO , ESBMIDATE , ESBMDNO , SBMDATE , SBMDNO , PRDTDESC, VHLNO , 
	                                    VSLNAME , SBDQTY , DISPSTATUS  , GIDID , ESBMID  , SBMID, -- , SBDID 
										EXPRTID, EXPRTNAME,DESTINATION
    FROM    @tblshippingaddmission     
	GROUP BY GIDATE , GIDNO , ESBMIDATE , ESBMDNO , SBMDATE , SBMDNO , PRDTDESC, VHLNO , 
	                                    VSLNAME , SBDQTY , DISPSTATUS  , GIDID , ESBMID  , SBMID , --, SBDID 
										EXPRTID, EXPRTNAME,DESTINATION

   
END



