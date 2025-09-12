alter table BONDTRANSACTIONDETAIL
add BILLDESC NVARCHAR(MAX) null
go

update BONDTRANSACTIONDETAIL
set BILLDESC = ''
where BILLDESC is null

ALTER TABLE tmp_usr_bond_add_dtl
add BILLDESC NVARCHAR(MAX) null
go
