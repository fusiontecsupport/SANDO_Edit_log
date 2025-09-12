-- =============================================
-- Author:		<>
-- Create date: <>
-- Modified date: <31/07/2021>
-- EXEC pr_Export_Invoice_Stuff_Flx_Assgn @PSTFMID=14,@PEDATE='2021-08-23',@PTRANMID=0
-- Description:	<GET Export Stuffing Details>
-- =============================================

alter PROCEDURE [dbo].[pr_Search_Export_Stuffing_Master]  /* CHANGE */
 @FilterTerm nvarchar(250) = NULL --parameter to search all columns by
        , @SortIndex INT = 1 -- a one based index to indicate which column to order by
        , @SortDirection CHAR(4) = 'ASC' --the direction to sort in, either ASC or DESC
        , @StartRowNum INT = 1 --the first row to return
        , @EndRowNum INT = 10 --the last row to return
        , @TotalRowsCount INT OUTPUT
        , @FilteredRowsCount INT OUTPUT
        , @PCompyId INT
        , @PSTFTId int
        , @PSDate Smalldatetime
        , @PEDate Smalldatetime
 
AS BEGIN

Declare @LCompyId int,@LSDate Smalldatetime, @LEDate Smalldatetime,@LSTFTId int
set @LCompyId = @PCompyId
set @LSTFTId = @PSTFTId
set @LSDate = @PSDate
set @LEDate = @PEDate

    --Wrap filter term with % to search for values that contain @FilterTerm
    SET @FilterTerm = '%' + @FilterTerm + '%'


     DECLARE @TableMaster TABLE
    (
      STFMDATE SMALLDATETIME
    , STFMDNO VARCHAR(15)
    , STFMNAME VARCHAR(100)
    , STFDSBDNO VARCHAR(25)
	, STFCORDNO VARCHAR(25)
    , CONTNRNO  VARCHAR(15)
    , STFDNOP  numeric(18,2)	
	, STFDID int
	, STFMID  INT
	, DISPSTATUS int	
    , RowNum INT
    )
    INSERT INTO @TableMaster(STFMDATE, STFMDNO, STFMNAME, STFDSBDNO, STFCORDNO, CONTNRNO, STFDNOP, STFDID, STFMID, DISPSTATUS, RowNum)
  
SELECT  dbo.STUFFINGMASTER.STFMDATE, dbo.STUFFINGMASTER.STFMDNO, dbo.STUFFINGMASTER.STFMNAME, 
        dbo.STUFFINGPRODUCTDETAIL.STFDSBDNO, dbo.STUFFINGPRODUCTDETAIL.STFCORDNO, 
        dbo.GATEINDETAIL.CONTNRNO, SUM(dbo.STUFFINGPRODUCTDETAIL.STFDNOP) AS STFDNOP,
        dbo.STUFFINGDETAIL.STFDID, dbo.STUFFINGDETAIL.STFMID,dbo.STUFFINGMASTER.DISPSTATUS
		
            , Row_Number() OVER (
            ORDER BY
            
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/
            
            CASE @SortDirection
                 WHEN 'ASC'  THEN
                      CASE @SortIndex
           				  WHEN 1 then STFMDNO
			  			  WHEN 2 then STFMNAME
					      WHEN 3 then STFDSBDNO
			  			  WHEN 4 then STFCORDNO
						  WHEN 5 then CONTNRNO
					   END             
				 END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                   CASE @SortIndex
           			    WHEN 1 then STFMDNO
			  		    WHEN 2 then STFMNAME
					    WHEN 3 then STFDSBDNO
			  			WHEN 4 then STFCORDNO
						WHEN 5 then CONTNRNO
                    END
               END DESC,
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                   CASE @SortIndex
                     WHEN 0 THEN STFMDATE
                   END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 0 THEN STFMDATE
                END
            END DESC            
            ) AS RowNum FROM dbo.STUFFINGMASTER INNER JOIN 
			dbo.STUFFINGDETAIL ON dbo.STUFFINGMASTER.STFMID = dbo.STUFFINGDETAIL.STFMID INNER JOIN 
			dbo.STUFFINGPRODUCTDETAIL ON dbo.STUFFINGDETAIL.STFDID = dbo.STUFFINGPRODUCTDETAIL.STFDID LEFT OUTER JOIN 
			dbo.GATEINDETAIL ON dbo.STUFFINGDETAIL.GIDID = dbo.GATEINDETAIL.GIDID RIGHT OUTER JOIN 
			dbo.CONTAINERSIZEMASTER  ON dbo.CONTAINERSIZEMASTER.CONTNRSID = dbo.GATEINDETAIL.CONTNRSID  			
