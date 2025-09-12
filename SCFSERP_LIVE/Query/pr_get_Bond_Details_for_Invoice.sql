-- exec pr_get_Bond_Details_for_Invoice @term= 'B12'
alter proc pr_get_Bond_Details_for_Invoice
@term varchar(100)
as
begin
	select * from vw_Get_Bond_Dtl_GI_Completed(nolock)
	where BNDDNO like '%' + @term + '%'
	order by BNDDNO
end