drop table #tmpstk

select * into #tmpstk
from GATEINDETAIL
where GIDATE <= '2021-09-30'
and SDPTID = 1
and CONTNRSID > 2

delete a
from #tmpstk a  join GATEOUTDETAIL b(nolock) on a.GIDID = b.GIDID and a.SDPTID = b.SDPTID
where GODATE < '2021-10-01'

delete a
--select b.godid, a.*
into oldstockdetail_bkup111021
update c
set CONTNRSID = 99, LMUSRID = 'administrator', SDPTID = 99
from #tmpstk a  join scfs.dbo.GATEOUTDETAIL b(nolock) on a.OLDGID= b.GIDID and a.SDPTID = b.SDPTID
				join scfs_erp.dbo.gateindetail c(nolock) on a.gidid = c.gidid and a.SDPTID = c.SDPTID
where GODATE < '2021-10-01'

select * from #tmpstk where OLDGID = 978853
select * from scfs_erp.dbo.GATEOUTDETAIL where  OLDGODID = 635789
select * from scfs.dbo.BILLENTRYDETAIL(nolock) where gidid = 978853
select * from scfs.dbo.transactiondetail (nolock) where BILLEDID= 307308
select * from scfs.dbo.deliveryorderdetail (nolock) where BILLEDID= 307308
select * from scfs_erp.dbo.deliveryorderdetail (nolock) where OLDDODID= 180336

select * from SCFS_ERP.dbo.OPENSHEETDETAIL (nolock) where GIDID = 33862
select * from SCFS_ERP.dbo.authorizationslipdetail (nolock) where OSDID = 15469
select * from SCFS_ERP.dbo.vehicleticketdetail (nolock) where ASLDID= 13788