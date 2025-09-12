-- select * from ApplicationUserGroups

alter view aspnet_user_group 
as 
select  a.UserId, a.GroupId, b.Name 'GroupName', c.UserName
from ApplicationUserGroups a(nolock) 
	join [Groups] b(nolock) on a.GroupId = b.Id
	join AspNetUsers c(nolock) on a.UserId = c.Id
