/*

DECLARE @total INT,
        @filtered INT

EXEC [dbo].[pr_SearchImportLorryHire] NULL, 1, 'ASC', 1, 10, @TotalRowsCount= @total OUTPUT,@FilteredRowsCount= @filtered OUTPUT,
@PSDATE = '2021-01-01',@PEDATE = '2021-09-01',@PCOMPYID = 32
SELECT @total, @filtered
*/

CREATE PROCEDURE [dbo].[pr_SearchImportLorryHire]  /* CHANGE */
 @FilterTerm nvarchar(250) = NULL --parameter to search all columns by
        , @SortIndex INT = 1 -- a one based index to indicate which column to order by
        , @SortDirection CHAR(4) = 'ASC' --the direction to sort in, either ASC or DESC
        , @StartRowNum INT = 1 --the first row to return
        , @EndRowNum INT = 10 --the last row to return
        , @TotalRowsCount INT OUTPUT
        , @FilteredRowsCount INT OUTPUT
		,@PSDATE SMALLDATETIME
		,@PEDATE SMALLDATETIME
		,@PCOMPYID INT
 
AS BEGIN
    --Wrap filter term with % to search for values that contain @FilterTerm
    SET @FilterTerm = '%' + @FilterTerm + '%'
	DECLARE @LSDATE SMALLDATETIME,@LEDATE SMALLDATETIME,@LCOMPYID INT
	SET @LSDATE=@PSDATE
	SET @LEDATE=@PEDATE
	SET @LCOMPYID=@PCOMPYID

    DECLARE @TableMaster TABLE
    ( 
	GILDATE SMALLDATETIME
      ,GILTIME DATETIME
	  ,GILNO INT
	  ,GILDNO VARCHAR(15)
	  ,CONTNRNO VARCHAR(100)
	  ,CONTNRSCODE VARCHAR(100)
	  ,GILBILLNO VARCHAR(100)
	  ,CHANAME VARCHAR(100)
	  ,TRNSPRTNAME VARCHAR(100)
	  ,IGMNO VARCHAR(100)
	  ,GPLNO VARCHAR(100)
	  ,VSLNAME VARCHAR(100)
	  ,STMRNAME VARCHAR(100)
	  ,PRDTDESC VARCHAR(100)
	  ,GIDID int
	  , GILDID int 
      , RowNum INT
    )

    INSERT INTO @TableMaster(GILDATE,
GILTIME ,GILNO,GILDNO,CONTNRNO ,CONTNRSCODE,GILBILLNO,CHANAME ,TRNSPRTNAME,IGMNO,GPLNO,VSLNAME,STMRNAME,PRDTDESC,GIDID,GILDID, RowNum)
    SELECT  GILDATE
            , GILTIME
			,GILNO
			,GILDNO
			,CONTNRNO 
			,CONTNRSCODE
			,GILBILLNO
			,CHANAME 
			,TRNSPRTNAME,IGMNO,GPLNO,VSLNAME,STMRNAME,PRDTDESC
			,GATEINLORRYHIREDETAIL.GIDID
			
            , GILDID
            , Row_Number() OVER (
            ORDER BY
            
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
                  WHEN 2 THEN GILNO
                  WHEN 3 THEN GILDNO
				    WHEN 4 THEN CONTNRNO
                  WHEN 5 THEN GILBILLNO
				    WHEN 6 THEN CHANAME
                  WHEN 7 THEN TRNSPRTNAME
	               WHEN 8 THEN IGMNO
                  WHEN 9 THEN GPLNO
				    WHEN 10 THEN VSLNAME
                  WHEN 11 THEN STMRNAME
				   WHEN 12 THEN PRDTDESC
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 2 THEN GILNO
                  WHEN 3 THEN GILDNO
				    WHEN 4 THEN CONTNRNO
                  WHEN 5 THEN GILBILLNO
				    WHEN 6 THEN CHANAME
                  WHEN 7 THEN TRNSPRTNAME
	 			WHEN 8 THEN IGMNO
                  WHEN 9 THEN GPLNO
				    WHEN 10 THEN VSLNAME
                  WHEN 11 THEN STMRNAME
				   WHEN 12 THEN PRDTDESC
               
                END
            END DESC ,
			 CASE @SortDirection    /*DATETIME ORDER BY*/
              WHEN 'ASC'  THEN
                CASE @SortIndex
                  WHEN 0 THEN GILDATE
                  WHEN 1 THEN GILTIME
	 
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 0 THEN GILDATE
                  WHEN 1 THEN GILTIME				
               
                END
            END DESC 
            
        
             

            
            ) AS RowNum
            
FROM   dbo.GATEINDETAIL INNER JOIN
                      dbo.CONTAINERSIZEMASTER ON dbo.GATEINDETAIL.CONTNRSID = dbo.CONTAINERSIZEMASTER.CONTNRSID  LEFT OUTER JOIN
                      dbo.GATEINLORRYHIREDETAIL ON dbo.GATEINDETAIL.GIDID = dbo.GATEINLORRYHIREDETAIL.GIDID           
WHERE (GATEINDETAIL.COMPYID= @LCOMPYID) AND (GILDATE BETWEEN @LSDATE AND @LEDATE) AND (GATEINDETAIL.SDPTID = 1) AND  (@FilterTerm IS NULL 
              OR GILDATE LIKE @FilterTerm
              OR GILTIME LIKE @FilterTerm
			  OR GILNO LIKE @FilterTerm
              OR GILDNO LIKE @FilterTerm
			  OR CONTNRNO LIKE @FilterTerm
              OR CONTNRSCODE LIKE @FilterTerm
			  OR GILBILLNO LIKE @FilterTerm
              OR CHANAME LIKE @FilterTerm
			  OR TRNSPRTNAME LIKE @FilterTerm
			    OR IGMNO LIKE @FilterTerm
              OR GPLNO LIKE @FilterTerm
			  OR VSLNAME LIKE @FilterTerm
              OR STMRNAME LIKE @FilterTerm
			  OR PRDTDESC LIKE @FilterTerm
            )

    SELECT GILDATE
            , GILTIME
			,GILNO
			,GILDNO
			,CONTNRNO 
			,CONTNRSCODE
			,GILBILLNO
			,CHANAME 
			,TRNSPRTNAME,IGMNO,GPLNO,VSLNAME,STMRNAME,PRDTDESC
			,GIDID
            , GILDID
    FROM    @TableMaster
    WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum
    ORDER BY GILDNO DESC
      
SELECT @TotalRowsCount = COUNT(*)
FROM     dbo.GATEINDETAIL INNER JOIN
                      dbo.CONTAINERSIZEMASTER ON dbo.GATEINDETAIL.CONTNRSID = dbo.CONTAINERSIZEMASTER.CONTNRSID  LEFT OUTER JOIN
                      dbo.GATEINLORRYHIREDETAIL ON dbo.GATEINDETAIL.GIDID = dbo.GATEINLORRYHIREDETAIL.GIDID         
WHERE (GATEINDETAIL.COMPYID= @LCOMPYID) AND (GILDATE BETWEEN @LSDATE AND @LEDATE) AND (GATEINDETAIL.SDPTID = 1)
      
    SELECT @FilteredRowsCount = COUNT(*)
    FROM   @TableMaster
        
END

