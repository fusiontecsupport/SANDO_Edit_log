/*

DECLARE @total INT,
        @filtered INT

EXEC [dbo].[pr_Search_NonPnr_VehicleTicket] NULL, 1, 'ASC', 1, 10, @TotalRowsCount= @total OUTPUT,@FilteredRowsCount= @filtered OUTPUT, @PCompyId = 32, @PSDate='2021-07-01',
@PEDate= '2021-07-29', @ASLMTYPE=1

SELECT @total, @filtered
*/

alter  PROCEDURE [dbo].[pr_Search_NonPnr_VehicleTicket]  /* CHANGE */
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
		--,@PVTTYPE int
		,@ASLMTYPE int
 
AS BEGIN

Declare @LCompyId int,@LSDate Smalldatetime, @LEDate Smalldatetime	,@LVTTYPE int
set @LCompyId = @PCompyId
set @LSDate = @PSDate
set @LEDate = @PEDate
--set @LVTTYPE=@PVTTYPE

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
	,VTTYPE VARCHAR(50) 
	,CHAID INT
	,CHANAME VARCHAR(100)
	,BOENO VARCHAR(50)

	,IGMNO VARCHAR(50)
	,GPLNO VARCHAR(50)
	, RowNum INT
    )
    INSERT INTO @TableMaster(VTDATE, VTDNO, CONTNRNO, CONTNRSDESC, ASLMDNO, VTDESC, VTQTY, VHLNO, EGIDID, GIDId, VTDID, CGIDID,
	VTSSEALNO, GODID, VTTYPE, CHAID, CHANAME,BOENO,IGMNO,GPLNO,RowNum
	)
  
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


FROM         dbo.VEHICLETICKETDETAIL LEFT JOIN    
             dbo.AUTHORIZATIONSLIPDETAIL ON  dbo.VEHICLETICKETDETAIL.ASLDID = dbo.AUTHORIZATIONSLIPDETAIL.ASLDID LEFT JOIN  
			 dbo.AUTHORIZATIONSLIPMASTER ON  dbo.AUTHORIZATIONSLIPDETAIL.ASLMID = dbo.AUTHORIZATIONSLIPMASTER.ASLMID LEFT JOIN  
			 dbo.GATEINDETAIL ON dbo.VEHICLETICKETDETAIL.GIDID = dbo.GATEINDETAIL.GIDID LEFT JOIN 
			 dbo.CONTAINERSIZEMASTER ON dbo.GATEINDETAIL.CONTNRSID = dbo.CONTAINERSIZEMASTER.CONTNRSID LEFT JOIN
			 dbo.GATEOUTDETAIL ON dbo.GATEINDETAIL.GIDID = dbo.GATEOUTDETAIL.GIDID

WHERE (dbo.VEHICLETICKETDETAIL.COMPYID = @LCompyId) -- and (dbo.VEHICLETICKETDETAIL.VTTYPE = @LVTTYPE) 
    and  (VTDATE BETWEEN @PSDate AND @PEDate) AND (VEHICLETICKETDETAIL.SDPTID=9) AND (@FilterTerm IS NULL 
             OR VTDATE LIKE @FilterTerm
           OR VTDNO LIKE @FilterTerm
              OR CONTNRNO  LIKE @FilterTerm
              OR CONTNRSDESC    LIKE @FilterTerm
              oR ASLMDNO LIKE @FilterTerm
              OR VTDESC    LIKE @FilterTerm
              OR VTQTY    LIKE @FilterTerm)
	AND DBO.AUTHORIZATIONSLIPMASTER.ASLMTYPE = @ASLMTYPE
			   
ORDER BY VTDNO 
            
    SELECT VTDATE, VTDNO, CONTNRNO, CONTNRSDESC, ASLMDNO, 
	--VTDESC,ISNULL(VTQTY,0) AS VTQTY, VHLNO, ISNULL(EGIDID,0) as EGIDID, GIDId, VTDID,ISNULL(CGIDID,0) AS CGIDID,  VTSSEALNO,GODID ,
	VTTYPE, CHANAME, isnull(BOENO,'') AS BOENO, isnull(IGMNO,'') AS IGMNO,isnull(GPLNO,'') AS GPLNO,ISNULL(EGIDID,0) as EGIDID, VTDID, 
		GIDId, ISNULL(CGIDID,0) AS CGIDID,  VTSSEALNO,isnull(GODID ,0) as GODID, case when isnull(GODID ,0) =0 then 'Not-Exists' else 'Exists' end 'GOSTS'
    FROM    @TableMaster
    WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum
	ORDER BY VTDNO  DESC
    

SELECT    @TotalRowsCount = isnull(COUNT(*),0)
FROM  dbo.VEHICLETICKETDETAIL LEFT JOIN    
             dbo.AUTHORIZATIONSLIPDETAIL ON  dbo.VEHICLETICKETDETAIL.ASLDID = dbo.AUTHORIZATIONSLIPDETAIL.ASLDID LEFT JOIN  
			 dbo.AUTHORIZATIONSLIPMASTER ON  dbo.AUTHORIZATIONSLIPDETAIL.ASLMID = dbo.AUTHORIZATIONSLIPMASTER.ASLMID LEFT JOIN  
			 dbo.GATEINDETAIL ON dbo.VEHICLETICKETDETAIL.GIDID = dbo.GATEINDETAIL.GIDID LEFT JOIN 
			 dbo.CONTAINERSIZEMASTER ON dbo.GATEINDETAIL.CONTNRSID = dbo.CONTAINERSIZEMASTER.CONTNRSID LEFT JOIN
			 dbo.GATEOUTDETAIL ON dbo.GATEINDETAIL.GIDID = dbo.GATEOUTDETAIL.GIDID     
WHERE  dbo.VEHICLETICKETDETAIL.SDPTID = 9 --AND (dbo.VEHICLETICKETDETAIL.VTTYPE = @LVTTYPE) 
     and  dbo.VEHICLETICKETDETAIL.COMPYID = @LCompyId AND VTDATE BETWEEN @LSDate AND @LEDate
AND DBO.AUTHORIZATIONSLIPMASTER.ASLMTYPE = @ASLMTYPE
    
    SELECT @FilteredRowsCount = isnull(COUNT(*),0)
    FROM   @TableMaster
        
END

