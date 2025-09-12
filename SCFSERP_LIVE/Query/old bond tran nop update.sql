
update a
set EXBNDVTSPC =  TRANDNOP, TRANDNOP = c.EBNDNOP
from BONDTRANSACTIONDETAIL a, bondtransactionmaster b , EXBONDMASTER c
where a.tranmid = b.tranmid -- and b.TRANDATE < '2022-09-30'
and  isnumeric(b.cusrid)=1
and a.TRANDEBNDNO = c.EBNDDNO
and SLABTID = 2
and TRANTID = 3
and TRANDNOP =0 



update a
set vtdid = b.vtdid
from BONDTRANSACTIONDETAIL a, BONDTRANSACTIONDETAIL b
where a.TRANDBNDNO = b.TRANDBNDNO
and a.VTDID = 0 and
b.VTDID>0


update a
set vtdid = b.vtdid
from BONDTRANSACTIONDETAIL a, BONDTRANSACTIONDETAIL b
where a.TRANDEBNDNO = b.TRANDEBNDNO
and a.VTDID = 0 and
b.VTDID>0
