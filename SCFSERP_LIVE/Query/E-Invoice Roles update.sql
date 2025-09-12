select * from AspNetRoles where name like '%einvoice%'

select * from AspNetRoles where name like 'import%invoice%'

update AspNetRoles 
set RMenuType = 'Import E-Invoice', RMenuGroupId = 25, RMenuGroupOrder = 1, RMenuIndex = 'Index', RControllerName = 'ImportEInvoice'
where name like '%importeinvoice%'

update AspNetRoles 
set RMenuType = 'Import Manual E-Invoice', RMenuGroupId = 25, RMenuGroupOrder = 2, RMenuIndex = 'Index', RControllerName = 'ImportManualEInvoice'
where name like '%importManualeinvoice%'

update AspNetRoles 
set RMenuType = 'Stuffing E-Invoice', RMenuGroupId = 25, RMenuGroupOrder = 3, RMenuIndex = 'Index', RControllerName = 'StuffingEInvoice'
where name like '%stuffingeinvoice%'

update AspNetRoles 
set RMenuType = 'Seal E-Invoice', RMenuGroupId = 25, RMenuGroupOrder = 4, RMenuIndex = 'Index', RControllerName = 'SealEInvoice'
where name like '%sealeinvoice%'


update AspNetRoles 
set RMenuType = 'Export Manual E-Invoice', RMenuGroupId = 25, RMenuGroupOrder =5, RMenuIndex = 'Index', RControllerName = 'ExportManualEInvoice'
where name like '%exportmanualeinvoice%'

update AspNetRoles 
set RMenuType = 'Bond E-Invoice', RMenuGroupId = 25, RMenuGroupOrder =6, RMenuIndex = 'Index', RControllerName = 'BondEInvoice'
where name like '%bondeinvoice%'


update AspNetRoles
set name = 'ExportManualEInvoiceIndex'
where name = 'ExportManualEInvoice'