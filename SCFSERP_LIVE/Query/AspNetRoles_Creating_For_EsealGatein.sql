select *from AspNetRoles WHERE Name LIKE '%ESealGateIn%'

update AspNetRoles set RMenuType='Gate In',RControllerName='ESealGateIn',RMenuGroupId=4,RMenuGroupOrder=1,RMenuIndex='Index' WHERE Name LIKE '%ESealGateIn%'
