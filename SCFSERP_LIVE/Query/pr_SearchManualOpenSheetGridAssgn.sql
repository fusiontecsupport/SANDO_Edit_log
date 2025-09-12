CREATE PROCEDURE [dbo].[pr_SearchManualOpenSheetGridAssgn]  /* CHANGE */
 @FilterTerm nvarchar(250) = NULL --parameter to search all columns by
        , @SortIndex INT = 1 -- a one based index to indicate which column to order by
        , @SortDirection CHAR(4) = 'ASC' --the direction to sort in, either ASC or DESC
        , @StartRowNum INT = 1 --the first row to return
        , @EndRowNum INT = 10 --the last row to return
        , @TotalRowsCount INT OUTPUT
        , @FilteredRowsCount INT OUTPUT
        , @PSDate Smalldatetime
        , @PEDate Smalldatetime
		, @PCompyid int
		, @fcltype int
 
AS BEGIN
    --Wrap filter term with % to search for values that contain @FilterTerm
    SET @FilterTerm = '%' + @FilterTerm + '%'

	declare @LCompyid int
	set @LCompyid=@PCompyid
    DECLARE @TableMaster TABLE
    (
      OSMDATE SMALLDATETIME
      , OSMNO INT
      , OSMDNO VARCHAR(15)
	  ,OSMIGMNO VARCHAR(15)
	  ,OSMLNO VARCHAR(15)
      , OSMNAME VARCHAR(100)
	  ,BOENO VARCHAR(20)
	  ,BOEDATE SMALLDATETIME
	  ,OSMVSLNAME VARCHAR(100)
	  ,DISPSTATUS SMALLINT
	  , OSMID int 
      , RowNum INT
    )

    INSERT INTO @TableMaster(OSMDATE
                        , OSMNO
                        , OSMDNO
	  ,OSMIGMNO 
	  ,OSMLNO 
      , OSMNAME
	   ,BOENO 
	  ,BOEDATE,OSMVSLNAME ,DISPSTATUS
	 ,OSMID, RowNum)
  
    SELECT  OSMDATE, OSMNO , OSMDNO
	  ,OSMIGMNO 
	  ,OSMLNO 
      , OSMNAME 
	  ,BOENO 
	  ,BOEDATE,OSMVSLNAME,DISPSTATUS, OSMID , Row_Number() OVER (
            ORDER BY
            
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
				
           	      WHEN 2 then OSMDNO
			  	 
				   WHEN 3 then OSMIGMNO
			  	  WHEN 4 then OSMLNO
				   WHEN 5 then OSMNAME
				   WHEN 6 then BOENO
				   when 8 then OSMVSLNAME
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex

           	     WHEN 2 then OSMDNO
			  	 
				 WHEN 3 then OSMIGMNO
			  	  WHEN 4 then OSMLNO
				   WHEN 5 then OSMNAME
				     WHEN 6 then BOENO
					 when 8 then OSMVSLNAME
                END
            END DESC,
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
                  WHEN 0 THEN OSMDATE
				   WHEN 7 then BOEDATE
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 0 THEN OSMDATE
				   WHEN 7 then BOEDATE
                END
            END DESC,
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
				    WHEN 1 then OSMNO
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
				  WHEN 1 then OSMNO
                END
            END DESC                            

            ) AS RowNum
   FROM         dbo.MANUALOPENSHEETMASTER  /* CHANGE  TABLE NAME */
    WHERE   (OSMDATE BETWEEN @PSDate AND @PEDate) and (OSMLDTYPE = @fcltype) and (COMPYID=@LCompyid) AND (@FilterTerm IS NULL 
              OR OSMDATE LIKE @FilterTerm
              OR OSMNO LIKE @FilterTerm
              OR  OSMDNO  LIKE @FilterTerm
              OR  OSMIGMNO    LIKE @FilterTerm
			   OR OSMLNO LIKE @FilterTerm
			     OR BOENO LIKE @FilterTerm
              OR  OSMNAME  LIKE @FilterTerm
			  OR OSMVSLNAME LIKE @FilterTerm)
    SELECT OSMDATE, OSMNO , OSMDNO
	  ,OSMIGMNO 
	  ,OSMLNO 
      , OSMNAME 
	  ,BOENO 
	  ,BOEDATE,OSMVSLNAME
	  ,CASE DISPSTATUS WHEN 1 THEN 'C' END AS DISPSTATUS
	  , OSMID
    FROM    @TableMaster
    WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum
    
    SELECT @TotalRowsCount = COUNT(*)
    FROM         dbo.MANUALOPENSHEETMASTER   /* CHANGE  TABLE NAME */
    WHERE  (OSMDATE BETWEEN @PSDate AND @PEDate) and (OSMLDTYPE = @fcltype) and (COMPYID=@LCompyid)
    
    SELECT @FilteredRowsCount = COUNT(*)
    FROM   @TableMaster
        
END


