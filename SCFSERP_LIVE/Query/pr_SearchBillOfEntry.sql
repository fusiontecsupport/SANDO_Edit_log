/*

DECLARE @total INT
        @filtered INT

EXEC [dbo].[pr_SearchPerson] NULL, 1, 'ASC', 1, 10, @TotalRowsCount= @total OUTPUT,@FilteredRowsCount= @filtered OUTPUT

SELECT @total, @filtered
*/

ALTER PROCEDURE [dbo].[pr_SearchBillOfEntry]  /* CHANGE */
 @FilterTerm nvarchar(250) = NULL --parameter to search all columns by
        , @SortIndex INT = 1 -- a one based index to indicate which column to order by
        , @SortDirection CHAR(4) = 'ASC' --the direction to sort in, either ASC or DESC
        , @StartRowNum INT = 1 --the first row to return
        , @EndRowNum INT = 10 --the last row to return
        , @TotalRowsCount INT OUTPUT
        , @FilteredRowsCount INT OUTPUT
        , @PSDate Smalldatetime
        , @PEDate Smalldatetime
		,@PCompyid int
 
AS BEGIN
    --Wrap filter term with % to search for values that contain @FilterTerm
    SET @FilterTerm = '%' + @FilterTerm + '%'

	declare @LCompyid int
	set @LCompyid=@PCompyid
    DECLARE @TableMaster TABLE
    (
      BILLEMTIME SMALLDATETIME
      , BILLEMNO INT
      , BILLEMDNO VARCHAR(15)
      , BILLEMNAME VARCHAR(100)
	  ,DISPSTATUS SMALLINT,NOC int
	  , BILLEMID int 
      , RowNum INT
    )

    INSERT INTO @TableMaster(BILLEMTIME
                        , BILLEMNO
                        , BILLEMDNO
      , BILLEMNAME,DISPSTATUS,NOC
	 ,BILLEMID, RowNum)
  
    SELECT  BILLEMTIME, BILLEMNO , BILLEMDNO
	 
      , BILLEMNAME ,DISPSTATUS
	  , 0
	  , BILLEMID , Row_Number() OVER (
            ORDER BY
            
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
				
           	      WHEN 2 then BILLEMDNO
			  	 
				   WHEN 3 then BILLEMNAME
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex

           	     WHEN 2 then BILLEMDNO
				   WHEN 3 then BILLEMNAME
                END
            END DESC,
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
                  WHEN 0 THEN BILLEMTIME
				  
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 0 THEN BILLEMTIME
				 
                END
            END DESC,
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
				    WHEN 1 then BILLEMNO
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
				  WHEN 1 then BILLEMNO
                END
            END DESC                            

            ) AS RowNum
   FROM         dbo.BILLENTRYMASTER  /* CHANGE  TABLE NAME */
    WHERE   (BILLEMDATE BETWEEN @PSDate AND @PEDate) and (COMPYID=@LCompyid) AND (@FilterTerm IS NULL 
              OR BILLEMTIME LIKE @FilterTerm
              OR BILLEMNO LIKE @FilterTerm
              OR  BILLEMDNO  LIKE @FilterTerm
              OR  BILLEMNAME  LIKE @FilterTerm)

	DECLARE @TBLDTL TABLE(BILLEMNO INT, NOC INT)

	INSERT INTO @TBLDTL
	SELECT DBO.BILLENTRYMASTER.BILLEMNO, COUNT('*') 
	FROM BILLENTRYMASTER LEFT JOIN 
		BILLENTRYDETAIL ON BILLENTRYMASTER.BILLEMID = BILLENTRYDETAIL.BILLEMID
	WHERE   (BILLEMDATE BETWEEN @PSDATE AND @PEDATE) AND (COMPYID=@LCOMPYID) 
	GROUP BY DBO.BILLENTRYMASTER.BILLEMNO

	UPDATE A
	SET NOC = ISNULL(B.NOC,0)
	FROM @TableMaster A , @TBLDTL B
	WHERE A.BILLEMID = B.BILLEMNO

    SELECT BILLEMTIME, BILLEMNO , BILLEMDNO
	 
      , BILLEMNAME 
	  ,CASE DISPSTATUS WHEN 1 THEN 'C' END AS DISPSTATUS,NOC
	  , BILLEMID
    FROM    @TableMaster
    WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum

	

    SELECT @TotalRowsCount = COUNT(*)
    FROM         dbo.BILLENTRYMASTER   /* CHANGE  TABLE NAME */
    WHERE  (BILLEMDATE BETWEEN @PSDate AND @PEDate) and (COMPYID=@LCompyid)
    
    SELECT @FilteredRowsCount = COUNT(*)
    FROM   @TableMaster
        
END


