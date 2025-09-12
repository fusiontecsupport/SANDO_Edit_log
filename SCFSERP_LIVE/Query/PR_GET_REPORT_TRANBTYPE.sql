CREATE PROC PR_GET_REPORT_TRANBTYPE
AS
BEGIN
	declare @tbl table
	(dval int,
	 dtxt varchar(100)
	 )

	 insert into @tbl
	 select 0, 'ALL'
	 union
	 select 1, 'LOAD'
	 union
	 select 2, 'DESTUFF'

	 select * from @tbl
END