select *from AspNetRoles where  RMenuGroupId=12 and Name Like '%Index%'
order by RMenuGroupId,RMenuGroupOrder 

select *from AspNetRoles where  RMenuGroupId=12 and Name Like '%ExportLoadSlip%'
order by RMenuGroupId,RMenuGroupOrder 

select *from AspNetRoles where  RMenuGroupId=12 and RControllerName ='ExportCartingOrder' and RMenuGroupOrder=2
order by RMenuGroupId,RMenuGroupOrder 

delete from AspNetRoles where RControllerName='ImportAdvance'

delete from AspNetRoles where RMenuGroupId=12 and Name Like '%ExportAdvance%'

