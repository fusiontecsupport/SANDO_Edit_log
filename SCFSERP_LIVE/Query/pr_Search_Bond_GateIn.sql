/*

DECLARE @total INT,
        @filtered INT

EXEC [dbo].[pr_Search_Bond_GateIn] NULL, 1, 'ASC', 1, 10, @total OUTPUT, @filtered OUTPUT, 
'2021-07-01', '2023-07-31', 31

SELECT @total, @filtered
*/

alter PROCEDURE [dbo].[pr_Search_Bond_GateIn]  /* CHANGE */
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
	  , GIDNO VARCHAR(50)
	  , BONDNO VARCHAR(100)
      , CONTNRNO   VARCHAR(50)
	  ,STMRNAME VARCHAR(200)
	  ,IMPRTRNAME VARCHAR(200)
	  ,CONTNRSIZE VARCHAR(50)
	  ,PRDTDESC VARCHAR(150)
	  ,IGMNO VARCHAR(100)
	  ,DISPSTATUS VARCHAR(100)
	  , GIDID INT
	  --,DISPSTATUS SMALLINT	  
      , RowNum INT
    )

    INSERT INTO @TableMaster(GIDATE , GIDNO , GINO ,  BONDNO, CONTNRNO   , STMRNAME  ,IMPRTRNAME , CONTNRSIZE, PRDTDESC, IGMNO, DISPSTATUS, GIDID,
	RowNum)
  
    SELECT  GIDATE, GIDNO, GINO , BND.BNDDNO, CONTNRNO  ,  B.CATENAME, C.CATENAME , D.CONTNRSDESC, A.PRDTDESC,
			a.IGMNO,	A.DISPSTATUS, A.GIDID,
                        
            Row_Number() OVER (
            ORDER BY
            
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
					WHEN 2 then GIDNO
					WHEN 3 then GINO
					WHEN 4 then BND.BNDDNO 
					WHEN 5 then CONTNRNO 
					WHEN 6 then B.CATENAME
					WHEN 7 then C.CATENAME
					WHEN 8 then D.CONTNRSDESC 
					WHEN 9 then A.PRDTDESC
					WHEN 10 then A.IGMNO
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
					WHEN 2 then GIDNO
					WHEN 3 then GINO
					WHEN 4 then BND.BNDDNO 
					WHEN 5 then CONTNRNO 
					WHEN 6 then B.CATENAME
					WHEN 7 then C.CATENAME
					WHEN 8 then D.CONTNRSDESC 
					WHEN 9 then A.PRDTDESC
					WHEN 10 then A.IGMNO
                END
            END DESC,
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
                  WHEN 1 THEN GIDATE
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 1 THEN GIDATE
                END
            END DESC,
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
					WHEN 2 then GIDNO
                  WHEN 4 THEN CONTNRNO
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
				  WHEN 1 then GIDNO
                  WHEN 3 THEN CONTNRNO
                END
            END DESC                            

            ) AS RowNum
   FROM         dbo.BONDGATEINDETAIL A (nolock) 
					JOIN BONDMASTER BND(NOLOCK) ON A.BNDID = BND.BNDID AND A.COMPYID = BND.COMPYID AND A.SDPTID = BND.SDPTID
					LEFT JOIN CATEGORYMASTER B(NOLOCK) ON A.STMRID = B.CATEID AND B.CATETID = 3
					LEFT JOIN CATEGORYMASTER C(NOLOCK) ON A.IMPRTID = C.CATEID AND C.CATETID = 1
					LEFT JOIN CONTAINERSIZEMASTER D(NOLOCK) ON A.CONTNRSID = D.CONTNRSID 
    WHERE   (GIDATE BETWEEN @PSDate AND @PEDate) and (A.COMPYID=@LCompyid) AND A.SDPTID=10
	AND (@FilterTerm IS NULL 
              OR GIDATE LIKE @FilterTerm
              OR GIDNO LIKE @FilterTerm
              OR  GINO  LIKE @FilterTerm
              OR  CONTNRNO    LIKE @FilterTerm
              OR  B.CATENAME  LIKE @FilterTerm
              OR  C.CATENAME   LIKE @FilterTerm
			  OR BND.BNDDNO  LIKE @FilterTerm
			  OR  D.CONTNRSDESC   LIKE @FilterTerm
			  OR  A.PRDTDESC   LIKE @FilterTerm
			   )
    SELECT GIDATE , GIDNO , GINO ,  BONDNO, CONTNRNO   , STMRNAME  ,IMPRTRNAME , CONTNRSIZE, PRDTDESC, IGMNO,
	CASE DISPSTATUS WHEN 0 THEN 'In-Books'
		when 1 then 'Cancelled' END AS DISPSTATUS
	  , GIDID
    FROM    @TableMaster
    WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum
    
    SELECT @TotalRowsCount = COUNT(*)
    FROM         dbo.BONDGATEINDETAIL A (nolock) 
					JOIN BONDMASTER BND(NOLOCK) ON A.BNDID = BND.BNDID AND A.COMPYID = BND.COMPYID AND A.SDPTID = BND.SDPTID
					LEFT JOIN CATEGORYMASTER B(NOLOCK) ON A.STMRID = B.CATEID AND B.CATETID = 3
					LEFT JOIN CATEGORYMASTER C(NOLOCK) ON A.IMPRTID = C.CATEID AND C.CATETID = 1
					LEFT JOIN CONTAINERSIZEMASTER D(NOLOCK) ON A.CONTNRSID = D.CONTNRSID 
    WHERE   (GIDATE BETWEEN @PSDate AND @PEDate) and (A.COMPYID=@LCompyid) AND A.SDPTID=10
	
    SELECT @FilteredRowsCount = COUNT(*)
    FROM   @TableMaster
        
END



