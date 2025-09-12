select * from Import_Slab_Handling_Type_Master

update Import_Slab_Handling_Type_Master
set HTYPEDESC = 'Nil'
where HTYPE = 1
and HTYPEDESC = 'Not Required'