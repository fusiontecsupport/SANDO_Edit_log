alter view vw_Get_Bond_Dtl_GI_Completed
as 
select bnd.COMPYID,bnd.BNDID, bnd.BNDDNO, BND20, BND40, 
isnull(count(bgi.bndid),0) as 'BGICnt'
from BONDMASTER BND(nolock) 
	left join BONDGATEINDETAIL BGI(nolock) on bnd.BNDID = bgi.BNDID 
		and bnd.COMPYID = bgi.COMPYID
group by bnd.COMPYID, bnd.BNDID, bnd.BNDDNO, BND20, BND40
having count(bgi.bndid) > 0 and count(bgi.bndid) = (isnull(BND20,0) + isnull(BND40,0))
