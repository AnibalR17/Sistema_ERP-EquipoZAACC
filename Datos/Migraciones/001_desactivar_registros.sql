-- Ejecutar antes de iniciar esta version. Conserva todos los registros existentes.
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF COL_LENGTH('dbo.Producto', 'activo') IS NULL
    ALTER TABLE dbo.Producto ADD activo bit NOT NULL CONSTRAINT DF_Producto_activo DEFAULT (1) WITH VALUES;
IF COL_LENGTH('dbo.Categoria', 'activo') IS NULL
    ALTER TABLE dbo.Categoria ADD activo bit NOT NULL CONSTRAINT DF_Categoria_activo DEFAULT (1) WITH VALUES;
IF COL_LENGTH('dbo.Proveedor', 'activo') IS NULL
    ALTER TABLE dbo.Proveedor ADD activo bit NOT NULL CONSTRAINT DF_Proveedor_activo DEFAULT (1) WITH VALUES;
COMMIT TRANSACTION;
