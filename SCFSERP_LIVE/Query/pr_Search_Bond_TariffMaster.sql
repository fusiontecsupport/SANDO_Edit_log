/*

DECLARE @total INT
        @filtered INT

EXEC [dbo].[pr_Search_Bond_TariffMaster] NULL, 1, 'ASC', 1, 10, @TotalRowsCount= @total OUTPUT,@FilteredRowsCount= @filtered OUTPUT

SELECT @total, @filtered
*/

CREATE PROCEDURE [dbo].[pr_Search_Bond_TariffMaster]  /* CHANGE */
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
	   TARIFFMCODE VARCHAR(100)
	   ,TARIFFMDESC VARCHAR(100)
	   ,DISPSTATUS smallint 
	   ,TARIFFMID int 
	   ,RowNum INT
    )

    INSERT INTO @TableMaster(TARIFFMCODE,
TARIFFMDESC,DISPSTATUS,TARIFFMID, RowNum)
    SELECT  TARIFFMCODE,
             TARIFFMDESC,BONDTARIFFMASTER.DISPSTATUS
            , TARIFFMID
            , Row_Number() OVER (
            ORDER BY
            
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
                  WHEN 0 THEN TARIFFMCODE
                  WHEN 1 THEN TARIFFMDESC
	 
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 0 THEN TARIFFMCODE
                  WHEN 1 THEN TARIFFMDESC				
               
                END
            END DESC 
            
            /*DATETIME ORDER BY*/
             

            
            ) AS RowNum
    FROM    dbo.BONDTARIFFMASTER (nolock)
	
    WHERE   @FilterTerm IS NULL 
              OR TARIFFMCODE LIKE @FilterTerm
              OR TARIFFMDESC LIKE @FilterTerm
            

    SELECT TARIFFMCODE
            , TARIFFMDESC
			,
			  CASE DISPSTATUS WHEN 1 THEN 'Disabled' WHEN 0 THEN 'Enabled' END  AS DISPSTATUS
			 ,TARIFFMID
    FROM    @TableMaster
    WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum
    
    SELECT @TotalRowsCount = COUNT(*)
    FROM   dbo.BONDTARIFFMASTER (nolock)   /* CHANGE  TABLE NAME */
    
    SELECT @FilteredRowsCount = COUNT(*)
    FROM   @TableMaster
        
END

