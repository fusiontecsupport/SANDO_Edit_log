if not exists (select '*' from EXPORT_INVOICE_REGISTER where REGSTRID = 31)
begin
	insert into EXPORT_INVOICE_REGISTER
	select 31, 'CUSTOMER'
	union
	select 32, 'NOTOINAL'
	union
	select 33, 'ZERO BILL'
end