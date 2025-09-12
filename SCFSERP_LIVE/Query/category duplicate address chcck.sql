select a.cateid,CATENAME, b.CATEATYPEDESC,   min(cateaid) miaid, max(cateaid) mxaid, count(cateaid) ctaidt
--select b.* into categoryaddrbkup27102022 
from CATEGORYMASTER a join CATEGORY_ADDRESS_DETAIL b on a.CATEID = b.cateid
group by a.cateid,CATENAME, b.CATEATYPEDESC
having count(cateaid) >1


select a.cateid,CATENAME, catetid,b.CATEATYPEDESC, b.CATEAADDR1,  min(cateaid) miaid, max(cateaid) mxaid, count(cateaid) ctaidt
--into Dupl_Addr_DelChk27102022
from CATEGORYMASTER a join CATEGORY_ADDRESS_DETAIL b on a.CATEID = b.cateid
and cateaid not in (select CATEAID from TRANSACTIONMASTER(nolock))
and cateaid not in (select BCATEAID from bondTRANSACTIONMASTER(nolock))
group by a.cateid,CATENAME, catetid,b.CATEATYPEDESC, b.CATEAADDR1
having count(cateaid) >1

--delete b 
--from Dupl_Addr_DelChk27102022 a join CATEGORY_ADDRESS_DETAIL b on a.CATEID = b.cateid and b.CATEAID > a.miaid and b.CATEAID<=mxaid



select  * from CATEGORYMASTER a join CATEGORY_ADDRESS_DETAIL b on a.CATEID = b.cateid
where catename = 'JETWAY FORWARDERS PVT LTD' --'MICRO LABS LIMITED'