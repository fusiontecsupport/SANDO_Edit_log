select * From spbw..TRANSACTIONDETAIL order by tranmid desc

select *from VW_BOND_RATECARDMASTER_FLX_ASSGN (NOLOCK) 
where TARIFFMID=5 
and SLABTID=2 
and HTYPE=1
AND YRDTYPE = 3 AND PERIODTID = 1 
--and CHRGETYPE=1 
and CONTNRSID= 3 and (PRDTGID = 217 or PRDTGID =0) order by SLABMIN
select SLABAMT,SLABMIN,SLABMAX ,*
from VW_BOND_RATECARDMASTER_FLX_ASSGN (NOLOCK) 
where TARIFFMID=5 
and SLABTID=4
and HTYPE=1 
AND YRDTYPE = 3 
AND PERIODTID = 1 
and CHRGETYPE=1 
and CONTNRSID= 3 
and (PRDTGID = 217 or PRDTGID =0) 
order by SLABMIN

select distinct chrgetype from BONDSLABMASTER
if exists (select '*' from BONDSLABMASTER where HTYPE = 0)
begin
	update BONDSLABMASTER
	set HTYPE = HTYPE + 1
end


select distinct SDTYPE from BONDSLABMASTER
if exists (select '*' from BONDSLABMASTER where SDTYPE = 0)
begin
	update BONDSLABMASTER
	set SDTYPE = SDTYPE + 1
end


select distinct PRDTGID from BONDSLABMASTER

select distinct PRDTGID from BONDSLABMASTER
update BONDSLABMASTER
set PRDTGID = 1
where PRDTGID = 0

select * From BONDPRODUCTGROUPMASTER

update BONDPRODUCTGROUPMASTER
set PRDTGDESC = 'Not Required', PRDTGCODE = 'NR', DISPSTATUS = 0
where PRDTGID = 1