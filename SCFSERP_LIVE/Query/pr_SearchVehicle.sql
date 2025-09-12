USE [SCFS_ERP]
GO
/****** Object:  StoredProcedure [dbo].[pr_SearchVehicle]    Script Date: 20/10/2021 17:19:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*

DECLARE @TotalRowsCount INT,
        @FilteredRowsCount INT

EXEC [pr_SearchVehicle] NULL, 1, 'ASC', 0, 10,  @TotalRowsCount output, @FilteredRowsCount output

SELECT @total, @filtered
select * from sysobjects where name like 'pr_SearchVehicle'
*/

ALTER PROCEDURE [dbo].[pr_SearchVehicle]  /* CHANGE */
 @FilterTerm nvarchar(250) = NULL --parameter to search all columns by
        , @SortIndex INT = 1 -- a one based index to indicate which column to order by
        , @SortDirection CHAR(4) = 'ASC' --the direction to sort in, either ASC or DESC
        , @StartRowNum INT = 1 --the first row to return
        , @EndRowNum INT = 10 --the last row to return
        , @TotalRowsCount INT OUTPUT
        , @FilteredRowsCount INT OUTPUT
 
AS BEGIN
    --Wrap filter term with % to search for values that contain @FilterTerm
    SET @FilterTerm = '%' + @FilterTerm + '%'

    DECLARE @TableMaster TABLE
    ( 
	    VHLPNRNO VARCHAR(20)
	  , VHLMDESC VARCHAR(100)
      , CATENAME VARCHAR(100)
	  , DISPSTATUS smallint
	  , VHLMID int 
      , RowNum INT
    )

    INSERT INTO @TableMaster(
	VHLPNRNO, VHLMDESC,CATENAME,DISPSTATUS,VHLMID,RowNum)
    SELECT  isnull(VHLPNRNO,''), VHLMDESC
            ,isnull(CATEGORYMASTER.CATENAME,'') as  CATENAME,VEHICLEMASTER.DISPSTATUS
            , VHLMID
            , Row_Number() OVER (
            ORDER BY
            
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                CASE @SortIndex
                  WHEN 0 THEN VHLMDESC
                  WHEN 1 THEN CATEGORYMASTER.CATENAME
	 
                END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 0 THEN VHLMDESC
                  WHEN 1 THEN CATEGORYMASTER.CATENAME				
               
                END
            END DESC 
            
            /*DATETIME ORDER BY*/
             

            
            ) AS RowNum
    FROM    dbo.VEHICLEMASTER (nolock) 
	        Left Join CATEGORYMASTER  (nolock ) On dbo.VEHICLEMASTER.TRNSPRTID = CATEGORYMASTER.CATEID 
	          /* CHANGE  TABLE NAME */
    WHERE   @FilterTerm IS NULL 
              OR VHLMDESC LIKE @FilterTerm
              OR CATEGORYMASTER.CATENAME LIKE @FilterTerm
            

    SELECT VHLPNRNO, VHLMDESC , CATENAME , CASE DISPSTATUS WHEN 1 THEN 'Disabled' WHEN 0 THEN 'Enabled' END  AS DISPSTATUS ,VHLMID
    FROM    @TableMaster
    WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum
    
    SELECT @TotalRowsCount = COUNT(*)
    FROM   dbo.VEHICLEMASTER  (nolock) 
	        Left Join CATEGORYMASTER  (nolock ) On dbo.VEHICLEMASTER.TRNSPRTID = CATEGORYMASTER.CATEID  /* CHANGE  TABLE NAME */
    
    SELECT @FilteredRowsCount = COUNT(*)
    FROM   @TableMaster
        
END

