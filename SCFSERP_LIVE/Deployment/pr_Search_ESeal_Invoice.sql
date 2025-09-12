alter PROCEDURE [dbo].[pr_Search_ESeal_Invoice]  /* CHANGE */
 @FilterTerm nvarchar(250) = NULL --parameter to search all columns by
        , @SortIndex INT = 1 -- a one based index to indicate which column to order by
        , @SortDirection CHAR(4) = 'ASC' --the direction to sort in, either ASC or DESC
        , @StartRowNum INT = 1 --the first row to return
        , @EndRowNum INT = 10 --the last row to return
        , @TotalRowsCount INT OUTPUT
        , @FilteredRowsCount INT OUTPUT
        , @PCompyId INT,@PREGSTRID int
        , @PSDate Smalldatetime
        , @PEDate Smalldatetime
 
AS BEGIN

Declare @LCompyId int,@LSDate Smalldatetime, @LEDate Smalldatetime,@LREGSTRID int 
set @LCompyId = @PCompyId
set @LSDate = @PSDate
set @LEDate = @PEDate
set @LREGSTRID=@PREGSTRID


    --Wrap filter term with % to search for values that contain @FilterTerm
    SET @FilterTerm = '%' + @FilterTerm + '%'


    DECLARE @TableMaster TABLE
    (
	    TRANDATE VARCHAR(20)
	  , TRANTIME VARCHAR(20)
	  , TRANDNO VARCHAR(50)
	  , TRANREFID int
	  , TRANREFNAME  VARCHAR(100)
	  , EXPRTRNAME  VARCHAR(100)
	  , CONTNRNO  VARCHAR(50)
	  , CONTNRSDESC  VARCHAR(50)
	  , CONTNRTDESC  VARCHAR(50)
	  , TRANNAMT NUMERIC(18,2) 
	  , GSTAMT smallint
	  , DISPSTATUS int
	  , TAXTYPE int
	  , REGSTRID int
	  ,GIDID int
	  , TRANDID int 	 
	  , TRANMID int
	  , RowNum INT
    )
    INSERT INTO @TableMaster(TRANDATE,TRANTIME, TRANDNO,TRANREFID, TRANREFNAME,EXPRTRNAME,CONTNRNO,CONTNRSDESC,CONTNRTDESC,
	                         TRANNAMT, GSTAMT,DISPSTATUS, TAXTYPE,REGSTRID, GIDID, TRANDID, TRANMID,  RowNum)
  
    SELECT convert(char(10), TRANDATE, 103) AS [TRANDATE],convert(char(8), TRANTIME, 108) AS [TRANTIME], TRANDNO,TRANREFID, TRANREFNAME, 
	GATEINDETAIL.EXPRTRNAME,GATEINDETAIL.CONTNRNO,CONTAINERSIZEMASTER.CONTNRSDESC,CONTAINERTYPEMASTER.CONTNRTDESC,	
	TRANNAMT,TRANCGSTAMT+TRANSGSTAMT+TRANIGSTAMT,TRANSACTIONMASTER.DISPSTATUS,case when regstrid = 54 then 0 when regstrid = 55 then 1 end,
	TRANSACTIONMASTER.REGSTRID,
	GATEINDETAIL.GIDID,	TRANSACTIONDETAIL.TRANDID, TRANSACTIONMASTER.TRANMID			
            , Row_Number() OVER (
            ORDER BY
            
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/
            
            CASE @SortDirection
                 WHEN 'ASC'  THEN
                      CASE @SortIndex
           				  WHEN 2 then TRANDNO
			  			  WHEN 3 then TRANREFNAME
			  			  WHEN 4 then TRANREFNO
						  WHEN 5 then GATEINDETAIL.CONTNRNO
						  WHEN 6 then CONTAINERSIZEMASTER.CONTNRSDESC
						  WHEN 7 then CONTAINERTYPEMASTER.CONTNRTDESC
						  WHEN 8 then GATEINDETAIL.EXPRTRNAME
						  
					   END             
				 END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                   CASE @SortIndex
           			    WHEN 2 then TRANDNO
			  		    WHEN 3 then TRANREFNAME
			  			WHEN 4 then TRANREFNO
						WHEN 5 then GATEINDETAIL.CONTNRNO
					    WHEN 6 then CONTAINERSIZEMASTER.CONTNRSDESC
						WHEN 7 then CONTAINERTYPEMASTER.CONTNRTDESC
						WHEN 8 then GATEINDETAIL.EXPRTRNAME
                    END
               END DESC,
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                   CASE @SortIndex
                     WHEN 0 THEN TRANDATE
					 WHEN 1 THEN TRANTIME
                   END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 0 THEN TRANDATE
				  WHEN 1 THEN TRANTIME
                END
            END DESC
			/*,
			  CASE @SortDirection
                 WHEN 'ASC'  THEN
                      CASE @SortIndex
           				  WHEN 6 then TRANNAMT
			  			 
						  
					   END             
				 END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                   CASE @SortIndex
           			    WHEN 6 then TRANNAMT
                    END
               END DESC*/
            
            ) AS RowNum
            
