-- select * from SLABNARRATIONMASTER
-- select * from transactionmaster where tranmid= 2639
-- select * from transactionmaster where tranmid= 2576
-- exec pr_Get_Import_Slab_Narration_Detail 2639
alter  proc dbo.pr_Get_Import_Slab_Narration_Detail
@PTranMID int
as 
begin
	select c.SLABTID,   SLABNARTN , round(sum(b.TRANDHAMT)/count(trandid),2) 'TRANHANDLINGAMT',
			 round(sum(b.TRANDAAMT)/count(trandid),2) 'TRANADDNLAMT'
	from TRANSACTIONMASTER a, TRANSACTIONDETAIL b(nolock), SLABNARRATIONMASTER c(nolock)
	where SDPTID  = 1  
	and a.tranmid = b.TRANMID
	and a.tranmid = @PTranMID
	and a.TRANBTYPE = c.BILLTID
	and b.TARIFFMID = c.TARIFFMID
	and c.SLABTID in (4,16)
	group by c.SLABTID,  SLABNARTN
	ORDER BY C.SLABTID
end

/*
update SLABNARRATIONMASTER
set tariffmid = 64053
where tariffmid = 352
*/