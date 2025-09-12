/*
declare @trows int, @frows int
exec [pr_SearchOpenSheetGridAssgn] @FilterTerm=null,@SortIndex=1,@SortDirection='asc',@StartRowNum=1,
@EndRowNum=100, @TotalRowsCount= @trows output, @FilteredRowsCount= @frows output,
@PSDate='2022-08-01',@PEDate='2022-09-01',@PCompyid=31,@PUsrname='ck', @fcltype=0

declare @trows int, @frows int
exec [pr_SearchOpenSheetGridAssgn] @FilterTerm=null,@SortIndex=1,@SortDirection='asc',@StartRowNum=1,
@EndRowNum=100, @TotalRowsCount= @trows output, @FilteredRowsCount= @frows output,
@PSDate='2022-08-01',@PEDate='2022-09-01',@PCompyid=31,@PUsrname='importuser', @fcltype=0
*/
CREATE PROCEDURE [dbo].[pr_SearchOpenSheetGridAssgn]  /* CHANGE */
 @FilterTerm nvarchar(250) = NULL --parameter to search all columns by
        , @SortIndex INT = 1 -- a one based index to indicate which column to order by
        , @SortDirection CHAR(4) = 'ASC' --the direction to sort in, either ASC or DESC
        , @StartRowNum INT = 1 --the first row to return
        , @EndRowNum INT = 10 --the last row to return
        , @TotalRowsCount INT OUTPUT
        , @FilteredRowsCount INT OUTPUT
        , @PSDate Smalldatetime
        , @PEDate Smalldatetime
		, @PCompyid int
		, @PUsrname varchar(50)
		, @fcltype int
 
AS BEGIN
    --Wrap filter term with % to search for values that contain @FilterTerm
    SET @FilterTerm = '%' + @FilterTerm + '%'

	declare @LCompyid int, @grp varchar(100)
	set @LCompyid=@PCompyid

	select @grp = isnull(b.Name,'')
	from ApplicationUserGroups a(nolock) join Groups b(nolock) on a.GroupId = b.Id
		join AspNetUsers c(nolock) on a.UserId = c.Id
	where b.name like '%admin%'
	and c.UserName = @PUsrname

    DECLARE @TableMaster TABLE
    (
      OSMDATE SMALLDATETIME
      , OSMNO INT
      , OSMDNO VARCHAR(15)
	  ,OSMIGMNO VARCHAR(15)
	  ,OSMLNO VARCHAR(15)
      , OSMNAME VARCHAR(100)
	  ,BOENO VARCHAR(20)
	  ,BOEDATE SMALLDATETIME
	  ,OSMVSLNAME VARCHAR(100)
	  ,DISPSTATUS SMALLINT
	  , OSMID int 
	  ,OOCNOSTS varchar(10)
	  , DOSTS varchar(10)
      , RowNum INT
    )

    INSERT INTO @TableMaster(OSMDATE
                        , OSMNO
                        , OSMDNO
	  ,OSMIGMNO 
	  ,OSMLNO 
      , OSMNAME
	   ,BOENO 
	  ,BOEDATE,OSMVSLNAME ,DISPSTATUS
	 ,OSMID, OOCNOSTS, RowNum)
  
    SELECT  OSMDATE, OSMNO , OSMDNO
	  ,OSMIGMNO 
	  ,OSMLNO 
      , OSMNAME 
	  ,BOENO 
	  ,BOEDATE,OSMVSLNAME,DISPSTATUS, OSMID , case when OOCNO ='' or @grp like '%admin%' then '' else 'hide' end, Row_Number() OVER (
            ORDER BY
            
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
				
           	      WHEN 2 then OSMDNO
			  	 
				   WHEN 3 then OSMIGMNO
			  	  WHEN 4 then OSMLNO
				   WHEN 5 then OSMNAME
				   WHEN 6 then BOENO
				   when 8 then OSMVSLNAME
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex

           	     WHEN 2 then OSMDNO
			  	 
				 WHEN 3 then OSMIGMNO
			  	  WHEN 4 then OSMLNO
				   WHEN 5 then OSMNAME
				     WHEN 6 then BOENO
					 when 8 then OSMVSLNAME
                END
            END DESC,
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
                  WHEN 0 THEN OSMDATE
				   WHEN 7 then BOEDATE
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 0 THEN OSMDATE
				   WHEN 7 then BOEDATE
                END
            END DESC,
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
				    WHEN 1 then OSMNO
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
				  WHEN 1 then OSMNO
                END
            END DESC                            

            ) AS RowNum
   FROM         dbo.OPENSHEETMASTER  /* CHANGE  TABLE NAME */
    WHERE   (OSMDATE BETWEEN @PSDate AND @PEDate) and (OSMLDTYPE = @fcltype) and (COMPYID=@LCompyid) 
	AND (@FilterTerm IS NULL 
              OR OSMDATE LIKE @FilterTerm
              OR OSMNO LIKE @FilterTerm
              OR  OSMDNO  LIKE @FilterTerm
              OR  OSMIGMNO    LIKE @FilterTerm
			   OR OSMLNO LIKE @FilterTerm
			     OR BOENO LIKE @FilterTerm
              OR  OSMNAME  LIKE @FilterTerm
			  OR OSMVSLNAME LIKE @FilterTerm)
    
	update t
	set DOSTS = case when isnull(DELIVERYORDERDETAIL.DOMID,0) = 0 then 'Not-Exists' else 'Exists' end
	from	dbo.DELIVERYORDERMASTER INNER JOIN
			dbo.DELIVERYORDERDETAIL ON DELIVERYORDERDETAIL.DOMID=DELIVERYORDERMASTER.DOMID INNER JOIN
			dbo.BILLENTRYDETAIL ON DELIVERYORDERDETAIL.BILLEDID=BILLENTRYDETAIL.BILLEDID INNER JOIN
			dbo.GATEINDETAIL ON dbo.BILLENTRYDETAIL.GIDID = dbo.GATEINDETAIL.GIDID INNER JOIN
			dbo.opensheetdetail ON dbo.GATEINDETAIL.GIDID = dbo.opensheetdetail.GIDID inner join
			@TableMaster t on opensheetdetail.osmid = t.osmid

	update @TableMaster 
	set DOSTS ='Not-Exists'
	where DOSTS is null

	SELECT OSMDATE, OSMNO , OSMDNO
	  ,OSMIGMNO 
	  ,OSMLNO 
      , OSMNAME 
	  ,BOENO 
	  ,BOEDATE,OSMVSLNAME
	  ,CASE DISPSTATUS WHEN 1 THEN 'C' END AS DISPSTATUS
	  , OSMID, OOCNOSTS, DOSTS
    FROM    @TableMaster
    WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum
    
    SELECT @TotalRowsCount = isnull(COUNT(*),0)
    FROM         dbo.OPENSHEETMASTER   /* CHANGE  TABLE NAME */
    WHERE  (OSMDATE BETWEEN @PSDate AND @PEDate) and (OSMLDTYPE = @fcltype) and (COMPYID=@LCompyid)
    
    SELECT @FilteredRowsCount = isnull(COUNT(*),0)
    FROM   @TableMaster
        
END



