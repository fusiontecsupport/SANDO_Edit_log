select * from AspNetRoles where Name like 'cheq%'

update AspNetRoles 
set RMenuGroupId = 24, RMenuGroupOrder = 4
where Name like 'cheq%'