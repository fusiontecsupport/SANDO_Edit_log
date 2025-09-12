select * From BONDTRANSACTIONMASTER 
where TRANDNO like 'BWH/2223/00%289'
order by tranmid

select ackno, count('*') 'cntackno'
into duplicatebondinvoices_191122
from BONDTRANSACTIONMASTER
where ackno is not null
group by ackno
having count('*')> 1

select * 
delete a
From BONDTRANSACTIONMASTER a join duplicatebondinvoices_191122 b on a.ACKNO = b.ACKNO
and TRANBILLREFNO like 'BWH/EXB/CUS/000%'

update c
set QRCODEPATH = a.QRCODEPATH
from BONDTRANSACTIONMASTER a join duplicatebondinvoices_191122 b on a.ACKNO = b.ACKNO
and a.TRANBILLREFNO like 'BWH/EXB/CUS/000%' join
BONDTRANSACTIONMASTER c on a.ACKNO = b.ACKNO
and c.TRANBILLREFNO like 'BWH/EXB/CU/00%'
where c.QRCODEPATH is null

update a
set COMPYID = YRID
from BONDTRANSACTIONMASTER a, ACCOUNTINGYEAR b
where a.TRANDATE between FDATE and TDATE
and COMPYID <> YRID

select * From ACCOUNTINGYEAR