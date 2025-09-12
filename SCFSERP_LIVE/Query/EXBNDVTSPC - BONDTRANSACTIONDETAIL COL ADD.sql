select * From BONDTRANSACTIONMASTER order by TRANMID desc

select * From BONDTRANSACTIONMASTER where TRANMID =14798
select * From BONDTRANSACTIONDETAIL where TRANMID =14798

select * From tmp_usr_exbond_vtinv_add_dtl

alter table  BONDTRANSACTIONDETAIL
add EXBNDVTSPC numeric(18,2) null
go


alter table  tmp_usr_exbond_vtinv_add_dtl
add EXBNDVTSPC numeric(18,2) null
go

update a
set EXBNDVTSPC = vtarea
from BONDTRANSACTIONDETAIL a, BONDVEHICLETICKETDETAIL b
where a.EBNDID = b.EBNDID
and  isnull(EXBNDVTSPC,0) = 0