use SCFS_ERP
go
alter table BONDPERFORMATRANSACTIONMASTER
add TAXTYPE int null
go

update BONDPERFORMATRANSACTIONMASTER
set taxtype = 1
where taxtype is null



alter table BONDPERFORMATRANSACTIONDETAIL
add BILLDESC NVARCHAR(MAX) null
go

update BONDPERFORMATRANSACTIONDETAIL
set BILLDESC = ''
where BILLDESC is null

select * into tmp_usr_proforma_bond_add_dtl from tmp_usr_bond_add_dtl where  1=0
alter table  BONDPERFORMATRANSACTIONDETAIL
add EXBNDVTSPC numeric(18,2) null
go

select* into tmp_usr_proforma_exbond_vtinv_add_dtl from tmp_usr_exbond_vtinv_add_dtl where 1=0

update a
set EXBNDVTSPC = vtarea
from BONDPERFORMATRANSACTIONDETAIL a, BONDVEHICLETICKETDETAIL b
where a.EBNDID = b.EBNDID
and  isnull(EXBNDVTSPC,0) = 0