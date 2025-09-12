create PROCEDURE [dbo].[z_pr_New_Export_Covid_Slab_Rate_Assgn] @PKUSRID varchar(100),
@PTARIFFMID int, @PSTMRID int, @PCHRGETYPE int, @PSLABTID int, @PSLABMIN int, @PCONTNRSID int,
@PSLABHTYPE int, @PCHRGDATE smalldatetime
AS



 BEGIN
	SET NOCOUNT ON
	
	--Declare @PTARIFFMID int, @PSTMRID int, @PCHRGETYPE int, @PSLABTID int, @PSLABMIN int, @PCONTNRSID int
	--Declare @PSLABHTYPE int, @PCHRGDATE smalldatetime

	 --Set @PTARIFFMID = 27
	 --Set @PSTMRID = 0
	 --Set @PCHRGETYPE = 1
	 --Set @PSLABTID = 2
	 --Set @PSLABMIN = 12
	 --Set @PCONTNRSID = 3
	 --Set @PSLABHTYPE = 0
	 --Set @PCHRGDATE = '19-nov-2018'

	  DECLARE @SLABMIN int
	  DECLARE @SLABMAX int
	  DECLARE @SLABAMT numeric(18,2)

	  Declare @TmpSMMin int
	  Declare @TmpSMMax int
	  Declare @TmpCDays int
	  Declare @TmpPSMMax int
	  Declare @TmpMaxDays int
	  Declare @TmpPMaxDays int
	  Declare @TmpTotDays int
	  Declare @TmpSlabAmt numeric(18,2)
	  Declare @TmpSMAmt  numeric(18,2)

	  Delete From TMP_CHA_OUTSTANDING_RPT Where KUSRID = @PKUSRID
	  
	  Set @TmpPSMMax = 0
	  Set @TmpPMaxDays = @PSLABMIN
	  Set @TmpMaxDays = @PSLABMIN

      DECLARE @Counter INT
      SET @Counter = 1
      

      --DECLARE THE CURSOR FOR A QUERY.
      DECLARE GRNDetail CURSOR READ_ONLY
      FOR
	  
	 SELECT TOP(1) SLABMIN, SLABMAX, SLABAMT FROM VW_EXPORT_RATECARDMASTER_FLX_ASSGN WHERE TARIFFMID = @PTARIFFMID AND SLABTID = @PSLABTID
	 AND HTYPE = @PSLABHTYPE AND SDTYPE=1 AND SLABMIN <= @PSLABMIN AND CHRGETYPE = @PCHRGETYPE 
	 AND CONTNRSID = @PCONTNRSID AND CHAID = @PSTMRID 
	   ORDER BY SLABMIN desc --AND SLABMDATE  =  @PCHRGDATE 
	  
      --OPEN CURSOR.
      OPEN GRNDetail
 
      --FETCH THE RECORD INTO THE VARIABLES.
      FETCH NEXT FROM GRNDetail INTO @SLABMIN, @SLABMAX, @SLABAMT
 
      --LOOP UNTIL RECORDS ARE AVAILABLE.
      WHILE @@FETCH_STATUS = 0
      BEGIN
				
			Set @TmpSMAmt =  @SLABAMT
			INSERT INTO TMP_CHA_OUTSTANDING_RPT (KUSRID,COMPYID,CATEID,COL1) 
			VALUES(@PKUSRID, 0, 0, @TmpSMAmt)
             --INCREMENT COUNTER.
			 
             SET @Counter = @Counter + 1
             
             --FETCH THE NEXT RECORD INTO THE VARIABLES.
             FETCH NEXT FROM GRNDetail INTO @SLABMIN, @SLABMAX, @SLABAMT
      END
 
      --CLOSE THE CURSOR.
      CLOSE GRNDetail
      DEALLOCATE GRNDetail


END