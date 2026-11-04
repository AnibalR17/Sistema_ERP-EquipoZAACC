














DECLARE @sql nvarchar(max) = N'';


SELECT @sql += N'DROP VIEW dbo.' + QUOTENAME(name) + N';' + CHAR(10)
FROM sys.views
WHERE schema_id = SCHEMA_ID('dbo');


SELECT @sql += N'ALTER TABLE dbo.' + QUOTENAME(OBJECT_NAME(parent_object_id))
             + N' DROP CONSTRAINT ' + QUOTENAME(name) + N';' + CHAR(10)
FROM sys.foreign_keys
WHERE schema_id = SCHEMA_ID('dbo');


SELECT @sql += N'DROP TABLE dbo.' + QUOTENAME(name) + N';' + CHAR(10)
FROM sys.tables
WHERE schema_id = SCHEMA_ID('dbo');

PRINT @sql;

GO
