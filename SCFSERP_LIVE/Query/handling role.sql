select * from AspNetUsers a(nolock) , 
	AspNetUserRoles b(nolock), AspNetRoles c(nolock)
where a.id = b.UserId
and b.RoleId = c.id
and a.UserName  = 'test'
and c.Name = 'HANDLING CHARGES UPDATE'
