alter proc pr_get_Bond_Operation_Types
as
begin

	declare @tbl table
	(dval int,
	 dtxt varchar(100)
	 )

	 insert into @tbl
	 select 1, 'Manual'
	 union
	 select 2, 'Mechanical'
	 union
	 select 3, 'Both'

	 select * from @tbl
end

