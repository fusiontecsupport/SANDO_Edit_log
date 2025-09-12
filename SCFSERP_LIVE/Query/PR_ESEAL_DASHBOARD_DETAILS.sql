-- EXEC [PR_ESEAL_DASHBOARD_DETAILS] '2021-10-20' , '2021-10-20', 11
alter proc [dbo].PR_ESEAL_DASHBOARD_DETAILS
@PFDT datetime,
@PTDT datetime,
@PSDPTID int
as
begin

	Declare @temptable table
	(
	Sno int,
	Descriptn varchar(250),
	c_20 int,
	c_40 int,
	c_45 int,
	c_tues int)

	insert into @temptable
	select	1, 'E-SEAL - GATEIN', SUM(CASE WHEN CONTNRSID = 3 THEN 1 ELSE 0 END) '20"', 
			SUM(CASE WHEN CONTNRSID = 4 THEN 1 ELSE 0 END) '40"',
			SUM(CASE WHEN CONTNRSID = 5 THEN 1 ELSE 0 END) '45"',
			0 'TUES'
	from GATEINDETAIL (nolock)
	where SDPTID = @PSDPTID
	and GIDATE between @PFDT and @PTDT
	and CONTNRSID > 2
	and DISPSTATUS = 0
	--group by CONTNRSID

	insert into @temptable
	select 2, 'E-SEAL - GATEOUT', SUM(CASE WHEN CONTNRSID = 3 THEN 1 ELSE 0 END) '20"', 
			SUM(CASE WHEN CONTNRSID = 4 THEN 1 ELSE 0 END) '40"',
			SUM(CASE WHEN CONTNRSID = 5 THEN 1 ELSE 0 END) '45"',
			0 'TUES'
	from GATEOUTDETAIL (NOLOCK) join GATEINDETAIL (nolock) 
						on GATEOUTDETAIL.GIDID = GATEINDETAIL.GIDID and 
						GATEOUTDETAIL.SDPTID = GATEINDETAIL.SDPTID
	where GATEOUTDETAIL.SDPTID = @PSDPTID
	and GODATE between @PFDT and @PTDT
	and CONTNRSID > 2
	and GATEOUTDETAIL.DISPSTATUS = 0
	--group by CONTNRSID

	update @temptable
	set c_tues = isnull(c_20,0) + (isnull(c_40,0)*2)+ (isnull(c_45,0)*2)
	
	select * from @temptable
end



