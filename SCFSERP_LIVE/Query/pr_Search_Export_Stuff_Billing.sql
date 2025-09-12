/*
DECLARE @total INT,
        @filtered INT

EXEC [dbo].[pr_Search_Export_Stuff_Billing] NULL, 1, 'ASC', 1, 10, @total OUTPUT, @filtered OUTPUT, 32, 1, 1, '2021-01-01', '2021-07-1'

SELECT @total, @filtered
*/
alter PROCEDURE [dbo].[pr_Search_Export_Stuff_Billing]  /* CHANGE */
 @FilterTerm nvarchar(250) = NULL --parameter to search all columns by
        , @SortIndex INT = 1 -- a one based index to indicate which column to order by
        , @SortDirection CHAR(4) = 'ASC' --the direction to sort in, either ASC or DESC
        , @StartRowNum INT = 1 --the first row to return
        , @EndRowNum INT = 10 --the last row to return
        , @TotalRowsCount INT OUTPUT
        , @FilteredRowsCount INT OUTPUT
        , @PCompyId INT
		,@PTRANBTYPE int
		,@PREGSTRID int
        , @PSDate Smalldatetime
        , @PEDate Smalldatetime
 
AS BEGIN

Declare @LCompyId int,@LTRANBTYPE int,@LREGSTRID int,@LSDate Smalldatetime, @LEDate Smalldatetime
set @LCompyId = @PCompyId
set @LTRANBTYPE=@PTRANBTYPE
set @LREGSTRID=@PREGSTRID
set @LSDate = @PSDate
set @LEDate = @PEDate

    --Wrap filter term with % to search for values that contain @FilterTerm
    SET @FilterTerm = '%' + @FilterTerm + '%'


    DECLARE @TableMaster TABLE
    (
      TRANDATE SMALLDATETIME
    , TRANTIME datetime
    , TRANDNO  VARCHAR(25)
    , TRANNO  int
    , TRANREFNAME VARCHAR(100)
	, NOC int
	, TRANNAMT numeric(18,2)
	,DISPSTATUS smallint
	,GSTAMT smallint
	, TRANMID INT
    , RowNum INT
    )
    INSERT INTO @TableMaster(TRANDATE, TRANTIME, TRANDNO , TRANNO,TRANREFNAME,NOC,TRANNAMT,DISPSTATUS, GSTAMT, TRANMID, RowNum)
  
    SELECT   TRANDATE, TRANTIME,  
   TRANDNO, TRANNO,isnull(STFBILLREFNAME,TRANREFNAME),0, TRANNAMT,DISPSTATUS, (CASE WHEN len(IRNNO) > 0 THEN 1 ELSE 0 END) ,TRANSACTIONMASTER.TRANMID

            , Row_Number() OVER (
            ORDER BY
            
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/
            
            CASE @SortDirection
                 WHEN 'ASC'  THEN
                      CASE @SortIndex
           				
			  			  WHEN 2 then TRANDNO
					      WHEN 3 then TRANNO
			  			  WHEN 4 then isnull(STFBILLREFNAME,TRANREFNAME)
						  WHEN 6 then TRANNAMT
					   END             
				 END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                   CASE @SortIndex
           			  
			  		    WHEN 2 then TRANDNO
					     WHEN 3 then TRANNO
			  			  WHEN 4 then isnull(STFBILLREFNAME,TRANREFNAME)
						  WHEN 6 then TRANNAMT
                    END
               END DESC,
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                   CASE @SortIndex
                     WHEN 0 THEN TRANDATE
					 WHEN 1 then TRANTIME
                   END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 0 THEN TRANDATE
				  WHEN 1 then TRANTIME
                END
            END DESC
            
            ) AS RowNum
            
FROM       TRANSACTIONMASTER LEFT OUTER JOIN
                         dbo.VW_EXPORT_STUFF_INVOICE_PRINT_08 ON dbo.TRANSACTIONMASTER.TRANMID = dbo.VW_EXPORT_STUFF_INVOICE_PRINT_08.TRANMID
WHERE (dbo.TRANSACTIONMASTER.COMPYID = @LCompyId) and (TRANSACTIONMASTER.REGSTRID=@LREGSTRID) and (TRANSACTIONMASTER.TRANBTYPE=@LTRANBTYPE) and
(TRANDATE BETWEEN @LSDate AND @LEDate) AND (TRANSACTIONMASTER.SDPTID=2) AND (@FilterTerm IS NULL 
             OR TRANDATE LIKE @FilterTerm
              OR TRANTIME LIKE @FilterTerm
              OR TRANDNO  LIKE @FilterTerm
              OR TRANNO    LIKE @FilterTerm
              oR ISNULL(STFBILLREFNAME,TRANREFNAME) LIKE @FilterTerm
              OR TRANNAMT    LIKE @FilterTerm
             )
			   
            
    SELECT TRANDATE, TRANTIME, TRANDNO , TRANNO,TRANREFNAME,NOC,TRANNAMT,case DISPSTATUS when 0 then '' when 1 then 'C' end as DISPSTATUS, GSTAMT, TRANMID, '' 'ACKNO'
    FROM    @TableMaster
    WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum
    ORDER BY TRANNO DESC 

SELECT    @TotalRowsCount = COUNT(*)
FROM       TRANSACTIONMASTER

WHERE (dbo.TRANSACTIONMASTER.COMPYID = @LCompyId) and (TRANSACTIONMASTER.REGSTRID=@LREGSTRID) and (TRANSACTIONMASTER.TRANBTYPE=@LTRANBTYPE) and
(TRANDATE BETWEEN @LSDate AND @LEDate) AND (TRANSACTIONMASTER.SDPTID=2)

    
    SELECT @FilteredRowsCount = COUNT(*)
    FROM   @TableMaster
        
END