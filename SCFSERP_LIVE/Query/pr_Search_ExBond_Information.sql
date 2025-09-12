/*

DECLARE @total INT,
        @filtered INT

EXEC [dbo].[pr_Search_ExBond_Information] NULL, 1, 'ASC', 1, 10, @total OUTPUT, @filtered OUTPUT, 
'2021-07-01', '2023-07-31', 31
select * From ExBondMASTER

SELECT @total, @filtered
*/

CREATE PROCEDURE [dbo].[pr_Search_ExBond_Information]  /* CHANGE */
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
      EBNDDATE varchar(12)      
      , EBNDNO INT
	  , EBNDDNO VARCHAR(50)
      , EBNDASSAMT  numeric(18,2)
	  ,CHANAME VARCHAR(200)
	  ,IMPRTRNAME VARCHAR(200)
	  ,DISPSTATUS VARCHAR(100)
	  --,DISPSTATUS SMALLINT
	  , EBNDID int 
      , RowNum INT
    )

    INSERT INTO @TableMaster(EBNDDATE , EBNDDNO , EBNDNO ,  EBNDASSAMT   , CHANAME  ,IMPRTRNAME ,DISPSTATUS, 
	EBNDID, RowNum)
  
    SELECT  Convert(varchar,EBNDDATE,103), EBNDDNO, EBNDNO , EBNDASSAMT,  B.CATENAME, C.CATENAME , A.DISPSTATUS, A.EBNDID
                        
            , Row_Number() OVER (
            ORDER BY
            
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
					WHEN 2 then EBNDDNO
					WHEN 3 then EBNDNO
					WHEN 4 then B.CATENAME
					WHEN 5 then C.CATENAME
					WHEN 6 then A.DISPSTATUS
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
					WHEN 2 then EBNDDNO
					WHEN 3 then EBNDNO
					WHEN 4 then B.CATENAME
					WHEN 5 then C.CATENAME
					WHEN 6 then A.DISPSTATUS
                END
            END DESC,
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
                  WHEN 1 THEN EBNDDATE
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 1 THEN EBNDDATE
                END
            END DESC,
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
					WHEN 2 then EBNDDNO
                  --WHEN 4 THEN EBNDASSAMT
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
				  WHEN 1 then EBNDDNO
                  --WHEN 3 THEN EBNDASSAMT
                END
            END DESC                            

            ) AS RowNum
   FROM         dbo.ExBondMASTER A (nolock)  
					LEFT JOIN CATEGORYMASTER B(NOLOCK) ON A.CHAID = B.CATEID AND B.CATETID = 4
					LEFT JOIN CATEGORYMASTER C(NOLOCK) ON A.IMPRTID = C.CATEID AND C.CATETID = 1
    WHERE   (EBNDDATE BETWEEN @PSDate AND @PEDate) and (COMPYID=@LCompyid) AND SDPTID=10
	AND (@FilterTerm IS NULL 
              OR EBNDDATE LIKE @FilterTerm
              OR EBNDDNO LIKE @FilterTerm
             -- OR  BNDNO  LIKE @FilterTerm
             -- OR  BNDCIFAMT    LIKE @FilterTerm
              OR  B.CATENAME  LIKE @FilterTerm
              OR  C.CATENAME   LIKE @FilterTerm
			   )
    SELECT 
	EBNDDATE , EBNDDNO , EBNDNO ,  EBNDASSAMT   , CHANAME  ,IMPRTRNAME,	
	  CASE DISPSTATUS WHEN 0 THEN 'In-Books'
		when 1 then 'Cancelled' END AS DISPSTATUS
	  , EBNDID
    FROM    @TableMaster
    WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum
    
    SELECT @TotalRowsCount = COUNT(*)
    FROM            dbo.ExBondMASTER A (nolock)  
					LEFT JOIN CATEGORYMASTER B(NOLOCK) ON A.CHAID = B.CATEID AND B.CATETID = 4
					LEFT JOIN CATEGORYMASTER C(NOLOCK) ON A.IMPRTID = B.CATEID AND B.CATETID = 1
    WHERE   (EBNDDATE BETWEEN @PSDate AND @PEDate) and (COMPYID=@LCompyid) AND SDPTID=10
	
    SELECT @FilteredRowsCount = COUNT(*)
    FROM   @TableMaster
        
END
