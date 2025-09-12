
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Yamuna J>
-- Create date: <22/12/2021>
-- Description:	<Update query for BOENO>
-- =============================================
CREATE PROCEDURE SP_UPDATE_OSBETRAN_BOENO

 @OSMID int , @OSBOENO Varchar(15) 
AS
BEGIN
	
	SET NOCOUNT ON;
	 
	 update a  set a.BOENO = @OSBOENO   from   OPENSHEETMASTER a   
	 inner join OPENSHEETDETAIL b (nolock) on a.OSMID = b.OSMID    
	 inner join BILLENTRYDETAIL c (nolock) on c.BILLEDID = b.BILLEDID 
	 inner join BILLENTRYMASTER d (nolock) on d.BILLEMID = c.BILLEMID  
	 where a.OSMID = @OSMID 
	 
	 update d  set d.BILLEMDNO = @OSBOENO from   OPENSHEETMASTER a   
	 inner join OPENSHEETDETAIL b (nolock) on a.OSMID = b.OSMID    
	 inner join BILLENTRYDETAIL c (nolock) on c.BILLEDID = b.BILLEDID   
	 inner join BILLENTRYMASTER d (nolock) on d.BILLEMID = c.BILLEMID   
	 where a.OSMID = @OSMID   
	 
	 update e  set e.TRANDREFNO = @OSBOENO  from   OPENSHEETMASTER a   
	 inner join OPENSHEETDETAIL b (nolock) on a.OSMID = b.OSMID     
	 inner join BILLENTRYDETAIL c (nolock) on c.BILLEDID = b.BILLEDID   
	 inner join BILLENTRYMASTER d (nolock) on d.BILLEMID = c.BILLEMID   
	 inner join TRANSACTIONDETAIL e (nolock) on b.GIDID = e.TRANDREFID  
	 inner join TRANSACTIONMASTER f  (nolock) on f.TRANMID = e.TRANMID   
	 where  b.GIDID = e.TRANDREFID and e.BILLEDID = c.BILLEDID and  b.OSMID = @OSMID and  a.OSMID = @OSMID

END
GO
