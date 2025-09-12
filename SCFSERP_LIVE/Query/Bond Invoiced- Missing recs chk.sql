select distinct b.TRANDEBNDNO 'Invoiced_Missing_EXBondNos'
from BONDTRANSACTIONDETAIL b(nolock )left  join EXBONDMASTER a(nolock) on b.ebndid = a.EBNDID
join BONDTRANSACTIONMASTER c (nolock) on b.TRANMID = c.TRANMID
where TRANDATE >= '2022-10-31'
union
select distinct b.TRANDEBNDNO 'Invoiced_Missing_EXBondNos'
from BONDTRANSACTIONDETAIL b(nolock )left  join EXBONDMASTER a(nolock) on b.TRANDEBNDNO = a.EBNDNO
join BONDTRANSACTIONMASTER c (nolock) on b.TRANMID = c.TRANMID
where TRANDATE >= '2022-10-31'

select distinct b.TRANDBNDNO 'Invoiced_Missing_BondNos'
from BONDTRANSACTIONDETAIL b(nolock )left  join BONDMASTER a(nolock) on b.bndid = a.BNDID
join BONDTRANSACTIONMASTER c (nolock) on b.TRANMID = c.TRANMID
where TRANDATE >= '2022-10-31'
union
select distinct b.TRANDBNDNO 'Invoiced_Missing_BondNos'
from BONDTRANSACTIONDETAIL b(nolock )left  join BONDMASTER a(nolock) on  b.TRANDBNDNO = a.BNDNO
join BONDTRANSACTIONMASTER c (nolock) on b.TRANMID = c.TRANMID
where TRANDATE >= '2022-10-31'