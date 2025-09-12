ALTER PROCEDURE [dbo].[pr_Search_NonPnr_DeStuffSlip]  /* CHANGE */
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
 
AS BEGIN

Declare @LCompyId int,@LSDate Smalldatetime, @LEDate Smalldatetime
set @LCompyId = @PCompyId
set @LSDate = @PSDate
set @LEDate = @PEDate

    --Wrap filter term with % to search for values that contain @FilterTerm
    SET @FilterTerm = '%' + @FilterTerm + '%'


    DECLARE @TableMaster TABLE
    (
      ASLMDATE SMALLDATETIME
    , ASLMNO VARCHAR(25)
    , CONTNRNO  VARCHAR(25)
    , CONTNRSDESC  VARCHAR(150)	
	, CHANAME VARCHAR(250)
    , BOENO VARCHAR(25)
	, BOEDATE smalldatetime
	, ASLMID int
	, ASLDID int 
	, OSDID int
	, DISPSTATUS int,ASLMDNO VARCHAR(50)
	, IGMNO VARCHAR(50)
	, GPLNO VARCHAR(50)	
    , RowNum INT
    )
    INSERT INTO @TableMaster(ASLMDATE, ASLMNO, CONTNRNO, CONTNRSDESC, CHANAME, BOENO, BOEDATE, ASLMID, ASLDID, OSDID, DISPSTATUS,ASLMDNO, 
	IGMNO, GPLNO, RowNum)
  
    SELECT   dbo.AUTHORIZATIONSLIPMASTER.ASLMDATE, dbo.AUTHORIZATIONSLIPMASTER.ASLMNO, isnull(dbo.GATEINDETAIL.CONTNRNO,''), 
                      isnull(dbo.CONTAINERSIZEMASTER.CONTNRSDESC,''),ISNULL(dbo.GATEINDETAIL.CHANAME,'') AS CHANAME,
					  isnull(dbo.GATEINDETAIL.BOENO,''), 
					  isnull(dbo.GATEINDETAIL.BOEDATE,''),
                      dbo.AUTHORIZATIONSLIPDETAIL.ASLMID,  dbo.AUTHORIZATIONSLIPDETAIL.ASLDID, ISNULL(dbo.GATEINDETAIL.GIDID, 0) AS OSDID, 
                      dbo.AUTHORIZATIONSLIPMASTER.DISPSTATUS,ASLMDNO, DBO.GATEINDETAIL.IGMNO, DBO.GATEINDETAIL.GPLNO


            , Row_Number() OVER (
            ORDER BY
            
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/
            
            CASE @SortDirection
                 WHEN 'ASC'  THEN
                      CASE @SortIndex
           				  WHEN 1 then ASLMNO
			  			  WHEN 2 then CONTNRNO
					      WHEN 3 then CONTNRSDESC
			  			  WHEN 4 then CHANAME
						  WHEN 5 then GATEINDETAIL.BOENO
						  WHEN 6 then GATEINDETAIL.IGMNO
						  WHEN 7 then GATEINDETAIL.GPLNO
						
						  WHEN 13 THEN ASLMDNO
					   END             
				 END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                   CASE @SortIndex
           			    WHEN 1 then ASLMNO
			  		    WHEN 2 then CONTNRNO
					    WHEN 3 then CONTNRSDESC
			  			WHEN 4 then CHANAME
						WHEN 5 then GATEINDETAIL.BOENO
						WHEN 6 then GATEINDETAIL.IGMNO
						WHEN 7 then GATEINDETAIL.GPLNO
						
						 WHEN 13 THEN ASLMDNO
                    END
               END DESC,
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                   CASE @SortIndex
                     WHEN 0 THEN ASLMDATE
					   WHEN 7 then GATEINDETAIL.BOEDATE
                   END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 0 THEN ASLMDATE
				   WHEN 7 then GATEINDETAIL.BOEDATE
                END
            END DESC
            
            ) AS RowNum
            
FROM   dbo.AUTHORIZATIONSLIPMASTER INNER JOIN   
       dbo.AUTHORIZATIONSLIPDETAIL ON dbo.AUTHORIZATIONSLIPMASTER.ASLMID=dbo.AUTHORIZATIONSLIPDETAIL.ASLMID LEFT JOIN 
	   dbo.GATEINDETAIL ON dbo.AUTHORIZATIONSLIPDETAIL.GIDID = dbo.GATEINDETAIL.GIDID LEFT JOIN 
	  
	   dbo.CONTAINERSIZEMASTER ON dbo.GATEINDETAIL.CONTNRSID = dbo.CONTAINERSIZEMASTER.CONTNRSID
       

WHERE (ASLMTYPE = 2) AND  (dbo.AUTHORIZATIONSLIPMASTER.COMPYID = @LCompyId) and
(ASLMDATE BETWEEN @PSDate AND @PEDate) AND (AUTHORIZATIONSLIPMASTER.SDPTID=9) AND (@FilterTerm IS NULL 
             OR ASLMDATE LIKE @FilterTerm
              OR ASLMNO LIKE @FilterTerm
              OR CONTNRNO  LIKE @FilterTerm
              OR CONTNRSDESC    LIKE @FilterTerm
              oR CHANAME LIKE @FilterTerm
              OR GATEINDETAIL.BOENO    LIKE @FilterTerm
              OR GATEINDETAIL.BOEDATE    LIKE @FilterTerm
			  OR GATEINDETAIL.IGMNO    LIKE @FilterTerm
              OR GATEINDETAIL.GPLNO    LIKE @FilterTerm
			  OR ASLMDNO    LIKE @FilterTerm)
			   
ORDER BY dbo.AUTHORIZATIONSLIPMASTER.ASLMNO, dbo.GATEINDETAIL.CONTNRNO
            
    SELECT ASLMDATE, ASLMNO, CONTNRNO, CONTNRSDESC, CHANAME, BOENO, BOEDATE, ASLMID,  ASLDID, OSDID, 
	case DISPSTATUS when 1 then 'C' else '' end as DISPSTATUS,ASLMDNO, IGMNO, GPLNO
    FROM    @TableMaster
    WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum
    ORDER BY ASLMDNO DESC

SELECT    @TotalRowsCount = COUNT(*)
FROM AUTHORIZATIONSLIPMASTER
WHERE DBO.AUTHORIZATIONSLIPMASTER.ASLMTYPE = 2  AND dbo.AUTHORIZATIONSLIPMASTER.SDPTID = 9 AND  dbo.AUTHORIZATIONSLIPMASTER.COMPYID = @LCompyId AND ASLMDATE BETWEEN @LSDate AND @LEDate

    
    SELECT @FilteredRowsCount = COUNT(*)
    FROM   @TableMaster
        
END

