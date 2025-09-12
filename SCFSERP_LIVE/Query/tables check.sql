select * from SOFT_TABLE_DELETE_DETAIL
select * from SOFT_TABLE_DELETE_DETAIL where OPTNSTR = 'AUTHORIZATIONSLIPDETAIL' and 

select * from gateindetail
update gateindetail
set COMPYID = 32, cusrid = 'test'
where gidid = 1013
--select * From AUTHORIZATIONSLIPMASTER
select * From AUTHORIZATIONSLIPDETAIL

select * from vehicleticketdetail
select *  from gateindetail where gidid = '1024'
select * From AUTHORIZATIONSLIPDETAIL WHERE GIDID = '1024'
select * From AUTHORIZATIONSLIPMASTER WHERE ASLMID = '1012'
select * From GATEINDETAIL LEFT JOIN  gateoutdetail  ON GATEINDETAIL.GIDID = GATEOUTDETAIL.GIDID WHERE GATEOUTDETAIL.GIDID IS NULL AND GATEINDETAIL.CONTNRNO = 'TCON0001200'
--select * into gateindetail_test190721 from gateindetail

select * into gateindetail_test190721 from gateindetail

select * from gateindetail_test190721 where gidid = '1024'
select * from gateindetail
insert into gateindetail
select * from gateindetail_test190721 where gidid = '1024'
sp_helptext PR_GATEIN_CONTAINER_CHK_ASSGN