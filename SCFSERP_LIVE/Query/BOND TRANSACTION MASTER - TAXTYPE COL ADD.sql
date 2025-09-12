use SCFS_ERP
go
alter table BONDTRANSACTIONMASTER
add TAXTYPE int null
go

update BONDTRANSACTIONMASTER
set taxtype = 1
where taxtype is null



