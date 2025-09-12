-- pr_Get_ExBond_Nos_VTInvoice 14721
alter proc pr_Get_ExBond_Nos_VTInvoice
@tranmid int=0
as

begin
	declare @tbl table
	(	
		dtxt varchar(100),
		dval int
	 )

	 if @tranmid=0
	 begin
		insert into @tbl
		select distinct e.EBNDDNO, b.EBNDID
		from BONDVEHICLETICKETDETAIL b(nolock) 
				join EXBONDMASTER e(nolock) on b.EBNDID= e.EBNDID
				left JOIN  BONDTRANSACTIONDETAIL d(nolock)  ON d.EBNDID = B.EBNDID and d.VTDID = b.VTDID
				left join BONDTRANSACTIONMASTER A(nolock)  on d.TRANMID= a.TRANMID and a.TRANTID=3
		where d.VTDID is null
	 end
	 ELSE
	 BEGIN
		insert into @tbl
		select distinct e.EBNDDNO, d.EBNDID
		from BONDTRANSACTIONMASTER A(nolock) 
			left join BONDTRANSACTIONDETAIL d(nolock) on a.TRANMID= d.TRANMID and a.TRANTID=3
			join EXBONDMASTER e(nolock) on d.EBNDID= e.EBNDID
			left JOIN BONDVEHICLETICKETDETAIL B (NOLOCK) ON d.EBNDID = B.EBNDID and d.VTDID = b.VTDID
		where a.tranmid = @tranmid
	 END
	 
	 select * from @tbl
	
end