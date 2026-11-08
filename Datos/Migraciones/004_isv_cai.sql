-- ISV adicional a precios base y CAI por tienda. Las líneas anteriores conservan impuesto cero.
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF COL_LENGTH('dbo.Venta','cai') IS NULL EXEC(N'ALTER TABLE dbo.Venta ADD cai varchar(37) NULL;');
DECLARE @tabla sysname,@sql nvarchar(max);
DECLARE tasas CURSOR LOCAL FAST_FORWARD FOR SELECT tabla FROM (VALUES(N'DetalleVenta'),(N'DetalleApartado')) x(tabla);
OPEN tasas; FETCH NEXT FROM tasas INTO @tabla;
WHILE @@FETCH_STATUS=0
BEGIN
 IF COL_LENGTH('dbo.'+@tabla,'tasa_isv') IS NULL
 BEGIN
  SET @sql=N'ALTER TABLE dbo.'+QUOTENAME(@tabla)+N' ADD tasa_isv decimal(5,2) NOT NULL CONSTRAINT DF_'+@tabla+N'_tasa_isv DEFAULT 0 WITH VALUES;
   ALTER TABLE dbo.'+QUOTENAME(@tabla)+N' DROP CONSTRAINT DF_'+@tabla+N'_tasa_isv;
   ALTER TABLE dbo.'+QUOTENAME(@tabla)+N' ADD CONSTRAINT DF_'+@tabla+N'_tasa_isv DEFAULT 15 FOR tasa_isv;';
  EXEC sys.sp_executesql @sql;
 END;
 IF COL_LENGTH('dbo.'+@tabla,'isv') IS NULL
 BEGIN
  SET @sql=N'ALTER TABLE dbo.'+QUOTENAME(@tabla)+N' ADD isv AS (CONVERT(decimal(18,2),ROUND(cantidad*precio_unitario*tasa_isv/100,2))),
   total_linea AS (CONVERT(decimal(18,2),cantidad*precio_unitario+ROUND(cantidad*precio_unitario*tasa_isv/100,2)));';
  EXEC sys.sp_executesql @sql;
 END;
 IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.'+@tabla) AND name=N'CK_'+@tabla+N'_tasa_isv')
 BEGIN
  SET @sql=N'ALTER TABLE dbo.'+QUOTENAME(@tabla)+N' WITH CHECK ADD CONSTRAINT CK_'+@tabla+N'_tasa_isv CHECK(tasa_isv IN(0,15,18));';
  EXEC sys.sp_executesql @sql;
 END;
 FETCH NEXT FROM tasas INTO @tabla;
END;
CLOSE tasas; DEALLOCATE tasas;
IF COL_LENGTH('dbo.DetalleDevolucion','isv') IS NULL
 EXEC(N'ALTER TABLE dbo.DetalleDevolucion ADD isv decimal(18,2) NOT NULL CONSTRAINT DF_DetalleDevolucion_isv DEFAULT 0 WITH VALUES;');
IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE parent_object_id=OBJECT_ID(N'dbo.DetalleDevolucion') AND name=N'CK_DetalleDevolucion_isv')
 EXEC(N'ALTER TABLE dbo.DetalleDevolucion WITH CHECK ADD CONSTRAINT CK_DetalleDevolucion_isv CHECK(isv>=0);');
IF OBJECT_ID(N'dbo.ConfiguracionFacturacion',N'U') IS NULL
 CREATE TABLE dbo.ConfiguracionFacturacion(
  id_tienda int NOT NULL CONSTRAINT PK_ConfiguracionFacturacion PRIMARY KEY,
  cai varchar(37) NULL,actualizado_en datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),actualizado_por int NOT NULL,version_fila rowversion NOT NULL,
  CONSTRAINT FK_ConfiguracionFacturacion_Tienda FOREIGN KEY(id_tienda) REFERENCES dbo.Tienda(id_tienda),
  CONSTRAINT FK_ConfiguracionFacturacion_Usuario FOREIGN KEY(actualizado_por) REFERENCES dbo.Usuario(id_usuario),
  CONSTRAINT CK_ConfiguracionFacturacion_cai CHECK(cai IS NULL OR DATALENGTH(cai)=37));
