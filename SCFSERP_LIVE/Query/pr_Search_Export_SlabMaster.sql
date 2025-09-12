/*

DECLARE @total INT,
        @filtered INT

EXEC [dbo].[pr_Search_Export_SlabMaster] NULL, 1, 'ASC', 1, 10,0,0,0,0,2

SELECT @total, @filtered
*/

ALTER PROCEDURE [dbo].[pr_Search_Export_SlabMaster]  /* CHANGE */
@FilterTerm nvarchar(250) = NULL --parameter to search all columns by
        , @SortIndex INT = 1 -- a one based index to indicate which column to order by
        , @SortDirection CHAR(4) = 'ASC' --the direction to sort in, either ASC or DESC
        , @StartRowNum INT = 1 --the first row to return
        , @EndRowNum INT = 10 --the last row to return
        , @TotalRowsCount INT = 0 OUTPUT
        , @FilteredRowsCount INT = 0 OUTPUT
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
	  ,EOPTID int
	  ,EOPTDESC VARCHAR(100)
	  , SLABMID int 
      , RowNum INT
    )

    INSERT INTO @TableMaster(SLABMDATE,
CONTNRSID,YRDTYPE,SDTYPE 
	  ,HTYPE 
	  ,WTYPE 
	  ,SLABMIN 
	  ,SLABMAX 
	  ,SLABAMT,CHRGETYPE,EOPTID,  EOPTDESC ,SLABMID, RowNum)
    SELECT  SLABMDATE
            , CONTNRSID,YRDTYPE,SDTYPE 
	  ,HTYPE 
	  ,WTYPE 
	  ,SLABMIN 
	  ,SLABMAX 
	  ,SLABAMT,CHRGETYPE, a.EOPTID, b.EOPTDESC
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
                
                  WHEN 1 THEN CONTNRSID
	              WHEN 2 THEN YRDTYPE
				  WHEN 3 THEN SDTYPE 
				    WHEN 4 THEN CHRGETYPE
					 WHEN 5 THEN B.EOPTDESC
	            WHEN 9 THEN CHRGETYPE
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                 
                   WHEN 1 THEN CONTNRSID
	              WHEN 2 THEN YRDTYPE
				  WHEN 3 THEN SDTYPE 
				    WHEN 4 THEN CHRGETYPE
					 WHEN 5 THEN B.EOPTDESC	
                 WHEN 6 THEN WTYPE
                END
            END DESC,

			    CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
                
               
	              WHEN 7 THEN SLABMIN
				  WHEN 8 THEN SLABMAX 
				   WHEN 9 THEN SLABAMT 
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                 
                  
	              WHEN 7 THEN SLABMIN
				  WHEN 8 THEN SLABMAX 
				   WHEN 9 THEN SLABAMT 		
               
                END
            END DESC
            
            ) AS RowNum
    FROM    dbo.VW_EXPORT_RATECARDMASTER_FLX_ASSGN a(nolock) join EXPORT_OPERATIONTYPEMASTER b(nolock) on a.EOPTID = b.EOPTID
	 /* CHANGE  TABLE NAME */
    WHERE    (TARIFFMID=@LTARIFFMID) and (SLABTID=@LSLABTID) and (CHRGETYPE=@LCHRGETYPE) and (  @FilterTerm IS NULL 
              OR SLABMDATE LIKE @FilterTerm
              OR CONTNRSID LIKE @FilterTerm
			  OR  YRDTYPE LIKE @FilterTerm
              OR SDTYPE LIKE @FilterTerm
			  OR HTYPE LIKE @FilterTerm
              OR WTYPE LIKE @FilterTerm
			  OR SLABMIN LIKE @FilterTerm
              OR SLABMAX LIKE @FilterTerm
			    OR SLABAMT LIKE @FilterTerm
				OR CHRGETYPE LIKE @FilterTerm
				OR a.EOPTID LIKE @FilterTerm
				OR b.EOPTDESC LIKE @FilterTerm)
	group by SLABMDATE,CONTNRSID,YRDTYPE,HTYPE,WTYPE,SDTYPE,SLABMIN,SLABMAX,SLABAMT,CHRGETYPE,SLABMID, A.EOPTID, B.EOPTDESC
	order by SLABMDATE,CONTNRSID,YRDTYPE,HTYPE,WTYPE,SDTYPE,SLABMIN,SLABMAX,SLABAMT,CHRGETYPE, EOPTID

    SELECT ISNULL(SLABMDATE,'') AS SLABMDATE
            ,CASE CONTNRSID WHEN 2 THEN 'NR' WHEN 3 THEN '20' WHEN 4 THEN '40'  WHEN 5 THEN '45' END  AS CONTNRSID
			,CASE YRDTYPE WHEN 0 THEN 'NR' WHEN 1 THEN 'Open' WHEN 2 THEN 'Closed' END  AS YRDTYPE
			,CASE SDTYPE WHEN 0 THEN 'NR' WHEN 1 THEN 'R'  END  AS SDTYPE
	  ,CASE HTYPE WHEN 0 THEN 'NR' END  AS HTYPE
	  , CASE WTYPE  WHEN 0 THEN 'NR' WHEN 1 THEN 'PACKAGE' WHEN 2 THEN 'WEIGHT' WHEN 3 THEN 'SCRAP'  WHEN 4 THEN 'L.CARGO'  END  AS WTYPE
	  ,ISNULL(SLABMIN,0) AS  SLABMIN 
	  ,ISNULL(SLABMAX,0) AS  SLABMAX
	  ,ISNULL(SLABAMT,0) AS SLABAMT
	  ,CASE CHRGETYPE  WHEN 1 THEN 'STUFF'  WHEN 2 THEN 'GRT' WHEN 3 THEN 'SEAL'  WHEN 4 THEN 'EMPTY' WHEN 5 THEN 'SHUTOUT' END  AS CHRGETYPE
	  ,EOPTID, EOPTDESC, SLABMID
    FROM    @TableMaster
    WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum
    
    SELECT @TotalRowsCount = ISNULL(COUNT(*),0)
    FROM   dbo.VW_EXPORT_RATECARDMASTER_FLX_ASSGN   /* CHANGE  TABLE NAME */
    where (TARIFFMID=@LTARIFFMID) and (SLABTID=@LSLABTID) and (CHRGETYPE=@LCHRGETYPE) 	
	group by SLABMDATE,CATEDNAME,COMPYID,CONTNRSID,YRDTYPE,HTYPE,WTYPE,SDTYPE,SLABMIN,SLABMAX,SLABAMT,CHRGETYPE,SLABMID,TARIFFMID,SLABTID,CHAID,XTRANSDATE,
	CONTNRSCODE,SLABUAMT, EOPTID
	order by SLABMDATE,CONTNRSID,YRDTYPE,HTYPE,WTYPE,SDTYPE,SLABMIN,SLABMAX,SLABAMT,CHRGETYPE,TARIFFMID,SLABTID,CHAID

    SELECT @FilteredRowsCount = isnull(COUNT(*),0)
    FROM   @TableMaster

	select @TotalRowsCount = isnull(@totalrowscount,0), 
			@FilteredRowsCount = isnull(@FilteredRowsCount,0)
        
END

--EXEC [dbo].[pr_Search_Export_SlabMaster] '', 1, 'ASC', 1, 10,100,100,9,1,2

--select *from EXPORTSLABMASTER



