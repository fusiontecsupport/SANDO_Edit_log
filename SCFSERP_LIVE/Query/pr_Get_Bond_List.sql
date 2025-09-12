/*
exec pr_Get_Bond_List @compyid  = 31 ,  @chaid  = 64139 ,  @imprtrid  = 3472, @opt =0
go
 SELECT * FROM BONDMASTER where bndid = 1003
 */
CREATE  PROC pr_Get_Bond_List
@compyid int,
@chaid int,
@imprtrid int,
@opt int
as
begin
	declare @tbl table
	(dval int,
	 dtxt varchar(100)
	 )

	 insert into @tbl	 
	 select A.BNDID ,  BNDDNO 
	 from BONDMASTER a (nolock) 
		join [VW_EXBOND_BOND_BALANCE_DETAIL_ASSGN] e (nolock) on a.BNDID = e.bndid
	where a.COMPYID = @compyid
	and a.CHAID = @chaid
	and a.IMPRTID = @imprtrid
	and ((e.BBNDNOP >0 and @opt =0) or (e.BBNDNOP >0 and @opt =1) )

	 SELECT * FROM @tbl
end
