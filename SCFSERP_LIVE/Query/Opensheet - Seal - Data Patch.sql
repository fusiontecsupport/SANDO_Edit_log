use SCFS_ERP
go
drop table #updosdidm
drop table #updosdidd
drop table #updosdidsd
select distinct OPENSHEETMASTER.* into #updosdidm 
from OPENSHEETMASTER join OPENSHEET_SEAL_DETAIL on OPENSHEETMASTER.osmid = OPENSHEET_SEAL_DETAIL.OSMID
where OSMDATE >= '2021-09-01'

select distinct OPENSHEETDETAIL.* into #updosdidd 
from #updosdidm join OPENSHEETDETAIL on #updosdidm.OSMID = OPENSHEETDETAIL.OSMID 


select distinct OPENSHEET_SEAL_DETAIL.* into #updosdidsd 
from #updosdidm  join OPENSHEET_SEAL_DETAIL on #updosdidm.osmid = OPENSHEET_SEAL_DETAIL.OSMID

update #updosdidsd
set OSDID = 0

--select * from #updosdidm

declare @osmid int , @osdid int, @gidid int, @csealnos varchar(50)

while exists(select '*' from #updosdidm)
begin

	select top 1 @osmid = osmid from #updosdidm order by OSMID

	drop table #updosdidd1
	select distinct OPENSHEETDETAIL.* into #updosdidd1 
	from OPENSHEETDETAIL (nolock)
	where OSMID = @osmid

	--select * from #updosdidd1
	while exists(select '*' from #updosdidd1)
	begin

		select top 1 @osdid = osdid, @gidid = gidid, @csealnos = CSEALNO 
		from #updosdidd1 
		where OSMID = @osmid 
		order by OSMID, OSDID

	
		update #updosdidsd
		set OSDID = @osdid
		where OSMID = @osmid
		and replace(@csealnos ,'-','') like '%'+ replace(OSSDESC,'-','') + '%'	

		--select count('*') from #updosdidd1 

		delete #updosdidd1 where OSDID = @osdid and  @csealnos = CSEALNO and OSMID = @osmid

	end
		
	delete #updosdidm where osmid = @osmid
end


/*
update OPENSHEET_SEAL_DETAIL
set osdid = b.osdid
-- select a.osdid , b.osdid
from OPENSHEET_SEAL_DETAIL a, #updosdidsd b
where a.osmid = b.osmid
and a.ossdesc = b.ossdesc

drop table #updosdidm
drop table #updosdidd
drop table #updosdidsd
drop table #updosdidd1

*/