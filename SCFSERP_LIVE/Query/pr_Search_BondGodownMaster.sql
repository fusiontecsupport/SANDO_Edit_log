/*

DECLARE @total INT,
        @filtered INT

EXEC [dbo].[pr_Search_BondGodownMaster] NULL, 1, 'ASC', 1, 10, @TotalRowsCount= @total OUTPUT,@FilteredRowsCount= @filtered OUTPUT

SELECT @total, @filtered
*/

alter PROCEDURE [dbo].[pr_Search_BondGodownMaster]  /* CHANGE */
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
	GWNCODE VARCHAR(100)
      ,GWNDESC VARCHAR(100)
	  ,GWNTYPDESC VARCHAR(100)
	  ,DISPSTATUS smallint
	  , GWNID int 
      , RowNum INT
    )

    INSERT INTO @TableMaster(GWNCODE,
GWNDESC,GWNTYPDESC,DISPSTATUS,GWNID, RowNum)
    SELECT  GWNCODE
            , GWNDESC,GWNTDESC, BondGodownMASTER.DISPSTATUS
            , GWNID
            , Row_Number() OVER (
            ORDER BY
            
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
                  WHEN 0 THEN GWNCODE
                  WHEN 1 THEN GWNDESC
	 
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 0 THEN GWNCODE
                  WHEN 1 THEN GWNDESC				
               
                END
            END DESC 
            
            /*DATETIME ORDER BY*/
             

            
            ) AS RowNum
    FROM    dbo.BondGodownMASTER  (nolock) left join dbo.BONDGODOWNTYPEMASTER (nolock) on 
	BondGodownMASTER.GWNTID = BONDGODOWNTYPEMASTER.GWNTID
    WHERE   @FilterTerm IS NULL 
              OR GWNCODE LIKE @FilterTerm
              OR GWNDESC LIKE @FilterTerm
            

    SELECT GWNCODE
            , GWNDESC
			,GWNTYPDESC,
			  CASE DISPSTATUS WHEN 1 THEN 'Disabled' WHEN 0 THEN 'Enabled' END  AS DISPSTATUS
			 ,GWNID
    FROM    @TableMaster
    WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum
    
    SELECT @TotalRowsCount = COUNT(*)
    FROM   dbo.BondGodownMASTER (nolock) left join dbo.BONDGODOWNTYPEMASTER (nolock) on 
	BondGodownMASTER.GWNTID = BONDGODOWNTYPEMASTER.GWNTID
	/* CHANGE  TABLE NAME */
    
    SELECT @FilteredRowsCount = COUNT(*)
    FROM   @TableMaster
        
END

