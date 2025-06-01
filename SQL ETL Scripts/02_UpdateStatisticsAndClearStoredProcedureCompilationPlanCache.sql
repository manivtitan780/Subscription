-- Step 1: Update statistics on all user tables
DECLARE @TableName NVARCHAR(500);

DECLARE table_cursor CURSOR FOR
SELECT 
	s.name + '.' + t.name
FROM 
	sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id
WHERE 
	t.is_ms_shipped = 0;

OPEN table_cursor;
FETCH NEXT FROM table_cursor INTO @TableName;

WHILE @@FETCH_STATUS = 0
BEGIN
    PRINT 'Updating statistics for ' + @TableName;
    EXEC('UPDATE STATISTICS ' + @TableName + ' WITH FULLSCAN;');

    FETCH NEXT FROM table_cursor INTO @TableName;
END

CLOSE table_cursor;
DEALLOCATE table_cursor;

-- Step 2 (optional): Clear and refresh the plan cache
-- NOTE: This affects *all* cached plans for this database!

PRINT 'Clearing plan cache...';
DBCC FREEPROCCACHE;

-- If you want to target a specific procedure:
-- DECLARE @ProcId INT = OBJECT_ID('YourProcedureName');
-- DBCC FREEPROCCACHE(@ProcId);


DECLARE @ProcName NVARCHAR(500);

DECLARE proc_cursor CURSOR FOR
SELECT 
	s.name + '.' + p.name
FROM 
	sys.procedures p JOIN sys.schemas s ON p.schema_id = s.schema_id
WHERE 
	p.is_ms_shipped = 0;

OPEN proc_cursor;
FETCH NEXT FROM proc_cursor INTO @ProcName;

WHILE @@FETCH_STATUS = 0
BEGIN
    PRINT 'Marking for recompilation: ' + @ProcName;

    BEGIN TRY
        EXEC sp_recompile @ProcName;
    END TRY
    BEGIN CATCH
        PRINT 'Failed to recompile ' + @ProcName + ': ' + ERROR_MESSAGE();
    END CATCH;

    FETCH NEXT FROM proc_cursor INTO @ProcName;
END

CLOSE proc_cursor;
DEALLOCATE proc_cursor;
