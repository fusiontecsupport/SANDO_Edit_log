/*

DECLARE @total INT
        @filtered INT

EXEC [dbo].[pr_SearchPerson] NULL, 1, 'ASC', 1, 10, @TotalRowsCount= @total OUTPUT,@FilteredRowsCount= @filtered OUTPUT

SELECT @total, @filtered
*/

CREATE PROCEDURE [dbo].[pr_SearchBondProductGroup]  /* CHANGE */
 @FilterTerm nvarchar(250) = NULL --parameter to search all columns by
        , @SortIndex INT = 1 -- a one based index to indicate which column to order by
        , @SortDirection CHAR(4) = 'ASC' --the direction to sort in, either ASC or DESC
        , @StartRowNum INT = 1 --the first row to return
        , @EndRowNum INT = 10 --the last row to return
        , @TotalRowsCount INT OUTPUT
        , @FilteredRowsCount INT OUTPUT
 
AS BEGIN
    --Wrap filter term with % to search for values that contain @FilterTerm
    SET @FilterTerm = '%' + @FilterTerm + '%'

    DECLARE @TableMaster TABLE
    ( 
	PRDTGCODE VARCHAR(100)
      ,PRDTGDESC VARCHAR(100)
	  ,DISPSTATUS smallint
	  , PRDTGID int 
      , RowNum INT
    )

    INSERT INTO @TableMaster(PRDTGCODE,
PRDTGDESC,DISPSTATUS,PRDTGID, RowNum)
    SELECT  PRDTGCODE
            , PRDTGDESC,DISPSTATUS
            , PRDTGID
            , Row_Number() OVER (
            ORDER BY
            
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
                  WHEN 0 THEN PRDTGCODE
                  WHEN 1 THEN PRDTGDESC
	 
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 0 THEN PRDTGCODE
                  WHEN 1 THEN PRDTGDESC				
               
                END
            END DESC 
            
            /*DATETIME ORDER BY*/
             

            
            ) AS RowNum
    FROM    dbo.BondProductGROUPMASTER  /* CHANGE  TABLE NAME */
    WHERE   @FilterTerm IS NULL 
              OR PRDTGCODE LIKE @FilterTerm
              OR PRDTGDESC LIKE @FilterTerm
            

    SELECT PRDTGCODE
            , PRDTGDESC
			,
			  CASE DISPSTATUS WHEN 1 THEN 'Disabled' WHEN 0 THEN 'Enabled' END  AS DISPSTATUS
			 ,PRDTGID
    FROM    @TableMaster
    WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum
    
    SELECT @TotalRowsCount = COUNT(*)
    FROM   dbo.BondProductGROUPMASTER   /* CHANGE  TABLE NAME */
    
    SELECT @FilteredRowsCount = COUNT(*)
    FROM   @TableMaster
        
END
