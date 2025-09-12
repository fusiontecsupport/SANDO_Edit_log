alter PROCEDURE [dbo].[pr_Search_ESeal_VehicleTicket]  /* CHANGE */
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
      VTDATE SMALLDATETIME
    , VTDNO VARCHAR(15)
    , CONTNRNO  VARCHAR(15)
    , CONTNRSDESC  VARCHAR(5)	   
	, CHANAME VARCHAR(100)
	, EXPRTRNAME VARCHAR(100)
	, TRNSPRTNAME VARCHAR(100)	
	, VHLNO  VARCHAR(25)
	, DRVNAME VARCHAR(100)		
	, VTSSEALNO VARCHAR(50) 	
	, DISPSTATUS smallint
	, GIDId int
	, VTDID int
	  , Edchk varchar(15)
    , RowNum INT
    )
    INSERT INTO @TableMaster(VTDATE, VTDNO, CONTNRNO, CONTNRSDESC,  CHANAME,EXPRTRNAME,TRNSPRTNAME,  VHLNO,DRVNAME, VTSSEALNO,DISPSTATUS, GIDId, VTDID, 
	   RowNum)

	SELECT   dbo.VEHICLETICKETDETAIL.VTDATE, dbo.VEHICLETICKETDETAIL.VTDNO,  
    dbo.GATEINDETAIL.CONTNRNO, dbo.CONTAINERSIZEMASTER.CONTNRSCODE, 
    dbo.GATEINDETAIL.CHANAME, dbo.GATEINDETAIL.EXPRTRNAME,dbo.VEHICLETICKETDETAIL.TRNSPRTNAME,dbo.VEHICLETICKETDETAIL.VHLNO,
	dbo.VEHICLETICKETDETAIL.DRVNAME, dbo.VEHICLETICKETDETAIL.VTSSEALNO,dbo.VEHICLETICKETDETAIL.DISPSTATUS, dbo.GATEINDETAIL.GIDID, dbo.VEHICLETICKETDETAIL.VTDID,   
	
	  Row_Number() OVER (
            ORDER BY
            
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/
            
            CASE @SortDirection
                 WHEN 'ASC'  THEN
                      CASE @SortIndex
           				  WHEN 1 then VTDNO
			  			  WHEN 2 then CONTNRNO
					      WHEN 3 then CONTNRSDESC			  			  
						  WHEN 4 then CHANAME
						  WHEN 5 then EXPRTRNAME
						  WHEN 6 then VEHICLETICKETDETAIL.TRNSPRTNAME 
						  WHEN 7 then VEHICLETICKETDETAIL.VHLNO 
						  WHEN 8 then VEHICLETICKETDETAIL.DRVNAME  
						  WHEN 9 then VTSSEALNO  
					   END             
				 END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                   CASE @SortIndex
           			    WHEN 1 then VTDNO
			  			  WHEN 2 then CONTNRNO
					      WHEN 3 then CONTNRSDESC			  			  
						  WHEN 4 then CHANAME
						  WHEN 5 then EXPRTRNAME
						  WHEN 6 then VEHICLETICKETDETAIL.TRNSPRTNAME 
						  WHEN 7 then VEHICLETICKETDETAIL.VHLNO 
						  WHEN 8 then VEHICLETICKETDETAIL.DRVNAME  
						  WHEN 9 then VTSSEALNO  
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
            END DESC,
			CASE @SortDirection
              WHEN 'ASC'  THEN
                   CASE @SortIndex
                     WHEN 10 THEN VEHICLETICKETDETAIL.DISPSTATUS
                   END             
            END ASC,
			CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 10 THEN VEHICLETICKETDETAIL.DISPSTATUS
                END
            END DESC
            
            ) AS RowNum

            


FROM      VEHICLETICKETDETAIL  (nolock) Inner Join
          GATEINDETAIL (nolock) ON VEHICLETICKETDETAIL.GIDID = GATEINDETAIL.GIDID Inner Join 
		  CONTAINERSIZEMASTER (nolock) ON GATEINDETAIL.CONTNRSID = CONTAINERSIZEMASTER.CONTNRSID  Left Outer Join 
		  GATEOUTDETAIL (nolock) ON VEHICLETICKETDETAIL.GIDID = GATEOUTDETAIL.GIDID 

  WHERE (dbo.VEHICLETICKETDETAIL.COMPYID = @LCompyId) and  
  (VTDATE BETWEEN @PSDate AND @PEDate) AND (VEHICLETICKETDETAIL.SDPTID=11)  
   AND (@FilterTerm IS NULL 
             OR VTDATE LIKE @FilterTerm
              OR VTDNO LIKE @FilterTerm
              OR CONTNRNO  LIKE @FilterTerm
              OR CONTNRSDESC    LIKE @FilterTerm              
              OR CHANAME    LIKE @FilterTerm 
			    OR EXPRTRNAME    LIKE @FilterTerm 
				OR VEHICLETICKETDETAIL.TRNSPRTNAME    LIKE @FilterTerm 
				OR VEHICLETICKETDETAIL.VHLNO    LIKE @FilterTerm 
				OR VEHICLETICKETDETAIL.DRVNAME    LIKE @FilterTerm 
				OR VEHICLETICKETDETAIL.VTSSEALNO    LIKE @FilterTerm 
              
			  
		)

  ORDER BY VTDNO 


	update a
	set Edchk = case when t.GODID is null then 'Not-Exists' else 'Exists' end
	from @TableMaster a 
	left join GATEOUTDETAIL t(nolock) on  a.GIDID = t.GIDID and t.SDPTID = 11
	
	update @TableMaster
	set Edchk = 'Not-Exists' 	
	where Edchk  is null

    SELECT   
	VTDATE, VTDNO, CONTNRNO, CONTNRSDESC,  CHANAME,EXPRTRNAME,TRNSPRTNAME,  VHLNO,DRVNAME, VTSSEALNO,
	(CASE WHEN DISPSTATUS = 1 THEN 'C' ELSE 'In Books' END)  AS DISPSTATUS, GIDId, VTDID   , Edchk
    FROM    @TableMaster
    WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum
	ORDER BY VTDNO  DESC
    

SELECT    @TotalRowsCount = COUNT(*)
FROM VEHICLETICKETDETAIL (nolock) 
WHERE  dbo.VEHICLETICKETDETAIL.SDPTID = 11 
AND  dbo.VEHICLETICKETDETAIL.COMPYID = @LCompyId
AND VTDATE BETWEEN @LSDate AND @LEDate


    
    SELECT @FilteredRowsCount = COUNT(*)
    FROM   @TableMaster
        
END



