CREATE PROCEDURE [dbo].[pr_Search_SCMTR_Stuffed_List]  /* CHANGE */
 @FilterTerm nvarchar(250) = NULL --parameter to search all columns by
        , @SortIndex INT = 1 -- a one based index to indicate which column to order by
        , @SortDirection CHAR(4) = 'ASC' --the direction to sort in, either ASC or DESC
        , @StartRowNum INT = 1 --the first row to return
        , @EndRowNum INT = 10 --the last row to return
        , @TotalRowsCount INT OUTPUT
        , @FilteredRowsCount INT OUTPUT
        , @PSDate Smalldatetime
        , @PEDate Smalldatetime
 
AS BEGIN

Declare @LSDate Smalldatetime, @LEDate Smalldatetime

set @LSDate = @PSDate
set @LEDate = @PEDate

    --Wrap filter term with % to search for values that contain @FilterTerm
    SET @FilterTerm = '%' + @FilterTerm + '%'


    DECLARE @TableMaster TABLE
    (
      STFDSBNO  VARCHAR(25)
    , STFDSBDATE smalldatetime
    , STFMDNO  VARCHAR(25)
    , STFMDATE  smalldatetime
    , STFBILLREFNAME VARCHAR(100)
	, CONTNRNO VARCHAR(15)
	, CONTNRSDESC VARCHAR(15)
	, STFDNOP numeric(18,2)
	,DISPSTATUS smallint
	, STFMID INT
	, STFDID INT
	, STFXID INT
    , RowNum INT
    )
    INSERT INTO @TableMaster(STFDSBNO, STFDSBDATE, STFMDNO , STFMDATE,STFBILLREFNAME,CONTNRNO,CONTNRSDESC,STFDNOP,DISPSTATUS, STFMID,STFDID,STFXID, RowNum)
  
    SELECT   MAX(dbo.STUFFINGPRODUCTDETAIL.STFDSBNO) AS STFDSBNO, MAX(dbo.STUFFINGPRODUCTDETAIL.STFDSBDATE) AS STFDSBDATE, dbo.STUFFINGMASTER.STFMDNO, 
                         dbo.STUFFINGMASTER.STFMDATE, dbo.STUFFINGMASTER.STFBILLREFNAME, dbo.GATEINDETAIL.CONTNRNO, dbo.CONTAINERSIZEMASTER.CONTNRSDESC, SUM(dbo.STUFFINGPRODUCTDETAIL.STFDNOP) 
                         AS STFDNOP,0,dbo.STUFFINGMASTER.STFMID,STUFFINGDETAIL.STFDID, ISNULL(dbo.STUFFING_JSON_DETAIL.STFXID, 0)

            , Row_Number() OVER (
            ORDER BY
            
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/
            
            CASE @SortDirection
                 WHEN 'ASC'  THEN
                      CASE @SortIndex
           				
			  			  WHEN 2 then max(STFDSBNO)
					      WHEN 3 then STFMDNO
			  			  WHEN 4 then STFBILLREFNAME
						  WHEN 5 then CONTNRNO
						  WHEN 6 then CONTNRSDESC
					   END             
				 END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                   CASE @SortIndex
           			  
			  			  WHEN 2 then max(STFDSBNO)
					      WHEN 3 then STFMDNO
			  			  WHEN 4 then STFBILLREFNAME
						  WHEN 5 then CONTNRNO
						  WHEN 6 then CONTNRSDESC
                    END
               END DESC,
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                   CASE @SortIndex
                     WHEN 0 THEN max(STFDSBDATE)
					 WHEN 1 then STFMDATE
                   END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                     WHEN 0 THEN max(STFDSBDATE)
					 WHEN 1 then STFMDATE
                END
            END DESC
            
            ) AS RowNum
