--sp_helptext pr_USER_MENU_DETAIL_ASSGN

--select * From MenuRoleMaster

--exec pr_USER_MENU_DETAIL_ASSGN 'import'
--go
--select * from MenuRoleMaster
--SELECT    dbo.AspNetRoles.RMenuType, dbo.AspNetRoles.RMenuIndex, dbo.AspNetRoles.RControllerName, dbo.AspNetUsers.UserName, 
--                      dbo.AspNetRoles.RMenuGroupId, dbo.AspNetRoles.RMenuGroupOrder
--FROM         dbo.AspNetUsers INNER JOIN
--                      dbo.AspNetUserRoles ON dbo.AspNetUsers.Id = dbo.AspNetUserRoles.UserId INNER JOIN
--                      dbo.AspNetRoles ON dbo.AspNetUserRoles.RoleId = dbo.AspNetRoles.Id INNER JOIN
--                      dbo.ApplicationUserGroups ON dbo.AspNetUsers.Id = dbo.ApplicationUserGroups.UserId INNER JOIN
--                      dbo.Groups ON dbo.ApplicationUserGroups.GroupId = dbo.Groups.Id
--GROUP BY dbo.AspNetUsers.UserName, dbo.AspNetRoles.RMenuType, dbo.AspNetRoles.RMenuIndex, dbo.AspNetRoles.RControllerName, dbo.AspNetRoles.RMenuGroupId, 
--                      dbo.AspNetRoles.RMenuGroupOrder
--HAVING      (dbo.AspNetUsers.UserName = 'test') AND (dbo.AspNetRoles.RMenuType IS NOT NULL)
--ORDER BY dbo.AspNetRoles.RMenuGroupId, dbo.AspNetRoles.RMenuGroupOrder

select max(rmenugroupid) from AspNetRoles 
update AspNetRoles 
 set rmenugroupid = 22 
 where RControllerName like 'nonpnr%'

 
update  AspNetRoles
set name = 'NonPnrGateInPrint'
 where name like '%NonNprGateInPrint%'

update AspNetRoles
set RMenuType = 'Gate In'
where RControllerName = 'NonPnrGateIn'
update AspNetRoles
set RMenuType = 'Load Slip'
where RControllerName = 'NonPnrLoadAuthorizationSlip'
update AspNetRoles
set RMenuType = 'DeStuff Slip'
where RControllerName = 'NonPnrDeStuffAuthorizationSlip'
update AspNetRoles
set RMenuType = 'Vehicle Ticket'
where RControllerName = 'NonPnrVehicleTicket'
update AspNetRoles
set RMenuType = 'Gate Out'
where RControllerName = 'NonPnrGateOut'
update AspNetRoles
set RMenuType = 'Invoice'
where RControllerName = 'NonPnrInvoice'
