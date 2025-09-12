select * from EXPORT_SLAB_CHARGE_TYPE_MASTER
if not exists(select * from EXPORT_SLAB_CHARGE_TYPE_MASTER where CHRGETYPE = 6)
begin
	insert into EXPORT_SLAB_CHARGE_TYPE_MASTER
	select 6, 'E-SEAL'
end
select * from EXPORT_SLAB_CHARGE_TYPE_MASTER