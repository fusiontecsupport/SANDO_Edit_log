alter proc pr_get_BondMaster_Status
as
begin

	declare @tbl table
	(dval int,
	 dtxt varchar(100)
	 )

	 insert into @tbl
	 select 0, 'INBOOKS'
	 union
	 select 1, 'CANCELLED'

	 select * from @tbl
end