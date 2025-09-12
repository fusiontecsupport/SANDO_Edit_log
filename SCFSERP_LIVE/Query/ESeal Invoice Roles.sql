select * from aspnetroles where name like 'esealinv%'
select * from aspnetroles where name like 'nonpnrinv%'

update AspNetRoles
set RMenuType = 'Invoice(ESeal)', RControllerName = 'ESealInvoice', RMenuGroupId = 23, RMenuGroupOrder = 4, RMenuIndex = 'Index'
where name like 'ESealInvoice%'	
and RMenuIndex is null

