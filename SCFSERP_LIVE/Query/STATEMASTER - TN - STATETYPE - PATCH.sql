select * From STATEMASTER

update STATEMASTER
set statetype = 0
where statecode = '33'
and statetype <> 0