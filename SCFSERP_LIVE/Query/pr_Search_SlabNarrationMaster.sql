USE [SCFS_ERP]
GO
/****** Object:  StoredProcedure [dbo].[pr_Search_SlabNarrationMaster]    Script Date: 26/10/2021 18:31:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*

DECLARE @total INT,
        @filtered INT

EXEC [dbo].[pr_Search_SlabNarrationMaster] @FilterTerm='',@SortIndex= 1, @SortDirection='DESC',@StartRowNum= 1,@EndRowNum= 10, 
 @TotalRowsCount =100, @FilteredRowsCount =1000,@PTARIFFMID=115,@PCHRGETYPE=1,@PSLABTID=16

SELECT @total, @filtered
*/

ALTER PROCEDURE [dbo].[pr_Search_SlabNarrationMaster]  /* CHANGE */
 @FilterTerm nvarchar(250) = NULL --parameter to search all columns by
        , @SortIndex INT = 1 -- a one based index to indicate which column to order by
        , @SortDirection CHAR(4) = 'ASC' --the direction to sort in, either ASC or DESC
        , @StartRowNum INT = 1 --the first row to return
        , @EndRowNum INT = 10 --the last row to return
        , @TotalRowsCount INT OUTPUT
        , @FilteredRowsCount INT OUTPUT
		, @PTARIFFMID INT
		, @PCHRGETYPE INT
		, @PSLABTID INT
		
 
AS BEGIN
    --Wrap filter term with % to search for values that contain @FilterTerm
    SET @FilterTerm = '%' + @FilterTerm + '%'
	declare @LTARIFFMID INT
		,@LCHRGETYPE INT
		,@LSLABTID INT

		set @LTARIFFMID=@PTARIFFMID
		set @LCHRGETYPE=@PCHRGETYPE
		set @LSLABTID=@PSLABTID

    DECLARE @TableMaster TABLE
    ( 
	    PRCSDATE smalldatetime
      , TARIFFMID smallint 
	  , SLABTID smallint
	  , BILLTID smallint
	  , SLABTYPE nvarchar(150) 	  
	  , TARIFF nvarchar(150)	 
	  , SLABNARTN varchar(250)	 
	  , SLABNID int 
      , RowNum INT
    )

    INSERT INTO @TableMaster(PRCSDATE
	   ,TARIFFMID,SLABTID,BILLTID 
	  ,SLABTYPE 
	  ,TARIFF ,SLABNARTN
	  ,SLABNID, RowNum)
    SELECT  SLABNARRATIONMASTER.PRCSDATE,
		SLABNARRATIONMASTER.TARIFFMID,SLABNARRATIONMASTER.SLABTID,SLABNARRATIONMASTER.BILLTID 
	  ,SLABTYPEMASTER.SLABTDESC 
	  ,TARIFFMASTER.TARIFFMDESC
	  ,SLABNARRATIONMASTER.SLABNARTN
	  , SLABNARRATIONMASTER.SLABNID
            , Row_Number() OVER (
            ORDER BY
            
            /*DATETIME ORDER BY*/
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
                  WHEN 0 THEN SLABNARRATIONMASTER.PRCSDATE
                 
	 
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 0 THEN SLABNARRATIONMASTER.PRCSDATE
                 
               
                END
            END DESC, 
            
            /*VARCHAR ORDER BY*/
              CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex                
                  
				    WHEN 1 THEN TARIFFMASTER.TARIFFMDESC
	              WHEN 2 THEN SLABTYPEMASTER.SLABTDESC	            
				 WHEN 3 THEN SLABNARRATIONMASTER.SLABNARTN

                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex                 
                   WHEN 1 THEN TARIFFMASTER.TARIFFMDESC
	              WHEN 2 THEN SLABTYPEMASTER.SLABTDESC
				  WHEN 3 THEN SLABNARRATIONMASTER.SLABNARTN
                END
            END DESC

			    
            
            ) AS RowNum

  FROM  SLABNARRATIONMASTER (nolock)  
        INNER JOIN  TARIFFMASTER (nolock) On SLABNARRATIONMASTER.TARIFFMID = TARIFFMASTER.TARIFFMID 
		INNER JOIN  SLABTYPEMASTER (nolock) On SLABNARRATIONMASTER.SLABTID = SLABTYPEMASTER.SLABTID  
                     
   WHERE  (SLABNARRATIONMASTER.SLABTID = @LSLABTID) AND  (SLABNARRATIONMASTER.TARIFFMID=@LTARIFFMID)  AND (SLABNARRATIONMASTER.BILLTID=@LCHRGETYPE) 
	and (@FilterTerm IS NULL 
              OR SLABNARRATIONMASTER.PRCSDATE LIKE @FilterTerm               
			  OR SLABNARRATIONMASTER.BILLTID LIKE @FilterTerm  
			  OR SLABTYPEMASTER.SLABTDESC LIKE @FilterTerm  
              OR TARIFFMASTER.TARIFFMDESC LIKE @FilterTerm  
			  OR SLABNARRATIONMASTER.SLABTID LIKE @FilterTerm  
              OR SLABNARRATIONMASTER.TARIFFMID LIKE @FilterTerm  
			  OR SLABNARRATIONMASTER.SLABNARTN LIKE @FilterTerm  
			 )
    SELECT Convert(varchar(10),PRCSDATE,103) as PRCSDATE
	       , TARIFFMID , SLABTID , BILLTID, SLABTYPE , TARIFF ,CASE BILLTID WHEN 1 THEN 'LD'  WHEN 2 THEN 'DS' END  AS CHRGETYPE  ,SLABNARTN
		   , SLABNID  

	         
            
			
    FROM    @TableMaster
     WHERE   
       RowNum BETWEEN @StartRowNum AND @EndRowNum
	   ORDER BY PRCSDATE DESC, SLABNID 
    
	SELECT @TotalRowsCount = COUNT(*)
    
  FROM SLABNARRATIONMASTER (nolock)  
        INNER JOIN  TARIFFMASTER (nolock) On SLABNARRATIONMASTER.TARIFFMID = TARIFFMASTER.TARIFFMID 
		INNER JOIN  SLABTYPEMASTER (nolock) On SLABNARRATIONMASTER.SLABTID = SLABTYPEMASTER.SLABTID  
   WHERE
	 (SLABNARRATIONMASTER.SLABTID = @LSLABTID) AND  (SLABNARRATIONMASTER.TARIFFMID=@LTARIFFMID)  AND (SLABNARRATIONMASTER.BILLTID=@LCHRGETYPE) 
    
    SELECT @FilteredRowsCount = COUNT(*)
    FROM   @TableMaster
        
END








