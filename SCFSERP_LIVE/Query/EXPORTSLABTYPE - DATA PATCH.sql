select * from EXPORTSLABTYPEMASTER
where SLABTDESC like 'on wheel%'

update EXPORTSLABTYPEMASTER
set DISPSTATUS = 1
where SLABTDESC like 'on wheel%'
and SLABTID in (8, 9, 18)