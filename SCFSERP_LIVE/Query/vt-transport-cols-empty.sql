select * from vehicleticketdetail
where TRNSPRTID is null

update vehicleticketdetail
set TRNSPRTID = 0,
	trnsprtname = '', 
	gtrnsprtname = '',
	vhlmid =0
where TRNSPRTID is null