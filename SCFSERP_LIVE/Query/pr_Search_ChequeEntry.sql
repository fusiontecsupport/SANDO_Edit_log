CREATE PROCEDURE [dbo].[pr_Search_ChequeEntry]  /* CHANGE */
 @FilterTerm nvarchar(250) = NULL --parameter to search all columns by
        , @SortIndex INT = 1 -- a one based index to indicate which column to order by
        , @SortDirection CHAR(4) = 'ASC' --the direction to sort in, either ASC or DESC
        , @StartRowNum INT = 1 --the first row to return
        , @EndRowNum INT = 10 --the last row to return
        , @TotalRowsCount INT OUTPUT
        , @FilteredRowsCount INT OUTPUT
        , @PCompyId INT
		,@PSDPTID INT
		,@PREGSTRID INT
        , @PSDate Smalldatetime
        , @PEDate Smalldatetime
 
AS BEGIN

Declare @LCompyId int,@LSDate Smalldatetime, @LEDate Smalldatetime,@LSDPTID INT,@LREGSTRID INT
set @LCompyId = @PCompyId
set @LSDate = @PSDate
set @LEDate = @PEDate
SET @LSDPTID=@PSDPTID
SET @LREGSTRID=@PREGSTRID

    --Wrap filter term with % to search for values that contain @FilterTerm
    SET @FilterTerm = '%' + @FilterTerm + '%'


    DECLARE @TableMaster TABLE
    (
      TRANDATE SMALLDATETIME
	  ,TRANTIME SMALLDATETIME
    , TRANDNO VARCHAR(50)
    , TRANREFNAME  VARCHAR(100)
	, TRANREFNO VARCHAR(25)
    , TRANREFDATE SMALLDATETIME
	, TRANREFAMT NUMERIC(18,2) 
	, TRANMID int
	, DISPSTATUS int
    , RowNum INT
    )
    INSERT INTO @TableMaster(TRANDATE,TRANTIME, TRANDNO, TRANREFNAME, TRANREFNO, TRANREFDATE, TRANREFAMT, TRANMID, DISPSTATUS, RowNum)
  
    SELECT TRANDATE,TRANTIME, TRANDNO, TRANREFNAME, TRANREFNO, TRANREFDATE, TRANREFAMT, TRANMID, DISPSTATUS
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
					 WHEN 5 then TRANREFDATE
                   END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 0 THEN TRANDATE
				  WHEN 1 THEN TRANTIME
				  WHEN 5 then TRANREFDATE
                END
            END DESC
			/*,
			  CASE @SortDirection
                 WHEN 'ASC'  THEN
                      CASE @SortIndex
           				  WHEN 6 then TRANREFAMT
			  			 
						  
					   END             
				 END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                   CASE @SortIndex
           			    WHEN 6 then TRANREFAMT
                    END
               END DESC*/
            
            ) AS RowNum
            
FROM TRANSACTIONMASTER
WHERE   (COMPYID = @LCompyId)and (REGSTRID=@LREGSTRID) AND (SDPTID=@LSDPTID) AND
(TRANDATE BETWEEN @PSDate AND @PEDate)  AND (@FilterTerm IS NULL 
             OR TRANDATE LIKE @FilterTerm
			  OR TRANTIME LIKE @FilterTerm
              OR TRANDNO LIKE @FilterTerm
              OR TRANREFNAME  LIKE @FilterTerm
              oR TRANREFNO LIKE @FilterTerm
              OR TRANREFDATE    LIKE @FilterTerm)
			   
ORDER BY TRANDNO
            
    SELECT TRANDATE,TRANTIME, TRANDNO, TRANREFNAME, TRANREFNO, TRANREFDATE, TRANREFAMT, TRANMID, 
	CASE DISPSTATUS WHEN 1 THEN 'C' WHEN 2 THEN 'RETURN' else '' end as DISPSTATUS
    FROM    @TableMaster
    WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum
    ORDER BY TRANDNO DESC

SELECT    @TotalRowsCount = COUNT(*)
FROM  TRANSACTIONMASTER
WHERE (COMPYID = @LCompyId) and (REGSTRID=@LREGSTRID) AND (SDPTID=@LSDPTID) AND(TRANDATE BETWEEN @LSDate AND @LEDate)
    SELECT @FilteredRowsCount = COUNT(*)
    FROM   @TableMaster
        
END

