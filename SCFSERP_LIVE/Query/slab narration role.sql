select * From AspNetRoles where name like 'importslabnarrationindex'


update AspNetRoles 
set RControllerName = 'ImportSlabNarration',RMenuGroupId=3,RMenuGroupOrder=3,RMenuType='Import Slab Narration',RMenuIndex='Index'
where name like 'importslabnarrationindex'


Update AspNetRoles set Name='ImportSlabNarrationIndex',RMenuType='Import Slab Narration',RControllerName='ImportSlabNarration',RMenuGroupId=3,RMenuGroupOrder=3,RMenuIndex='Index'

Where Id='ea8a64da-9413-4de9-924c-6c9aede66c27'