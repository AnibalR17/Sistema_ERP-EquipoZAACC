SET XACT_ABORT ON;
IF OBJECT_ID(N'dbo.Venta',N'U') IS NULL
    THROW 50001,'Ejecute primero el esquema de la base de datos.',1;
BEGIN TRANSACTION;
IF COL_LENGTH('dbo.Venta','rtn_cliente') IS NULL
    EXEC(N'ALTER TABLE dbo.Venta ADD rtn_cliente varchar(30) NULL;');
IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.Venta') AND name=N'CK_Venta_rtn_cliente')
    EXEC(N'ALTER TABLE dbo.Venta WITH CHECK ADD CONSTRAINT CK_Venta_rtn_cliente
        CHECK(rtn_cliente IS NULL OR (DATALENGTH(rtn_cliente)=14 AND rtn_cliente COLLATE Latin1_General_100_BIN2 NOT LIKE ''%[^0-9]%''));');
COMMIT;