COMMIT;
GO
CREATE OR ALTER VIEW dbo.vw_VentaTotal AS
SELECT v.id_venta,
       CONVERT(decimal(18,2), ISNULL(SUM(d.subtotal),0)) AS subtotal,
       CONVERT(decimal(18,2), ISNULL(SUM(d.isv),0)) AS isv,
       CONVERT(decimal(18,2), ISNULL(SUM(d.total_linea),0)) AS total
FROM dbo.Venta v
LEFT JOIN dbo.DetalleVenta d ON d.id_venta = v.id_venta
GROUP BY v.id_venta;
GO
CREATE OR ALTER VIEW dbo.vw_ApartadoTotal AS
SELECT a.id_apartado,
       CONVERT(decimal(18,2), ISNULL(SUM(d.subtotal),0)) AS subtotal,
       CONVERT(decimal(18,2), ISNULL(SUM(d.isv),0)) AS isv,
       CONVERT(decimal(18,2), ISNULL(SUM(d.total_linea),0)) AS total
FROM dbo.Apartado a
LEFT JOIN dbo.DetalleApartado d ON d.id_apartado = a.id_apartado
GROUP BY a.id_apartado;
GO
CREATE OR ALTER VIEW dbo.vw_DevolucionDetalle AS
SELECT dd.id_detalle_devolucion, dd.id_devolucion, dd.id_detalle_venta,
       dd.cantidad, dv.precio_unitario,
       CONVERT(decimal(18,2), dd.cantidad * dv.precio_unitario) AS subtotal,
       dd.isv,CONVERT(decimal(18,2),dd.cantidad*dv.precio_unitario+dd.isv) AS total_linea,
       dd.reintegra_stock
FROM dbo.DetalleDevolucion dd
JOIN dbo.DetalleVenta dv ON dv.id_detalle_venta = dd.id_detalle_venta;
GO
CREATE OR ALTER VIEW dbo.vw_DevolucionTotal AS
SELECT d.id_devolucion,
       CONVERT(decimal(18,2), ISNULL(SUM(x.subtotal),0)) AS subtotal,
       CONVERT(decimal(18,2), ISNULL(SUM(x.isv),0)) AS isv,
       CONVERT(decimal(18,2), ISNULL(SUM(x.total_linea),0)) AS total
