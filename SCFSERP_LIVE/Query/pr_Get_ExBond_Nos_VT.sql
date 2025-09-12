alter proc pr_Get_ExBond_Nos_VT
@vtid int=0,
@term varchar(100)=''
as

begin
	declare @tbl table
	(	 dtxt varchar(100),
	dval int
	 )

	 if @vtid=0
	 begin
		insert into @tbl
		select A.EBNDDNO, A.EBNDID
		from EXBONDMASTER A(nolock) JOIN [VW_VT_EXBOND_BALANCE_DETAIL_ASSGN] B (NOLOCK) ON A.EBNDID = B.EBNDID
		where A.EBNDDNO like @term + '%'
		and a.DISPSTATUS = 0
	 end
	 ELSE
	 BEGIN
		insert into @tbl
		select A.EBNDDNO, A.EBNDID
		from EXBONDMASTER A(nolock) JOIN BONDVEHICLETICKETDETAIL B(NOLOCK) ON A.EBNDID= B.EBNDID
		WHERE B.VTDID= @vtid
		and a.DISPSTATUS = 0
	 END

	 select * from @tbl
	
end