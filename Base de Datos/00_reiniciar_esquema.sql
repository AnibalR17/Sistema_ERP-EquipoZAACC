/* =============================================================================
   00_reiniciar_esquema.sql
   ELIMINA todas las vistas, llaves foraneas y tablas del esquema dbo
   de la base de datos activa. Solo para entornos de prueba.

   Pasos:
     1. Seleccionar la base correcta (USE o el selector de SSMS).
     2. Ejecutar este script: solo MUESTRA las sentencias (PRINT).
     3. Si todo es lo esperado, descomentar la linea EXEC y volver a ejecutar.
     4. Ejecutar despues 01_esquema_erp_3fn.sql.
   ============================================================================= */

-- USE Inventario;
-- GO

DECLARE @sql nvarchar(max) = N'';

-- 1) vistas
SELECT @sql += N'DROP VIEW dbo.' + QUOTENAME(name) + N';' + CHAR(10)
FROM sys.views
WHERE schema_id = SCHEMA_ID('dbo');

-- 2) llaves foraneas (para poder borrar las tablas en cualquier orden)
SELECT @sql += N'ALTER TABLE dbo.' + QUOTENAME(OBJECT_NAME(parent_object_id))
             + N' DROP CONSTRAINT ' + QUOTENAME(name) + N';' + CHAR(10)
FROM sys.foreign_keys
WHERE schema_id = SCHEMA_ID('dbo');

-- 3) tablas
SELECT @sql += N'DROP TABLE dbo.' + QUOTENAME(name) + N';' + CHAR(10)
FROM sys.tables
WHERE schema_id = SCHEMA_ID('dbo');

PRINT @sql;                          -- revisar antes de ejecutar
-- EXEC sys.sp_executesql @sql;      -- descomentar para borrar de verdad
GO
