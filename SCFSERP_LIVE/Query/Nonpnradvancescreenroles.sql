
--update AspNetRoles set RMenuType='Advance (NPnr)',RControllerName='NonPnrAdvance',RMenuGroupId=2,RMenuGroupOrder=7,RMenuIndex='Index'
--where id='8d7132cd-e095-4efc-96ef-94e1bc3ac4c6'

--update AspNetRoles set RMenuType='Advance (NPnr)',RControllerName='NonPnrAdvance',RMenuGroupId=2,RMenuGroupOrder=7,RMenuIndex='Index'
--where id='6256599c-10be-4838-8966-93f350df70d7'

--update AspNetRoles set RMenuType='Advance (NPnr)',RControllerName='NonPnrAdvance',RMenuGroupId=2,RMenuGroupOrder=7,RMenuIndex='Index'
--where id='a197517d-cf37-4813-a4d2-dcc7c9bd4e87'

--update AspNetRoles set RMenuType='Advance (NPnr)',RControllerName='NonPnrAdvance',RMenuGroupId=2,RMenuGroupOrder=7,RMenuIndex='Index'
--where id='d669854f-66dd-4b3d-865a-00afb688c5b3'

--update AspNetRoles set RMenuType='Advance (NPnr)',RControllerName='NonPnrAdvance',RMenuGroupId=2,RMenuGroupOrder=7,RMenuIndex='Index'
--where id='355109e3-674a-4334-8b3e-b70167e346a2'

--<<<<<<<<<<<<LIVE SERVER>>>>>>>>>>>>>>

--update AspNetRoles set Name = 'NonPnrAdvanceIndex', Description ='Can View NonPnr Advance', RMenuType='Advance (NPnr)',RControllerName='NonPnrAdvance',RMenuGroupId=2,RMenuGroupOrder=7,RMenuIndex='Index'
--where id='9b235ecb-c42d-4b02-b958-01d7873c02e4'

--update AspNetRoles  set Name = 'NonPnrAdvanceEdit', Description ='Can Edit NonPnr Advance', RMenuType='Advance (NPnr)',RControllerName='NonPnrAdvance',RMenuGroupId=2,RMenuGroupOrder=7,RMenuIndex='Index'
--where id='2d7494f7-7ad5-4a44-a6ea-3e2e2cc62d77'

--update AspNetRoles set Name = 'NonPnrAdvanceCreate', Description ='Can Create NonPnr Advance',  RMenuType='Advance (NPnr)',RControllerName='NonPnrAdvance',RMenuGroupId=2,RMenuGroupOrder=7,RMenuIndex='Index'
--where id='fa4aad30-a1be-4f87-8059-dd2ad11d6737'

--update AspNetRoles set Name = 'NonPnrAdvancePrint', Description ='Can Print NonPnr Advance', RMenuType='Advance (NPnr)',RControllerName='NonPnrAdvance',RMenuGroupId=2,RMenuGroupOrder=7,RMenuIndex='Index'
--where id='84aade62-4e14-436f-9c28-5893bf438a72'

--update AspNetRoles set Name = 'NonPnrAdvanceDelete', Description ='Can Delete NonPnr Advance',  RMenuType='Advance (NPnr)',RControllerName='NonPnrAdvance',RMenuGroupId=2,RMenuGroupOrder=7,RMenuIndex='Index'
--where id='7bf6d461-87bd-4cac-8172-ebd89bfeafe6'

select * from AspNetRoles where name like 'NonPnrAdvanceIndex'
select * from AspNetRoles where name like 'NonPnrAdvanceEdit'
select * from AspNetRoles where name like 'NonPnrAdvanceCreate'
select * from AspNetRoles where name like 'NonPnrAdvancePrint'
select * from AspNetRoles where name like 'NonPnrAdvanceDelete'
