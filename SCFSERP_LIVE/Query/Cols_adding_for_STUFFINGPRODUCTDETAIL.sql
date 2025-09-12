alter table STUFFINGPRODUCTDETAIL 
add  STF_PACKETS_FROM	int	null default 0 

alter table STUFFINGPRODUCTDETAIL 
add  STF_PACKETS_TO	int	null default 0 


alter table CompanyMaster 
add  COMP_CUSTODIAN_CODE varchar(50) null default ''

alter table CompanyMaster 
add  COMP_RPT_LOCT_NAME	varchar(50) null default ''

alter table CompanyMaster 
add  COMP_RPT_LOCT_CODE	varchar(50) null default ''

alter table CompanyMaster 
add  COMP_AUTH_PAN_NO	varchar(50) null default ''


select *from CompanyMaster

Update CompanyMaster set COMP_CUSTODIAN_CODE='AAACS7690F',COMP_RPT_LOCT_NAME='SANCO CFS',COMP_RPT_LOCT_CODE='INMAA1STL1',COMP_AUTH_PAN_NO='ASZPG8123B' 
where COMPID=1