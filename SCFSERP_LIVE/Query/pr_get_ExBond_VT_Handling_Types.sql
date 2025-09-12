alter proc pr_get_ExBond_VT_Handling_Types
as
begin

	declare @tbl table
	(dval int,
	 dtxt varchar(100)
	 )

	 insert into @tbl
	 select 0, 'Not Required'
	 union
	 select 1, 'Loading'
	 union
	 select 2, 'Unloading'

	 select * from @tbl
end