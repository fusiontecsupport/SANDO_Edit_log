--select * from AspNetRoles where Name like 'opensheet%'
select * from AspNetRoles where Name like 'mail%'
select * from AspNetRoles where Name like 'manualopensheet%'
select * from AspNetRoles where Name like 'eseal%'
select distinct RMenuGroupId from AspNetRoles
--select * from AspNetRoles where RMenuGroupId =5

--select * from AspNetRoles where Name like 'importlorry%'
update AspNetRoles 
set RMenuGroupId		=	11 , 
	RMenuGroupOrder		=	12, 
	RMenuType			=	'Manual OpenSheet', 
	RControllerName		=	'ManualOpenSheet',
	RMenuIndex			=	'Index'
where name like 'manualopensheet%'


update AspNetRoles set RMenuType='Gate In (ES)',
RControllerName='ESealGateIn',RMenuGroupId=23,RMenuGroupOrder=1,RMenuIndex='Index' WHERE Name LIKE '%ESealGateIn%'

update AspNetRoles SET RMenuType='Gate Out (ES)',
RControllerName='ESealGateOut',RMenuGroupId=23,RMenuGroupOrder=3,RMenuIndex='Index' where NAme Like '%ESealGO%'

update AspNetRoles SET RMenuType='Vehicle Ticket (ES)',
RControllerName='ESealVehicleTicket',RMenuGroupId=23,RMenuGroupOrder=2,RMenuIndex='Index' where NAme Like '%ESealVT%'


--MailFormSend	Can Send Mail
--MailFormIndex	Can View Mail Form
--ManualOpenSheetDelete	Can Delete Manual Open Sheet Details
--ManualOpenSheetCreate	Can Add Manual Open Sheet Details
--ManualOpenSheetPrint	Can Print Manual Open Sheet Details
--ManualOpenSheetIndex	Can View Manual Open Sheet Details
--ManualOpenSheetEdit		Can Edit Manual Open Sheet Details

--ESealVTIndex	Can View ESealVTIndex
--ESealVTEdit	Can Edit ESealVTEdit
--ESealGOPrint	Can Print ESealGOPrint
--ESealGateInIndex	Can View ESealGateInIndex
--ESealGateInPrint	Can Print ESealGateInPrint
--ESealGOCreate	Can Create ESealGOCreate
--ESealGODelete	Can Delete ESealGODelete
--ESealGateInEdit	Can Edit ESealGateInEdit
--ESealVTDelete	Can Delete ESealVTDelete
--ESealGOEdit	Can Edit ESealGOEdit
--ESealGateInCreate	Can Create ESealGateInCreate
--ESealGateInDelete	Can Delete ESealGateInDelete
--ESealGOIndex	Can View ESealGOIndex
--ESealVTPrint	Can Print ESealVTPrint
--ESealVTCreate	Can Create ESealVTCreate
