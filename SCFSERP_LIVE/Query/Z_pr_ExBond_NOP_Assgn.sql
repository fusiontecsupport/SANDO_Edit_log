create PROCEDURE Z_pr_ExBond_NOP_Assgn @PBNDID INT, @PEBNDEDATE SMALLDATETIME
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DECLARE @TableMaster TABLE
    (
      BNDID INT
    , EBNDNOP  numeric(18,2)
    )

    -- Insert statements for procedure here
	Insert into @TableMaster(BNDID, EBNDNOP)
	select @PBNDID, SUM(EBNDNOP) as EBNDNOP  from EXBONDMASTER where BNDID = @PBNDID AND EBNDEDATE < @PEBNDEDATE

	Select * From @TableMaster


END

