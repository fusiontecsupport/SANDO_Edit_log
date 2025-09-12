ALTER PROCEDURE [dbo].[pr_Search_BondPerformaInvoice]  /* CHANGE */
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
		, @PTAXTYPE int=1
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
    , TRANDNO  VARCHAR(25)
    , TRANNO  int
    , TRANREFNAME VARCHAR(100)
	, ACKNO VARCHAR(50)
	, TRANNAMT numeric(18,2)
	,DISPSTATUS smallint
	,GSTAMT smallint
	, TRANBTYPE smallint
	, TRANMID INT
    , RowNum INT
    )
    INSERT INTO @TableMaster(TRANDATE, TRANDNO , TRANNO,TRANREFNAME,ACKNO,TRANNAMT,DISPSTATUS, GSTAMT,TRANBTYPE,TRANMID, RowNum)
  
    SELECT   TRANDATE,   
   TRANDNO, TRANNO, TRANREFNAME, ACKNO ,TRANNAMT,DISPSTATUS, (CASE WHEN len(IRNNO) > 0 THEN 1 ELSE 0 END),TRANBTYPE, TRANMID

            , Row_Number() OVER (
            ORDER BY
            
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/
            
            CASE @SortDirection
                 WHEN 'ASC'  THEN
                      CASE @SortIndex
           				
			  			  WHEN 2 then TRANDNO
			  			  WHEN 3 then TRANREFNAME
						  WHEN 4 then ACKNO
						  WHEN 5 then TRANNAMT
					   END             
				 END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                   CASE @SortIndex
           			  
			  		      WHEN 2 then TRANDNO
			  			  WHEN 3 then TRANREFNAME
						  WHEN 4 then ACKNO
						  WHEN 5 then TRANNAMT
                    END
               END DESC,
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                   CASE @SortIndex
                     WHEN 0 THEN TRANDATE
                   END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 0 THEN TRANDATE
                END
            END DESC,
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                   CASE @SortIndex
                     WHEN 1 THEN TRANNO
                   END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 1 THEN TRANNO
                END
            END DESC            
            ) AS RowNum
            
FROM       BONDPERFORMATRANSACTIONMASTER (NOLOCK)
WHERE (dbo.BONDPERFORMATRANSACTIONMASTER.COMPYID = @LCompyId) and TRANTID = 2 
--AND TAXTYPE = @PTAXTYPE 
AND (TRANDATE BETWEEN @LSDate AND @LEDate)
--and (BONDPERFORMATRANSACTIONMASTER.REGSTRID=@LREGSTRID) 
--and (BONDPERFORMATRANSACTIONMASTER.TRANBTYPE IN(1, 2, 3, 4, 5)) and
  AND (@FilterTerm IS NULL 
             OR TRANDATE LIKE @FilterTerm
              OR TRANDNO  LIKE @FilterTerm
              OR TRANNO    LIKE @FilterTerm
			  OR ACKNO    LIKE @FilterTerm
              oR TRANREFNAME LIKE @FilterTerm
              OR TRANNAMT    LIKE @FilterTerm
             )
			   
            
    SELECT TRANDATE, TRANDNO , TRANNO,TRANREFNAME,ACKNO,TRANNAMT,
	      case DISPSTATUS when 0 then '' when 1 then 'C' end as DISPSTATUS, GSTAMT, TRANBTYPE,TRANMID
    FROM    @TableMaster
    WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum
    ORDER BY TRANNO DESC 

SELECT    @TotalRowsCount = COUNT(*)
FROM       BONDPERFORMATRANSACTIONMASTER (nolock)

WHERE (dbo.BONDPERFORMATRANSACTIONMASTER.COMPYID = @LCompyId) and TRANTID = 2 
--AND TAXTYPE = @PTAXTYPE 
AND (TRANDATE BETWEEN @LSDate AND @LEDate)

    
    SELECT @FilteredRowsCount = COUNT(*)
    FROM   @TableMaster
        
END


