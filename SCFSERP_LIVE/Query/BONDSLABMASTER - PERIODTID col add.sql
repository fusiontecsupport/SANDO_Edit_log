alter table bondslabmaster
add PERIODTID INT NULL
go

update bondslabmaster
set PERIODTID =1 
where PERIODTID  is null


alter table BONDTRANSACTIONDETAIL
add PERIODTID INT NULL
go

update BONDTRANSACTIONDETAIL
set PERIODTID =1 
where PERIODTID  is null