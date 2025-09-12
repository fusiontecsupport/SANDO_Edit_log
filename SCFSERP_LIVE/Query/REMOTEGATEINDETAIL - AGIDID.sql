use scfs_erp
go

alter table REMOTEGATEINDETAIL
add  AGIDID int default (0)

update REMOTEGATEINDETAIL
set agidid = 0 
where agidid is null