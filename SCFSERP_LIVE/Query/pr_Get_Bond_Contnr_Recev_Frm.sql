alter proc pr_Get_Bond_Contnr_Recev_Frm
as
begin

	declare @tbl table
	(dval int,
	 dtxt varchar(100)
	 )

	 insert into @tbl
	 select 1, 'SANCO'
	 union
	 select 2, 'OTHERS'
	 

	 select * from @tbl
end