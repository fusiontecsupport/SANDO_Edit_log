select * from AspNetUsers  u 
		join ApplicationUserGroups g on u.id = g.UserId
		join Groups gr on g.GroupId = gr.Id
where gr.Name like '%admin%'
order by gr.Name


select * from BONDTRANSACTIONMASTER 
--where tranmid in (4, 6) 
order by tranmid desc
select * from BONDTRANSACTIONDETAIL 
--where tranmid in (4, 6) 
order by tranmid desc
select distinct tranbtype,substring(trandno,1,3) from BONDTRANSACTIONMASTER 
select distinct trantid from BONDTRANSACTIONMASTER 

53

select * from BONDTRANSACTIONMASTERFACTOR


PROACTIVE SYSTEMS