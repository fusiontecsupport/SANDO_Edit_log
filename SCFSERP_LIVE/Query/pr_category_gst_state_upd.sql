alter proc pr_category_gst_state_upd
@cateid int
as
begin

	declare @tmptbl table
	(
	cateid int,
	recent_cateaid int
	)

	insert into @tmptbl 
	select cateid, max(cateaid) 'recent_cateaid'
	from CATEGORY_ADDRESS_DETAIL (nolock)
	where CATEID = @cateid
	group by CATEID

	update a
	set CATEBGSTNO = c.CATEAGSTNO, CATEBPANNO = c.CATEAPANNO, STATEID = c.STATEID
	from CATEGORYMASTER a , @tmptbl b, CATEGORY_ADDRESS_DETAIL c
	where a.CATEID = b.cateid
	and a.CATEID = c.CATEID
	and b.recent_cateaid = c.CATEAID

end