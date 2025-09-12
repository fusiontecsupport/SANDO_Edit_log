select * from AspNetRoles where RControllerName = 'invoice'

update AspNetRoles 
set RControllerName = 'ImportInvoice'
where RControllerName = 'invoice'


update AspNetRoles 
set RMenuType = 'Invoice (Imp)'
where RControllerName = 'ImportInvoice'