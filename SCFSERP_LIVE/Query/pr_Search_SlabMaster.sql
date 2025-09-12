/*

DECLARE @total INT,
        @filtered INT

EXEC [dbo].[pr_Search_SlabMaster] NULL, 1, 'ASC', 1, 10,  @total OUTPUT, @filtered OUTPUT,
344,1,3,9

SELECT @total, @filtered
*/

ALTER PROCEDURE [dbo].[pr_Search_SlabMaster]  /* CHANGE */
 @FilterTerm nvarchar(250) = NULL --parameter to search all columns by
        , @SortIndex INT = 1 -- a one based index to indicate which column to order by
        , @SortDirection CHAR(4) = 'ASC' --the direction to sort in, either ASC or DESC
        , @StartRowNum INT = 1 --the first row to return
        , @EndRowNum INT = 10 --the last row to return
        , @TotalRowsCount INT OUTPUT
        , @FilteredRowsCount INT OUTPUT
		,@PTARIFFMID INT
		,@PCHRGETYPE INT
		,@PSLABTID INT
		,@PSDPTTYPEID INT
 
AS BEGIN
    --Wrap filter term with % to search for values that contain @FilterTerm
    SET @FilterTerm = '%' + @FilterTerm + '%'
	declare @LTARIFFMID INT
		,@LCHRGETYPE INT
		,@LSLABTID INT

		set @LTARIFFMID=@PTARIFFMID
		set @LCHRGETYPE=@PCHRGETYPE
		set @LSLABTID=@PSLABTID

    DECLARE @TableMaster TABLE
    ( 
	SLABMDATE smalldatetime
      ,CONTNRSID int
	  ,YRDTYPE smallint
	  ,SDTYPE smallint
	  ,HTYPE smallint
	  ,WTYPE smallint
	  ,SLABMIN numeric(18,2)
	  ,SLABMAX numeric(18,2)
	  ,SLABAMT numeric(18,2)
	  ,CHRGETYPE smallint
	  , SLABMID int 
      , RowNum INT
    )

    INSERT INTO @TableMaster(SLABMDATE
	   ,CONTNRSID,YRDTYPE,SDTYPE 
	  ,HTYPE 
	  ,WTYPE 
	  ,SLABMIN 
	  ,SLABMAX 
	  ,SLABAMT,CHRGETYPE,SLABMID, RowNum)
    SELECT  SLABMDATE,
		SLABMASTER.CONTNRSID,SLABMASTER.YRDTYPE,SLABMASTER.SDTYPE 
	  ,SLABMASTER.HTYPE 
	  ,SLABMASTER.WTYPE 
	  ,SLABMIN 
	  ,SLABMAX 
	  ,SLABAMT,SLABMASTER.CHRGETYPE
            , SLABMID
            , Row_Number() OVER (
            ORDER BY
            
            /*DATETIME ORDER BY*/
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
                  WHEN 0 THEN SLABMDATE
                 
	 
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 0 THEN SLABMDATE
                 
               
                END
            END DESC, 
            
            /*VARCHAR ORDER BY*/
              CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex                
                  
				  WHEN 1 THEN SLABMASTER.CONTNRSID
	              WHEN 2 THEN SLABMASTER.YRDTYPE
				  WHEN 3 THEN SLABMASTER.SDTYPE 
				  WHEN 4 THEN SLABMASTER.HTYPE
				  WHEN 5 THEN SLABMASTER.WTYPE
	            WHEN 9 THEN SLABMASTER.CHRGETYPE
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex                 
                   WHEN 1 THEN SLABMASTER.CONTNRSID
	              WHEN 2 THEN SLABMASTER.YRDTYPE
				  WHEN 3 THEN SLABMASTER.SDTYPE 
				    WHEN 4 THEN SLABMASTER.HTYPE
					 WHEN 5 THEN SLABMASTER.WTYPE			
                 WHEN 9 THEN SLABMASTER.CHRGETYPE
                END
            END DESC,

			    CASE @SortDirection
              WHEN 'ASC'  THEN
 CASE @SortIndex
                
          
	              WHEN 6 THEN SLABMIN
				  WHEN 7 THEN SLABMAX 
				   WHEN 8 THEN SLABAMT 
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                 
                  
	              WHEN 6 THEN SLABMIN
				  WHEN 7 THEN SLABMAX 
				   WHEN 8 THEN SLABAMT 		
               
                END
            END DESC
            
            ) AS RowNum

  FROM SLABMASTER INNER JOIN
                      dbo.VW_IMPORT_RATECARDMASTER_FLX_ASSGN_02 ON dbo.SLABMASTER.TARIFFMID = dbo.VW_IMPORT_RATECARDMASTER_FLX_ASSGN_02.TARIFFMID AND 
                      dbo.SLABMASTER.SLABTID = dbo.VW_IMPORT_RATECARDMASTER_FLX_ASSGN_02.SLABTID AND 
                      dbo.SLABMASTER.SLABMDATE = dbo.VW_IMPORT_RATECARDMASTER_FLX_ASSGN_02.XTRANSDATE AND
					  dbo.SLABMASTER.SDPTTYPEID = dbo.VW_IMPORT_RATECARDMASTER_FLX_ASSGN_02.SDPTTYPEID  AND 
					  dbo.SLABMASTER.CHRGETYPE = dbo.VW_IMPORT_RATECARDMASTER_FLX_ASSGN_02.CHRGETYPE  AND
					  dbo.SLABMASTER.CONTNRSID = dbo.VW_IMPORT_RATECARDMASTER_FLX_ASSGN_02.CONTNRSID   AND 
					  dbo.SLABMASTER.HTYPE = dbo.VW_IMPORT_RATECARDMASTER_FLX_ASSGN_02.HTYPE  AND
					  dbo.SLABMASTER.SDTYPE = dbo.VW_IMPORT_RATECARDMASTER_FLX_ASSGN_02.SDTYPE AND
					  dbo.SLABMASTER.WTYPE = dbo.VW_IMPORT_RATECARDMASTER_FLX_ASSGN_02.WTYPE AND 
					  dbo.SLABMASTER.YRDTYPE = dbo.VW_IMPORT_RATECARDMASTER_FLX_ASSGN_02.YRDTYPE
   WHERE
   -- FROM    dbo.VW_IMPORT_RATECARDMASTER_FLX_ASSGN
   --   WHERE  --   (SLABMDATE <=
      --                    (SELECT     MAX(TRANNDATE) AS Expr1
      --                      FROM          dbo.TRANSACTION_DAYEND_MASTER))  /* CHANGE  TABLE NAME */
	  --CODE ADDED BY RAJESH ON 28-07-2021 <S>
	  (SLABMASTER.SDPTTYPEID = @PSDPTTYPEID) AND
	 --CODE ADDED BY RAJESH ON 28-07-2021 <E>
    (SLABMASTER.TARIFFMID=@LTARIFFMID) 
	and (SLABMASTER.SLABTID=@LSLABTID) 
	and (SLABMASTER.CHRGETYPE=@LCHRGETYPE) 
	and (@FilterTerm IS NULL 
              OR SLABMDATE LIKE @FilterTerm              
			  OR SLABMASTER.CONTNRSID LIKE @FilterTerm
			  OR SLABMASTER.YRDTYPE LIKE @FilterTerm
              OR SLABMASTER.SDTYPE LIKE @FilterTerm
			  OR SLABMASTER.HTYPE LIKE @FilterTerm
              OR SLABMASTER.WTYPE LIKE @FilterTerm
			  OR SLABMASTER.SLABMIN LIKE @FilterTerm
              OR SLABMASTER.SLABMAX LIKE @FilterTerm
			    OR SLABMASTER.SLABAMT LIKE @FilterTerm
				OR SLABMASTER.CHRGETYPE LIKE @FilterTerm)
    SELECT SLABMDATE
            ,CASE CONTNRSID WHEN 2 THEN 'NR' WHEN 3 THEN '20' WHEN 4 THEN '40'  WHEN 5 THEN '45' END  AS CONTNRSID
			,CASE YRDTYPE WHEN 1 THEN 'NOT REQUIRED' WHEN 2 THEN 'OPEN YARD' WHEN 3 THEN 'CLOSED YARD' END  AS YRDTYPE
			,CASE SDTYPE WHEN 0 THEN 'NR' WHEN 1 THEN 'R'  END  AS SDTYPE
			,CASE HTYPE WHEN 1 THEN 'Nil' WHEN 2 THEN 'FLT'  WHEN 3 THEN 'TLT' WHEN 4 THEN 'CRANE'  WHEN 5 THEN 'MANUAL' WHEN 6 THEN 'OWN' else 'NR'  END  AS HTYPE 
			,CASE WTYPE  WHEN 1 THEN 'NR' WHEN 2 THEN 'PACKAGE' WHEN 3 THEN 'WEIGHT' WHEN 4 THEN 'SCRAP'  WHEN 5 THEN 'L.CARGO'  END  AS WTYPE
	  ,SLABMIN 
	  ,SLABMAX 
	  ,SLABAMT
	  ,CASE CHRGETYPE WHEN 1 THEN 'LD'  WHEN 2 THEN 'DS' END  AS CHRGETYPE
			 ,SLABMID
    FROM    @TableMaster
     WHERE    -- (SLABMDATE <=
                   --       (SELECT     MAX(TRANNDATE) AS Expr1
                     --       FROM          dbo.TRANSACTION_DAYEND_MASTER))
 /* CHANGE  TABLE NAME */
   -- AND 
       RowNum BETWEEN @StartRowNum AND @EndRowNum
	   ORDER BY SLABMDATE DESC, SLABMID
    
	SELECT @TotalRowsCount = COUNT(*)
    
  FROM SLABMASTER INNER JOIN
                      dbo.VW_IMPORT_RATECARDMASTER_FLX_ASSGN_02 ON dbo.SLABMASTER.TARIFFMID = dbo.VW_IMPORT_RATECARDMASTER_FLX_ASSGN_02.TARIFFMID AND 
                      dbo.SLABMASTER.SLABTID = dbo.VW_IMPORT_RATECARDMASTER_FLX_ASSGN_02.SLABTID AND 
                      dbo.SLABMASTER.SLABMDATE = dbo.VW_IMPORT_RATECARDMASTER_FLX_ASSGN_02.XTRANSDATE AND
					  dbo.SLABMASTER.SDPTTYPEID = dbo.VW_IMPORT_RATECARDMASTER_FLX_ASSGN_02.SDPTTYPEID  AND 
					  dbo.SLABMASTER.CHRGETYPE = dbo.VW_IMPORT_RATECARDMASTER_FLX_ASSGN_02.CHRGETYPE  AND
					  dbo.SLABMASTER.CONTNRSID = dbo.VW_IMPORT_RATECARDMASTER_FLX_ASSGN_02.CONTNRSID   AND 
					  dbo.SLABMASTER.HTYPE = dbo.VW_IMPORT_RATECARDMASTER_FLX_ASSGN_02.HTYPE  AND
					  dbo.SLABMASTER.SDTYPE = dbo.VW_IMPORT_RATECARDMASTER_FLX_ASSGN_02.SDTYPE AND
					  dbo.SLABMASTER.WTYPE = dbo.VW_IMPORT_RATECARDMASTER_FLX_ASSGN_02.WTYPE AND 
					  dbo.SLABMASTER.YRDTYPE = dbo.VW_IMPORT_RATECARDMASTER_FLX_ASSGN_02.YRDTYPE
   WHERE
	 --CODE ADDED BY RAJESH ON 28-07-2021 <S>
	  (SLABMASTER.SDPTTYPEID = @PSDPTTYPEID) AND
	 --CODE ADDED BY RAJESH ON 28-07-2021 <E>
    (SLABMASTER.TARIFFMID=@LTARIFFMID) 
	and (SLABMASTER.SLABTID=@LSLABTID) 
	and (SLABMASTER.CHRGETYPE=@LCHRGETYPE) 
    

 --   SELECT @TotalRowsCount = COUNT(*)
 --   FROM   dbo.VW_IMPORT_RATECARDMASTER_FLX_ASSGN  
 --   WHERE   --  (dbo.SLABMASTER.SLABMDATE <=
 --                      --   (SELECT     MAX(TRANNDATE) AS Expr1
 --                       --    FROM          dbo.TRANSACTION_DAYEND_MASTER))
 --/* CHANGE  TABLE NAME */
 --     (TARIFFMID=@LTARIFFMID) and (SLABTID=@LSLABTID) and (CHRGETYPE=@LCHRGETYPE) 


    SELECT @FilteredRowsCount = COUNT(*)
    FROM   @TableMaster
        
END

--  exec pr_Search_SlabMaster @FilterTerm ='', @SortIndex  = 1 , @SortDirection = 'ASC', @StartRowNum = 1 , @EndRowNum = 10 , @TotalRowsCount =100,  @FilteredRowsCount=100,@PTARIFFMID =287,@PCHRGETYPE =2,@PSLABTID =2






