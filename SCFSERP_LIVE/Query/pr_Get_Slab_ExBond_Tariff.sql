create proc pr_Get_Slab_ExBond_Tariff
@tariffmid int,
@vtdid int,
@tranmid int
as

begin

	select a.* 
	From BONDSLABMASTER a(nolock) join BONDTARIFFMASTER b(nolock) on a.TARIFFMID = b.TARIFFMID
		left join bo
	Where a.DISPSTATUS = 0
	and b.TARIFFMID = 0
end