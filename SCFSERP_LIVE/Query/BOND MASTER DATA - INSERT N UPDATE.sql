select * from [BONDGODOWNTYPEMASTER]

select * from [SPBW]..[gODOWNTYPEMASTER]

--SET IDENTITY_INSERT [BONDGODOWNTYPEMASTER] ON
insert into [BONDGODOWNTYPEMASTER]
select * from [SPBW]..[gODOWNTYPEMASTER]

--select * from GODOWNMASTER
--select * from [BONDGODOWNMASTER]

insert into [BONDGODOWNMASTER]
select * from [SPBW]..[gODOWnMASTER]
