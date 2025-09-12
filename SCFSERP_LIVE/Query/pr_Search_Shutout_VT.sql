-- SELECT * FROM SHUTOUTVTDETAIL
ALTER PROCEDURE [dbo].[pr_Search_Shutout_VT]  /* CHANGE */
 @FilterTerm nvarchar(250) = NULL --parameter to search all columns by
        , @SortIndex INT = 1 -- a one based index to indicate which column to order by
        , @SortDirection CHAR(4) = 'ASC' --the direction to sort in, either ASC or DESC
        , @StartRowNum INT = 1 --the first row to return
        , @EndRowNum INT = 10 --the last row to return
        , @TotalRowsCount INT OUTPUT
        , @FilteredRowsCount INT OUTPUT
        , @PCompyId INT
        , @PSDate Smalldatetime
        , @PEDate Smalldatetime
		, @PVTCTYPE INT
 
AS BEGIN

Declare @LCompyId int,@LSDate Smalldatetime, @LEDate Smalldatetime, @LVTCTYPE INT
set @LCompyId = @PCompyId
set @LSDate = @PSDate
set @LEDate = @PEDate
set @LVTCTYPE = @PVTCTYPE

    --Wrap filter term with % to search for values that contain @FilterTerm
    SET @FilterTerm = '%' + @FilterTerm + '%'


    DECLARE @TableMaster TABLE
    (
      VTDATE SMALLDATETIME
    , VTDNO VARCHAR(15)
    , CONTNRNO  VARCHAR(15)
    , CONTNRSDESC  VARCHAR(5)	
    , STFMDNO VARCHAR(25)
	, VTDESC VARCHAR(100)
	, VTQTY numeric(18,2)
	, VHLNO  VARCHAR(25)
	, EGIDID int 
	, GIDId int
	, VTDID int
	, CGIDID int,VTSSEALNO VARCHAR(50)
	, GODID INT
	, VTCTYPE smallint
	, VTCTYPEDESC VARCHAR(25)	
	, DISPSTATUS smallint
	, PRDTDESC varchar(150)
	, CHANAME varchar(150)
	, VEHICLE VARCHAR(100)
    , RowNum INT
    )
    INSERT INTO @TableMaster(VTDATE, VTDNO, CONTNRNO, CONTNRSDESC, STFMDNO, VTDESC, VTQTY, VHLNO, EGIDID, GIDId, VTDID, CGIDID,VTSSEALNO, 
	GODID, VTCTYPE, DISPSTATUS, PRDTDESC, CHANAME, RowNum)
  
    SELECT   dbo.SHUTOUTVTDETAIL.VTDATE, dbo.SHUTOUTVTDETAIL.VTDNO,  
    dbo.GATEINDETAIL.CONTNRNO, dbo.CONTAINERSIZEMASTER.CONTNRSCODE, dbo.STUFFINGMASTER.STFMDNO,
    dbo.SHUTOUTVTDETAIL.VTDESC, dbo.SHUTOUTVTDETAIL.VTQTY, case when VTCTYPE = 1 then SHUTOUTVTDETAIL.GTRNSPRTNAME when VTCTYPE = 2 then SHUTOUTVTDETAIL.VHLNO end,
    dbo.SHUTOUTVTDETAIL.EGIDID, dbo.GATEINDETAIL.GIDID, dbo.SHUTOUTVTDETAIL.VTDID,
    dbo.SHUTOUTVTDETAIL.CGIDID,dbo.SHUTOUTVTDETAIL.VTSSEALNO, ISNULL(dbo.GATEOUTDETAIL.GODID, 0) AS GODID, 
	VTCTYPE,
	dbo.SHUTOUTVTDETAIL.DISPSTATUS, GATEINDETAIL.PRDTDESC, STFMNAME
            , Row_Number() OVER (
            ORDER BY
            
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/
            
            CASE @SortDirection
                 WHEN 'ASC'  THEN
                      CASE @SortIndex
           				  WHEN 1 then VTDNO
			  			  WHEN 2 then STFMNAME
					      WHEN 3 then case when VTCTYPE = 1 then SHUTOUTVTDETAIL.GTRNSPRTNAME when VTCTYPE = 2 then SHUTOUTVTDETAIL.VHLNO end
			  			  WHEN 4 then STFMDNO
						  WHEN 5 then VTDESC
					   END             
				 END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                   CASE @SortIndex
           			    WHEN 1 then VTDNO
			  		    WHEN 2 then STFMNAME
					    WHEN 3 then case when VTCTYPE = 1 then SHUTOUTVTDETAIL.GTRNSPRTNAME when VTCTYPE = 2 then SHUTOUTVTDETAIL.VHLNO end
			  			WHEN 4 then STFMDNO
						WHEN 5 then VTDESC
                    END
               END DESC,
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                   CASE @SortIndex
                     WHEN 0 THEN VTDATE
                   END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 0 THEN VTDATE
                END
            END DESC
            
            ) AS RowNum


