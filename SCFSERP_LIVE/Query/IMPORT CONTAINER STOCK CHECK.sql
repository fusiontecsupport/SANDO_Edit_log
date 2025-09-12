--drop view vw_container_wise_recent_gatein_dtl_old_sys
--create view vw_container_wise_recent_gatein_dtl_old_sys
--as
SELECT CONTNRNO, MAX(GIDID) 'XGIDID' 
into tbl_container_wise_recent_gatein_dtl_old_sys
FROM scfs.dbo.GATEINDETAIL  (nolock)
where SDPTID = 1
group by CONTNRNO

--drop view vw_container_wise_recent_gatein_dtl_new_sys
--create view vw_container_wise_recent_gatein_dtl_new_sys
--as
SELECT CONTNRNO, MAX(GIDID) 'NXGIDID' 
into tbl_container_wise_recent_gatein_dtl_new_sys
FROM SCFS_erp.dbo.GATEINDETAIL (nolock)
where SDPTID = 1 
group by CONTNRNO

--DROP TABLE tbl_pending_Containers_OldSys
select a.CONTNRNO, f.INDate, b.OSDID,  c.ASLDID, d.VTDID, D.VTDNO, d.VTDATE, e.GODID , E.GODNO, e.GODATE, e.GOTIME
into tbl_pending_Containers_OldSys
from tbl_container_wise_recent_gatein_dtl_old_sys a(nolock)  
	left join SCFS.dbo.opensheetdetail b(nolock) on a.XGIDID = b.GIDID  AND B.DISPSTATUS = 0
	left join  SCFS.dbo.AUTHORIZATIONSLIPDETAIL c(nolock) on b.OSDID = c.OSDID  AND C.DISPSTATUS = 0
	left join  SCFS.dbo.VEHICLETICKETDETAIL d(nolock) on a.XGIDID = d.GIDID and d.SDPTID = 1  AND D.DISPSTATUS = 0
	left join  SCFS.dbo.GATEOUTDETAIL e(nolock) on a.XGIDID = e.GIDID and e.SDPTID = 1  AND E.DISPSTATUS = 0
	left join scfs_erp.dbo.Import_Closing_Container_Stock_AsOn_20210930 f(nolock) on a.CONTNRNO = f.ContainerNo 
group by a.CONTNRNO, f.INDate, b.OSDID, c.ASLDID, d.VTDID,  D.VTDNO, d.VTDATE, e.GODID , E.GODNO, e.GODATE, e.GOTIME

--drop table tbl_pending_Containers_NewSys
select a.CONTNRNO, f.INDate, b.OSDID,  c.ASLDID, d.VTDID,  D.VTDNO, d.VTDATE, e.GODID , E.GODNO, e.GODATE, e.GOTIME
into tbl_pending_Containers_NewSys
from tbl_container_wise_recent_gatein_dtl_new_sys a(nolock)  
	left join SCFS_erp.dbo.opensheetdetail b(nolock) on a.NXGIDID = b.GIDID AND B.DISPSTATUS = 0
	left join  SCFS_erp.dbo.AUTHORIZATIONSLIPDETAIL c(nolock) on b.OSDID = c.OSDID AND C.DISPSTATUS = 0
	left join  SCFS_erp.dbo.VEHICLETICKETDETAIL d(nolock) on a.NXGIDID = d.GIDID and d.SDPTID = 1 AND D.DISPSTATUS = 0
	left join  SCFS_erp.dbo.GATEOUTDETAIL e(nolock) on a.NXGIDID = e.GIDID and e.SDPTID = 1 AND E.DISPSTATUS = 0
	left join scfs_erp.dbo.Import_Closing_Container_Stock_AsOn_20210930 f(nolock) on a.CONTNRNO = f.ContainerNo 
group by a.CONTNRNO, f.INDate, b.OSDID, c.ASLDID, d.VTDID,  D.VTDNO, d.VTDATE, e.GODID , E.GODNO, e.GODATE, e.GOTIME

select * from tbl_pending_Containers_OldSys ORDER BY CONTNRNO, OSDID, indate
select * from tbl_pending_Containers_NewSys ORDER BY CONTNRNO, OSDID, indate


select a.CONTNRNO, f.INDate, b.OSDID, c.ASLDID, d.VTDID,  D.VTDNO, d.VTDATE, e.GODID , E.GODNO, e.GODATE, e.GOTIME
from tbl_container_wise_recent_gatein_dtl_new_sys a(nolock)  
	left join SCFS_erp.dbo.opensheetdetail b(nolock) on a.NXGIDID = b.GIDID AND B.DISPSTATUS = 0
	left join  SCFS_erp.dbo.AUTHORIZATIONSLIPDETAIL c(nolock) on b.OSDID = c.OSDID AND C.DISPSTATUS = 0
	left join  SCFS_erp.dbo.VEHICLETICKETDETAIL d(nolock) on a.NXGIDID = d.GIDID and d.SDPTID = 1 AND D.DISPSTATUS = 0
	left join  SCFS_erp.dbo.GATEOUTDETAIL e(nolock) on a.NXGIDID = e.GIDID and e.SDPTID = 1 AND E.DISPSTATUS = 0
	left join scfs_erp.dbo.Import_Closing_Container_Stock_AsOn_20210930 f(nolock) on a.CONTNRNO = f.ContainerNo 
where c.asldid in (13412, 15901)
group by a.CONTNRNO, f.INDate, b.OSDID, c.ASLDID, d.VTDID,  D.VTDNO, d.VTDATE, e.GODID , E.GODNO, e.GODATE, e.GOTIME
