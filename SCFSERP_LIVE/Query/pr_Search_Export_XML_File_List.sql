ALTER PROCEDURE [dbo].[pr_Search_Export_XML_File_List]  /* CHANGE */
 @FilterTerm nvarchar(250) = NULL --parameter to search all columns by
        , @SortIndex INT = 1 -- a one based index to indicate which column to order by
        , @SortDirection CHAR(4) = 'ASC' --the direction to sort in, either ASC or DESC
        , @StartRowNum INT = 1 --the first row to return
        , @EndRowNum INT = 10 --the last row to return
        , @TotalRowsCount INT OUTPUT
        , @FilteredRowsCount INT OUTPUT
        , @PXMLType INT
 
AS BEGIN

    --Wrap filter term with % to search for values that contain @FilterTerm
    SET @FilterTerm = '%' + @FilterTerm + '%'


    DECLARE @TableMaster TABLE
    (
      XMLFileName  VARCHAR(100)
    , XMLPath VARCHAR(max)
	, LoadedDateTime datetime
	, XMLStatus int
	, XMLId INT
    , RowNum INT
    )
    INSERT INTO @TableMaster(XMLFileName, XMLPath, LoadedDateTime, XMLStatus,XMLId, RowNum)
  
    SELECT XMLFileName, XMLPath, LoadedDateTime, XMLStatus, XMLId, Row_Number() OVER (ORDER BY
            
            /*VARCHAR, NVARCHAR, CHAR ORDER BY*/
            
            CASE @SortDirection
                 WHEN 'ASC'  THEN
                      CASE @SortIndex
			  			  WHEN 2 then XMLFileName
					      WHEN 3 then XMLPath
					   END             
				 END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                   CASE @SortIndex
			  		    WHEN 2 then XMLFileName
					    WHEN 3 then XMLPath

                    END
               END DESC,
            
            CASE @SortDirection
              WHEN 'ASC'  THEN
                   CASE @SortIndex
                     WHEN 0 THEN LoadedDateTime
                   END             
            END ASC,
            CASE @SortDirection
              WHEN 'DESC' THEN 
                CASE @SortIndex
                  WHEN 0 THEN LoadedDateTime
                END
            END DESC
            
            ) AS RowNum
            
FROM       XMLwithOpenXML 
WHERE (XMLStatus = 0) and (XMLType = 1) and (@FilterTerm IS NULL 
             OR XMLFileName LIKE @FilterTerm
              OR XMLPath LIKE @FilterTerm
              OR LoadedDateTime  LIKE @FilterTerm
             )
			   
            
    SELECT XMLFileName, XMLPath, LoadedDateTime,case XMLStatus when 0 then '' when 1 then '' end as XMLStatus, XMLId
    FROM    @TableMaster
    WHERE   RowNum BETWEEN @StartRowNum AND @EndRowNum
    ORDER BY XMLId DESC 

SELECT    @TotalRowsCount = COUNT(*)
FROM       XMLwithOpenXML
WHERE (XMLStatus = 0) and (XMLType = 1)

    
    SELECT @FilteredRowsCount = COUNT(*)
    FROM   @TableMaster
        
END

