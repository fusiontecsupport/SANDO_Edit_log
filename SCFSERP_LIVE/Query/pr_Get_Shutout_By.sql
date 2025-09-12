create proc pr_Get_Shutout_By
as
begin
	declare @tbl table 
	(
		dval int,
		dtxt varchar(100)
	)

	insert into @tbl
	select 1, 'By Hand'
	union
	select 2, 'By Vehicle'

	select * from @tbl
end