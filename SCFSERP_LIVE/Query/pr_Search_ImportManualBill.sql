alter PROCEDURE [dbo].[pr_Search_ImportManualBill]  /* CHANGE */
 @FilterTerm nvarchar(250) = NULL --parameter to search all columns by
        , @SortIndex INT = 1 -- a one based index to indicate which column to order by
        , @SortDirection CHAR(4) = 'ASC' --the direction to sort in, either ASC or DESC
        , @StartRowNum INT = 1 --the first row to return
        , @EndRowNum INT = 10 --the last row to return
        , @TotalRowsCount INT OUTPUT
        , @FilteredRowsCount INT OUTPUT
        , @PCompyId INT,@PREGSTRID int,@PTRANBTYPE int
        , @PSDate Smalldatetime
        , @PEDate Smalldatetime
 
AS BEGIN

Declare @LCompyId int,@LSDate Smalldatetime, @LEDate Smalldatetime,@LREGSTRID int,@LTRANBTYPE int
set @LCompyId = @PCompyId
set @LSDate = @PSDate
set @LEDate = @PEDate
set @LREGSTRID=@PREGSTRID
set @LTRANBTYPE=@PTRANBTYPE

    --Wrap filter term with % to search for values that contain @FilterTerm
    SET @FilterTerm = '%' + @FilterTerm + '%'


    DECLARE @TableMaster TABLE
    (
      TRANDATE SMALLDATETIME
	  ,TRANTIME SMALLDATETIME
    , TRANDNO VARCHAR(50)
    , TRANREFNAME  VARCHAR(100)
	, TRANNAMT NUMERIC(18,2) 
	, TRANMID int
	, GSTAMT NUMERIC(18,2) 
	, DISPSTATUS int
	, TAXTYPE int
	, ACKNO varchar(50)
    , RowNum INT
    )
    INSERT INTO @TableMaster(TRANDATE,TRANTIME, TRANDNO, TRANREFNAME, TRANNAMT, TRANMID, GSTAMT, DISPSTATUS, TAXTYPE, ACKNO, RowNum)
  
    SELECT TRANDATE,TRANTIME, TRANDNO, TRANREFNAME, TRANNAMT, TRANMID, TRANCGSTAMT+TRANSGSTAMT+TRANIGSTAMT, DISPSTATUS, 
			case when regstrid = 15 then 1 else 0 end,ACKNO
            , Row_Number() OVER (
            ORDER BY
            
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/
            
            CASE @SortDirection
                 WHEN 'ASC'  THEN
                      CASE @SortIndex
           				  WHEN 2 then TRANDNO
			  			  WHEN 3 then TRANREFNAME
			  			  WHEN 4 then TRANREFNO
						  
					   END             
				 END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                   CASE @SortIndex
           			    WHEN 2 then TRANDNO
			  		    WHEN 3 then TRANREFNAME
			  			WHEN 4 then TRANREFNO
						
                    END
               END DESC,
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                   CASE @SortIndex
                     WHEN 0 THEN TRANDATE
					 WHEN 1 THEN TRANTIME
                   END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 0 THEN TRANDATE
				  WHEN 1 THEN TRANTIME
                END
            END DESC
			/*,
			  CASE @SortDirection
                 WHEN 'ASC'  THEN
                      CASE @SortIndex
           				  WHEN 6 then TRANNAMT
			  			 
						  
					   END             
				 END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                   CASE @SortIndex
           			    WHEN 6 then TRANNAMT
                    END
               END DESC*/
            
            ) AS RowNum
            
FROM TRANSACTIONMASTER
WHERE   (COMPYID = @LCompyId) and (REGSTRID=@LREGSTRID) AND  (TRANBTYPE=@LTRANBTYPE) AND
(TRANDATE BETWEEN @PSDate AND @PEDate)  AND (@FilterTerm IS NULL 
             OR TRANDATE LIKE @FilterTerm
			  OR TRANTIME LIKE @FilterTerm
              OR TRANDNO LIKE @FilterTerm
              OR TRANREFNAME  LIKE @FilterTerm
			  OR TRANNAMT LIKE @FilterTerm)
			   
ORDER BY TRANDNO
            
    SELECT TRANDATE,TRANTIME, TRANDNO, TRANREFNAME, TRANNAMT, TRANMID, 
	CASE DISPSTATUS WHEN 1 THEN 'C' WHEN 2 THEN 'RETURN' else '' end as DISPSTATUS, GSTAMT, ACKNO
    FROM    @TableMaster
    WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum
    ORDER BY TRANDNO DESC

SELECT    @TotalRowsCount = COUNT(*)
FROM  TRANSACTIONMASTER
WHERE (COMPYID = @LCompyId) and (REGSTRID=@LREGSTRID) 
AND (TRANBTYPE=@LTRANBTYPE) 
AND(TRANDATE BETWEEN @LSDate AND @LEDate)
    SELECT @FilteredRowsCount = COUNT(*)
    FROM   @TableMaster
        
END