FROM dbo.Devolucion d
LEFT JOIN dbo.vw_DevolucionDetalle x ON x.id_devolucion = d.id_devolucion
GROUP BY d.id_devolucion;
GO
CREATE OR ALTER TRIGGER dbo.trg_Venta_contabilizar ON dbo.Venta AFTER UPDATE AS
BEGIN
 SET NOCOUNT ON;
 IF NOT UPDATE(estado) RETURN;
 DECLARE @actor int=TRY_CONVERT(int,SESSION_CONTEXT(N'id_usuario'));
 IF EXISTS(SELECT 1 FROM inserted i JOIN deleted d ON d.id_venta=i.id_venta WHERE i.estado<>d.estado)
 AND NOT EXISTS(SELECT 1 FROM dbo.Usuario WHERE id_usuario=@actor AND activo=1)
 THROW 50714,'Establezca SESSION_CONTEXT id_usuario con un usuario activo.',1;
 IF EXISTS(SELECT 1 FROM inserted i JOIN deleted d ON d.id_venta=i.id_venta
 JOIN dbo.Devolucion dev WITH(UPDLOCK,HOLDLOCK) ON dev.id_venta=i.id_venta
 WHERE d.estado='CONFIRMADA' AND i.estado='ANULADA' AND dev.estado<>'ANULADA')
 THROW 50715,'Anule las devoluciones antes de anular la venta.',1;

 IF EXISTS(SELECT 1 FROM inserted i JOIN deleted d ON d.id_venta=i.id_venta
 JOIN dbo.Apartado a WITH(UPDLOCK,HOLDLOCK) ON a.id_venta=i.id_venta
 WHERE d.estado='BORRADOR' AND i.estado='CONFIRMADA' AND
 (a.estado<>'ACTIVO' OR a.id_tienda<>i.id_tienda OR a.id_cliente<>i.id_cliente
 OR EXISTS(SELECT id_producto,precio_unitario,tasa_isv,SUM(CONVERT(bigint,cantidad)) FROM dbo.DetalleApartado
 WHERE id_apartado=a.id_apartado GROUP BY id_producto,precio_unitario,tasa_isv
 EXCEPT SELECT id_producto,precio_unitario,tasa_isv,SUM(CONVERT(bigint,cantidad)) FROM dbo.DetalleVenta
 WHERE id_venta=i.id_venta GROUP BY id_producto,precio_unitario,tasa_isv)
 OR EXISTS(SELECT id_producto,precio_unitario,tasa_isv,SUM(CONVERT(bigint,cantidad)) FROM dbo.DetalleVenta
 WHERE id_venta=i.id_venta GROUP BY id_producto,precio_unitario,tasa_isv
 EXCEPT SELECT id_producto,precio_unitario,tasa_isv,SUM(CONVERT(bigint,cantidad)) FROM dbo.DetalleApartado
 WHERE id_apartado=a.id_apartado GROUP BY id_producto,precio_unitario,tasa_isv)))
 THROW 50716,'Los datos de venta no coinciden con el apartado enlazado.',1;
 IF EXISTS(SELECT 1 FROM inserted i JOIN deleted d ON d.id_venta=i.id_venta
 JOIN dbo.Apartado a ON a.id_venta=i.id_venta JOIN dbo.AbonoApartado ab ON ab.id_apartado=a.id_apartado
 LEFT JOIN dbo.PagoVenta p ON p.id_pago_venta=ab.id_pago_venta
 WHERE d.estado='BORRADOR' AND i.estado='CONFIRMADA'
 AND(p.id_pago_venta IS NULL OR p.id_venta<>i.id_venta OR p.monto<>ab.monto OR p.id_metodo_pago<>ab.id_metodo_pago))
 THROW 50717,'Aplique cada abono a un pago de la venta antes de confirmar.',1;

 UPDATE a SET estado='ENTREGADO' FROM dbo.Apartado a
 JOIN inserted i ON i.id_venta=a.id_venta JOIN deleted d ON d.id_venta=i.id_venta
 WHERE d.estado='BORRADOR' AND i.estado='CONFIRMADA' AND a.estado='ACTIVO';
 INSERT dbo.MovimientoInventario(id_tienda,id_producto,tipo,cantidad_delta,id_usuario,id_detalle_venta)
 SELECT i.id_tienda,dv.id_producto,'VENTA',-dv.cantidad,@actor,dv.id_detalle_venta
 FROM inserted i JOIN deleted d ON d.id_venta=i.id_venta JOIN dbo.DetalleVenta dv ON dv.id_venta=i.id_venta
 WHERE d.estado='BORRADOR' AND i.estado='CONFIRMADA';
 INSERT dbo.MovimientoInventario(id_tienda,id_producto,tipo,cantidad_delta,id_usuario,id_movimiento_revertido,motivo)
 SELECT m.id_tienda,m.id_producto,'REVERSO',-m.cantidad_delta,@actor,m.id_movimiento,i.motivo_anulacion
 FROM inserted i JOIN deleted d ON d.id_venta=i.id_venta
 JOIN dbo.DetalleVenta dv ON dv.id_venta=i.id_venta
 JOIN dbo.MovimientoInventario m ON m.id_detalle_venta=dv.id_detalle_venta AND m.tipo='VENTA'
 WHERE d.estado='CONFIRMADA' AND i.estado='ANULADA';
END;
GO



CREATE OR ALTER TRIGGER dbo.trg_Bitacora_ConfiguracionFacturacion ON dbo.ConfiguracionFacturacion AFTER INSERT,UPDATE,DELETE AS
BEGIN
 SET NOCOUNT ON;
 INSERT dbo.Auditoria(id_usuario,modulo,accion,entidad,id_registro,descripcion,valores_antes_json,valores_despues_json,equipo)
 SELECT TRY_CONVERT(int,SESSION_CONTEXT(N'id_usuario')),'CONFIGURACION',
 CASE WHEN d.id_tienda IS NULL THEN 'CREAR' WHEN i.id_tienda IS NULL THEN 'ELIMINAR' ELSE 'MODIFICAR' END,
 'ConfiguracionFacturacion',CONVERT(nvarchar(100),COALESCE(i.id_tienda,d.id_tienda)),N'Configuración del CAI por tienda',
 CASE WHEN d.id_tienda IS NOT NULL THEN (SELECT d.* FOR JSON PATH,WITHOUT_ARRAY_WRAPPER) END,
 CASE WHEN i.id_tienda IS NOT NULL THEN (SELECT i.* FOR JSON PATH,WITHOUT_ARRAY_WRAPPER) END,HOST_NAME()
 FROM inserted i FULL JOIN deleted d ON d.id_tienda=i.id_tienda;
END;
GO
