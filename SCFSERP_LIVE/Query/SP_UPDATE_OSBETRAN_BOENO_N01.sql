-- =============================================  
-- Author:  <Yamuna J>  
-- Create date: <22/12/2021>  
-- Description: <Update query for BOENO>  
-- =============================================  
CREATE PROCEDURE SP_UPDATE_OSBETRAN_BOENO_N01  
  
 @OSMID int , @OSBOENO Varchar(15) , @OSBOEDATE smalldatetime
AS  
BEGIN  
   
 SET NOCOUNT ON;  
    
  update a  set a.BOENO = @OSBOENO, a.BOEDATE = @OSBOEDATE from   OPENSHEETMASTER a     
  inner join OPENSHEETDETAIL b (nolock) on a.OSMID = b.OSMID      
  inner join BILLENTRYDETAIL c (nolock) on c.BILLEDID = b.BILLEDID   
  inner join BILLENTRYMASTER d (nolock) on d.BILLEMID = c.BILLEMID    
  where a.OSMID = @OSMID   
    
  update d  set d.BILLEMDNO = @OSBOENO, d.BILLEMDATE = @OSBOEDATE from   OPENSHEETMASTER a     
  inner join OPENSHEETDETAIL b (nolock) on a.OSMID = b.OSMID      
  inner join BILLENTRYDETAIL c (nolock) on c.BILLEDID = b.BILLEDID     
  inner join BILLENTRYMASTER d (nolock) on d.BILLEMID = c.BILLEMID     
  where b.OSMID = @OSMID     
    
  update e  set e.TRANDREFNO = @OSBOENO  from   OPENSHEETMASTER a     
  inner join OPENSHEETDETAIL b (nolock) on a.OSMID = b.OSMID       
  inner join BILLENTRYDETAIL c (nolock) on c.BILLEDID = b.BILLEDID     
  inner join BILLENTRYMASTER d (nolock) on d.BILLEMID = c.BILLEMID     
  inner join TRANSACTIONDETAIL e (nolock) on b.GIDID = e.TRANDREFID    
  inner join TRANSACTIONMASTER f  (nolock) on f.TRANMID = e.TRANMID     
  where  b.GIDID = e.TRANDREFID and e.BILLEDID = c.BILLEDID and  b.OSMID = @OSMID --and  a.OSMID = @OSMID  
  
END  