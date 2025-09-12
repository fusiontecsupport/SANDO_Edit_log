if exists (select * from AspNetRoles where RControllerName like 'ExportCartingOrder%' and RMenuGroupOrder =3)
begin

update AspNetRoles
set RMenuGroupOrder = 9999
where name like 'export%'
and RMenuGroupOrder = 2


update AspNetRoles
set RMenuGroupOrder = 2
where name like 'export%'
and RMenuGroupOrder = 3

update AspNetRoles
set RMenuGroupOrder = 3
where name like 'export%'
and RMenuGroupOrder = 9999

end