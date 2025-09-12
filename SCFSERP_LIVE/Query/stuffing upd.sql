drop table #updstf
select STFMNO, min(STFMID) 'minstfmid', max(case when compyid = 32 then stfmid else 0 end) 'maxastfid',  max(stfmid) 'maxstfmid'
into #updstf
from STUFFINGmaster where STFMNO in(1390,1376,1378,1385,1373,1374)
--and COMPYID = 32
group by STFMNO
order by stfmno
select * from #updstf
update a
set STFMID = b.maxastfid
--select *
from STUFFINGDETAIL a, #updstf b
where a.STFMID = b.maxstfmid

select * from st

select * from STUFFINGDETAIL where STFMID in
(select stfmid from STUFFINGmaster where STFMNO in(1378,1385)  --1390,1376,1378,1385,,1374
)

select stfmid, COMPYID from STUFFINGmaster where STFMNO in(1385) 
select* from STUFFINGmaster where STFMID in (12140, 12693)
update STUFFINGDETAIL
set STFMID = 12008
where stfmid in ( 12155, 11883, 12326)

stfmid	COMPYID
12140	15
12693	15

1390,1376,1378,1385,1373,1374