/*

DECLARE @total INT,
        @filtered INT

EXEC [dbo].[pr_Search_NonPnr_GateInGridAssgn] NULL, 1, 'ASC', 1, 10, @total OUTPUT, @filtered OUTPUT, 
'2021-07-01', '2021-07-31', 32

SELECT @total, @filtered
*/

alter PROCEDURE [dbo].[pr_Search_NonPnr_GateInGridAssgn]  /* CHANGE */
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
      GIDATE SMALLDATETIME
      , GINO INT
      , GIDNO VARCHAR(15)
      , CONTNRNO  VARCHAR(15)
      , CONTNRSID  int
	  ,IGMNO VARCHAR(15)
	  ,GPLNO VARCHAR(15)
      , STMRNAME VARCHAR(100)
	  --,VSLNAME VARCHAR(100) --CODE COMMENTED AND MODIFIED BY RAJESH ON 26-07-2021
	  ,IMPRTNAME VARCHAR(100)
	  ,PRDTDESC VARCHAR(100) 
	  ,DISPSTATUS VARCHAR(100)
	  --,DISPSTATUS SMALLINT
	  , GIDID int 
      , RowNum INT
    )

    INSERT INTO @TableMaster(GIDATE
                        , GINO
                        , GIDNO
                         , CONTNRNO 
      , CONTNRSID  
	  ,IGMNO 
	  ,GPLNO 
      , STMRNAME 
	  ,IMPRTNAME 
	  ,PRDTDESC,DISPSTATUS, GIDID, RowNum)
  
    SELECT  GIDATE, GINO
                        , GIDNO
                         , CONTNRNO 
      , CONTNRSID  
	  ,IGMNO 
	  ,GPLNO 
      , STMRNAME 
	  ,IMPRTNAME 
	  ,PRDTDESC
	  ,dbo.VW_NONPNR_GATEIN_STATUS.CURRENTSTATUS
            --,DISPSTATUS , 
		, dbo.GATEINDETAIL.GIDID
                        
            , Row_Number() OVER (
            ORDER BY
            
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
				
           	      WHEN 2 then GIDNO
			  	  WHEN 3 then CONTNRNO
			  	 
				   WHEN 5 then IGMNO
			  	  WHEN 6 then GPLNO
				   WHEN 7 then STMRNAME
				   WHEN 8 then IMPRTNAME
			  	  WHEN 9 then PRDTDESC
				  WHEN 10 then CURRENTSTATUS
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex

           	     WHEN 2 then GIDNO
			  	  WHEN 3 then CONTNRNO
			  	 
				   WHEN 5 then IGMNO
			  	  WHEN 6 then GPLNO
				   WHEN 7 then STMRNAME
				   WHEN 8 then IMPRTNAME
			  	  WHEN 9 then PRDTDESC
				   WHEN 10 then CURRENTSTATUS
                END
            END DESC,
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
                  WHEN 0 THEN GIDATE
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 0 THEN GIDATE
                END
            END DESC,
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
								  WHEN 1 then GINO
                  WHEN 3 THEN CONTNRSID
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
				  WHEN 1 then GINO
                  WHEN 3 THEN CONTNRSID
                END
            END DESC                            

            ) AS RowNum
   FROM         dbo.GATEINDETAIL 
            LEFT JOIN dbo.VW_NONPNR_GATEIN_STATUS (nolock) ON  dbo.GATEINDETAIL.GIDID = dbo.VW_NONPNR_GATEIN_STATUS.GIDID 
            /* CHANGE  TABLE NAME */
    WHERE   (GIDATE BETWEEN @PSDate AND @PEDate) and (COMPYID=@LCompyid) AND SDPTID=9 AND CONTNRID >= 1 AND (@FilterTerm IS NULL 
              OR GIDATE LIKE @FilterTerm
              OR GINO LIKE @FilterTerm
              OR  GIDNO  LIKE @FilterTerm
              OR  CONTNRSID    LIKE @FilterTerm
          OR  CONTNRNO    LIKE @FilterTerm
              OR  IGMNO    LIKE @FilterTerm
			   OR GPLNO LIKE @FilterTerm
              OR  STMRNAME  LIKE @FilterTerm
              OR  IMPRTNAME    LIKE @FilterTerm
			   OR  PRDTDESC    LIKE @FilterTerm)
    SELECT GIDATE
                        , GINO
                        , GIDNO
                         , CONTNRNO 
      ,CASE CONTNRSID  WHEN 2 THEN 'NR' WHEN 3 THEN '20' WHEN 4 THEN '40'  WHEN 5 THEN '45' END  AS CONTNRSID
	  ,IGMNO 
	  ,GPLNO 
      , STMRNAME 
	  ,IMPRTNAME 
	  ,PRDTDESC
	  ,DISPSTATUS
	  --,CASE DISPSTATUS WHEN 1 THEN 'C' END AS CANCELSTATUS
	  , GIDID
    FROM    @TableMaster
    WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum
    
    SELECT @TotalRowsCount = COUNT(*)
    FROM         dbo.GATEINDETAIL   /* CHANGE  TABLE NAME */
    WHERE  (GIDATE BETWEEN @PSDate AND @PEDate) and (COMPYID=@LCompyid) AND SDPTID=9 AND CONTNRID >= 1
    
    SELECT @FilteredRowsCount = COUNT(*)
    FROM   @TableMaster
        
END

