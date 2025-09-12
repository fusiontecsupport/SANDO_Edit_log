alter PROC PR_GET_REPORT_IMPORT_REGISTER
AS
BEGIN
	declare @tbl table
	(dval int,
	 dtxt varchar(100)
	 )

	 insert into @tbl
	 select 0, 'ALL'
	 union
	 SELECT REGSTRID, REGSTRDESC
	 from EXPORT_INVOICE_REGISTER (NOLOCK)
	 where REGSTRID in (1,2,6)
	 union
	 SELECT 99, 'BoS UNBILLED'

	 select * from @tbl
END