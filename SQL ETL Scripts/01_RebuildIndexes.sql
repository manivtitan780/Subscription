DECLARE @TableName NVARCHAR(256)
DECLARE @IndexName NVARCHAR(256)
DECLARE @SQL NVARCHAR(MAX)

DECLARE cur CURSOR FOR
SELECT 
    t.name AS TableName,
    i.name AS IndexName
FROM sys.indexes i
JOIN sys.tables t ON i.object_id = t.object_id
WHERE i.type_desc IN ('CLUSTERED', 'NONCLUSTERED')
  AND i.is_disabled = 0
  AND i.is_hypothetical = 0

OPEN cur
FETCH NEXT FROM cur INTO @TableName, @IndexName

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @SQL = 'ALTER INDEX [' + @IndexName + '] ON [' + @TableName + '] REBUILD;'
    PRINT @SQL
    -- Uncomment the line below to actually run it:
    BEGIN TRY
        EXEC sp_executesql @SQL
    END TRY
    BEGIN CATCH
        PRINT 'Error rebuilding index [' + @IndexName + '] on [' + @TableName + ']: ' + ERROR_MESSAGE()
    END CATCH

    FETCH NEXT FROM cur INTO @TableName, @IndexName
END

CLOSE cur
DEALLOCATE cur
