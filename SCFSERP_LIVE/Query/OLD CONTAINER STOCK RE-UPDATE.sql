sp_helptext VW_IMPORT_EINVOICE_TRANSACTION_GST_PRINT_ASSGN
select 8 from oldc

select * from TRANSACTIONDETAIL where tranmid = 1070

select * from GATEINDETAIL where GIDID = 34558

Update GATEINDETAIL Set SDPTID = 1 where GIDID = 34558

select * FROM GATEINDETAIL where GIDID = 34558
select * FROM oldstockdetail_bkup111021 where GIDID = 34558

update B
set CONTNRSID = a.CONTNRSID, LMUSRID = a.LMUSRID, SDPTID = a.sdptid
--SELECT *
from oldstockdetail_bkup111021 a  join GATEINDETAIL b on a.GIDID = b.GIDID 
where B.GIDID = 34558