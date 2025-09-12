alter PROCEDURE [dbo].[pr_Search_Export_Vehicle_Ticket_Master]  /* CHANGE */
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
    , RowNum INT
    )
    INSERT INTO @TableMaster(VTDATE, VTDNO, CONTNRNO, CONTNRSDESC, STFMDNO, VTDESC, VTQTY, VHLNO, EGIDID, GIDId, VTDID, CGIDID,VTSSEALNO, 
	GODID, VTCTYPE, DISPSTATUS, RowNum)
  
    SELECT   dbo.VEHICLETICKETDETAIL.VTDATE, dbo.VEHICLETICKETDETAIL.VTDNO,  
    dbo.GATEINDETAIL.CONTNRNO, dbo.CONTAINERSIZEMASTER.CONTNRSCODE, dbo.STUFFINGMASTER.STFMDNO,
    dbo.VEHICLETICKETDETAIL.VTDESC, dbo.VEHICLETICKETDETAIL.VTQTY, dbo.VEHICLETICKETDETAIL.VHLNO,
    dbo.VEHICLETICKETDETAIL.EGIDID, dbo.GATEINDETAIL.GIDID, dbo.VEHICLETICKETDETAIL.VTDID,
    dbo.VEHICLETICKETDETAIL.CGIDID,dbo.VEHICLETICKETDETAIL.VTSSEALNO, ISNULL(dbo.GATEOUTDETAIL.GODID, 0) AS GODID, 
	VTCTYPE,
	dbo.VEHICLETICKETDETAIL.DISPSTATUS
            , Row_Number() OVER (
            ORDER BY
            
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/
            
            CASE @SortDirection
                 WHEN 'ASC'  THEN
                      CASE @SortIndex
           				  WHEN 1 then VTDNO
			  			  WHEN 2 then CONTNRNO
					      WHEN 3 then CONTNRSDESC
			  			  WHEN 4 then STFMDNO
						  WHEN 5 then VTDESC
					   END             
				 END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                   CASE @SortIndex
           			    WHEN 1 then VTDNO
			  		    WHEN 2 then CONTNRNO
					    WHEN 3 then CONTNRSDESC
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


FROM         dbo.STUFFINGMASTER (nolock) RIGHT OUTER JOIN
                      dbo.VEHICLETICKETDETAIL (nolock) INNER JOIN
                      dbo.GATEINDETAIL (nolock) INNER JOIN
                      dbo.CONTAINERSIZEMASTER (nolock) ON dbo.GATEINDETAIL.CONTNRSID = dbo.CONTAINERSIZEMASTER.CONTNRSID ON 
                      dbo.VEHICLETICKETDETAIL.GIDID = dbo.GATEINDETAIL.GIDID LEFT OUTER JOIN
                      dbo.STUFFINGDETAIL (nolock) ON dbo.GATEINDETAIL.GIDID = dbo.STUFFINGDETAIL.GIDID LEFT OUTER JOIN
                      dbo.GATEOUTDETAIL  (nolock) ON dbo.GATEINDETAIL.GIDID = dbo.GATEOUTDETAIL.GIDID ON dbo.STUFFINGMASTER.STFMID = dbo.STUFFINGDETAIL.STFMID
                                           
WHERE (dbo.VEHICLETICKETDETAIL.COMPYID = @LCompyId) and
(VTDATE BETWEEN @PSDate AND @PEDate) AND (VEHICLETICKETDETAIL.SDPTID=2) 
AND (@LVTCTYPE = VTCTYPE OR @LVTCTYPE = 99) 
AND (@FilterTerm IS NULL 
             OR VTDATE LIKE @FilterTerm
              OR VTDNO LIKE @FilterTerm
              OR CONTNRNO  LIKE @FilterTerm
              OR CONTNRSDESC    LIKE @FilterTerm
              oR STFMDNO LIKE @FilterTerm
              OR VTDESC    LIKE @FilterTerm
              OR VTQTY    LIKE @FilterTerm
			  OR (case when VTCTYPE = 1 then 'Direct VT' when VTCTYPE = 0 then 'Stuff VT' end) LIKE @FilterTerm)			   
ORDER BY VTDNO 
            
    SELECT VTDATE, VTDNO, CONTNRNO, CONTNRSDESC, STFMDNO, VTDESC, VTQTY, VHLNO,
	isnull( EGIDID,0) as EGIDID, GIDId, VTDID, CGIDID,VTSSEALNO,isnull(GODID,0) as GODID,
	VTCTYPE, VTCTYPEDESC,
	(CASE WHEN DISPSTATUS = 1 THEN 'C' ELSE 'In Books' END)  AS DISPSTATUS
    FROM    @TableMaster
    WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum
	ORDER BY VTDNO  DESC
    

SELECT    @TotalRowsCount = COUNT(*)
FROM VEHICLETICKETDETAIL
WHERE  dbo.VEHICLETICKETDETAIL.SDPTID = 2 
AND  dbo.VEHICLETICKETDETAIL.COMPYID = @LCompyId
AND VTDATE BETWEEN @LSDate AND @LEDate
AND (@LVTCTYPE = VTCTYPE OR @LVTCTYPE = 99)

    
    SELECT @FilteredRowsCount = COUNT(*)
    FROM   @TableMaster
        
END


