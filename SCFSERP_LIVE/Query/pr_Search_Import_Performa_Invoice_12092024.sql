ALTER PROCEDURE [dbo].[pr_Search_Import_Performa_Invoice]  /* CHANGE */  
 @FilterTerm nvarchar(250) = NULL --parameter to search all columns by  
        , @SortIndex INT = 1 -- a one based index to indicate which column to order by  
        , @SortDirection CHAR(4) = 'ASC' --the direction to sort in, either ASC or DESC  
        , @StartRowNum INT = 1 --the first row to return  
        , @EndRowNum INT = 10 --the last row to return  
        , @TotalRowsCount INT OUTPUT  
        , @FilteredRowsCount INT OUTPUT  
        , @PCompyId INT  
  ,@PTRANBTYPE int  
  ,@PREGSTRID int  
        , @PSDate Smalldatetime  
        , @PEDate Smalldatetime  
   
AS BEGIN  
  
Declare @LCompyId int,@LTRANBTYPE int,@LREGSTRID int,@LSDate Smalldatetime, @LEDate Smalldatetime  
set @LCompyId = @PCompyId  
set @LTRANBTYPE=@PTRANBTYPE  
set @LREGSTRID=@PREGSTRID  
set @LSDate = @PSDate  
set @LEDate = @PEDate  
  
    --Wrap filter term with % to search for values that contain @FilterTerm  
    SET @FilterTerm = '%' + @FilterTerm + '%'  
  
  
    DECLARE @TableMaster TABLE  
    (  
      TRANDATE SMALLDATETIME  
    , TRANTIME datetime  
    , TRANDNO  VARCHAR(25)  
    , TRANNO  int  
    , TRANREFNAME VARCHAR(100)  
 , NOC int  
 , TRANNAMT numeric(18,2)  
 ,DISPSTATUS smallint  
 ,GSTAMT smallint  
 , TRANMID INT  
    , RowNum INT  
 , ACKNO varchar(50)  
 , dono int  
    )  
    INSERT INTO @TableMaster(TRANDATE, TRANTIME, TRANDNO , TRANNO,TRANREFNAME,NOC,TRANNAMT,DISPSTATUS, GSTAMT,TRANMID, RowNum, ACKNO,dono)  
    
    SELECT TRANDATE, TRANTIME,    
   TRANDNO, TRANNO,isnull(TRANREFNAME, TRANREFNAME),0,TRANNAMT,DISPSTATUS, TRANCGSTAMT+TRANSGSTAMT+TRANIGSTAMT, TRANMID  
  
            , Row_Number() OVER (  
            ORDER BY  
              
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/  
              
            CASE @SortDirection  
                 WHEN 'ASC'  THEN  
                      CASE @SortIndex  
                 
          WHEN 2 then TRANDNO  
           WHEN 3 then TRANNO  
          WHEN 4 then isnull(TRANCSNAME, TRANREFNAME)  
        WHEN 6 then TRANNAMT  
        END               
     END ASC,  
            CASE @SortDirection  
              WHEN 'DESC' THEN   
                   CASE @SortIndex  
                  
           WHEN 2 then TRANDNO  
          WHEN 3 then TRANNO  
          WHEN 4 then isnull(TRANCSNAME, TRANREFNAME)  
        WHEN 6 then TRANNAMT  
                    END  
               END DESC,  
              
            CASE @SortDirection  
              WHEN 'ASC'  THEN  
                   CASE @SortIndex  
                     WHEN 0 THEN TRANDATE  
      WHEN 1 then TRANTIME  
                   END               
            END ASC,  
            CASE @SortDirection  
              WHEN 'DESC' THEN   
                CASE @SortIndex  
                  WHEN 0 THEN TRANDATE  
      WHEN 1 then TRANTIME  
                END  
            END DESC  
              
            ) AS RowNum, ACKNO, isnull(tranlmid,0)  
              
FROM       PERFORMA_TRANSACTIONMASTER  
  
WHERE (dbo.PERFORMA_TRANSACTIONMASTER.COMPYID = @LCompyId) and (PERFORMA_TRANSACTIONMASTER.REGSTRID=@LREGSTRID)   
 and (PERFORMA_TRANSACTIONMASTER.TRANBTYPE=@LTRANBTYPE)   
 and (TRANDATE BETWEEN @LSDate AND @LEDate) AND (PERFORMA_TRANSACTIONMASTER.SDPTID=1) AND (@FilterTerm IS NULL   
             OR TRANDATE LIKE @FilterTerm  
              OR TRANTIME LIKE @FilterTerm  
              OR TRANDNO  LIKE @FilterTerm  
              OR TRANNO    LIKE @FilterTerm  
              oR isnull(TRANCSNAME, TRANREFNAME) LIKE @FilterTerm  
              OR TRANNAMT    LIKE @FilterTerm  
             )  
        
              
    SELECT TRANDATE, TRANTIME, TRANDNO , TRANNO,TRANREFNAME,NOC,TRANNAMT, TRANMID,  
       case DISPSTATUS when 0 then '' when 1 then 'C' end as DISPSTATUS, GSTAMT,   
    ACKNO, dono  
    FROM    @TableMaster  
    WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum  
    ORDER BY TRANNO DESC   
  
SELECT    @TotalRowsCount = COUNT(*)  
FROM       PERFORMA_TRANSACTIONMASTER  
  
WHERE (dbo.PERFORMA_TRANSACTIONMASTER.COMPYID = @LCompyId) and (PERFORMA_TRANSACTIONMASTER.REGSTRID=@LREGSTRID) and   
(PERFORMA_TRANSACTIONMASTER.TRANBTYPE=@LTRANBTYPE) and  
(TRANDATE BETWEEN @LSDate AND @LEDate) AND (PERFORMA_TRANSACTIONMASTER.SDPTID=1)  
  
      
    SELECT @FilteredRowsCount = COUNT(*)  
    FROM   @TableMaster  
          
END