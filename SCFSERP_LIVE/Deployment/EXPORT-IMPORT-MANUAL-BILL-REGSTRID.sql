select * from EXPORT_INVOICE_REGISTER

if not exists (select '*' from EXPORT_INVOICE_REGISTER where REGSTRID=49)
begin
	insert into EXPORT_INVOICE_REGISTER
	select 49, 'TAX INVOICE'

	insert into EXPORT_INVOICE_REGISTER
	select 50, 'BILL OF SUPPLY'
end


if not exists (select '*' from EXPORT_INVOICE_REGISTER where REGSTRID=15)
begin
	insert into EXPORT_INVOICE_REGISTER
	select 15, 'TAX INVOICE'

	insert into EXPORT_INVOICE_REGISTER
	select 34, 'BILL OF SUPPLY'
end

select * from EXPORT_INVOICE_REGISTER
