select * From BONDTRANSACTIONDETAIL order by tranmid desc
select * from EXBONDMASTER where EBNDID = 14004

update a
set TRANDEBNDNO = ebnddno
from BONDTRANSACTIONDETAIL a join EXBONDMASTER e on a.EBNDID = e.ebndid