select * from SOFT_TABLE_DELETE_DETAIL
where OPTNSTR = 'categorymaster'

update SOFT_TABLE_DELETE_DETAIL
set PFLDNAME = REPLACE(DCONDTNSTR,'=','')  --CATEID
where OPTNSTR = 'categorymaster'
and PFLDNAME = 'CATEID'

select * from SOFT_TABLE_DELETE_DETAIL
where OPTNSTR = 'categorymaster'