FROM  TRANSACTIONMASTER (nolock) Inner Join  TRANSACTIONDETAIL (nolock) On TRANSACTIONMASTER.TRANMID = TRANSACTIONDETAIL.TRANMID Left JOIn 
GATEINDETAIL (NOLOCK) on TRANSACTIONDETAIL.TRANDREFID = GATEINDETAIL.GIDID  and TRANSACTIONMASTER.SDPTID = GATEINDETAIL.SDPTID LEFT JOIN 
CONTAINERSIZEMASTER (nolock) On GATEINDETAIL.CONTNRSID = CONTAINERSIZEMASTER.CONTNRSID Left Join 
CONTAINERTYPEMASTER (nolock) On GATEINDETAIL.CONTNRTID = CONTAINERTYPEMASTER.CONTNRTID   

WHERE  (TRANSACTIONMASTER.SDPTID = 11) AND   (TRANSACTIONMASTER.COMPYID = @LCompyId) and (REGSTRID=@LREGSTRID) AND  --(TRANBTYPE=@LTRANBTYPE or @LTRANBTYPE = 0) AND
(TRANDATE BETWEEN @PSDate AND @PEDate)  AND (@FilterTerm IS NULL 
             OR TRANDATE LIKE @FilterTerm
			  OR TRANTIME LIKE @FilterTerm
              OR TRANDNO LIKE @FilterTerm
              OR TRANREFNAME  LIKE @FilterTerm 
			  OR GATEINDETAIL.EXPRTRNAME  LIKE @FilterTerm  
			  OR GATEINDETAIL.CONTNRNO  LIKE @FilterTerm  
			  OR CONTAINERTYPEMASTER.CONTNRTDESC  LIKE @FilterTerm
			  OR CONTAINERSIZEMASTER.CONTNRSDESC  LIKE @FilterTerm
			  OR TRANNAMT LIKE @FilterTerm)
			   
 ORDER BY TRANDNO
            
    
  SELECT TRANDATE, TRANTIME , TRANDNO , TRANREFID , TRANREFNAME , EXPRTRNAME, CONTNRNO , CONTNRSDESC , CONTNRTDESC , 
        TRANNAMT , GSTAMT , CASE DISPSTATUS WHEN 1 THEN 'C' WHEN 2 THEN 'RETURN' else '' end as DISPSTATUS , TAXTYPE ,
		REGSTRID,
		GIDID , TRANDID , TRANMID FROM    @TableMaster
    WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum
    ORDER BY TRANDNO DESC  

  SELECT  @TotalRowsCount = COUNT(*)  FROM   TRANSACTIONMASTER (nolock) Inner Join   
  TRANSACTIONDETAIL (nolock) On TRANSACTIONMASTER.TRANMID = TRANSACTIONDETAIL.TRANMID Left JOIn 
GATEINDETAIL (NOLOCK) on TRANSACTIONDETAIL.TRANDREFID = GATEINDETAIL.GIDID LEFT JOIN 
  CONTAINERSIZEMASTER (nolock) On GATEINDETAIL.CONTNRSID = CONTAINERSIZEMASTER.CONTNRSID Left Join 
  CONTAINERTYPEMASTER (nolock) On GATEINDETAIL.CONTNRTID = CONTAINERTYPEMASTER.CONTNRTID    
  WHERE (TRANSACTIONMASTER.SDPTID = 11) AND (TRANSACTIONMASTER.COMPYID = @LCompyId) and (REGSTRID=@LREGSTRID) 
    -- AND (TRANBTYPE=@LTRANBTYPE or @LTRANBTYPE = 0) 
	 AND(TRANDATE BETWEEN @LSDate AND @LEDate) 
	 
	 SELECT @FilteredRowsCount = COUNT(*)
    FROM   @TableMaster
        
END



