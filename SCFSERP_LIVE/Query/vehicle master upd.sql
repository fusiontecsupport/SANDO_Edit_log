
set identity_insert VEHICLEMASTER on 
insert into VEHICLEMASTER (VHLMID, VHLMDESC, AVHLMDESC, VHLDRVNAME, TRNSPRTID, CUSRID, LMUSRID, DISPSTATUS, PRCSDATE, VHLPNRNO)
select * from scfs.dbo.VEHICLEMASTER
where VHLMDESC not in (select VHLMDESC from VEHICLEMASTER)

update a
set vhlpnrno = b.VHLPNRNO
from  VEHICLEMASTER a, scfs.dbo.VEHICLEMASTER b
where a.vhlmdesc = b.VHLMDESC
set identity_insert VEHICLEMASTER off