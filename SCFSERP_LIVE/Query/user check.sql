select * From AspNetRoles a join ApplicationRoleGroups b on a.id = b.RoleId
join AspNetUserRoles c on b.RoleId = c.RoleId
join ApplicationUserGroups d on  c.UserId = d.UserId
join AspNetUsers e on  d.UserId = e.Id
join groups f on d.GroupId = f.id
where RControllerName like '%Search%'

select * From AspNetUsers where username like 'banu%'
select * From BONDMASTER where BNDDNO = '2001944625'
select * From OLDLIVEBONDMASTER where BNDDNO = '2001944625'