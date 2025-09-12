select * from aspnetroles where name like 'NONPNRADV%'

update aspnetroles 
set name ='NonPnrAdvanceCreate', Description = 'Can Add NonPnr Advance'
where name like 'NONPNRADVANCECREATE'

update aspnetroles 
set name ='NonPnrAdvanceDelete', Description = 'Can Delete NonPnr Advance'
where name like 'NONPNRADVANCEDELETE'

update aspnetroles 
set name ='NonPnrAdvanceEdit', Description = 'Can Modify NonPnr Advance'
where name like 'NONPNRADVANCEEDIT'

update aspnetroles 
set name ='NonPnrAdvanceIndex', Description = 'Can View NonPnr Advance Details'
where name like 'NONPNRADVANCEINDEX'


update aspnetroles 
set name ='NonPnrAdvancePrint', Description = 'Can Print NonPnr Advance'
where name like 'NONPNRADVANCEPRINT'

select * from aspnetroles where name like 'NONPNRADV%'


select * from aspnetroles where name like '%'


update aspnetroles 
set name ='ExportShippingAdmissionCreate', Description = 'Can Create Export Shipping Admission'
where name like 'EXPORTSHIPPINGADMISSIONCREATE'



update aspnetroles 
set name ='ExportShippingAdmissionIndex', Description = 'Can View Export Shipping Admission'
where name like 'EXPORTSHIPPINGADMISSIONIndex'



update AspNetRoles set  RMenuType='Advance (NPnr)',RControllerName='NonPnrAdvance',RMenuGroupId=2,RMenuGroupOrder=7,RMenuIndex='Index'
where Name = 'NonPnrAdvanceIndex'

update AspNetRoles  set RMenuType='Advance (NPnr)',RControllerName='NonPnrAdvance',RMenuGroupId=2,RMenuGroupOrder=7,RMenuIndex='Index'
where Name = 'NonPnrAdvanceEdit'

update AspNetRoles set RMenuType='Advance (NPnr)',RControllerName='NonPnrAdvance',RMenuGroupId=2,RMenuGroupOrder=7,RMenuIndex='Index'
where Name = 'NonPnrAdvanceCreate'

update AspNetRoles set RMenuType='Advance (NPnr)',RControllerName='NonPnrAdvance',RMenuGroupId=2,RMenuGroupOrder=7,RMenuIndex='Index'
where Name = 'NonPnrAdvancePrint'

update AspNetRoles set RMenuType='Advance (NPnr)',RControllerName='NonPnrAdvance',RMenuGroupId=2,RMenuGroupOrder=7,RMenuIndex='Index'
where Name = 'NonPnrAdvanceDelete'

update AspNetRoles set RMenuType='Shipping Admission',RControllerName='ExportCartingOrder',RMenuGroupId=12,RMenuGroupOrder=4,RMenuIndex='SAIndex'
where Name = 'ExportShippingAdmissionIndex'


update AspNetRoles set RMenuGroupId=22
where RControllerName='NonPnrAdvance'

select * from AspNetRoles where RControllerName = 'ExportCartingOrder'
select * from AspNetRoles where RControllerName like 'nonpnr%'