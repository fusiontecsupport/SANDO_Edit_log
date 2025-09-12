
update AspNetRoles set RMenuGroupOrder=1 where RMenuGroupId=12 and RControllerName = 'ExportGateIn' 

update AspNetRoles set RMenuGroupOrder=2 where RMenuGroupId=12 and RControllerName ='ExportCartingOrder' and RMenuGroupOrder=2 

update AspNetRoles set RMenuGroupOrder=3 where RMenuGroupId=12 and RControllerName = 'ExportShippingBillMaster' 

update AspNetRoles set RMenuGroupOrder=4 where RMenuGroupId=12 and RControllerName = 'ExportCartingOrder' and RMenuGroupOrder=4 and RMenuIndex='SAIndex' 

update AspNetRoles set RMenuGroupOrder=5 where RMenuGroupId=12 and RControllerName= 'Stuffing'

update AspNetRoles set RMenuGroupOrder=6 where RMenuGroupId=12 and RControllerName= 'Seal'

update AspNetRoles set RMenuGroupOrder=7 where RMenuGroupId=12 and RControllerName = 'AuthorizationSlip' and Name Like '%ExportLoadSlip%'

update AspNetRoles set RMenuGroupOrder=8 where RMenuGroupId=12 and RControllerName = 'ExportVehicleTicket'

update AspNetRoles set RMenuGroupOrder=9 where RMenuGroupId=12 and RControllerName = 'ExportGateOut'

update AspNetRoles set RMenuGroupOrder=10 where RMenuGroupId=12 and RControllerName = 'ExportTruckOut' 

update AspNetRoles set RMenuGroupOrder=11 where RMenuGroupId=12 and RControllerName = 'StuffingBill' 

update AspNetRoles set RMenuGroupOrder=12 where RMenuGroupId=12 and RControllerName = 'SealBill'

update AspNetRoles set RMenuGroupOrder=13 where RMenuGroupId=12 and RControllerName = 'ExportManualBill'

update AspNetRoles set RMenuGroupOrder=14 where RMenuGroupId=12 and RControllerName = 'BSStuffingBill'

update AspNetRoles set RMenuGroupOrder=15 where RMenuGroupId=12 and RControllerName = 'BSSealBill'




