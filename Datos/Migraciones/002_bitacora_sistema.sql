-- Complementa Triggers2.sql. No guarda contraseñas, tokens ni datos sensibles.
SET XACT_ABORT ON;
BEGIN TRANSACTION;
DECLARE @tabla sysname,@obj int,@sql nvarchar(max),@clave nvarchar(max),@actor nvarchar(300),@descripcion nvarchar(max),@modulo varchar(50);
DECLARE tablas CURSOR LOCAL FAST_FORWARD FOR
 SELECT t.name,t.object_id FROM sys.tables t JOIN sys.schemas s ON s.schema_id=t.schema_id
 WHERE s.name='dbo' AND t.name<>'Auditoria'
 AND NOT EXISTS(SELECT 1 FROM sys.triggers tr WHERE tr.parent_id=t.object_id AND tr.name IN
 ('trg_Categoria_auditoria','trg_Compra_auditoria','trg_Venta_auditoria','trg_Devolucion_auditoria','trg_Traspaso_auditoria','trg_Apartado_auditoria'));
OPEN tablas; FETCH NEXT FROM tablas INTO @tabla,@obj;
WHILE @@FETCH_STATUS=0
BEGIN
 SELECT @clave=STRING_AGG(CONVERT(nvarchar(max),'r.'+QUOTENAME(c.name)+' AS '+QUOTENAME(c.name)),',') WITHIN GROUP(ORDER BY ic.key_ordinal)
 FROM sys.indexes ix JOIN sys.index_columns ic ON ic.object_id=ix.object_id AND ic.index_id=ix.index_id
 JOIN sys.columns c ON c.object_id=ic.object_id AND c.column_id=ic.column_id
 WHERE ix.object_id=@obj AND ix.is_primary_key=1;
 IF @clave IS NOT NULL
 BEGIN
 SET @modulo=CASE
 WHEN @tabla IN ('Producto','Categoria') THEN 'PRODUCTOS'
 WHEN @tabla='Proveedor' THEN 'PROVEEDORES'
 WHEN @tabla IN ('Cliente','Empleado') THEN 'DIRECTORIO'
 WHEN @tabla IN ('Usuario','Rol','Permiso','UsuarioRol','RolPermiso','UsuarioTienda','SesionUsuario','EventoAcceso','RecuperacionAcceso') THEN 'SEGURIDAD'
 WHEN @tabla IN ('InventarioTienda','MovimientoInventario') THEN 'INVENTARIO'
 WHEN @tabla IN ('Compra','DetalleCompra') THEN 'COMPRAS'
 WHEN @tabla IN ('Venta','DetalleVenta','PagoVenta') THEN 'VENTAS'
 WHEN @tabla IN ('Traspaso','DetalleTraspaso','EventoTraspaso') THEN 'TRASPASOS'
 WHEN @tabla IN ('Apartado','DetalleApartado','AbonoApartado') THEN 'APARTADOS'
 WHEN @tabla IN ('Devolucion','DetalleDevolucion','Reembolso') THEN 'DEVOLUCIONES'
 WHEN @tabla='Inversion' THEN 'INVERSIONES'
 WHEN @tabla IN ('Importacion','DetalleImportacion') THEN 'IMPORTACIONES'
 WHEN @tabla IN ('Notificacion','EventoNotificacion') THEN 'NOTIFICACIONES'
 WHEN @tabla IN ('Tienda','MetodoPago','ConfiguracionFacturacion') THEN 'CONFIGURACION'
 ELSE 'SISTEMA' END;
 -- En seguridad el usuario afectado puede ser distinto del administrador responsable.
 SET @actor=CASE WHEN @tabla IN ('SesionUsuario','EventoAcceso')
 THEN N'COALESCE(TRY_CONVERT(int,SESSION_CONTEXT(N''id_usuario'')),r.id_usuario)' ELSE N'TRY_CONVERT(int,SESSION_CONTEXT(N''id_usuario''))' END;
 SET @descripcion=CASE @tabla
 WHEN 'EventoAcceso' THEN N'CONCAT(N''Acceso '',r.resultado,N'': '',r.nombre_intentado)'
 WHEN 'SesionUsuario' THEN N'CASE WHEN r.revocada_en IS NULL THEN N''Sesión iniciada / actualizada'' ELSE N''Sesión cerrada o revocada'' END'
 ELSE N'CONCAT(@accion,N'' en '+@tabla+N''',CASE WHEN SESSION_CONTEXT(N''motivo'') IS NOT NULL THEN CONCAT(N'': '',CONVERT(nvarchar(500),SESSION_CONTEXT(N''motivo''))) ELSE N'''' END)' END;
 SET @sql=N'CREATE OR ALTER TRIGGER dbo.'+QUOTENAME('trg_Bitacora_'+@tabla)+N' ON dbo.'+QUOTENAME(@tabla)+N'
 AFTER INSERT,UPDATE,DELETE AS BEGIN
 SET NOCOUNT ON;
 DECLARE @accion varchar(50)=CASE WHEN EXISTS(SELECT 1 FROM inserted) AND EXISTS(SELECT 1 FROM deleted) THEN ''MODIFICAR'' WHEN EXISTS(SELECT 1 FROM inserted) THEN ''CREAR'' ELSE ''ELIMINAR'' END;
 INSERT dbo.Auditoria(id_usuario,modulo,accion,entidad,id_registro,descripcion,equipo)
 SELECT '+@actor+N','''+@modulo+N''',@accion,N'''+@tabla+N''',LEFT((SELECT '+@clave+N' FOR JSON PATH,WITHOUT_ARRAY_WRAPPER),100),
 '+@descripcion+N',HOST_NAME()
 FROM (SELECT * FROM inserted UNION ALL SELECT * FROM deleted WHERE NOT EXISTS(SELECT 1 FROM inserted)) r;
 END;';
 EXEC sys.sp_executesql @sql;
 END;
 FETCH NEXT FROM tablas INTO @tabla,@obj;
END;
CLOSE tablas; DEALLOCATE tablas;
COMMIT;
