alter view VW_Bond_Bal_NOP
as
select   b.BNDID , b.BNDDNO, b.BNDNOP 'BNDNOP', b.BNDNOP- (sum(e.EBNDNOP)) 'BalBNDQty'
from BONDMASTER b(nolock) join EXBONDMASTER e(nolock)  on e.BNDID = b.BNDID
where b.DISPSTATUS = 0
and e.DISPSTATUS = 0
group by   b.BNDID , b.BNDDNO, b.BNDNOP 
--order by   b.BNDID , b.BNDNOP 
