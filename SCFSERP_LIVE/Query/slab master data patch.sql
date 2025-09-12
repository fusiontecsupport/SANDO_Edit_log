if exists(select '*' from SLABMASTER where WTYPE = 0)
begin
	update SLABMASTER
	set  WTYPE = WTYPE + 1
end


if exists(select '*' from SLABMASTER where YRDTYPE = 0)
begin
	update SLABMASTER
	set  YRDTYPE = YRDTYPE + 1
end


-- select * from SLABMASTER where htype = 0 and ( isnumeric(cusrid) = 1 or  cusrid = 1 or  cusrid = 0)
if exists(select '*' from SLABMASTER where htype = 0 and ( isnumeric(cusrid) = 1 or  cusrid = 1 or  cusrid = 0))
begin
	update SLABMASTER
	set  htype = htype + 1
	 where htype = 0 and ( isnumeric(cusrid) = 1 or  cusrid = 1 or  cusrid = 0)
end