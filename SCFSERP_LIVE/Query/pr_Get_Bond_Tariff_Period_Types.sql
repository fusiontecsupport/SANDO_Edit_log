-- exec pr_Get_Bond_Tariff_Period_Types 0,0,0
-- select distinct periodtid  from BONDSLABMASTER 
-- update BONDSLABMASTER set PERIODTID = PERIODTID + 1
alter proc pr_Get_Bond_Tariff_Period_Types
@tariffmid int=0
--,@slabtid int,
--@prdtid int
as
begin
	declare @tbl table
	(dval int,
	dtxt varchar(100))

	if @tariffmid=0
	begin
		insert into @tbl(dval,dtxt)
		select 1, 'Not Required'
		union
		select 2, 'Weekly'
		union
		select 3, 'Daily'
	end

	else
	begin

		insert into @tbl(dval)
		select 1
		union
		select distinct PERIODTID from BONDSLABMASTER (nolock) 
		where (TARIFFMID = @tariffmid or TARIFFMID=0)
		--and (SLABTID = @slabtid or @slabtid=0)
		--and (PERIODTID = @prdtid or @prdtid=0)

		update @tbl
		set dtxt = case when dval = 1 then 'Not Required' 
						when dval = 2 then 'Weekly' else 'Daily' end
	end

	select * From @tbl
end
