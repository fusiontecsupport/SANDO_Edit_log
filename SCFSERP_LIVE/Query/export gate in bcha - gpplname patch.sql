select * from GATEINDETAIL where sdptid = 2
and GPPLCNAME <> BCHANAME

update GATEINDETAIL
set GPPLCNAME = BCHANAME
where sdptid = 2 
and GPPLCNAME <> BCHANAME