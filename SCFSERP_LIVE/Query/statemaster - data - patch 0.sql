select * from TRANSACTIONMASTER
select * From STATEMASTER_bkup260821
select * From STATEMASTER
--select * into STATEMASTER_bkup260821 from scfs_erp..STATEMASTER_bkup260821
select * into STATEMASTER_bkup260821 from STATEMASTER order by stateid
update a
set STATEID = c.STATEID
from TRANSACTIONMASTER a, STATEMASTER_bkup260821 b, STATEMASTER c
where a.STATEID = b.stateid
and b.STATECODE = c.STATECODE

update a
set STFBCHASTATEID = c.STATEID
from STUFFINGMASTER a, STATEMASTER_bkup260821 b, STATEMASTER c
where a.STFBCHASTATEID = b.stateid
and b.STATECODE = c.STATECODE

update a
set STATEID = c.STATEID
from TRANSACTIONMASTER a, STATEMASTER_bkup260821 b, STATEMASTER c
where a.STATEID = b.stateid
and b.STATECODE = c.STATECODE

update a
set STATEID = c.STATEID
from CATEGORY_ADDRESS_DETAIL a, STATEMASTER_bkup260821 b, STATEMASTER c
where a.STATEID = b.stateid
and b.STATECODE = c.STATECODE


update a
set STATEID = c.STATEID
from CATEGORYMASTER a, STATEMASTER_bkup260821 b, STATEMASTER c
where a.STATEID = b.stateid
and b.STATECODE = c.STATECODE