WHERE (dbo.STUFFINGMASTER.STFTID = @LSTFTID) AND  (dbo.STUFFINGMASTER.COMPYID = @LCompyId) and
(STFMDATE BETWEEN @LSDate AND @LEDate)AND (@FilterTerm IS NULL 
             OR STFMDATE LIKE @FilterTerm
              OR STFMDNO LIKE @FilterTerm
              OR STFMNAME  LIKE @FilterTerm
              OR STFDSBDNO    LIKE @FilterTerm
              oR STFCORDNO LIKE @FilterTerm
              OR STFDNOP    LIKE @FilterTerm
              OR CONTNRNO LIKE @FilterTerm)
              
GROUP BY dbo.STUFFINGMASTER.STFMDATE, dbo.STUFFINGMASTER.STFMDNO, dbo.STUFFINGMASTER.STFMNAME, 
        dbo.STUFFINGPRODUCTDETAIL.STFDSBDNO, dbo.STUFFINGPRODUCTDETAIL.STFCORDNO, 
        dbo.GATEINDETAIL.CONTNRNO, 
        dbo.STUFFINGDETAIL.STFDID, dbo.STUFFINGDETAIL.STFMID,dbo.STUFFINGMASTER.DISPSTATUS 
ORDER BY dbo.STUFFINGMASTER.STFMDNO

            
    SELECT STFMDATE, STFMDNO, STFMNAME, STFDSBDNO, STFCORDNO, CONTNRNO, STFDNOP, STFDID, STFMID, CASE DISPSTATUS WHEN 0 THEN '' ELSE 'C' END AS DISPSTATUS 
    FROM    @TableMaster
    WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum
    ORDER BY STFMDNO DESC

SELECT    @TotalRowsCount =  COUNT(dbo.STUFFINGDETAIL.STFDID) 
FROM         dbo.STUFFINGMASTER INNER JOIN
             dbo.STUFFINGDETAIL ON dbo.STUFFINGMASTER.STFMID = dbo.STUFFINGDETAIL.STFMID INNER JOIN  
			 dbo.STUFFINGPRODUCTDETAIL ON dbo.STUFFINGDETAIL.STFDID = dbo.STUFFINGPRODUCTDETAIL.STFDID LEFT OUTER JOIN  
			 dbo.GATEINDETAIL ON dbo.STUFFINGDETAIL.GIDID = dbo.GATEINDETAIL.GIDID RIGHT OUTER JOIN 
			 dbo.CONTAINERSIZEMASTER ON dbo.CONTAINERSIZEMASTER.CONTNRSID = dbo.GATEINDETAIL.CONTNRSID  
                 
WHERE     (dbo.STUFFINGMASTER.STFTID = @LSTFTId) AND (dbo.STUFFINGMASTER.COMPYID = @LCompyId) AND 
(dbo.STUFFINGMASTER.STFMDATE BETWEEN @LSDate AND @LEDate)
    
    SELECT @FilteredRowsCount = COUNT(*)
    FROM   @TableMaster
        
END

-- EXEC pr_Search_Export_Stuffing_Master  @FilterTerm = '',@SortIndex = 1 , @SortDirection = 'ASC', @StartRowNum = 1, @EndRowNum = 10 , @TotalRowsCount =100,@FilteredRowsCount=10,@PCompyId=32,@PSTFTId =1, @PSDate ='2021-01-01',@PEDate='2021-08-31'
-- select getdate()

