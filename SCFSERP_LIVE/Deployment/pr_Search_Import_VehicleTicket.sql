CREATE  PROCEDURE [dbo].[pr_Search_Import_VehicleTicket]  /* CHANGE */
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
		,@PVTTYPE int
 
AS BEGIN

Declare @LCompyId int,@LSDate Smalldatetime, @LEDate Smalldatetime	,@LVTTYPE int
set @LCompyId = @PCompyId
set @LSDate = @PSDate
set @LEDate = @PEDate
set @LVTTYPE=@PVTTYPE

    --Wrap filter term with % to search for values that contain @FilterTerm
    SET @FilterTerm = '%' + @FilterTerm + '%'


    DECLARE @TableMaster TABLE
    (
      VTDATE SMALLDATETIME
    , VTDNO VARCHAR(15)
    , CONTNRNO  VARCHAR(15)
    , CONTNRSDESC  VARCHAR(5)	
    , ASLMDNO VARCHAR(25)
	, VTDESC VARCHAR(100)
	, VTQTY numeric(18,2)
	, VHLNO  VARCHAR(25)
	, EGIDID int 
	, GIDId int
	, VTDID int
	, CGIDID int
	, VTSSEALNO VARCHAR(50)
	, GODID INT
	, VTTYPE VARCHAR(50) 
	, CHAID INT
	, CHANAME VARCHAR(100)
	, BOENO VARCHAR(50)
	, IGMNO VARCHAR(50)
	, GPLNO VARCHAR(50)
    , RowNum INT
    )
    INSERT INTO @TableMaster(VTDATE, VTDNO, CONTNRNO, CONTNRSDESC, ASLMDNO, VTDESC, VTQTY, VHLNO, EGIDID, GIDId, VTDID, CGIDID,
	VTSSEALNO, GODID, VTTYPE ,CHAID ,CHANAME,BOENO,IGMNO ,GPLNO, RowNum)
  
    SELECT   dbo.VEHICLETICKETDETAIL.VTDATE, dbo.VEHICLETICKETDETAIL.VTDNO,  
    dbo.GATEINDETAIL.CONTNRNO, dbo.CONTAINERSIZEMASTER.CONTNRSCODE, dbo.AUTHORIZATIONSLIPMASTER.ASLMDNO,
    dbo.VEHICLETICKETDETAIL.VTDESC, dbo.VEHICLETICKETDETAIL.VTQTY, dbo.VEHICLETICKETDETAIL.VHLNO,
    dbo.VEHICLETICKETDETAIL.EGIDID, dbo.GATEINDETAIL.GIDID, dbo.VEHICLETICKETDETAIL.VTDID,
    dbo.VEHICLETICKETDETAIL.CGIDID,dbo.VEHICLETICKETDETAIL.VTSSEALNO, ISNULL(dbo.GATEOUTDETAIL.GODID, 0) AS GODID,
	case when cast(dbo.AUTHORIZATIONSLIPMASTER.ASLMTYPE as varchar(10))= '1' then 'Load' 
	when cast(dbo.AUTHORIZATIONSLIPMASTER.ASLMTYPE as varchar(10)) = '2' then 'DeStuff' Else cast(dbo.AUTHORIZATIONSLIPMASTER.ASLMTYPE as varchar(10)) end,
	dbo.GATEINDETAIL.CHAID, dbo.GATEINDETAIL.CHANAME, dbo.GATEINDETAIL.BOENO,ISNULL(GATEINDETAIL.IGMNO,'') AS IGMNO,
		  ISNULL(GATEINDETAIL.GPLNO,'') AS GPLNO

            , Row_Number() OVER (
            ORDER BY
            
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/
            
            CASE @SortDirection
                 WHEN 'ASC'  THEN
                      CASE @SortIndex
           				  WHEN 1 then VTDNO
			  			  WHEN 2 then CONTNRNO
					      WHEN 3 then CONTNRSDESC
			  			  WHEN 4 then ASLMDNO
						  WHEN 5 then VTDESC
					   END             
				 END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                   CASE @SortIndex
           			    WHEN 1 then VTDNO
			  		    WHEN 2 then CONTNRNO
					    WHEN 3 then CONTNRSDESC
			  			WHEN 4 then ASLMDNO
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

/*            
FROM         dbo.STUFFINGDETAIL INNER JOIN
                      dbo.STUFFINGMASTER ON dbo.STUFFINGDETAIL.STFMID = dbo.STUFFINGMASTER.STFMID INNER JOIN
                      dbo.GATEINDETAIL INNER JOIN
                      dbo.CONTAINERSIZEMASTER ON dbo.GATEINDETAIL.CONTNRSID = dbo.CONTAINERSIZEMASTER.CONTNRSID ON 
                      dbo.STUFFINGDETAIL.GIDID = dbo.GATEINDETAIL.GIDID INNER JOIN
                      dbo.AUTHORIZATIONSLIPDETAIL ON dbo.STUFFINGDETAIL.STFDID = dbo.AUTHORIZATIONSLIPDETAIL.STFDID INNER JOIN
                      dbo.VEHICLETICKETDETAIL ON dbo.AUTHORIZATIONSLIPDETAIL.ASLDID = dbo.VEHICLETICKETDETAIL.ASLDID LEFT OUTER JOIN
                      dbo.GATEOUTDETAIL ON dbo.GATEINDETAIL.GIDID = dbo.GATEOUTDETAIL.GIDID
*/
FROM         dbo.VEHICLETICKETDETAIL INNER JOIN
                      dbo.AUTHORIZATIONSLIPDETAIL ON  dbo.AUTHORIZATIONSLIPDETAIL.ASLDID=dbo.VEHICLETICKETDETAIL.ASLDID INNER JOIN
					   dbo.AUTHORIZATIONSLIPMASTER ON  dbo.AUTHORIZATIONSLIPMASTER.ASLMID= dbo.AUTHORIZATIONSLIPDETAIL.ASLMID INNER JOIN
                      dbo.GATEINDETAIL ON dbo.GATEINDETAIL.GIDID=dbo.VEHICLETICKETDETAIL.GIDID INNER JOIN
                      dbo.CONTAINERSIZEMASTER ON dbo.GATEINDETAIL.CONTNRSID = dbo.CONTAINERSIZEMASTER.CONTNRSID LEFT OUTER JOIN
                      dbo.GATEOUTDETAIL ON dbo.GATEINDETAIL.GIDID = dbo.GATEOUTDETAIL.GIDID
                      
WHERE (dbo.VEHICLETICKETDETAIL.COMPYID = @LCompyId) and (dbo.VEHICLETICKETDETAIL.VTTYPE = @LVTTYPE) and
(VTDATE BETWEEN @PSDate AND @PEDate) AND (VEHICLETICKETDETAIL.SDPTID=1) AND (@FilterTerm IS NULL 
             OR VTDATE LIKE @FilterTerm
              OR VTDNO LIKE @FilterTerm
              OR CONTNRNO  LIKE @FilterTerm
              OR CONTNRSDESC    LIKE @FilterTerm
              oR ASLMDNO LIKE @FilterTerm
              OR VTDESC    LIKE @FilterTerm
              OR VTQTY    LIKE @FilterTerm)
			   
ORDER BY VTDNO 
            
    SELECT VTDATE, VTDNO, CONTNRNO, CONTNRSDESC, ASLMDNO, 
	--VTDESC,ISNULL(VTQTY,0) AS VTQTY, VHLNO, ISNULL(EGIDID,0) as EGIDID, GIDId, VTDID,ISNULL(CGIDID,0) AS CGIDID,  VTSSEALNO,GODID ,
	VTTYPE, CHANAME, isnull(BOENO,'') AS BOENO, isnull(IGMNO,'') AS IGMNO,isnull(GPLNO,'') AS GPLNO,ISNULL(EGIDID,0) as EGIDID, VTDID, GIDId, ISNULL(CGIDID,0) AS CGIDID,  VTSSEALNO,GODID 
    FROM    @TableMaster
    WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum
	ORDER BY VTDNO  DESC
    

SELECT    @TotalRowsCount = COUNT(*)
FROM  dbo.VEHICLETICKETDETAIL INNER JOIN
                      dbo.AUTHORIZATIONSLIPDETAIL ON  dbo.AUTHORIZATIONSLIPDETAIL.ASLDID=dbo.VEHICLETICKETDETAIL.ASLDID INNER JOIN
					   dbo.AUTHORIZATIONSLIPMASTER ON  dbo.AUTHORIZATIONSLIPMASTER.ASLMID= dbo.AUTHORIZATIONSLIPDETAIL.ASLMID INNER JOIN
                      dbo.GATEINDETAIL ON dbo.GATEINDETAIL.GIDID=dbo.VEHICLETICKETDETAIL.GIDID INNER JOIN
                      dbo.CONTAINERSIZEMASTER ON dbo.GATEINDETAIL.CONTNRSID = dbo.CONTAINERSIZEMASTER.CONTNRSID LEFT OUTER JOIN
                      dbo.GATEOUTDETAIL ON dbo.GATEINDETAIL.GIDID = dbo.GATEOUTDETAIL.GIDID
WHERE  dbo.VEHICLETICKETDETAIL.SDPTID = 1 AND (dbo.VEHICLETICKETDETAIL.VTTYPE = @LVTTYPE) and  dbo.VEHICLETICKETDETAIL.COMPYID = @LCompyId AND VTDATE BETWEEN @LSDate AND @LEDate

    
    SELECT @FilteredRowsCount = COUNT(*)
    FROM   @TableMaster
        
END