FROM         dbo.SHUTOUTVTDETAIL (nolock) left JOIN dbo.STUFFINGMASTER (nolock) on SHUTOUTVTDETAIL.STFMID = STUFFINGMASTER.STFMID 
			left OUTER JOIN dbo.STUFFINGDETAIL (nolock) ON STUFFINGMASTER.STFMID = STUFFINGDETAIL.STFMID
			LEFT JOIN dbo.GATEINDETAIL (nolock) ON Dbo.STUFFINGDETAIL.GIDID = dbo.GATEINDETAIL.GIDID 
			LEFT OUTER JOIN dbo.CONTAINERSIZEMASTER (nolock) ON dbo.GATEINDETAIL.CONTNRSID = dbo.CONTAINERSIZEMASTER.CONTNRSID 
			LEFT OUTER JOIN dbo.GATEOUTDETAIL  (nolock) ON dbo.GATEINDETAIL.GIDID = dbo.GATEOUTDETAIL.GIDID 
                                          
WHERE (dbo.SHUTOUTVTDETAIL.COMPYID = @LCompyId) and
(VTDATE BETWEEN @PSDate AND @PEDate) AND (SHUTOUTVTDETAIL.SDPTID=12) 
AND (@LVTCTYPE = VTCTYPE OR @LVTCTYPE = 99) 
AND STUFFINGMASTER.STFTID = 2
AND (@FilterTerm IS NULL 
             OR VTDATE LIKE @FilterTerm
              OR VTDNO LIKE @FilterTerm
              OR CONTNRNO  LIKE @FilterTerm
              OR CONTNRSDESC    LIKE @FilterTerm
              oR STFMDNO LIKE @FilterTerm
              OR VTDESC    LIKE @FilterTerm
              OR VTQTY    LIKE @FilterTerm
			  OR (case when VTCTYPE = 1 then 'By Hand' when VTCTYPE = 0 then 'By Vehicle' end) LIKE @FilterTerm)			   
ORDER BY VTDNO 
            
    SELECT VTDATE, VTDNO, CONTNRNO, CONTNRSDESC, STFMDNO, VTDESC, VTQTY, VHLNO,
	isnull( EGIDID,0) as EGIDID, GIDId, VTDID, CGIDID,VTSSEALNO,isnull(GODID,0) as GODID,
	VTCTYPE, VTCTYPEDESC, PRDTDESC, CHANAME, VEHICLE,
	(CASE WHEN DISPSTATUS = 1 THEN 'C' ELSE 'In Books' END)  AS DISPSTATUS
    FROM    @TableMaster
    WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum
	ORDER BY VTDNO  DESC
    

SELECT    @TotalRowsCount = COUNT(*)
FROM SHUTOUTVTDETAIL
WHERE  dbo.SHUTOUTVTDETAIL.SDPTID = 12 
AND  dbo.SHUTOUTVTDETAIL.COMPYID = @LCompyId
AND VTDATE BETWEEN @LSDate AND @LEDate
AND (@LVTCTYPE = VTCTYPE OR @LVTCTYPE = 99)

    
    SELECT @FilteredRowsCount = COUNT(*)
    FROM   @TableMaster
        
END



