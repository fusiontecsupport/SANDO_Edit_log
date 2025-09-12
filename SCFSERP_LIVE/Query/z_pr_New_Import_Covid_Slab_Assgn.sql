alter PROCEDURE [dbo].[z_pr_New_Import_Covid_Slab_Assgn]  @PKUSRID varchar(100), @PSDATE smalldatetime, @PEDATE smalldatetime,
@PTARIFFMID int, @PSTMRID int, @PCHRGETYPE int, @PSLABTID int, @PSLABMIN int, @PCONTNRSID int,
@PSLABHTYPE int, @PCHRGDATE smalldatetime, @PCDate1 smalldatetime, @PCDate2 smalldatetime
AS
BEGIN

	SET NOCOUNT ON;
	Select 0.0 as SAMT, 0.0 AS DISCAMT 
	Return

	DECLARE @CovidLockDownMaster TABLE
    (
      GIDATE SMALLDATETIME
    , GODATE SMALLDATETIME
    )

	DECLARE @TableMaster TABLE
    (
      GIDATE SMALLDATETIME
    , GODATE SMALLDATETIME
    , DDAYS  INT
	, SAMT   NUMERIC(18,2)
	, DISCAMT   NUMERIC(18,2)
	, CCOUNT INT
    )

    DECLARE @DIFFDAYS int, @count int, @scount int, @covidcount int
	DECLARE @SDATE SMALLDATETIME, @EDATE SMALLDATETIME

	Insert Into @CovidLockDownMaster(GIDATE, GODATE) Values(@PCDate1, @PCDate2)

	Declare @smamt numeric(18,2)

	--Set @PSDATE = '23-FEB-2020'
	--Set @PEDATE = '02-MAY-2020'
	Set @DIFFDAYS = DATEDIFF(D, @PSDATE, @PEDATE) + 1
	Set @count = 0

	WHILE (@count <  @DIFFDAYS)
		BEGIN
		    
			Set @scount = @count+1
			EXEC z_pr_New_Import_Covid_Slab_Rate_Assgn @PKUSRID, @PTARIFFMID,@PSTMRID,@PCHRGETYPE,@PSLABTID,@scount,@PCONTNRSID,@PSLABHTYPE,@PCHRGDATE 

			Select @smamt =  COL1 From TMP_CHA_OUTSTANDING_RPT Where KUSRID  = @PKUSRID
			Set @smamt = ISNULL(@smamt, 0)

			Set @SDATE = @PSDATE + @count

			Select @covidcount = Count(*) From  @CovidLockDownMaster Where GIDATE <= @SDATE and GODATE >= @SDATE

			Set @covidcount = ISNULL(@covidcount, 0)

			INSERT INTO @TableMaster (GIDATE,GODATE,DDAYS,SAMT,DISCAMT,CCOUNT) 
			VALUES (@PSDATE, @SDATE, @scount, @smamt,(Case When @covidcount > 0 then @smamt Else 0 End),@covidcount) 
        
			SET @count = (@count + 1 )  

		END
		--Select @covidcount
		--Select * From @TableMaster
		Select SUM(SAMT) as SAMT, SUM(DISCAMT) AS DISCAMT From @TableMaster

		Return
END



