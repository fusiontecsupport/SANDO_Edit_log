If not exists(select '*' from EXPORT_INVOICE_REGISTER where REGSTRID = 17)
begin
	insert into EXPORT_INVOICE_REGISTER
	select 17, 'NOTIONAL'
	union
	select 18, 'ZERO BILL'
end
select * from EXPORT_INVOICE_REGISTER
