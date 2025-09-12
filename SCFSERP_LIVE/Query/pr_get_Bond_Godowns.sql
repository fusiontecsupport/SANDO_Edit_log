alter proc pr_get_Bond_Godowns
@gwtid int=0
as
begin

	declare @tbl table
	(dval int,
	 dtxt varchar(100)
	 )

	 insert into @tbl
	 select GWNID, GWNDESC
	 from bondgodownmaster (nolock)
	 where GWNTID = @gwtid or @gwtid=0

	 select * from @tbl
end
