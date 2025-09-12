--STF Cursor
CREATE PROCEDURE [dbo].[Z_EXPORT_VTGO_Issues_STUFFING_MASTERDETAIL_Auto_Generate_XML]
AS
BEGIN
	SET NOCOUNT ON
	
      DECLARE @Counter INT
      SET @Counter = 1
      
	  --Delete A
	  --From STUFFINGMASTER A JOIN STUFFINGDETAIL B ON A.STFMID = B.STFMID
	  --WHERE OLDSTFDID > 0 

	  
	  Declare @STFDID int, @OSTFMID INT, @PRVOSTFMID INT, @PRVOSTFDID INT
	  
	  SET @PRVOSTFMID= 0 
	  SET @PRVOSTFDID= 0 

      --DECLARE THE CURSOR FOR A QUERY.
      DECLARE STFDetail CURSOR READ_ONLY
      FOR
	  
	  Select DISTINCT 
			--TOP 25  
			B.STFDID ,  case when cymaxstfmid >0 then cymaxstfmid 
				 when pymaxstfmid >0 then  pymaxstfmid 
				  when  cymaxstfmid >  pymaxstfmid then  cymaxstfmid 
				 else  pymaxstfmid end AS OSTFMID
	  From [SCFS_ERP].dbo.VTGOIssueStuffing_261021_1015 B (NOLOCK)
	  WHERE B.STFDID > 0
	  ORDER BY  case when cymaxstfmid >0 then cymaxstfmid 
				 when pymaxstfmid >0 then  pymaxstfmid 
				  when  cymaxstfmid >  pymaxstfmid then  cymaxstfmid 
				 else  pymaxstfmid end , B.STFDID
  
      --OPEN CURSOR.
      OPEN STFDetail
 
      --FETCH THE RECORD INTO THE VARIABLES.
      FETCH NEXT FROM STFDetail INTO @STFDID, @OSTFMID
 
      --LOOP UNTIL RECORDS ARE AVAILABLE.
      WHILE @@FETCH_STATUS = 0
      BEGIN

			

			--SELECT @STFDID, @OSTFMID, @PRVOSTFMID , 'CHK'
			--EXEC Z_EXPORT_STUFFING_MASTERDETAIL_INSERT_XML @STFDID
			EXEC [Z_EXPORT_STUFFING_MSTDTL_INSERT_XML] @STFDID, @OSTFMID, @PRVOSTFMID, @PRVOSTFDID
             --INCREMENT COUNTER.
			 
			 IF @PRVOSTFMID <> @OSTFMID
			SET @PRVOSTFMID = @OSTFMID
			SET @PRVOSTFDID = @STFDID
			

             SET @Counter = @Counter + 1
             
             --FETCH THE NEXT RECORD INTO THE VARIABLES.
             FETCH NEXT FROM STFDetail INTO @STFDID, @OSTFMID
      END
 
      --CLOSE THE CURSOR.
      CLOSE STFDetail
      DEALLOCATE STFDetail


END

