/*  
  
DECLARE @total INT,  
        @filtered INT  
  
EXEC [dbo].[pr_Search_CategoryMaster] NULL, 1, 'ASC', 1, 10,  @total OUTPUT, @filtered OUTPUT, 1  
  
SELECT @total, @filtered  
SELECT * FROM CATEGORYMASTER WHERE CATENAME = 'RAJ'  
*/  
  
alter PROCEDURE [dbo].[pr_Search_CategoryMaster]  /* CHANGE */  
 @FilterTerm nvarchar(250) = NULL --parameter to search all columns by  
        , @SortIndex INT = 1 -- a one based index to indicate which column to order by  
        , @SortDirection CHAR(4) = 'ASC' --the direction to sort in, either ASC or DESC  
        , @StartRowNum INT = 1 --the first row to return  
        , @EndRowNum INT = 10 --the last row to return  
        , @TotalRowsCount INT OUTPUT  
        , @FilteredRowsCount INT OUTPUT  
  ,@PCATETID INT  
   
AS BEGIN  
    --Wrap filter term with % to search for values that contain @FilterTerm  
  
    SET @FilterTerm = '%' + @FilterTerm + '%'  
 DECLARE @LCATETID INT  
 SET @LCATETID=@PCATETID  
    DECLARE @TableMaster TABLE  
    (   
      CATECODE VARCHAR(100),  
   CATENAME VARCHAR(100)  
   ,STATEDESC VARCHAR(100)  
      ,HSNDESC VARCHAR(100)  
   ,CATEGSTNO VARCHAR(50)  
   ,CATEPANNO VARCHAR(50)  
   ,CATETANNO VARCHAR(50)  
   ,CUSTGDESC VARCHAR(50) 
   ,DISPSTATUS SMALLINT  
   , CATEID int   
      , RowNum INT  
    )  
  
    INSERT INTO @TableMaster(CATECODE,CATENAME,STATEDESC,HSNDESC,CATEGSTNO,CATEPANNO,CATETANNO,CUSTGDESC,DISPSTATUS,CATEID , RowNum)  
    SELECT  CATECODE,CATENAME,STATEDESC,HSNDESC,CATEBGSTNO,CATEBPANNO,CATEBTANNO,ISNULL(CUSTGDESC,'Nil'),
	CATEGORYMASTER.DISPSTATUS, CATEID  
            , Row_Number() OVER (  
            ORDER BY  
              
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/  
              
            CASE @SortDirection  
              WHEN 'ASC'  THEN  
                CASE @SortIndex  
                  WHEN 0 THEN CATECODE  
                  WHEN 1 THEN CATENAME  
				  WHEN 2 THEN ISNULL(CUSTGDESC,'Nil')
            --      WHEN 1 THEN STATEDESC  
               --WHEN 2 THEN HSNCODE  
      --WHEN 3 THEN CATEGSTNO  
      --WHEN 4 THEN CATEPANNO  
      --WHEN 5 THEN CATETANNO  
  
                END               
            END ASC,  
            CASE @SortDirection  
              WHEN 'DESC' THEN   
                CASE @SortIndex  
                  WHEN 0 THEN CATECODE  
                  WHEN 1 THEN CATENAME  
				  WHEN 2 THEN ISNULL(CUSTGDESC,'Nil')
             --     WHEN 1 THEN STATEDESC  
      --         --WHEN 2 THEN HSNCODE  
      --WHEN 3 THEN CATEGSTNO  
      --WHEN 4 THEN CATEPANNO  
      --WHEN 5 THEN CATETANNO  
                 
                END  
            END DESC  
   
              
            /*DATETIME ORDER BY*/  
               
  
              
            ) AS RowNum  
    FROM dbo.CATEGORYMASTER (nolock)  /* CHANGE  TABLE NAME */  
	LEFT OUTER JOIN dbo.HSNCODEMASTER (nolock) ON dbo.CATEGORYMASTER.HSNID = dbo.HSNCODEMASTER.HSNID 
	LEFT OUTER JOIN dbo.STATEMASTER (nolock) ON dbo.CATEGORYMASTER.STATEID = dbo.STATEMASTER.STATEID 
	LEFT OUTER JOIN dbo.CUSTOMERGROUPMASTER (nolock) ON dbo.CATEGORYMASTER.CUSTGID = dbo.CUSTOMERGROUPMASTER.CUSTGID 
    WHERE   ( @FilterTerm IS NULL   
              OR CATECODE LIKE @FilterTerm  
     OR CATENAME LIKE @FilterTerm  
              OR STATEDESC LIKE @FilterTerm  
     OR HSNCODE LIKE @FilterTerm  
              OR CATEBGSTNO LIKE @FilterTerm  
     OR CATEBPANNO LIKE @FilterTerm  
     OR CATEBTANNO LIKE @FilterTerm
	 OR CUSTGDESC LIKE @FilterTerm)  
  
             AND (CATETID=@LCATETID)   
  
   
  
    SELECT CATECODE,CATENAME,STATEDESC,HSNDESC,CATEGSTNO,CATEPANNO,CATETANNO,CUSTGDESC  
   ,CASE DISPSTATUS WHEN 1 THEN 'Disabled' WHEN 0 THEN 'Enabled' END AS DISPSTATUS  
   , CATEID  
    FROM    @TableMaster  
 WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum  
      
    SELECT @TotalRowsCount = COUNT(*)  
    FROM dbo.CATEGORYMASTER (nolock)  /* CHANGE  TABLE NAME */  
	LEFT OUTER JOIN dbo.HSNCODEMASTER (nolock) ON dbo.CATEGORYMASTER.HSNID = dbo.HSNCODEMASTER.HSNID 
	LEFT OUTER JOIN dbo.STATEMASTER (nolock) ON dbo.CATEGORYMASTER.STATEID = dbo.STATEMASTER.STATEID 
	LEFT OUTER JOIN dbo.CUSTOMERGROUPMASTER (nolock) ON dbo.CATEGORYMASTER.CUSTGID = dbo.CUSTOMERGROUPMASTER.CUSTGID 
    WHERE (CATETID=@LCATETID)  
    SELECT @FilteredRowsCount = COUNT(*)  
    FROM   @TableMaster  
          
END  
  
  
  