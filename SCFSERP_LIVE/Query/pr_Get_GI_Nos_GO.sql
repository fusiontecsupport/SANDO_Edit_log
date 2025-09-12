-- select *From BONDGATEINDETAIL
alter proc pr_Get_GI_Nos_GO
@GOID int=0
as

begin
	declare @tbl table
	(	 dtxt varchar(100),
	dval int
	 )

	 if @GOID=0
	 begin
		insert into @tbl
		select A.CONTNRNO, A.GIDID
		from BONDGATEINDETAIL A(nolock) LEFT JOIN BONDGATEOUTDETAIL B(NOLOCK) ON A.COMPYID = B.COMPYID
		AND A.GIDID = B.GIDID
		WHERE B.GIDID IS NULL		
	 end
	 ELSE
	 BEGIN
		insert into @tbl
		select A.CONTNRNO, A.GIDID
		from BONDGATEINDETAIL A(nolock) JOIN BONDGATEOUTDETAIL B(NOLOCK) ON A.COMPYID = B.COMPYID
		AND A.GIDID = B.GIDID		
		WHERE B.GODID= @GOID
	 END

	 select * from @tbl
	
end