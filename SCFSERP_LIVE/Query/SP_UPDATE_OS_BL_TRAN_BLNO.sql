-- =============================================  
-- Author:  <Yamuna J>  
-- Create date: <22/12/2021>  
-- Description: <Update query for BOENO>  
-- =============================================  
CREATE PROCEDURE SP_UPDATE_OS_BL_TRAN_BLNO  
  
 @OSMID int , @OSBLNO Varchar(15)   
AS  
BEGIN  
   
 SET NOCOUNT ON;  
    
  update a  set a.OSMBLNO = @OSBLNO   from   OPENSHEETMASTER a     
  inner join OPENSHEETDETAIL b (nolock) on a.OSMID = b.OSMID      
  inner join BILLENTRYDETAIL c (nolock) on c.BILLEDID = b.BILLEDID   
  inner join BILLENTRYMASTER d (nolock) on d.BILLEMID = c.BILLEMID    
  where a.OSMID = @OSMID   
    
  update d  set d.BLNO = @OSBLNO from   OPENSHEETMASTER a     
  inner join OPENSHEETDETAIL b (nolock) on a.OSMID = b.OSMID      
  inner join BILLENTRYDETAIL c (nolock) on c.BILLEDID = b.BILLEDID     
  inner join BILLENTRYMASTER d (nolock) on d.BILLEMID = c.BILLEMID     
  where b.OSMID = @OSMID     
  
  update g  set g.GPBLNO = @OSBLNO from   OPENSHEETMASTER a     
  inner join OPENSHEETDETAIL b (nolock) on a.OSMID = b.OSMID      
  inner join BILLENTRYDETAIL c (nolock) on c.BILLEDID = b.BILLEDID     
  inner join BILLENTRYMASTER d (nolock) on d.BILLEMID = c.BILLEMID
  inner join GATEINDETAIL g (nolock) on b.GIDID = g.GIDID
  where b.OSMID = @OSMID   

  /*
  update e  set e.TRANDREFNO = @OSBOENO  from   OPENSHEETMASTER a     
  inner join OPENSHEETDETAIL b (nolock) on a.OSMID = b.OSMID       
  inner join BILLENTRYDETAIL c (nolock) on c.BILLEDID = b.BILLEDID     
  inner join BILLENTRYMASTER d (nolock) on d.BILLEMID = c.BILLEMID     
  inner join TRANSACTIONDETAIL e (nolock) on b.GIDID = e.TRANDREFID    
  inner join TRANSACTIONMASTER f  (nolock) on f.TRANMID = e.TRANMID     
  where  b.GIDID = e.TRANDREFID and e.BILLEDID = c.BILLEDID and  b.OSMID = @OSMID --and  a.OSMID = @OSMID  
  */

END  