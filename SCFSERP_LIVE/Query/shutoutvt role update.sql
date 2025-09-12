select * from AspNetRoles where  RMenuGroupId=12 and RMenuGroupOrder=26, name like 'shutout%'

update aspnetroles
set RMenuType = 'Shutout VT', RControllerName='ShutoutVehicleTicket', RMenuGroupId=12, RMenuGroupOrder=26, RMenuIndex='Index'
where name ='ShutoutVehicleTicketIndex'

--delete aspnetroles 
--where name ='ShutoutVehicleTicketIndex'
