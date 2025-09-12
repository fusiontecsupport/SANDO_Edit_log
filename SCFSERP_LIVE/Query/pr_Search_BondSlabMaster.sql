/*

DECLARE @total INT,
        @filtered INT

EXEC [dbo].[pr_Search_BondSlabMaster] NULL, 1, 'ASC', 1, 10,  @total OUTPUT, @filtered OUTPUT,
344,1,3,9

SELECT @total, @filtered
*/

alter PROCEDURE [dbo].[pr_Search_BondSlabMaster]  /* CHANGE */
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
		BondSlabMaster.CONTNRSID,BondSlabMaster.YRDTYPE,BondSlabMaster.SDTYPE 
	  ,BondSlabMaster.HANDTYPE 
	  ,BondSlabMaster.WTYPE 
	  ,SLABMIN 
	  ,SLABMAX 
	  ,SLABAMT,BondSlabMaster.CHRGETYPE
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
                  
				  WHEN 1 THEN BondSlabMaster.CONTNRSID
	              WHEN 2 THEN BondSlabMaster.YRDTYPE
				  WHEN 3 THEN BondSlabMaster.SDTYPE 
				  WHEN 4 THEN BondSlabMaster.HANDTYPE
				  WHEN 5 THEN BondSlabMaster.WTYPE
	            WHEN 9 THEN BondSlabMaster.CHRGETYPE
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex                 
                   WHEN 1 THEN BondSlabMaster.CONTNRSID
	              WHEN 2 THEN BondSlabMaster.YRDTYPE
				  WHEN 3 THEN BondSlabMaster.SDTYPE 
				    WHEN 4 THEN BondSlabMaster.HANDTYPE
					 WHEN 5 THEN BondSlabMaster.WTYPE			
                 WHEN 9 THEN BondSlabMaster.CHRGETYPE
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

  FROM BondSlabMaster (nolock) INNER JOIN
                      dbo.VW_BOND_RATECARDMASTER_FLX_ASSGN_02 (nolock) ON dbo.BondSlabMaster.TARIFFMID = dbo.VW_BOND_RATECARDMASTER_FLX_ASSGN_02.TARIFFMID AND 
                      dbo.BondSlabMaster.SLABTID = dbo.VW_BOND_RATECARDMASTER_FLX_ASSGN_02.SLABTID AND 
                      dbo.BondSlabMaster.SLABMDATE = dbo.VW_BOND_RATECARDMASTER_FLX_ASSGN_02.XTRANSDATE AND					  
					  dbo.BondSlabMaster.CHRGETYPE = dbo.VW_BOND_RATECARDMASTER_FLX_ASSGN_02.CHRGETYPE  AND
					  dbo.BondSlabMaster.CONTNRSID = dbo.VW_BOND_RATECARDMASTER_FLX_ASSGN_02.CONTNRSID   AND 
					  dbo.BondSlabMaster.HTYPE = dbo.VW_BOND_RATECARDMASTER_FLX_ASSGN_02.HTYPE  AND
					  dbo.BondSlabMaster.HANDTYPE = dbo.VW_BOND_RATECARDMASTER_FLX_ASSGN_02.HANDTYPE  AND
					  dbo.BondSlabMaster.SDTYPE = dbo.VW_BOND_RATECARDMASTER_FLX_ASSGN_02.SDTYPE AND
					  dbo.BondSlabMaster.WTYPE = dbo.VW_BOND_RATECARDMASTER_FLX_ASSGN_02.WTYPE AND 
					  dbo.BondSlabMaster.YRDTYPE = dbo.VW_BOND_RATECARDMASTER_FLX_ASSGN_02.YRDTYPE
   WHERE
   -- FROM    dbo.VW_BOND_RATECARDMASTER_FLX_ASSGN
   --   WHERE  --   (SLABMDATE <=
      --                    (SELECT     MAX(TRANNDATE) AS Expr1
      --                      FROM          dbo.TRANSACTION_DAYEND_MASTER))  /* CHANGE  TABLE NAME */
	  
    (BondSlabMaster.TARIFFMID=@LTARIFFMID) 
	and (BondSlabMaster.SLABTID=@LSLABTID) 
	and (BondSlabMaster.CHRGETYPE=@LCHRGETYPE) 
	and (@FilterTerm IS NULL 
              OR SLABMDATE LIKE @FilterTerm              
			  OR BondSlabMaster.CONTNRSID LIKE @FilterTerm
			  OR BondSlabMaster.YRDTYPE LIKE @FilterTerm
              OR BondSlabMaster.SDTYPE LIKE @FilterTerm
			  OR BondSlabMaster.HANDTYPE LIKE @FilterTerm
              OR BondSlabMaster.WTYPE LIKE @FilterTerm
			  OR BondSlabMaster.SLABMIN LIKE @FilterTerm
              OR BondSlabMaster.SLABMAX LIKE @FilterTerm
			    OR BondSlabMaster.SLABAMT LIKE @FilterTerm
				OR BondSlabMaster.CHRGETYPE LIKE @FilterTerm)
    SELECT SLABMDATE
            ,CASE CONTNRSID WHEN 2 THEN 'NR' WHEN 3 THEN '20' WHEN 4 THEN '40'  WHEN 5 THEN '45' END  AS CONTNRSID
			,CASE YRDTYPE WHEN 1 THEN 'NOT REQUIRED' WHEN 2 THEN 'OPEN YARD' WHEN 3 THEN 'CLOSED YARD' END  AS YRDTYPE
			,CASE SDTYPE WHEN 0 THEN 'NR' WHEN 1 THEN 'R'  END  AS SDTYPE
			,CASE HTYPE WHEN 0 THEN 'NOT REQUIRED' WHEN 1 THEN 'Loading'  else 'Unloading'  END  AS HTYPE 
			--,CASE WTYPE  WHEN 1 THEN 'NR' WHEN 2 THEN 'PACKAGE' WHEN 3 THEN 'WEIGHT' WHEN 4 THEN 'SCRAP'  WHEN 5 THEN 'L.CARGO'  END  AS WTYPE
			,CASE WTYPE  WHEN 1 THEN 'NR' WHEN 2 THEN 'PACKAGE' WHEN 3 THEN 'SCRAP' WHEN 4 THEN 'PAPER'  WHEN 5 THEN 'FLT'  WHEN 6 THEN 'MANUAL'  END  AS WTYPE
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
    
  FROM BondSlabMaster INNER JOIN
                      dbo.VW_BOND_RATECARDMASTER_FLX_ASSGN_02 ON dbo.BondSlabMaster.TARIFFMID = dbo.VW_BOND_RATECARDMASTER_FLX_ASSGN_02.TARIFFMID AND 
                      dbo.BondSlabMaster.SLABTID = dbo.VW_BOND_RATECARDMASTER_FLX_ASSGN_02.SLABTID AND 
                      dbo.BondSlabMaster.SLABMDATE = dbo.VW_BOND_RATECARDMASTER_FLX_ASSGN_02.XTRANSDATE AND					  
					  dbo.BondSlabMaster.CHRGETYPE = dbo.VW_BOND_RATECARDMASTER_FLX_ASSGN_02.CHRGETYPE  AND
					  dbo.BondSlabMaster.CONTNRSID = dbo.VW_BOND_RATECARDMASTER_FLX_ASSGN_02.CONTNRSID   AND 
					  dbo.BondSlabMaster.HTYPE = dbo.VW_BOND_RATECARDMASTER_FLX_ASSGN_02.HTYPE  AND
					  dbo.BondSlabMaster.SDTYPE = dbo.VW_BOND_RATECARDMASTER_FLX_ASSGN_02.SDTYPE AND
					  dbo.BondSlabMaster.WTYPE = dbo.VW_BOND_RATECARDMASTER_FLX_ASSGN_02.WTYPE AND 
					  dbo.BondSlabMaster.YRDTYPE = dbo.VW_BOND_RATECARDMASTER_FLX_ASSGN_02.YRDTYPE and 
					  dbo.BondSlabMaster.HANDTYPE = dbo.VW_BOND_RATECARDMASTER_FLX_ASSGN_02.HANDTYPE
   WHERE
    (BondSlabMaster.TARIFFMID=@LTARIFFMID) 
	and (BondSlabMaster.SLABTID=@LSLABTID) 
	and (BondSlabMaster.CHRGETYPE=@LCHRGETYPE) 
    

 --   SELECT @TotalRowsCount = COUNT(*)
 --   FROM   dbo.VW_BOND_RATECARDMASTER_FLX_ASSGN  
 --   WHERE   --  (dbo.BondSlabMaster.SLABMDATE <=
 --                      --   (SELECT     MAX(TRANNDATE) AS Expr1
 --                       --    FROM          dbo.TRANSACTION_DAYEND_MASTER))
 --/* CHANGE  TABLE NAME */
 --     (TARIFFMID=@LTARIFFMID) and (SLABTID=@LSLABTID) and (CHRGETYPE=@LCHRGETYPE) 


    SELECT @FilteredRowsCount = COUNT(*)
    FROM   @TableMaster
        
END

--  exec pr_Search_BondSlabMaster @FilterTerm ='', @SortIndex  = 1 , @SortDirection = 'ASC', @StartRowNum = 1 , @EndRowNum = 10 , @TotalRowsCount =100,  @FilteredRowsCount=100,@PTARIFFMID =287,@PCHRGETYPE =2,@PSLABTID =2
