USE [SCFS_ERP]
GO
/****** Object:  StoredProcedure [dbo].[pr_Get_Finance_Transaction_Details]    Script Date: 06/10/2021 20:09:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[pr_Finance_TranSactionBillNo]  
          @FilterTerm varchar(100) 
        , @Sdptid Int
		, @Tranrffid Int
        
AS BEGIN

Declare @LSDPTID int , @LCHAID int

set @LSDPTID = @Sdptid 
set @LCHAID = @Tranrffid

 SET @FilterTerm = @FilterTerm + '%'    

    DECLARE @TableMaster Table 
	( 
	  TRANDNO nvarchar(100) not null,
	  TRANMID int
	)

	
   Insert Into  @TableMaster(TRANDNO ,TRANMID )  
  SELECT TRANSACTIONMASTER.TRANDNO ,TRANSACTIONMASTER.TRANMID
 FROM TRANSACTIONMASTER (Nolock)
 WHERE (@FilterTerm IS NULL  OR TRANSACTIONMASTER.TRANDNO LIKE @FilterTerm   )  AND TRANSACTIONMASTER.SDPTID= @LSDPTID  AND TRANSACTIONMASTER.TRANREFID = @LCHAID 
 GROUP BY TRANDNO ,TRANMID
      

  Select TRANDNO ,TRANMID 
	    FROM @TableMaster
        
END

