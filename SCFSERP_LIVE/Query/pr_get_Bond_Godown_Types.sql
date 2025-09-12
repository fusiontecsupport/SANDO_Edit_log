alter proc pr_get_Bond_Godown_Types
as
begin

	declare @tbl table
	(dval int,
	 dtxt varchar(100)
	 )

	 insert into @tbl
	 select GWNTID, GWNTDESC
	 from BONDGODOWNTYPEMASTER (nolock)

	 select * from @tbl
end