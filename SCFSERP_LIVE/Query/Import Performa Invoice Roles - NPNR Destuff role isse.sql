--select * from AspNetRoles where name like 'import%'
--and rmenugroupid  = 11
--and RControllerName = 'ImportInvoice'
--order by RMenuGroupId, RMenuGroupOrder

--select * from AspNetRoles where 
----and rmenugroupid  = 11
--name like '%Performa%Invoice%'
--order by RMenuGroupId, RMenuGroupOrder

update  AspNetRoles
set RMenuGroupOrder = 15
where name like 'import%'
and rmenugroupid  = 11
and RControllerName = 'ImportManualBill'
and RMenuGroupOrder = 20

update AspNetRoles
set RMenuType = 'Performa Invoice', RControllerName = 'PerformaInvoice', RMenuGroupId = 11, RMenuGroupOrder = 16, RMenuIndex = 'Index'
where name like 'ImportPerformaInvoice%'	
and RMenuIndex is null

--select * From aspnetroles where name like 'ImportPerformaInvoice%'	
--select * From aspnetroles where RMenuType like 'DeStuff Slip'

update aspnetroles 
set RMenuType = 'DeStuff Slip (Imp)'
where RMenuType like 'DeStuff Slip'
and name like 'import%'
--select * from menurolemaster where roles = 'importuser'
--order by MenuGId, MenuGIndex