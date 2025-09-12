alter proc pr_get_Bond_Types
as
begin

	declare @tbl table
	(dval int,
	 dtxt varchar(100)
	 )

	 insert into @tbl
	 select 1, 'FCL'
	 union
	 select 2, 'LCL'

	 select * from @tbl
end