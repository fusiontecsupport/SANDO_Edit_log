select b.* --into stuffingcataidupd161021
 from STUFFINGDETAIL a(nolock) join STUFFINGMASTER b(nolock) on a.STFMID = b.STFMID

  where OLDSTFDID > 0
  and STFBCATEAID =0
  and STFCATEAID=0
  order by a.PRCSDATE desc

  update b 
  set STFBCATEAID = stfcateaid,
STFBCHAGSTNO = CATEAGSTNO,
STFBCHAADDR1 = TRANIMPADDR1, 
STFBCHAADDR2 = TRANIMPADDR2,
STFBCHAADDR3 = TRANIMPADDR3,
STFBCHAADDR4 = TRANIMPADDR4,
STFBCHASTATEID = STATEID
  from STUFFINGDETAIL a(nolock) join STUFFINGMASTER b  on a.STFMID = b.STFMID
  where OLDSTFDID > 0
  and STFBCATEAID =0

  update b 
  set STFBCATEAID = stfcateaid,
STFBCHAGSTNO = CATEAGSTNO,
STFBCHAADDR1 = TRANIMPADDR1, 
STFBCHAADDR2 = TRANIMPADDR2,
STFBCHAADDR3 = TRANIMPADDR3,
STFBCHAADDR4 = TRANIMPADDR4,
STFBCHASTATEID = STATEID

select b.* into #catechk
  from STUFFINGDETAIL a(nolock) join STUFFINGMASTER b  on a.STFMID = b.STFMID
  where OLDSTFDID > 0
  and STFBCATEAID =0

  select cateid, max(cateaid) 'recentcateaid' into #maxaddr
  from CATEGORY_ADDRESS_DETAIL join #catechk on CATEID = CHAID
  group by cateid

  select * from #catechk

  update b
  set STFBILLREFID = b.CHAID
  from STUFFINGDETAIL a(nolock) join STUFFINGMASTER b  on a.STFMID = b.STFMID, #catechk c 
  where OLDSTFDID > 0
    and b.STFBILLREFID =0
	and b.stfmid = c.STFMID

	
  update b
  set STFBCATEAID = d.recentcateaid, STFBCHAGSTNO = e.CATEAGSTNO, STFBCHAADDR1 = CATEAADDR1, STFBCHAADDR2 = CATEAADDR2,
		STFBCHAADDR3 = CATEAADDR3, STFBCHAADDR4 = CATEAADDR4, STFBCHASTATEID = e.STATEID
 from STUFFINGDETAIL a(nolock) join STUFFINGMASTER b  on a.STFMID = b.STFMID, #catechk c, #maxaddr d ,
  CATEGORY_ADDRESS_DETAIL e
  where OLDSTFDID > 0
    and b.STFBCATEAID =0
	and b.stfmid = c.STFMID
	and c.CHAID = d.CATEID
	and d.recentcateaid = e.CATEAID