FROM            dbo.STUFFINGMASTER INNER JOIN
                         dbo.STUFFINGDETAIL ON dbo.STUFFINGMASTER.STFMID = dbo.STUFFINGDETAIL.STFMID INNER JOIN
                         dbo.STUFFINGPRODUCTDETAIL ON dbo.STUFFINGDETAIL.STFDID = dbo.STUFFINGPRODUCTDETAIL.STFDID INNER JOIN
                         dbo.XML_CHPOE07Payload_Leo ON dbo.STUFFINGPRODUCTDETAIL.STFDSBNO = dbo.XML_CHPOE07Payload_Leo.SBNo INNER JOIN
                         dbo.GATEINDETAIL ON dbo.STUFFINGDETAIL.GIDID = dbo.GATEINDETAIL.GIDID INNER JOIN
                         dbo.CONTAINERSIZEMASTER ON dbo.GATEINDETAIL.CONTNRSID = dbo.CONTAINERSIZEMASTER.CONTNRSID LEFT OUTER JOIN
                         dbo.STUFFING_JSON_DETAIL ON dbo.STUFFINGDETAIL.STFDID = dbo.STUFFING_JSON_DETAIL.STFDID
WHERE (STFMDATE BETWEEN @LSDate AND @LEDate) AND (@FilterTerm IS NULL 
             OR STFMDATE LIKE @FilterTerm
              OR STFMDNO LIKE @FilterTerm
              OR STFMDATE  LIKE @FilterTerm
              OR CONTNRNO    LIKE @FilterTerm
			   OR STFDSBNO    LIKE @FilterTerm
              oR STFBILLREFNAME LIKE @FilterTerm
              OR CONTNRSDESC    LIKE @FilterTerm
             )
GROUP BY dbo.STUFFINGMASTER.STFMDNO, dbo.STUFFINGMASTER.STFMDATE, dbo.GATEINDETAIL.CONTNRNO, dbo.CONTAINERSIZEMASTER.CONTNRSDESC, dbo.STUFFINGMASTER.STFMID, 
                         dbo.STUFFINGMASTER.STFBILLREFNAME,STUFFINGDETAIL.STFDID,ISNULL(dbo.STUFFING_JSON_DETAIL.STFXID, 0)		   
            
    SELECT STFDSBNO, STFDSBDATE, STFMDNO , STFMDATE,STFBILLREFNAME,CONTNRNO,CONTNRSDESC,STFDNOP,DISPSTATUS, STFMID,STFDID, STFXID
    FROM    @TableMaster
    WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum
    ORDER BY STFMDNO DESC 

SELECT    @TotalRowsCount = COUNT(*)
FROM            dbo.STUFFINGMASTER INNER JOIN
                         dbo.STUFFINGDETAIL ON dbo.STUFFINGMASTER.STFMID = dbo.STUFFINGDETAIL.STFMID INNER JOIN
                         dbo.STUFFINGPRODUCTDETAIL ON dbo.STUFFINGDETAIL.STFDID = dbo.STUFFINGPRODUCTDETAIL.STFDID INNER JOIN
                         dbo.XML_CHPOE07Payload_Leo ON dbo.STUFFINGPRODUCTDETAIL.STFDSBNO = dbo.XML_CHPOE07Payload_Leo.SBNo INNER JOIN
                         dbo.GATEINDETAIL ON dbo.STUFFINGDETAIL.GIDID = dbo.GATEINDETAIL.GIDID INNER JOIN
                         dbo.CONTAINERSIZEMASTER ON dbo.GATEINDETAIL.CONTNRSID = dbo.CONTAINERSIZEMASTER.CONTNRSID LEFT OUTER JOIN
                         dbo.STUFFING_JSON_DETAIL ON dbo.STUFFINGDETAIL.STFDID = dbo.STUFFING_JSON_DETAIL.STFDID
WHERE (STFMDATE BETWEEN @LSDate AND @LEDate)
--GROUP BY dbo.STUFFINGMASTER.STFMDNO, dbo.STUFFINGMASTER.STFMDATE, dbo.GATEINDETAIL.CONTNRNO, dbo.CONTAINERSIZEMASTER.CONTNRSDESC, dbo.STUFFINGMASTER.STFMID, 
 --                        dbo.STUFFINGMASTER.STFBILLREFNAME,STUFFINGDETAIL.STFDID		
    
    SELECT @FilteredRowsCount = COUNT(*)
    FROM   @TableMaster
        
END

