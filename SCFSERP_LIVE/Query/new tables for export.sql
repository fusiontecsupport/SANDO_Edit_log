select * into EXPORT_SEALTYPE_MASTER from cfs..EXPORT_SEALTYPE_MASTER
select * into PRODUCTTYPEMASTER from cfs..PRODUCTTYPEMASTER
select * into EXPORT_VEHICLE_TYPE_MASTER from cfs..EXPORT_VEHICLE_TYPE_MASTER
select * into EXPORT_VEHICLE_GROUP_MASTER from cfs..EXPORT_VEHICLE_GROUP_MASTER
select * into EXPORTSHIPPINGBILLMASTER from cfs..EXPORTSHIPPINGBILLMASTER

select * from gateindetail WHERE SDPTID =2


-- select * from EXPORTSHIPPINGBILLMASTER
-- delete EXPORTSHIPPINGBILLMASTER
s
select * from 

update a
set VHLPNRNO = b.VHLPNRNO
from scfs_erp..VEHICLEMASTER a, SCFS..vehiclemaster b
where a.TRNSPRTID = b.TRNSPRTID 
and a.VHLMID = b.VHLMID

select 'update scfs_erp..vehiclemaster set vhlpnrno = ' + ''''+ vhlpnrno +'''' +
' where vhlmid = '+ '''' + cast(VHLMID as varchar(10))+'''' + ' and TRNSPRTID = ' + '''' + cast(TRNSPRTID as varchar(10))+''''
+ ' and vhlmdesc = ' + '''' + vhlmdesc+'''' 'qry',*
from scfs_erp..VEHICLEMASTER
where VHLPNRNO <> '' and VHLPNRNO <> '-'