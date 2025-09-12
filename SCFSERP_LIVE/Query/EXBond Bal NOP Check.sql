
select  b.bndid, b.BNDDNO, b.BNDNOP 'BNDNOP',   isnull(sum(t1.trandnop),0) 'CurVTQty' ,  ---isnull(sum(t2.trandnop),0)
isnull(sum(t2.trandnop),0) 'UptoLastInvQTY',
b.BNDNOP - (isnull(sum(t2.trandnop),0) + isnull(sum(t1.trandnop),0) ) 'BalVTQty',
 t1.TRANMID, b.BNDNOP -  isnull((sum(t2.TRANDNOP)),0) 'ActualBalQTY1'
/*
select t2.tranmid,t1.* ,b.bndid, b.BNDDNO, b.BNDNOP 'BNDNOP',   isnull((t1.trandnop),0) 'CurVTQty' ,  ---isnull(sum(t2.trandnop),0)
(t2.trandnop) 'UptoLastInvQTY',
b.BNDNOP - ((t2.TRANDNOP))  'BalVTQty',
 t1.TRANMID, BalBNDQty 'ActualBalQTY1'*/
from BONDMASTER e(nolock) join VW_Bond_Bal_NOP b(nolock) on e.BNDID = b.BNDID
join BONDTRANSACTIONDETAIL t1(nolock) on e.BNDID = t1.BNDID and t1.slabtid = 2 --and cur.VTDID = t1.VTDID
left join BONDTRANSACTIONDETAIL t2(nolock) on t1.BNDID = t2.BNDID and  e.BNDID = t2.BNDID and t1.TRANMID > t2.TRANMID and t2.slabtid = 2
where e.DISPSTATUS = 0
and b.BNDDNO = '2002038260'
group by   b.bndid,  b.BNDDNO,b.BNDNOP , t1.TRANMID