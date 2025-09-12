--select * from menurolemaster where roles = 'test'

--select * From AspNetRoles where Name like '%import%gate%'
--select * From AspNetRoles where Name like '%nonpnr%slip%'

update AspNetRoles set RControllerName='ImportRemoteGateIn' where RControllerName like 'RemoteGateIn'

update AspNetRoles set RControllerName='ImportGateIn' where RControllerName like 'GateInDetail' and RMenuType like 'Gate In(Imp)'

update AspNetRoles set RControllerName='ImportGateInBlock' where RControllerName like 'GateInBlock'

update AspNetRoles set RControllerName='ImportLoadSlip' where RControllerName like 'LoadAuthorizationSlip' and RMenuType like 'Load Slip(Imp)'

update AspNetRoles set RControllerName='ImportDeStuff' where RControllerName like 'DestuffAuthorizationSlip' and name like 'importDeStuff%'