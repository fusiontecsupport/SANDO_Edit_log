alter VIEW [dbo].[VW_VT_EXBOND_BALANCE_DETAIL_ASSGN_01]
AS
SELECT     a.EBNDID, EBNDDNO, sum(isnull(TRANDNOP,0)) AS VTNOP, SUM(isnull(VTAREA,0)) AS VTSPC --SUM(isnull(VTQTY,0))+
FROM    EXBONDMASTER a(nolock) left join dbo.BONDVEHICLETICKETDETAIL b(nolock) on a.EBNDID = b.EBNDID
		left join bondtransactiondetail c(nolock)  on a.BNDID = c.BNDID and  slabtid  = 3 --c.VTDID = 0 and
--where EBNDDNO ='839'
GROUP BY a.EBNDID,EBNDDNO