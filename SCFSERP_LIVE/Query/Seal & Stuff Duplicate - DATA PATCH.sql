--seal data patch
SELECT     dbo.STUFFINGDETAIL.STFMID, dbo.STUFFINGMASTER.STFMDNO
into #sealdtl
FROM         dbo.STUFFINGDETAIL INNER JOIN
                      dbo.STUFFINGMASTER ON dbo.STUFFINGDETAIL.STFMID = dbo.STUFFINGMASTER.STFMID INNER JOIN
                      dbo.STUFFINGPRODUCTDETAIL ON dbo.STUFFINGDETAIL.STFDID = dbo.STUFFINGPRODUCTDETAIL.STFDID LEFT OUTER JOIN
                      dbo.VW_EXPORT_INVOICE_SBILL_NO_CBX_ASSGN_01 ON dbo.STUFFINGDETAIL.STFDID = dbo.VW_EXPORT_INVOICE_SBILL_NO_CBX_ASSGN_01.STFDID
WHERE     (ISNULL(dbo.VW_EXPORT_INVOICE_SBILL_NO_CBX_ASSGN_01.TRANDID, 0) = 0) AND 
	(dbo.STUFFINGMASTER.STFTID = 1) and (dbo.STUFFINGDETAIL.DISPSTATUS=0)
GROUP BY dbo.STUFFINGDETAIL.STFMID, dbo.STUFFINGMASTER.STFMDNO
Order  by dbo.STUFFINGMASTER.STFMDNO


select a.STFMDNO, min(b.stfmid) 'minstfmid', count(b.stfmid) 'cnt', max(case when compyid = 32 then b.stfmid else 0 end) 'cymaxstfmid',
 max(case when compyid < 32 then b.stfmid else 0 end) 'pymaxstfmid'
into #sealminstfmid
from #sealdtl a  join dbo.STUFFINGMASTER b ON a.STFMDNO = b.STFMDNO 
group by a.STFMDNO
having count(a.stfmid) >1

select * from #sealminstfmid

update a
set stfmid = case when c.cymaxstfmid >0 then c.cymaxstfmid 
				 when c.pymaxstfmid >0 then c.pymaxstfmid 
				  when c.cymaxstfmid > c.pymaxstfmid then c.cymaxstfmid 
				 else c.pymaxstfmid end
--select a.*,c.*, d.stfmid, case when c.cymaxstfmid >0 then c.cymaxstfmid when c.pymaxstfmid >0 then c.pymaxstfmid when c.cymaxstfmid > c.pymaxstfmid then c.cymaxstfmid  else c.pymaxstfmid end
from STUFFINGDETAIL a, STUFFINGMASTER b(nolock), #sealminstfmid c(nolock),
	scfs.dbo.STUFFINGDETAIL d(nolock)
where a.STFMID = b.STFMID
and b.STFMDNO = c.STFMDNO
and a.OLDSTFDID = d.stfdid
--order by c.STFMDNO, a.STFMID

update a
set STFMID = e.STFMID
-- select a.STFDID , e.STFDID, a.STFMID , e.STFMID,*
from scfs_erp.dbo.STUFFINGDETAIL a, scfs_erp.dbo.STUFFINGMASTER b(nolock), #sealminstfmid c(nolock),
	scfs.dbo.STUFFINGDETAIL d(nolock), SCFS_ERP_2010210800.dbo.STUFFINGDETAIL e(nolock)
where a.STFMID = b.STFMID
and b.STFMDNO = c.STFMDNO
and a.OLDSTFDID = d.stfdid
and a.STFDID = e.STFDID
and a.STFMID <> e.STFMID


select a.*, d.stfmid 
--delete a
from STUFFINGDETAIL a, STUFFINGMASTER b(nolock),	scfs.dbo.STUFFINGDETAIL d(nolock)
where a.STFMID = b.STFMID
and b.STFMDNO = 686
--and a.STFDID in (12254, 12079)
and a.OLDSTFDID = d.stfdid
order by a.STFMID


--stuffing data patch

SELECT     dbo.STUFFINGDETAIL.STFMID, dbo.STUFFINGMASTER.STFMDNO
into #stuffingdtl
FROM         dbo.STUFFINGDETAIL INNER JOIN
                      dbo.STUFFINGMASTER ON dbo.STUFFINGDETAIL.STFMID = dbo.STUFFINGMASTER.STFMID INNER JOIN
                      dbo.STUFFINGPRODUCTDETAIL ON dbo.STUFFINGDETAIL.STFDID = dbo.STUFFINGPRODUCTDETAIL.STFDID LEFT OUTER JOIN
                      dbo.VW_EXPORT_INVOICE_SBILL_NO_CBX_ASSGN_01 ON dbo.STUFFINGDETAIL.STFDID = dbo.VW_EXPORT_INVOICE_SBILL_NO_CBX_ASSGN_01.STFDID
WHERE     (ISNULL(dbo.VW_EXPORT_INVOICE_SBILL_NO_CBX_ASSGN_01.TRANDID, 0) = 0) 
	 AND 
	(dbo.STUFFINGMASTER.STFTID <> 1)
GROUP BY dbo.STUFFINGDETAIL.STFMID, dbo.STUFFINGMASTER.STFMDNO
Order  by dbo.STUFFINGMASTER.STFMDNO



select a.STFMDNO, min(b.stfmid) 'minstfmid', count(b.stfmid) 'cnt', max(case when compyid = 32 then b.stfmid else 0 end) 'cymaxstfmid',
 max(case when compyid < 32 then b.stfmid else 0 end) 'pymaxstfmid'
into #stuffingminstfmid
from #stuffingdtl  a  join dbo.STUFFINGMASTER b ON a.STFMDNO = b.STFMDNO
group by a.STFMDNO
having count(b.stfmid) >1

select * from #stuffingminstfmid

update a
set stfmid =  case when c.cymaxstfmid >0 then c.cymaxstfmid 
				 when c.pymaxstfmid >0 then c.pymaxstfmid 
				  when c.cymaxstfmid > c.pymaxstfmid then c.cymaxstfmid 
				 else c.pymaxstfmid end
--select a.*,c.*, d.stfmid , case when c.cymaxstfmid >0 then c.cymaxstfmid when c.pymaxstfmid >0 then c.pymaxstfmid when c.cymaxstfmid > c.pymaxstfmid then c.cymaxstfmid  else c.pymaxstfmid end
from STUFFINGDETAIL a, STUFFINGMASTER b(nolock), #stuffingminstfmid c(nolock),
	scfs.dbo.STUFFINGDETAIL d(nolock)
where a.STFMID = b.STFMID
and b.STFMDNO = c.STFMDNO
and a.OLDSTFDID = d.stfdid
order by c.STFMDNO, a.STFMID


update a
set STFMID = e.STFMID
-- select a.STFDID , e.STFDID, a.STFMID , e.STFMID,*
from scfs_erp.dbo.STUFFINGDETAIL a, scfs_erp.dbo.STUFFINGMASTER b(nolock), #stuffingminstfmid c(nolock),
	scfs.dbo.STUFFINGDETAIL d(nolock), SCFS_ERP_2010210800.dbo.STUFFINGDETAIL e(nolock)
where a.STFMID = b.STFMID
and b.STFMDNO = c.STFMDNO
and a.OLDSTFDID = d.stfdid
and a.STFDID = e.STFDID
and a.STFMID <> e.STFMID



select * from STUFFINGMASTER where STFMDNO in (1375, 1378, 1385)
select * from STUFFINGDETAIL where STFMID in (
select  STFMID from STUFFINGMASTER where STFMDNO in (1375, 1378, 1385))