	select max(RMenuGroupId) from  AspNetRoles

select * from  AspNetRoles where RMenuGroupId=5
select * from  AspNetRoles where name like 'godown%'

select * from  AspNetRoles where name like 'bondgodown%'


BondGodownMasterDelete	Can Delete Bond Godown Master
BondGodownMasterIndex	Can View Bond Godown master
BondGodownMasterEdit	Can Modify Bond Godown master
BondGodownMasterCreate	Can Add Bond Godown Master

select * from  AspNetRoles where name like 'bondinfo%'


select * from aspnetusers a join ApplicationUserGroups b on a.Id = b.UserId
join groups c on b.GroupId = c.Id
where c.Name like '%admin%'




update AspNetRoles 
set RMenuType ='In-Bond Information',
RControllerName	='BondInformation', RMenuGroupId = 28,	RMenuGroupOrder	=1, RMenuIndex= 'Index'
where name like 'bondinfo%'



update AspNetRoles 
set RMenuType ='In-Bond Gate IN',
RControllerName	='BondGateIn', RMenuGroupId = 28,	RMenuGroupOrder	=2, RMenuIndex= 'Index'
where name like 'bondgatein%'

update AspNetRoles 
set RMenuType ='Bond Godown',
RControllerName	='BondGodownMaster', RMenuGroupId = 5,	RMenuGroupOrder	=1, RMenuIndex= 'Index'
where name like 'bondgodown%'


update AspNetRoles 
set RMenuType ='Bond Slab Type',
RControllerName	='BondSlabType', RMenuGroupId = 5,	RMenuGroupOrder	=2, RMenuIndex= 'Index'
where name like 'BondSlabType%'


update AspNetRoles 
set RMenuType ='Bond Slab',
RControllerName	='BondSlabMaster', RMenuGroupId = 5,	RMenuGroupOrder	=4, RMenuIndex= 'Index'
where name like 'BondSlabMaster%'




update AspNetRoles 
set RMenuType ='Bond Tariff',
RControllerName	='BondTariffMaster', RMenuGroupId = 5,	RMenuGroupOrder	=3, RMenuIndex= 'Index'
where name like 'BondTariffMaster%'




update AspNetRoles 
set RMenuType ='Bond Product Group',
RControllerName	='BondProductGroupMaster', RMenuGroupId = 5,	RMenuGroupOrder	=5, RMenuIndex= 'Index'
where name like 'BondProductGroup%'


update AspNetRoles 
set RMenuType ='In-Bond Invoice',
RControllerName	='BondInvoice', RMenuGroupId = 28,	RMenuGroupOrder	=3, RMenuIndex= 'Index'
where name like 'bondinvoice%'


update AspNetRoles 
set RMenuType ='Ex-Bond Information',
RControllerName	='ExBondInformation', RMenuGroupId = 28,	RMenuGroupOrder	=4, RMenuIndex= 'Index'
where name like 'exbondinfo%'


update AspNetRoles 
set RMenuType ='Ex-Bond Vehicle Ticket', --[Description]='Can View ExBond Vehicle Ticket',
RControllerName	='ExBondVehicleTicket', RMenuGroupId = 28,	RMenuGroupOrder	=5, RMenuIndex= 'Index'
where name like 'ExBondVehicleTicket%'

update AspNetRoles 
set RMenuType ='ExBond Vehicle Ticket', --[Description]='Can View ExBond Vehicle Ticket',
RControllerName	='ExBondVehicleTicket', RMenuGroupId = 28,	RMenuGroupOrder	=5, RMenuIndex= 'Index'
where name like 'ExBondVT%'
select * From AspNetRoles where name like 'exbond%'
--delete from AspNetRoles where name like 'ExBondVehicleTicketIndex'
--delete from AspNetRoles where name like 'ExBondVTIndex'
select * From aspnet_user_group a 
join ApplicationRoleGroups b on a.GroupId = b.GroupId
where a.UserName = 'ck'
