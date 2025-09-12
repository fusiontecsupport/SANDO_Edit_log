-- select * from BONDBONDVEHICLETICKETDETAIL

alter PROCEDURE [dbo].[pr_Search_Bond_Vehicle_Ticket]  /* CHANGE */
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
	, PRDTGDESC  VARCHAR(150)	
    , EBNDDNO VARCHAR(25)
	, BNDDNO VARCHAR(100)
	, VTQTY numeric(18,2)
	, VHLNO  VARCHAR(25)
	, VTDID int
	, DRIVERNAME VARCHAR(150)
	, DISPSTATUS smallint
    , RowNum INT
    )
    INSERT INTO @TableMaster(VTDATE, VTDNO, PRDTGDESC, EBNDDNO, BNDDNO, VTQTY, VHLNO, VTDID, DRIVERNAME, 
	  DISPSTATUS, RowNum)
  
    SELECT   dbo.BONDVEHICLETICKETDETAIL.VTDATE, dbo.BONDVEHICLETICKETDETAIL.VTDNO,  dbo.BONDPRODUCTGROUPMASTER.PRDTGDESC,
	dbo.EXBONDMASTER.EBNDDNO, dbo.BONDMASTER.BNDDNO,  BONDVEHICLETICKETDETAIL.VTQTY, dbo.BONDVEHICLETICKETDETAIL.VHLNO, 
	dbo.BONDVEHICLETICKETDETAIL.VTDID,BONDVEHICLETICKETDETAIL.DRVNAME,  dbo.BONDVEHICLETICKETDETAIL.DISPSTATUS
            , Row_Number() OVER (
            ORDER BY
            
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/
            
            CASE @SortDirection
                 WHEN 'ASC'  THEN
                      CASE @SortIndex
           				  WHEN 1 then VTDNO
			  			  WHEN 2 then PRDTGDESC
					      WHEN 3 then EBNDDNO
			  			  WHEN 4 then BNDDNO
						  WHEN 5 then DRVNAME
					   END             
				 END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                   CASE @SortIndex
           			     WHEN 1 then VTDNO
			  			  WHEN 2 then PRDTGDESC
					      WHEN 3 then EBNDDNO
			  			  WHEN 4 then BNDDNO
						  WHEN 5 then DRVNAME
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


FROM    dbo.BONDVEHICLETICKETDETAIL (nolock)  JOIN dbo.EXBONDMASTER (nolock)  ON dbo.BONDVEHICLETICKETDETAIL.EBNDID =dbo.EXBONDMASTER.EBNDID
		JOIN BONDMASTER (NOLOCK) ON EXBONDMASTER.BNDID = BONDMASTER.BNDID
		JOIN BONDPRODUCTGROUPMASTER (NOLOCK) ON EXBONDMASTER.PRDTGID = BONDPRODUCTGROUPMASTER.PRDTGID
		
WHERE (dbo.BONDVEHICLETICKETDETAIL.COMPYID = @LCompyId) 
and (VTDATE BETWEEN @PSDate AND @PEDate) AND (BONDVEHICLETICKETDETAIL.SDPTID=10) 
AND (@FilterTerm IS NULL 
             OR VTDATE LIKE @FilterTerm
              OR VTDNO LIKE @FilterTerm
			  OR PRDTGDESC    LIKE @FilterTerm
              oR EBNDDNO LIKE @FilterTerm
			  oR DRVNAME LIKE @FilterTerm
              OR BNDDNO    LIKE @FilterTerm
              OR VTQTY    LIKE @FilterTerm)			   
ORDER BY VTDNO 
            
    SELECT VTDATE, VTDNO,  PRDTGDESC, EBNDDNO, BNDDNO, VTQTY, VHLNO, VTDID, DRIVERNAME, 
	 (CASE WHEN DISPSTATUS = 1 THEN 'CANCELLED' ELSE 'IN BOOKS' END)  AS DISPSTATUS
    FROM    @TableMaster
    WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum
	ORDER BY VTDNO  DESC
    

	SELECT    @TotalRowsCount = COUNT(*)
	FROM BONDVEHICLETICKETDETAIL
	WHERE  dbo.BONDVEHICLETICKETDETAIL.SDPTID =10 
	AND  dbo.BONDVEHICLETICKETDETAIL.COMPYID = @LCompyId
	AND VTDATE BETWEEN @LSDate AND @LEDate

    
    SELECT @FilteredRowsCount = COUNT(*)
    FROM   @TableMaster
        
END


