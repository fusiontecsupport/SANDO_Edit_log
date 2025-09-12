select * from SPBW..SLABMASTER where WTYPE=0

alter table bondslabmaster
add HANDTYPE smallint null


update  bondslabmaster
set HANDTYPE  = 0
where HANDTYPE is null