-- =============================================
-- Author:		<Yamuna.J>
-- Create date: <24/07/2021>
-- Description:	<Export Gate In details>
-- =============================================
alter PROCEDURE [dbo].[pr_Search_ESeal_GateIn]  /* CHANGE */
 @FilterTerm nvarchar(250) = NULL --parameter to search all columns by
        , @SortIndex INT = 1 -- a one based index to indicate which column to order by
        , @SortDirection CHAR(4) = 'ASC' --the direction to sort in, either ASC or DESC
        , @StartRowNum INT = 1 --the first row to return
        , @EndRowNum INT = 10 --the last row to return
        , @TotalRowsCount INT OUTPUT
        , @FilteredRowsCount INT OUTPUT
        , @PSDate Smalldatetime
        , @PEDate Smalldatetime
		,@PCOMPYID INT
		
 
AS BEGIN
    --Wrap filter term with % to search for values that contain @FilterTerm
    SET @FilterTerm = '%' + @FilterTerm + '%'
	declare @LSDate Smalldatetime
        , @LEDate Smalldatetime,@LCOMPYID INT
		set @LSDate=@PSDate
		set @LEDate=@PEDate
		set @LCOMPYID=@PCOMPYID
    DECLARE @TableMaster TABLE
    (
      GIDATE SMALLDATETIME
      ,GITIME DATETIME
      , GINO int
      , CONTNRNO  VARCHAR(100)
	  ,CONTNRSID int
      , CHANAME  VARCHAR(100)
	  , EXPORTER  VARCHAR(100)
	  ,VHLNO VARCHAR(25)
	  ,DRVNAME VARCHAR(100)
	   ,DISPSTATUS smallint,GIDNO VARCHAR(100)
	  , GIDID int 
	  , Edchk varchar(15)
      , RowNum INT
    )

    INSERT INTO @TableMaster(
	  GIDATE
	  ,GITIME
      , GINO 
      , CONTNRNO  ,CONTNRSID ,CHANAME,EXPORTER
	,VHLNO
	 ,DRVNAME
	   ,DISPSTATUS,GIDNO
	  , GIDID
        , RowNum)
    SELECT  GIDATE
      , GITIME
      , GINO 
      , CONTNRNO ,CONTNRSID  ,CHANAME,GATEINDETAIL.EXPRTRNAME AS EXPORTER,VHLNO
	 ,DRVNAME
	    ,GATEINDETAIL.DISPSTATUS,GIDNO
	  , GIDID
            , Row_Number() OVER (
            ORDER BY
            
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
           	      WHEN 2 then GINO
			  	  WHEN 3 then CONTNRNO
				    WHEN 5 then CHANAME
			  	     WHEN 6 then VHLNO
					     WHEN 7 then DRVNAME
						 WHEN 9 THEN GIDNO
                END             
            END ASC,
            
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
           	      WHEN 2 then GINO
			  	  WHEN 3 then CONTNRNO
				   WHEN 5 then CHANAME
			  	 WHEN 6 then VHLNO
					     WHEN 7 then DRVNAME
						  WHEN 9 THEN GIDNO
                END
            END DESC,
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
                  WHEN 0 THEN GIDATE
                  WHEN 1 THEN GITIME
                END             
            END ASC,

            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 0 THEN GIDATE
				  when 1 then GITIME
                END
            END DESC,
            
       
			  CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex  
                
				   WHEN 4 then CONTNRSID
                END             
            END ASC,

            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
				
				    WHEN 4 then CONTNRSID
                END
            END DESC                             

            ) AS RowNum
   FROM         dbo.GATEINDETAIL   /* CHANGE  TABLE NAME */
    WHERE (SDPTID=11) And DISPSTATUS=0 
	        and (COMPYID=@LCOMPYID) and  (GIDATE BETWEEN @LSDate AND @LEDate) AND (@FilterTerm IS NULL 
              OR GIDATE LIKE @FilterTerm
              OR GITIME LIKE @FilterTerm
              OR GINO LIKE @FilterTerm
              OR  CONTNRNO  LIKE @FilterTerm
			  OR  CONTNRSID LIKE @FilterTerm
			    OR  CHANAME  LIKE @FilterTerm
				  OR  VHLNO LIKE @FilterTerm
			    OR  DRVNAME  LIKE @FilterTerm
				OR GIDNO LIKE @FilterTerm
           )
	
	update a
	set Edchk = case when t.VTDID is null then 'Not-Exists' else 'Exists' end
	from @TableMaster a 
	left join VEHICLETICKETDETAIL t(nolock) on  a.GIDID = t.GIDID and t.SDPTID = 11
	
	update @TableMaster
	set Edchk = 'Not-Exists' 	
	where Edchk  is null

    SELECT GIDATE
      , GITIME
      , GINO 
      , CONTNRNO ,case CONTNRSID when 1 then 'ALL' when 2 then 'NOT REQUIRED' when 3 then '20'  when 4 then '40' when 5 then '45' end as  CONTNRSID,CHANAME , EXPORTER,VHLNO
	 ,DRVNAME
	    ,case DISPSTATUS when 1 then 'C' end as  DISPSTATUS,GIDNO
	  , GIDID, Edchk
    FROM    @TableMaster
    WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum
    ORDER BY GIDNO DESC

    SELECT @TotalRowsCount = COUNT(*)
   FROM        dbo.GATEINDETAIL  /* CHANGE  TABLE NAME */
  WHERE (SDPTID=11)  And DISPSTATUS=0 
	        and (COMPYID=@LCOMPYID) and  (GIDATE BETWEEN @LSDate AND @LEDate) 
    
    SELECT @FilteredRowsCount = COUNT(*)
    FROM   @TableMaster where DISPSTATUS = 0
        
END


