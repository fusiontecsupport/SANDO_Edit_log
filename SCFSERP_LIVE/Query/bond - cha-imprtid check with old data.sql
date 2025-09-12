select * from EXBONDMASTER where BNDID is null
select * from BONDMASTER where BNDID is null


se
select * from BONDTRANSACTIONDETAIL a join BONDMASTER b on a.TRANDBNDNO = b.BNDDNO
EBNDID
14042


select a.CHAID, b.chaid
from SCFS_ERP..BONDMASTER a, SCFS_ERP_bkup191122..BONDMASTER b(nolock)
where a.BNDDNO = b.bnddno
and a.CHAID <> b.chaid
and isnumeric(a.CUSRID)<>1

select a.IMPRTID, b.IMPRTID
from SCFS_ERP..BONDMASTER a, SCFS_ERP_bkup191122..BONDMASTER b(nolock)
where a.BNDDNO = b.bnddno
and a.IMPRTID <> b.IMPRTID
and isnumeric(a.CUSRID)<>1


select a.BILLREFID, b.BILLREFID
from SCFS_ERP..BONDTRANSACTIONMASTER a, SCFS_ERP_bkup191122..BONDTRANSACTIONMASTER b(nolock)
where a.TRANMID = b.TRANMID
and a.BILLREFID <> b.BILLREFID
and isnumeric(a.CUSRID)<>1

select a.IMPRTID, b.IMPRTID
from SCFS_ERP..BONDMASTER a, SCFS_ERP_bkup191122 b(nolock)
where a.BNDDNO = b.bnddno
and isnumeric(a.CUSRID)<>1
