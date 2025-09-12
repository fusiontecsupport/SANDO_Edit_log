create table Ex_Bond_VT_Handling_Type_Master
(
HandTypeID int identity(0,1) primary key,
HandTypeDesc varchar(50)
)
go

insert into Ex_Bond_VT_Handling_Type_Master
Select 'Not Required'


insert into Ex_Bond_VT_Handling_Type_Master
Select 'Loading'


insert into Ex_Bond_VT_Handling_Type_Master
Select 'Unloading'

select * From Ex_Bond_VT_Handling_Type_Master