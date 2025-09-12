-- exec pr_Get_ExBond_VT_Nos_Invoice @vtid=0,@ebndid= 1
/*
select b.VTDNO, b.VTDID, b.ebndid
		from BONDVEHICLETICKETDETAIL b(nolock)
				left JOIN BONDTRANSACTIONDETAIL d (NOLOCK) ON b.EBNDID = d.EBNDID and b.VTDID = d.VTDID
				left join BONDTRANSACTIONMASTER A(nolock) on d.TRANMID= a.TRANMID and a.TRANTID=3	
exec pr_Get_ExBond_VT_Nos_Invoice @vtid=0,@ebndid= 14000,@tranmid= 14723
exec pr_Get_ExBond_VT_Nos_Invoice @vtid=0,@ebndid= 3,@tranmid= 14725
exec pr_Get_ExBond_VT_Nos_Invoice @vtid=0,@ebndid= 14016,@tranmid= 0
*/
alter proc pr_Get_ExBond_VT_Nos_Invoice
@vtid int=0,
@ebndid int=0,
@tranmid int=0
as

begin
	declare @tbl table
	(	 dtxt varchar(100),
	dval int
	 )

	 declare @SelectFinalVT table
	 (
	 ebndid int,
	 ebndno varchar(100),
	 vtdid int,
	 ebndnop int,
	 vtnop int)

	 insert into @SelectFinalVT
	 select  e.EBNDID, e.EBNDDNO,  max(b.VTDID), e.EBNDNOP, sum(b.vtqty)
	from BONDVEHICLETICKETDETAIL b(nolock)
			JOIN EXBONDMASTER e (NOLOCK) ON  b.EBNDID = e.EBNDID
			left JOIN BONDTRANSACTIONDETAIL d (NOLOCK) ON d.TRANDEBNDNO = e.EBNDDNO and b.VTDID = d.VTDID
			left join BONDTRANSACTIONMASTER A(nolock) on d.TRANMID= a.TRANMID and a.TRANTID=3
	where   b.EBNDID = @ebndid 	and 
	d.VTDID is null
	group by  e.EBNDID, e.EBNDDNO,  e.EBNDNOP
	having sum(b.vtqty) =  e.EBNDNOP

	 if @vtid=0 and @tranmid  = 0
	 begin
		insert into @tbl
		select distinct b.VTDNO, b.VTDID
		from BONDVEHICLETICKETDETAIL b(nolock)
				left JOIN BONDTRANSACTIONDETAIL d (NOLOCK) ON b.EBNDID = d.EBNDID and b.VTDID = d.VTDID
				left join BONDTRANSACTIONMASTER A(nolock) on d.TRANMID= a.TRANMID and a.TRANTID=3					
				join @SelectFinalVT s on b.VTDID = s.vtdid
		where  b.EBNDID = @ebndid
		and d.VTDID is null
	 end
	 else if @tranmid  > 0
	 begin
		insert into @tbl
		select  distinct b.VTDNO, b.VTDID
		from BONDVEHICLETICKETDETAIL b(nolock)
				JOIN BONDTRANSACTIONDETAIL d (NOLOCK) ON b.EBNDID = d.EBNDID and b.VTDID = d.VTDID
				 join BONDTRANSACTIONMASTER A(nolock) on d.TRANMID= a.TRANMID and a.TRANTID=3					
		where   a.tranmid = @tranmid
		
	 end
	 ELSE
	 BEGIN
		insert into @tbl
		select distinct b.VTDNO, b.VTDID
		from BONDVEHICLETICKETDETAIL b(nolock)
				JOIN BONDTRANSACTIONDETAIL d (NOLOCK) ON b.EBNDID = d.EBNDID and b.VTDID = d.VTDID
				join BONDTRANSACTIONMASTER A(nolock) on d.TRANMID= a.TRANMID and a.TRANTID=3
				join @SelectFinalVT s on b.VTDID = s.vtdid
		where  b.EBNDID = @ebndid
		and b.VTDID = @vtid
	 END

	 select * from @tbl
	
end
