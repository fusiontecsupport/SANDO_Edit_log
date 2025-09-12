select * from SLABTYPEMASTER where SLABTID  = 2
select * from SLABTYPEMASTER where SLABTID  in (14,15,16)
select * from VW_IMPORT_SLABTYPE_HSN_DETAIL_ASSGN where SLABTID = 2 and tariffmid = 344
select * from SLABMASTER where SLABTID = 2

select * from SLABMASTER where SLABTID  in (14,15,16)

exec PR_NEW_NONPNR_RATECARDMASTER_FLX_ASSGN 344,2, 1, 1,4,15


SELECT SLABTID,SLABAMT,* FROM VW_NONPNR_RATECARDMASTER_FLX_ASSGN
WHERE TARIFFMID = 344 AND SLABTID = 2 AND HTYPE = 1 AND 
CHRGETYPE = 1 AND CONTNRSID = 4 AND SLABMIN <= 15


select * from GATEINDETAIL where GIDID = 1026
update GATEINDETAIL 
set GIDATE='2021-07-15'
where GIDID = 1026

sp_who
select * from TRANSACTIONDETAIL

select * from TRANSACTIONmaster
select * from TRANSACTIONMASTERFACTOR
select * from  VW_NONPNR_RATECARDMASTER_FLX_ASSGN (nolock)
select * from categorymaster where catetid = 6
select * from SLABTYPEMASTER where slabtid in (2,3,4,14,15,16) 
select * from slabmaster where slabtid in (14,15,16) SLABMDATE >= '2021-07-01'
select * from slabmaster where slabtid = 4
select distinct slabtid, tariffmid,stmrid,chrgetype,contnrsid, sdtype, yrdtype,htype,wtype from slabmaster where SLABMDATE >= '2021-07-01'
select * from tariffmaster