use scfs_erp
go

select * From EXPORT_SEAL_TYPE_MASTER
if not exists (select '*' from EXPORT_SEAL_TYPE_MASTER)
begin
	insert into EXPORT_SEAL_TYPE_MASTER (ESLTID, ESLTDESC)
	select 0,'ON WHEEL'

	insert into EXPORT_SEAL_TYPE_MASTER (ESLTID, ESLTDESC)
	select 1,'ON WHEEL-GROUND'

	insert into EXPORT_SEAL_TYPE_MASTER (ESLTID, ESLTDESC)
	select 2, 'SELF SEAL-ON WHEEL'

	insert into EXPORT_SEAL_TYPE_MASTER (ESLTID, ESLTDESC)
	select 3, 'SELF SEAL-GROUND'

	insert into EXPORT_SEAL_TYPE_MASTER (ESLTID, ESLTDESC)
	select 4,'CENTRAL EXCISE-ON WHEEL'

	insert into EXPORT_SEAL_TYPE_MASTER (ESLTID, ESLTDESC)
	select 5, 'CENTRAL EXCISE-GROUND'
end
--select * From scfs_erp..EXPORT_SEAL_TYPE_MASTER
--select * From scfs_erp_devpt..EXPORT_SEAL_TYPE_MASTER