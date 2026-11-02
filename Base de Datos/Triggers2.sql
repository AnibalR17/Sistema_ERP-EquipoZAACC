/* VERSION AMPLIADA - 25/10/2026
   Ejecutar con SSMS o sqlcmd en Inventario, despues del esquema 3FN.
   Incluye el original y ampliaciones al final; las definiciones posteriores prevalecen.
   Requiere SQL Server 2016 SP1+, nested triggers habilitado y RECURSIVE_TRIGGERS OFF.
   No ejecutar sobre movimientos ya contabilizados sin reconciliar primero inventario.
   Desde esta version los movimientos actualizan InventarioTienda automaticamente:
   la aplicacion NO debe actualizar existencias por segunda vez.
   Identidad del operador: EXEC sys.sp_set_session_context N'id_usuario', <id real>;
   Las operaciones con documentos requieren ese contexto de sesion.
   El script instala objetos; NO confirma documentos ni procesa vencimientos existentes.
*/
-- Ejecutar en la misma base de datos configurada en la aplicación.
GO
-- Se conserva la base de datos seleccionada; no se cambia a Inventario.
IF OBJECT_ID(N'dbo.InventarioTienda', N'U') IS NULL
    THROW 50991, 'Ejecute primero el esquema 01_esquema_erp_3fn.sql.', 1;
IF (SELECT value_in_use FROM sys.configurations WHERE name = 'nested triggers') <> 1
    THROW 50992, 'Esta version requiere nested triggers habilitado.', 1;
IF DATABASEPROPERTYEX(DB_NAME(), 'IsRecursiveTriggersEnabled') = 1
    THROW 50993, 'Esta version requiere RECURSIVE_TRIGGERS OFF.', 1;
GO
/* =============================================================================
   atriggers.sql
   Triggers del esquema Inventario (ejecutar DESPUES de 01_esquema_erp_3fn.sql).

   Requiere SQL Server 2016 SP1 o superior (CREATE OR ALTER) y THROW (2012+).
   Es re-ejecutable: CREATE OR ALTER reemplaza los triggers existentes.

   Contenido:
     1. actualizado_en automatico en las 11 tablas maestras y de acceso
     2. Seguridad de cuentas: cambio de clave, version de seguridad y sesiones
     3. Tablas inmutables (libro de movimientos, auditoria e historiales)
     4. Transiciones de estado y reglas de confirmacion de documentos
     5. Bloqueo de detalles cuando el documento ya no es editable
     6. Integridad del libro de inventario (MovimientoInventario)
     7. Devoluciones y reembolsos

   Reglas generales:
     - Los triggers de validacion hacen ROLLBACK de la transaccion y lanzan un
       error (THROW 50xxx) con un mensaje en espanol.
     - actualizado_por NO lo puede llenar un trigger (no sabe quien es el usuario
       de la aplicacion): lo escribe la aplicacion en cada UPDATE.
     - Para cargas masivas de prueba: DISABLE TRIGGER ALL ON dbo.<tabla>;
       y luego ENABLE TRIGGER ALL ON dbo.<tabla>;
   ============================================================================= */

-- USE Inventario;
-- GO

SET NOCOUNT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO


/* =============================================================================
   1. actualizado_en AUTOMATICO (AFTER UPDATE)
      Un DEFAULT solo actua en el INSERT; este trigger registra la fecha de
      ultima modificacion. Si la aplicacion ya envia actualizado_en, se respeta.
      Se generan 11 triggers iguales: trg_<Tabla>_actualizado.
   ============================================================================= */

DECLARE @maestras TABLE (tabla sysname, pk sysname);
INSERT INTO @maestras (tabla, pk) VALUES
    ('Categoria',  'id_categoria'),  ('Proveedor', 'id_proveedor'), ('Cliente', 'id_cliente'),
    ('Tienda',     'id_tienda'),     ('MetodoPago','id_metodo_pago'), ('Empleado', 'id_empleado'),
    ('Usuario',    'id_usuario'),    ('Rol',       'id_rol'),       ('Permiso', 'id_permiso'),
    ('Producto',   'id_producto'),   ('Inversion', 'id_inversion');

DECLARE @tabla sysname, @pk sysname, @sql nvarchar(max);
DECLARE c CURSOR LOCAL FAST_FORWARD FOR SELECT tabla, pk FROM @maestras;
OPEN c;
FETCH NEXT FROM c INTO @tabla, @pk;
WHILE @@FETCH_STATUS = 0
BEGIN
    SET @sql = N'
CREATE OR ALTER TRIGGER dbo.trg_' + @tabla + N'_actualizado
ON dbo.' + QUOTENAME(@tabla) + N'
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF TRIGGER_NESTLEVEL(@@PROCID) > 1 OR UPDATE(actualizado_en) RETURN;

    UPDATE t
       SET actualizado_en = SYSUTCDATETIME()
      FROM dbo.' + QUOTENAME(@tabla) + N' AS t
      JOIN inserted AS i ON i.' + QUOTENAME(@pk) + N' = t.' + QUOTENAME(@pk) + N';
END;';
    EXEC sys.sp_executesql @sql;
    FETCH NEXT FROM c INTO @tabla, @pk;
END
CLOSE c;
DEALLOCATE c;
GO


/* =============================================================================
   2. SEGURIDAD DE CUENTAS
      - Cambiar la clave actualiza contrasena_actualizada_en.
      - Cambiar la clave, desactivar la cuenta o cambiar sus roles, permisos o
        tiendas genera una nueva version_seguridad y revoca sus sesiones activas.
   ============================================================================= */

IF TYPE_ID('dbo.tt_IdUsuario') IS NULL
    CREATE TYPE dbo.tt_IdUsuario AS TABLE (id_usuario int NOT NULL PRIMARY KEY);
GO

CREATE OR ALTER PROCEDURE dbo.usp_InvalidarSesiones
    @ids dbo.tt_IdUsuario READONLY
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE u
       SET version_seguridad = NEWID()
      FROM dbo.Usuario AS u
      JOIN @ids AS x ON x.id_usuario = u.id_usuario;

    UPDATE s
       SET revocada_en = SYSUTCDATETIME()
      FROM dbo.SesionUsuario AS s
      JOIN @ids AS x ON x.id_usuario = s.id_usuario
     WHERE s.revocada_en IS NULL;
END;
GO

CREATE OR ALTER TRIGGER dbo.trg_Usuario_seguridad
ON dbo.Usuario
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF TRIGGER_NESTLEVEL(@@PROCID) > 1 RETURN;
    IF NOT (UPDATE(contrasena_hash) OR UPDATE(desactivado_en)) RETURN;

    -- fecha del cambio de clave
    UPDATE u
       SET contrasena_actualizada_en = SYSUTCDATETIME()
      FROM dbo.Usuario AS u
      JOIN inserted AS i ON i.id_usuario = u.id_usuario
      JOIN deleted  AS d ON d.id_usuario = u.id_usuario
     WHERE i.contrasena_hash <> d.contrasena_hash;

    -- invalidar sesiones: clave cambiada o cuenta recien desactivada
    DECLARE @ids dbo.tt_IdUsuario;
    INSERT INTO @ids (id_usuario)
    SELECT DISTINCT i.id_usuario
      FROM inserted AS i
      JOIN deleted  AS d ON d.id_usuario = i.id_usuario
     WHERE i.contrasena_hash <> d.contrasena_hash
        OR (d.desactivado_en IS NULL AND i.desactivado_en IS NOT NULL);

    IF EXISTS (SELECT 1 FROM @ids) EXEC dbo.usp_InvalidarSesiones @ids;
END;
GO

CREATE OR ALTER TRIGGER dbo.trg_UsuarioRol_seguridad
ON dbo.UsuarioRol
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @ids dbo.tt_IdUsuario;
    INSERT INTO @ids (id_usuario)
    SELECT id_usuario FROM inserted UNION SELECT id_usuario FROM deleted;
    IF EXISTS (SELECT 1 FROM @ids) EXEC dbo.usp_InvalidarSesiones @ids;
END;
GO

CREATE OR ALTER TRIGGER dbo.trg_UsuarioTienda_seguridad
ON dbo.UsuarioTienda
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @ids dbo.tt_IdUsuario;
    INSERT INTO @ids (id_usuario)
    SELECT id_usuario FROM inserted UNION SELECT id_usuario FROM deleted;
    IF EXISTS (SELECT 1 FROM @ids) EXEC dbo.usp_InvalidarSesiones @ids;
END;
GO

-- Cambiar los permisos de un rol afecta a todos los usuarios que lo tienen
CREATE OR ALTER TRIGGER dbo.trg_RolPermiso_seguridad
ON dbo.RolPermiso
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @ids dbo.tt_IdUsuario;
    INSERT INTO @ids (id_usuario)
    SELECT DISTINCT ur.id_usuario
      FROM dbo.UsuarioRol AS ur
     WHERE ur.id_rol IN (SELECT id_rol FROM inserted UNION SELECT id_rol FROM deleted);
    IF EXISTS (SELECT 1 FROM @ids) EXEC dbo.usp_InvalidarSesiones @ids;
END;
GO


/* =============================================================================
   3. TABLAS INMUTABLES: no se permite UPDATE ni DELETE
   ============================================================================= */

CREATE OR ALTER TRIGGER dbo.trg_MovimientoInventario_inmutable
ON dbo.MovimientoInventario
AFTER UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM deleted) RETURN;
    ROLLBACK TRANSACTION;
    THROW 50101, 'MovimientoInventario es inmutable: para corregir un asiento registra un movimiento REVERSO.', 1;
END;
GO

CREATE OR ALTER TRIGGER dbo.trg_Auditoria_inmutable
ON dbo.Auditoria
AFTER UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM deleted) RETURN;
    ROLLBACK TRANSACTION;
    THROW 50102, 'Auditoria es inmutable: no se puede modificar ni eliminar.', 1;
END;
GO

CREATE OR ALTER TRIGGER dbo.trg_EventoTraspaso_inmutable
ON dbo.EventoTraspaso
AFTER UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM deleted) RETURN;
    ROLLBACK TRANSACTION;
    THROW 50103, 'EventoTraspaso es un historial: no se puede modificar ni eliminar.', 1;
END;
GO

CREATE OR ALTER TRIGGER dbo.trg_EventoAcceso_inmutable
ON dbo.EventoAcceso
AFTER UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM deleted) RETURN;
    ROLLBACK TRANSACTION;
    THROW 50104, 'EventoAcceso es un registro de accesos: no se puede modificar ni eliminar.', 1;
END;
GO


/* =============================================================================
   4. TRANSICIONES DE ESTADO Y REGLAS DE CONFIRMACION
   ============================================================================= */

-- Traspaso: PENDIENTE -> APROBADO | RECHAZADO | CANCELADO
--           APROBADO  -> EN_TRANSITO | CANCELADO
--           EN_TRANSITO -> RECIBIDO
--           RECIBIDO, RECHAZADO y CANCELADO son finales.
CREATE OR ALTER TRIGGER dbo.trg_Traspaso_estado
ON dbo.Traspaso
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT UPDATE(estado) RETURN;

    IF EXISTS (
        SELECT 1
          FROM inserted AS i
          JOIN deleted  AS d ON d.id_traspaso = i.id_traspaso
         WHERE i.estado <> d.estado
           AND NOT (   (d.estado = 'PENDIENTE'   AND i.estado IN ('APROBADO','RECHAZADO','CANCELADO'))
                    OR (d.estado = 'APROBADO'    AND i.estado IN ('EN_TRANSITO','CANCELADO'))
                    OR (d.estado = 'EN_TRANSITO' AND i.estado = 'RECIBIDO'))
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50201, 'Traspaso: transicion de estado no permitida.', 1;
    END
END;
GO

-- Apartado: ACTIVO -> ENTREGADO | CANCELADO | VENCIDO (los demas son finales)
CREATE OR ALTER TRIGGER dbo.trg_Apartado_estado
ON dbo.Apartado
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT UPDATE(estado) RETURN;

    IF EXISTS (
        SELECT 1
          FROM inserted AS i
          JOIN deleted  AS d ON d.id_apartado = i.id_apartado
         WHERE i.estado <> d.estado
           AND NOT (d.estado = 'ACTIVO' AND i.estado IN ('ENTREGADO','CANCELADO','VENCIDO'))
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50202, 'Apartado: transicion de estado no permitida.', 1;
    END
END;
GO

-- Compra: BORRADOR -> CONFIRMADA | ANULADA ; CONFIRMADA -> ANULADA.
--         Para confirmar debe tener al menos una linea.
CREATE OR ALTER TRIGGER dbo.trg_Compra_estado
ON dbo.Compra
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT UPDATE(estado) RETURN;

    IF EXISTS (
        SELECT 1
          FROM inserted AS i
          JOIN deleted  AS d ON d.id_compra = i.id_compra
         WHERE i.estado <> d.estado
           AND NOT (   (d.estado = 'BORRADOR'   AND i.estado IN ('CONFIRMADA','ANULADA'))
                    OR (d.estado = 'CONFIRMADA' AND i.estado = 'ANULADA'))
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50203, 'Compra: transicion de estado no permitida.', 1;
    END

    IF EXISTS (
        SELECT 1
          FROM inserted AS i
          JOIN deleted  AS d ON d.id_compra = i.id_compra
         WHERE i.estado = 'CONFIRMADA' AND d.estado <> 'CONFIRMADA'
           AND NOT EXISTS (SELECT 1 FROM dbo.DetalleCompra AS x WHERE x.id_compra = i.id_compra)
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50204, 'Compra: no se puede confirmar una compra sin detalles.', 1;
    END
END;
GO

-- Venta: BORRADOR -> CONFIRMADA | ANULADA ; CONFIRMADA -> ANULADA.
--        Para confirmar: al menos una linea y la suma de pagos igual al total.
CREATE OR ALTER TRIGGER dbo.trg_Venta_estado
ON dbo.Venta
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT UPDATE(estado) RETURN;

    IF EXISTS (
        SELECT 1
          FROM inserted AS i
          JOIN deleted  AS d ON d.id_venta = i.id_venta
         WHERE i.estado <> d.estado
           AND NOT (   (d.estado = 'BORRADOR'   AND i.estado IN ('CONFIRMADA','ANULADA'))
                    OR (d.estado = 'CONFIRMADA' AND i.estado = 'ANULADA'))
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50205, 'Venta: transicion de estado no permitida.', 1;
    END

    IF EXISTS (
        SELECT 1
          FROM inserted AS i
          JOIN deleted  AS d ON d.id_venta = i.id_venta
         WHERE i.estado = 'CONFIRMADA' AND d.estado <> 'CONFIRMADA'
           AND NOT EXISTS (SELECT 1 FROM dbo.DetalleVenta AS x WHERE x.id_venta = i.id_venta)
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50206, 'Venta: no se puede confirmar una venta sin detalles.', 1;
    END

    IF EXISTS (
        SELECT 1
          FROM inserted AS i
          JOIN deleted  AS d ON d.id_venta = i.id_venta
          JOIN dbo.vw_VentaTotal AS t ON t.id_venta = i.id_venta
         WHERE i.estado = 'CONFIRMADA' AND d.estado <> 'CONFIRMADA'
           AND (SELECT ISNULL(SUM(p.monto), 0) FROM dbo.PagoVenta AS p WHERE p.id_venta = i.id_venta) <> t.total
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50207, 'Venta: la suma de pagos debe coincidir con el total antes de confirmar.', 1;
    END
END;
GO

-- Devolucion: BORRADOR -> CONFIRMADA | ANULADA ; CONFIRMADA -> ANULADA.
--             Para confirmar: al menos una linea y la venta original confirmada.
CREATE OR ALTER TRIGGER dbo.trg_Devolucion_estado
ON dbo.Devolucion
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT UPDATE(estado) RETURN;

    IF EXISTS (
        SELECT 1
          FROM inserted AS i
          JOIN deleted  AS d ON d.id_devolucion = i.id_devolucion
         WHERE i.estado <> d.estado
           AND NOT (   (d.estado = 'BORRADOR'   AND i.estado IN ('CONFIRMADA','ANULADA'))
                    OR (d.estado = 'CONFIRMADA' AND i.estado = 'ANULADA'))
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50208, 'Devolucion: transicion de estado no permitida.', 1;
    END

    IF EXISTS (
        SELECT 1
          FROM inserted AS i
          JOIN deleted  AS d ON d.id_devolucion = i.id_devolucion
         WHERE i.estado = 'CONFIRMADA' AND d.estado <> 'CONFIRMADA'
           AND NOT EXISTS (SELECT 1 FROM dbo.DetalleDevolucion AS x WHERE x.id_devolucion = i.id_devolucion)
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50209, 'Devolucion: no se puede confirmar una devolucion sin detalles.', 1;
    END

    IF EXISTS (
        SELECT 1
          FROM inserted AS i
          JOIN deleted  AS d ON d.id_devolucion = i.id_devolucion
          JOIN dbo.Venta AS v ON v.id_venta = i.id_venta
         WHERE i.estado = 'CONFIRMADA' AND d.estado <> 'CONFIRMADA'
           AND v.estado <> 'CONFIRMADA'
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50210, 'Devolucion: la venta original debe estar CONFIRMADA.', 1;
    END
END;
GO


/* =============================================================================
   5. BLOQUEO DE DETALLES
      Como el total de cada documento ya no se guarda, se calcula sumando sus
      lineas: editar una linea de un documento cerrado cambiaria su historial.
      Los detalles solo se pueden insertar, modificar o borrar mientras la
      cabecera esta en BORRADOR (Apartado: ACTIVO).
   ============================================================================= */

CREATE OR ALTER TRIGGER dbo.trg_DetalleCompra_bloqueo
ON dbo.DetalleCompra
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1
          FROM (SELECT id_compra FROM inserted UNION SELECT id_compra FROM deleted) AS x
          JOIN dbo.Compra AS h ON h.id_compra = x.id_compra
         WHERE h.estado <> 'BORRADOR'
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50301, 'DetalleCompra: la compra ya no esta en BORRADOR; sus detalles no se pueden modificar.', 1;
    END
END;
GO

CREATE OR ALTER TRIGGER dbo.trg_DetalleVenta_bloqueo
ON dbo.DetalleVenta
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1
          FROM (SELECT id_venta FROM inserted UNION SELECT id_venta FROM deleted) AS x
          JOIN dbo.Venta AS h ON h.id_venta = x.id_venta
         WHERE h.estado <> 'BORRADOR'
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50302, 'DetalleVenta: la venta ya no esta en BORRADOR; sus detalles no se pueden modificar.', 1;
    END
END;
GO

CREATE OR ALTER TRIGGER dbo.trg_DetalleDevolucion_bloqueo
ON dbo.DetalleDevolucion
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1
          FROM (SELECT id_devolucion FROM inserted UNION SELECT id_devolucion FROM deleted) AS x
          JOIN dbo.Devolucion AS h ON h.id_devolucion = x.id_devolucion
         WHERE h.estado <> 'BORRADOR'
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50303, 'DetalleDevolucion: la devolucion ya no esta en BORRADOR; sus detalles no se pueden modificar.', 1;
    END
END;
GO

CREATE OR ALTER TRIGGER dbo.trg_DetalleApartado_bloqueo
ON dbo.DetalleApartado
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1
          FROM (SELECT id_apartado FROM inserted UNION SELECT id_apartado FROM deleted) AS x
          JOIN dbo.Apartado AS h ON h.id_apartado = x.id_apartado
         WHERE h.estado <> 'ACTIVO'
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50304, 'DetalleApartado: el apartado ya no esta ACTIVO; sus detalles no se pueden modificar.', 1;
    END
END;
GO


/* =============================================================================
   6. INTEGRIDAD DEL LIBRO DE INVENTARIO (MovimientoInventario)
      id_tienda e id_producto se guardan en el movimiento para indexar (excepcion
      documentada en la normalizacion); este trigger garantiza que coincidan con
      el documento de origen y que el signo y la cantidad sean coherentes.
   ============================================================================= */

CREATE OR ALTER TRIGGER dbo.trg_MovimientoInventario_validar
ON dbo.MovimientoInventario
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    -- signo: entran COMPRA, TRASPASO_ENTRADA y DEVOLUCION; salen VENTA y TRASPASO_SALIDA
    IF EXISTS (
        SELECT 1 FROM inserted AS i
         WHERE (i.tipo IN ('COMPRA','TRASPASO_ENTRADA','DEVOLUCION') AND i.cantidad_delta < 0)
            OR (i.tipo IN ('VENTA','TRASPASO_SALIDA')               AND i.cantidad_delta > 0)
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50401, 'MovimientoInventario: el signo de cantidad_delta no corresponde al tipo de movimiento.', 1;
    END

    -- tienda, producto y cantidad deben coincidir con el detalle de origen
    IF EXISTS (
        SELECT 1
          FROM inserted AS i
          LEFT JOIN dbo.DetalleCompra     AS dc  ON dc.id_detalle_compra     = i.id_detalle_compra
          LEFT JOIN dbo.Compra            AS c   ON c.id_compra              = dc.id_compra
          LEFT JOIN dbo.DetalleVenta      AS dv  ON dv.id_detalle_venta      = i.id_detalle_venta
          LEFT JOIN dbo.Venta             AS v   ON v.id_venta               = dv.id_venta
          LEFT JOIN dbo.DetalleTraspaso   AS dt  ON dt.id_detalle_traspaso   = i.id_detalle_traspaso
          LEFT JOIN dbo.Traspaso          AS t   ON t.id_traspaso            = dt.id_traspaso
          LEFT JOIN dbo.DetalleDevolucion AS dd  ON dd.id_detalle_devolucion = i.id_detalle_devolucion
          LEFT JOIN dbo.DetalleVenta      AS dvd ON dvd.id_detalle_venta     = dd.id_detalle_venta
          LEFT JOIN dbo.Devolucion        AS dev ON dev.id_devolucion        = dd.id_devolucion
         WHERE (i.tipo = 'COMPRA'           AND (dc.id_producto  <> i.id_producto OR c.id_tienda          <> i.id_tienda OR dc.cantidad  <> ABS(i.cantidad_delta)))
            OR (i.tipo = 'VENTA'            AND (dv.id_producto  <> i.id_producto OR v.id_tienda          <> i.id_tienda OR dv.cantidad  <> ABS(i.cantidad_delta)))
            OR (i.tipo = 'TRASPASO_SALIDA'  AND (dt.id_producto  <> i.id_producto OR t.id_tienda_origen   <> i.id_tienda OR dt.cantidad  <> ABS(i.cantidad_delta)))
            OR (i.tipo = 'TRASPASO_ENTRADA' AND (dt.id_producto  <> i.id_producto OR t.id_tienda_destino  <> i.id_tienda OR dt.cantidad  <> ABS(i.cantidad_delta)))
            OR (i.tipo = 'DEVOLUCION'       AND (dvd.id_producto <> i.id_producto OR dev.id_tienda        <> i.id_tienda OR dd.cantidad  <> ABS(i.cantidad_delta) OR dd.reintegra_stock = 0))
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50402, 'MovimientoInventario: tienda, producto o cantidad no coinciden con el documento de origen.', 1;
    END

    -- reverso: misma tienda y producto, y cantidad opuesta al movimiento original
    IF EXISTS (
        SELECT 1
          FROM inserted AS i
          JOIN dbo.MovimientoInventario AS o ON o.id_movimiento = i.id_movimiento_revertido
         WHERE i.tipo = 'REVERSO'
           AND (o.id_tienda <> i.id_tienda OR o.id_producto <> i.id_producto OR o.cantidad_delta <> -i.cantidad_delta)
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50403, 'MovimientoInventario: el REVERSO debe tener la misma tienda y producto y la cantidad opuesta al movimiento original.', 1;
    END
END;
GO


/* =============================================================================
   7. DEVOLUCIONES Y REEMBOLSOS
   ============================================================================= */

CREATE OR ALTER TRIGGER dbo.trg_DetalleDevolucion_validar
ON dbo.DetalleDevolucion
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- la linea debe pertenecer a la venta de la cabecera
    IF EXISTS (
        SELECT 1
          FROM inserted AS i
          JOIN dbo.Devolucion  AS d ON d.id_devolucion    = i.id_devolucion
          JOIN dbo.DetalleVenta AS v ON v.id_detalle_venta = i.id_detalle_venta
         WHERE v.id_venta <> d.id_venta
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50501, 'DetalleDevolucion: la linea de venta no pertenece a la venta de la devolucion.', 1;
    END

    -- lo devuelto acumulado (sin devoluciones anuladas) no supera lo vendido
    IF EXISTS (
        SELECT 1
          FROM (SELECT DISTINCT id_detalle_venta FROM inserted) AS x
          JOIN dbo.DetalleVenta AS v ON v.id_detalle_venta = x.id_detalle_venta
         WHERE (SELECT SUM(dd.cantidad)
                  FROM dbo.DetalleDevolucion AS dd
                  JOIN dbo.Devolucion AS d ON d.id_devolucion = dd.id_devolucion
                 WHERE dd.id_detalle_venta = x.id_detalle_venta
                   AND d.estado <> 'ANULADA') > v.cantidad
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50502, 'DetalleDevolucion: lo devuelto acumulado supera la cantidad vendida.', 1;
    END
END;
GO

CREATE OR ALTER TRIGGER dbo.trg_Reembolso_validar
ON dbo.Reembolso
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- con devolucion, el reembolso es de un pago de la venta devuelta
    IF EXISTS (SELECT 1 FROM inserted AS i WHERE i.id_devolucion IS NOT NULL AND i.id_pago_venta IS NULL)
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50601, 'Reembolso: un reembolso con devolucion debe indicar el pago de la venta (id_pago_venta).', 1;
    END

    IF EXISTS (
        SELECT 1
          FROM inserted AS i
          JOIN dbo.Devolucion AS d ON d.id_devolucion = i.id_devolucion
          JOIN dbo.PagoVenta  AS p ON p.id_pago_venta = i.id_pago_venta
         WHERE p.id_venta <> d.id_venta
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50602, 'Reembolso: el pago no corresponde a la venta de la devolucion.', 1;
    END

    -- un abono ya aplicado a una venta se reembolsa por su PagoVenta
    IF EXISTS (
        SELECT 1
          FROM inserted AS i
          JOIN dbo.AbonoApartado AS a ON a.id_abono = i.id_abono
         WHERE a.id_pago_venta IS NOT NULL
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50603, 'Reembolso: el abono ya se aplico a una venta; reembolsalo por su PagoVenta.', 1;
    END

    -- no superar el saldo reembolsable del cobro original
    IF EXISTS (
        SELECT 1
          FROM (SELECT DISTINCT id_pago_venta FROM inserted WHERE id_pago_venta IS NOT NULL) AS x
          JOIN dbo.PagoVenta AS p ON p.id_pago_venta = x.id_pago_venta
         WHERE (SELECT SUM(r.monto) FROM dbo.Reembolso AS r WHERE r.id_pago_venta = x.id_pago_venta) > p.monto
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50604, 'Reembolso: el total reembolsado supera el monto del pago original.', 1;
    END

    IF EXISTS (
        SELECT 1
          FROM (SELECT DISTINCT id_abono FROM inserted WHERE id_abono IS NOT NULL) AS x
          JOIN dbo.AbonoApartado AS a ON a.id_abono = x.id_abono
         WHERE (SELECT SUM(r.monto) FROM dbo.Reembolso AS r WHERE r.id_abono = x.id_abono) > a.monto
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50605, 'Reembolso: el total reembolsado supera el monto del abono original.', 1;
    END
END;
GO


/* =============================================================================
   8. VERIFICACION DE LOS TRIGGERS ORIGINALES
   ============================================================================= */

SELECT OBJECT_NAME(parent_id) AS tabla, name AS trigger_nombre, is_disabled
FROM sys.triggers
WHERE parent_class = 1 AND is_ms_shipped = 0
ORDER BY OBJECT_NAME(parent_id), name;

SELECT COUNT(*) AS total_triggers
FROM sys.triggers
WHERE parent_class = 1 AND is_ms_shipped = 0;
GO

/* AMPLIACIONES: requieren los triggers originales definidos arriba y el esquema 3FN. */

/* 9. Una confirmacion no puede eludirse insertando una cabecera cerrada.
   Las propiedades que determinan tienda, cliente y origen no se cambian una vez
   creados los detalles. Se bloquean tambien los DELETE de cabeceras. */
DECLARE @docs TABLE(tabla sysname, pk sysname, inicial varchar(20));
INSERT @docs VALUES ('Compra','id_compra','BORRADOR'),('Venta','id_venta','BORRADOR'),
 ('Devolucion','id_devolucion','BORRADOR'),('Traspaso','id_traspaso','PENDIENTE'),
 ('Apartado','id_apartado','ACTIVO');
DECLARE @t sysname, @pk sysname, @inicial varchar(20), @sql nvarchar(max);
DECLARE documentos CURSOR LOCAL FAST_FORWARD FOR SELECT tabla,pk,inicial FROM @docs;
OPEN documentos; FETCH NEXT FROM documentos INTO @t,@pk,@inicial;
WHILE @@FETCH_STATUS=0
BEGIN
 SET @sql=N'CREATE OR ALTER TRIGGER dbo.trg_'+@t+N'_alta_baja ON dbo.'+QUOTENAME(@t)+N'
 AFTER INSERT, DELETE AS
 BEGIN
 SET NOCOUNT ON;
 IF EXISTS(SELECT 1 FROM deleted) THROW 50701, ''Use la anulacion/cancelacion; no elimine documentos.'',1;
 IF EXISTS(SELECT 1 FROM inserted WHERE estado <> '''+@inicial+N''')
 THROW 50702, ''El documento debe crearse en su estado inicial.'',1;
 END;';
 EXEC sys.sp_executesql @sql;
 FETCH NEXT FROM documentos INTO @t,@pk,@inicial;
END
CLOSE documentos; DEALLOCATE documentos;
GO

CREATE OR ALTER TRIGGER dbo.trg_Compra_cabecera ON dbo.Compra AFTER UPDATE AS
BEGIN
 SET NOCOUNT ON;
 IF EXISTS(SELECT 1 FROM inserted i JOIN deleted d ON d.id_compra=i.id_compra
 WHERE i.id_tienda<>d.id_tienda OR i.id_proveedor<>d.id_proveedor OR i.id_usuario<>d.id_usuario)
 THROW 50703,'No cambie tienda, proveedor ni autor de una compra existente.',1;
END;
GO
CREATE OR ALTER TRIGGER dbo.trg_Venta_cabecera ON dbo.Venta AFTER UPDATE AS
BEGIN
 SET NOCOUNT ON;
 IF EXISTS(SELECT 1 FROM inserted i JOIN deleted d ON d.id_venta=i.id_venta
 WHERE i.id_tienda<>d.id_tienda OR i.id_cliente<>d.id_cliente OR i.id_usuario<>d.id_usuario)
 THROW 50704,'No cambie tienda, cliente ni autor de una venta existente.',1;
END;
GO
CREATE OR ALTER TRIGGER dbo.trg_Devolucion_cabecera ON dbo.Devolucion AFTER UPDATE AS
BEGIN
 SET NOCOUNT ON;
 IF EXISTS(SELECT 1 FROM inserted i JOIN deleted d ON d.id_devolucion=i.id_devolucion
 WHERE i.id_venta<>d.id_venta OR i.id_tienda<>d.id_tienda OR i.id_usuario<>d.id_usuario)
 THROW 50705,'No cambie la venta, tienda ni autor de una devolucion existente.',1;
END;
GO
CREATE OR ALTER TRIGGER dbo.trg_Traspaso_cabecera ON dbo.Traspaso AFTER UPDATE AS
BEGIN
 SET NOCOUNT ON;
 IF EXISTS(SELECT 1 FROM inserted i JOIN deleted d ON d.id_traspaso=i.id_traspaso
 WHERE i.id_tienda_origen<>d.id_tienda_origen OR i.id_tienda_destino<>d.id_tienda_destino
 OR i.solicitado_por<>d.solicitado_por)
 THROW 50706,'No cambie las tiendas ni el solicitante del traspaso.',1;
END;
GO
CREATE OR ALTER TRIGGER dbo.trg_Apartado_cabecera ON dbo.Apartado AFTER UPDATE AS
BEGIN
 SET NOCOUNT ON;
 IF EXISTS(SELECT 1 FROM inserted i JOIN deleted d ON d.id_apartado=i.id_apartado
 WHERE i.id_tienda<>d.id_tienda OR i.id_cliente<>d.id_cliente OR i.id_usuario<>d.id_usuario
 OR (d.estado<>'ACTIVO' AND (ISNULL(i.id_venta,0)<>ISNULL(d.id_venta,0) OR i.vence_en<>d.vence_en)))
 THROW 50707,'No cambie la identidad del apartado ni un enlace de venta cerrado.',1;
 IF EXISTS(SELECT id_venta FROM dbo.Apartado WITH(UPDLOCK,HOLDLOCK)
 WHERE id_venta IN(SELECT id_venta FROM inserted WHERE id_venta IS NOT NULL)
 GROUP BY id_venta HAVING COUNT(*)>1)
 THROW 50708,'Una venta no puede entregar mas de un apartado.',1;
END;
GO

/* Bloqueo concurrente del documento padre antes de comprobar detalles o pagos.
   Evita que otra sesion confirme mientras se alteran lineas o cobros. */
DECLARE @detalles TABLE(tabla sysname,padre sysname,fk sysname,editable varchar(20));
INSERT @detalles VALUES ('DetalleCompra','Compra','id_compra','BORRADOR'),
 ('DetalleVenta','Venta','id_venta','BORRADOR'),
 ('DetalleDevolucion','Devolucion','id_devolucion','BORRADOR'),
 ('DetalleTraspaso','Traspaso','id_traspaso','PENDIENTE'),
 ('DetalleApartado','Apartado','id_apartado','ACTIVO'),
 ('PagoVenta','Venta','id_venta','BORRADOR');
DECLARE @t sysname,@p sysname,@fk sysname,@e varchar(20),@sql nvarchar(max);
DECLARE detalles CURSOR LOCAL FAST_FORWARD FOR SELECT tabla,padre,fk,editable FROM @detalles;
OPEN detalles; FETCH NEXT FROM detalles INTO @t,@p,@fk,@e;
WHILE @@FETCH_STATUS=0
BEGIN
 SET @sql=N'CREATE OR ALTER TRIGGER dbo.trg_'+@t+N'_bloqueo ON dbo.'+QUOTENAME(@t)+N'
 AFTER INSERT,UPDATE,DELETE AS
 BEGIN
 SET NOCOUNT ON;
 IF EXISTS(SELECT 1 FROM (SELECT '+QUOTENAME(@fk)+N' FROM inserted UNION SELECT '+QUOTENAME(@fk)+N' FROM deleted)x
 JOIN dbo.'+QUOTENAME(@p)+N' h WITH(UPDLOCK,HOLDLOCK) ON h.'+QUOTENAME(@fk)+N'=x.'+QUOTENAME(@fk)+N'
 WHERE h.estado<>'''+@e+N''') THROW 50709,''El documento ya no permite modificar detalles o pagos.'',1;
 END;';
 EXEC sys.sp_executesql @sql;
 FETCH NEXT FROM detalles INTO @t,@p,@fk,@e;
END
CLOSE detalles; DEALLOCATE detalles;
GO

/* 10. El movimiento es la unica fuente de cambios en existencia.
   Los indices unicos del esquema impiden duplicar un movimiento por detalle.
   Los bloqueos de rango protegen tambien pares tienda/producto aun inexistentes. */
CREATE OR ALTER TRIGGER dbo.trg_MovimientoInventario_aplicar
ON dbo.MovimientoInventario AFTER INSERT AS
BEGIN
 SET NOCOUNT ON;
 DECLARE @d TABLE(id_tienda int,id_producto int,delta bigint,PRIMARY KEY(id_tienda,id_producto));
 INSERT @d SELECT id_tienda,id_producto,SUM(CONVERT(bigint,cantidad_delta))
 FROM inserted GROUP BY id_tienda,id_producto;
 INSERT dbo.InventarioTienda(id_tienda,id_producto)
 SELECT d.id_tienda,d.id_producto FROM @d d
 WHERE NOT EXISTS(SELECT 1 FROM dbo.InventarioTienda t WITH(UPDLOCK,HOLDLOCK)
 WHERE t.id_tienda=d.id_tienda AND t.id_producto=d.id_producto);
 IF EXISTS(SELECT 1 FROM @d d JOIN dbo.InventarioTienda t WITH(UPDLOCK,HOLDLOCK)
 ON t.id_tienda=d.id_tienda AND t.id_producto=d.id_producto
 WHERE CONVERT(bigint,t.existencia)+d.delta<t.reservado
 OR CONVERT(bigint,t.existencia)+d.delta>2147483647)
 THROW 50710,'Inventario insuficiente, comprometido en reservas o fuera de rango.',1;
 UPDATE t SET existencia=CONVERT(int,CONVERT(bigint,t.existencia)+d.delta)
 FROM dbo.InventarioTienda t JOIN @d d
 ON t.id_tienda=d.id_tienda AND t.id_producto=d.id_producto;
END;
GO

/* El trigger original valida signo, origen y cantidades. Este agrega el estado
   del documento. No permite contabilizar movimientos de borradores. */
CREATE OR ALTER TRIGGER dbo.trg_MovimientoInventario_documento
ON dbo.MovimientoInventario AFTER INSERT AS
BEGIN
 SET NOCOUNT ON;
 IF EXISTS(SELECT 1 FROM inserted i
 LEFT JOIN dbo.DetalleCompra dc ON dc.id_detalle_compra=i.id_detalle_compra
 LEFT JOIN dbo.Compra c ON c.id_compra=dc.id_compra
 LEFT JOIN dbo.DetalleVenta dv ON dv.id_detalle_venta=i.id_detalle_venta
 LEFT JOIN dbo.Venta v ON v.id_venta=dv.id_venta
 LEFT JOIN dbo.DetalleTraspaso dt ON dt.id_detalle_traspaso=i.id_detalle_traspaso
 LEFT JOIN dbo.Traspaso t ON t.id_traspaso=dt.id_traspaso
 LEFT JOIN dbo.DetalleDevolucion dd ON dd.id_detalle_devolucion=i.id_detalle_devolucion
 LEFT JOIN dbo.Devolucion dev ON dev.id_devolucion=dd.id_devolucion
 WHERE (i.tipo='COMPRA' AND c.estado<>'CONFIRMADA')
 OR(i.tipo='VENTA' AND v.estado<>'CONFIRMADA')
 OR(i.tipo='TRASPASO_SALIDA' AND t.estado<>'EN_TRANSITO')
 OR(i.tipo='TRASPASO_ENTRADA' AND t.estado<>'RECIBIDO')
 OR(i.tipo='DEVOLUCION' AND dev.estado<>'CONFIRMADA'))
 THROW 50711,'El documento de origen no esta en el estado requerido para contabilizar.',1;
END;
GO

/* 11. Apartados: reservas al cambiar detalles y liberacion al cerrar.
   Reservar no mueve la existencia fisica; reduce el disponible. */
CREATE OR ALTER TRIGGER dbo.trg_DetalleApartado_reservas
ON dbo.DetalleApartado AFTER INSERT,UPDATE,DELETE AS
BEGIN
 SET NOCOUNT ON;
 IF EXISTS(SELECT 1 FROM dbo.vw_ApartadoTotal t
 JOIN(SELECT id_apartado FROM inserted UNION SELECT id_apartado FROM deleted)x ON x.id_apartado=t.id_apartado
 WHERE(SELECT ISNULL(SUM(monto),0) FROM dbo.AbonoApartado WITH(UPDLOCK,HOLDLOCK)
 WHERE id_apartado=t.id_apartado)>t.total)
 THROW 50734,'No reduzca el total del apartado por debajo de sus abonos.',1;
 DECLARE @d TABLE(id_tienda int,id_producto int,delta bigint,PRIMARY KEY(id_tienda,id_producto));
 INSERT @d
 SELECT a.id_tienda,x.id_producto,SUM(x.delta) FROM
 (SELECT id_apartado,id_producto,CONVERT(bigint,cantidad) delta FROM inserted
 UNION ALL SELECT id_apartado,id_producto,-CONVERT(bigint,cantidad) FROM deleted)x
 JOIN dbo.Apartado a WITH(UPDLOCK,HOLDLOCK) ON a.id_apartado=x.id_apartado
 GROUP BY a.id_tienda,x.id_producto;
 IF EXISTS(SELECT 1 FROM @d d LEFT JOIN dbo.InventarioTienda t WITH(UPDLOCK,HOLDLOCK)
 ON t.id_tienda=d.id_tienda AND t.id_producto=d.id_producto
 WHERE (t.id_producto IS NULL AND d.delta<>0)
 OR CONVERT(bigint,t.reservado)+d.delta NOT BETWEEN 0 AND t.existencia)
 THROW 50712,'No hay inventario disponible para reservar el apartado.',1;
 UPDATE t SET reservado=CONVERT(int,CONVERT(bigint,t.reservado)+d.delta)
 FROM dbo.InventarioTienda t JOIN @d d
 ON t.id_tienda=d.id_tienda AND t.id_producto=d.id_producto;
END;
GO

CREATE OR ALTER TRIGGER dbo.trg_Apartado_cierre ON dbo.Apartado AFTER UPDATE AS
BEGIN
 SET NOCOUNT ON;
 IF EXISTS(SELECT 1 FROM inserted i JOIN deleted d ON d.id_apartado=i.id_apartado
 WHERE d.estado='ACTIVO' AND i.estado='ENTREGADO')
 AND TRIGGER_NESTLEVEL(OBJECT_ID(N'dbo.trg_Venta_contabilizar'),'AFTER','DML')=0
 THROW 50733,'Enlace el apartado a la venta borrador y confirme la venta para entregarlo.',1;
 IF EXISTS(SELECT 1 FROM inserted i JOIN deleted d ON d.id_apartado=i.id_apartado
 LEFT JOIN dbo.Venta v ON v.id_venta=i.id_venta
 WHERE d.estado='ACTIVO' AND i.estado='ENTREGADO'
 AND (v.id_venta IS NULL OR v.estado<>'CONFIRMADA' OR v.id_cliente<>i.id_cliente OR v.id_tienda<>i.id_tienda))
 THROW 50713,'Para entregar el apartado enlace una venta confirmada del mismo cliente y tienda.',1;
 ;WITH cantidades AS
 (SELECT i.id_tienda,da.id_producto,SUM(CONVERT(bigint,da.cantidad)) cantidad
 FROM inserted i JOIN deleted d ON d.id_apartado=i.id_apartado
 JOIN dbo.DetalleApartado da ON da.id_apartado=i.id_apartado
 WHERE d.estado='ACTIVO' AND i.estado<>'ACTIVO' GROUP BY i.id_tienda,da.id_producto)
 UPDATE t SET reservado=CONVERT(int,CONVERT(bigint,t.reservado)-c.cantidad)
 FROM dbo.InventarioTienda t JOIN cantidades c
 ON c.id_tienda=t.id_tienda AND c.id_producto=t.id_producto;
END;
GO

/* 12. Compra y venta: confirmar contabiliza; anular revierte los asientos.
   Una anulacion con stock insuficiente falla completa, sin dejar datos a medias.
   Los reembolsos requieren datos reales de pago: no se inventan automaticamente. */
CREATE OR ALTER TRIGGER dbo.trg_Compra_contabilizar ON dbo.Compra AFTER UPDATE AS
BEGIN
 SET NOCOUNT ON;
 IF NOT UPDATE(estado) RETURN;
 DECLARE @actor int=TRY_CONVERT(int,SESSION_CONTEXT(N'id_usuario'));
 IF EXISTS(SELECT 1 FROM inserted i JOIN deleted d ON d.id_compra=i.id_compra WHERE i.estado<>d.estado)
 AND NOT EXISTS(SELECT 1 FROM dbo.Usuario WHERE id_usuario=@actor AND activo=1)
 THROW 50714,'Establezca SESSION_CONTEXT id_usuario con un usuario activo.',1;
 INSERT dbo.MovimientoInventario(id_tienda,id_producto,tipo,cantidad_delta,id_usuario,id_detalle_compra)
 SELECT i.id_tienda,dc.id_producto,'COMPRA',dc.cantidad,@actor,dc.id_detalle_compra
 FROM inserted i JOIN deleted d ON d.id_compra=i.id_compra
 JOIN dbo.DetalleCompra dc ON dc.id_compra=i.id_compra
 WHERE d.estado='BORRADOR' AND i.estado='CONFIRMADA';
 INSERT dbo.MovimientoInventario(id_tienda,id_producto,tipo,cantidad_delta,id_usuario,id_movimiento_revertido,motivo)
 SELECT m.id_tienda,m.id_producto,'REVERSO',-m.cantidad_delta,@actor,m.id_movimiento,i.motivo_anulacion
 FROM inserted i JOIN deleted d ON d.id_compra=i.id_compra
 JOIN dbo.DetalleCompra dc ON dc.id_compra=i.id_compra
 JOIN dbo.MovimientoInventario m ON m.id_detalle_compra=dc.id_detalle_compra AND m.tipo='COMPRA'
 WHERE d.estado='CONFIRMADA' AND i.estado='ANULADA';
END;
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
 -- Los detalles de la venta que entrega un apartado deben coincidir exactamente.
 IF EXISTS(SELECT 1 FROM inserted i JOIN deleted d ON d.id_venta=i.id_venta
 JOIN dbo.Apartado a WITH(UPDLOCK,HOLDLOCK) ON a.id_venta=i.id_venta
 WHERE d.estado='BORRADOR' AND i.estado='CONFIRMADA' AND
 (a.estado<>'ACTIVO' OR a.id_tienda<>i.id_tienda OR a.id_cliente<>i.id_cliente
 OR EXISTS(SELECT id_producto,precio_unitario,SUM(CONVERT(bigint,cantidad)) FROM dbo.DetalleApartado
 WHERE id_apartado=a.id_apartado GROUP BY id_producto,precio_unitario
 EXCEPT SELECT id_producto,precio_unitario,SUM(CONVERT(bigint,cantidad)) FROM dbo.DetalleVenta
 WHERE id_venta=i.id_venta GROUP BY id_producto,precio_unitario)
 OR EXISTS(SELECT id_producto,precio_unitario,SUM(CONVERT(bigint,cantidad)) FROM dbo.DetalleVenta
 WHERE id_venta=i.id_venta GROUP BY id_producto,precio_unitario
 EXCEPT SELECT id_producto,precio_unitario,SUM(CONVERT(bigint,cantidad)) FROM dbo.DetalleApartado
 WHERE id_apartado=a.id_apartado GROUP BY id_producto,precio_unitario)))
 THROW 50716,'Los datos de venta no coinciden con el apartado enlazado.',1;
 IF EXISTS(SELECT 1 FROM inserted i JOIN deleted d ON d.id_venta=i.id_venta
 JOIN dbo.Apartado a ON a.id_venta=i.id_venta JOIN dbo.AbonoApartado ab ON ab.id_apartado=a.id_apartado
 LEFT JOIN dbo.PagoVenta p ON p.id_pago_venta=ab.id_pago_venta
 WHERE d.estado='BORRADOR' AND i.estado='CONFIRMADA'
 AND(p.id_pago_venta IS NULL OR p.id_venta<>i.id_venta OR p.monto<>ab.monto OR p.id_metodo_pago<>ab.id_metodo_pago))
 THROW 50717,'Aplique cada abono a un pago de la venta antes de confirmar.',1;
 -- La cabecera ya esta confirmada dentro de esta transaccion; cerrar libera la reserva.
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

/* 13. Traspasos: aprobar reserva; enviar libera reserva y descuenta origen;
   recibir ingresa en destino; cancelar aprobado libera lo reservado. */
CREATE OR ALTER TRIGGER dbo.trg_Traspaso_operar ON dbo.Traspaso AFTER INSERT,UPDATE AS
BEGIN
 SET NOCOUNT ON;
 DECLARE @actor int=TRY_CONVERT(int,SESSION_CONTEXT(N'id_usuario'));
 IF EXISTS(SELECT 1 FROM inserted i LEFT JOIN deleted d ON d.id_traspaso=i.id_traspaso
 WHERE d.id_traspaso IS NULL OR i.estado<>d.estado)
 AND NOT EXISTS(SELECT 1 FROM dbo.Usuario WHERE id_usuario=@actor AND activo=1)
 THROW 50714,'Establezca SESSION_CONTEXT id_usuario con un usuario activo.',1;
 IF EXISTS(SELECT 1 FROM inserted i JOIN deleted d ON d.id_traspaso=i.id_traspaso
 WHERE d.estado='PENDIENTE' AND i.estado='APROBADO'
 AND NOT EXISTS(SELECT 1 FROM dbo.DetalleTraspaso x WHERE x.id_traspaso=i.id_traspaso))
 THROW 50718,'No puede aprobar un traspaso sin detalles.',1;
 DECLARE @r TABLE(id_tienda int,id_producto int,delta bigint,PRIMARY KEY(id_tienda,id_producto));
 INSERT @r SELECT i.id_tienda_origen,dt.id_producto,
 SUM(CONVERT(bigint,dt.cantidad)*CASE WHEN d.estado='PENDIENTE' THEN 1 ELSE -1 END)
 FROM inserted i JOIN deleted d ON d.id_traspaso=i.id_traspaso
 JOIN dbo.DetalleTraspaso dt ON dt.id_traspaso=i.id_traspaso
 WHERE(d.estado='PENDIENTE' AND i.estado='APROBADO')
 OR(d.estado='APROBADO' AND i.estado IN('EN_TRANSITO','CANCELADO'))
 GROUP BY i.id_tienda_origen,dt.id_producto;
 IF EXISTS(SELECT 1 FROM @r r LEFT JOIN dbo.InventarioTienda t WITH(UPDLOCK,HOLDLOCK)
 ON t.id_tienda=r.id_tienda AND t.id_producto=r.id_producto
 WHERE t.id_producto IS NULL OR CONVERT(bigint,t.reservado)+r.delta NOT BETWEEN 0 AND t.existencia)
 THROW 50719,'Inventario insuficiente para reservar o liberar el traspaso.',1;
 UPDATE t SET reservado=CONVERT(int,CONVERT(bigint,t.reservado)+r.delta)
 FROM dbo.InventarioTienda t JOIN @r r ON r.id_tienda=t.id_tienda AND r.id_producto=t.id_producto;
 INSERT dbo.MovimientoInventario(id_tienda,id_producto,tipo,cantidad_delta,id_usuario,id_detalle_traspaso)
 SELECT i.id_tienda_origen,dt.id_producto,'TRASPASO_SALIDA',-dt.cantidad,@actor,dt.id_detalle_traspaso
 FROM inserted i JOIN deleted d ON d.id_traspaso=i.id_traspaso
 JOIN dbo.DetalleTraspaso dt ON dt.id_traspaso=i.id_traspaso
 WHERE d.estado='APROBADO' AND i.estado='EN_TRANSITO';
 INSERT dbo.MovimientoInventario(id_tienda,id_producto,tipo,cantidad_delta,id_usuario,id_detalle_traspaso)
 SELECT i.id_tienda_destino,dt.id_producto,'TRASPASO_ENTRADA',dt.cantidad,@actor,dt.id_detalle_traspaso
 FROM inserted i JOIN deleted d ON d.id_traspaso=i.id_traspaso
 JOIN dbo.DetalleTraspaso dt ON dt.id_traspaso=i.id_traspaso
 WHERE d.estado='EN_TRANSITO' AND i.estado='RECIBIDO';
 INSERT dbo.EventoTraspaso(id_traspaso,estado_anterior,estado_nuevo,id_usuario,motivo)
 SELECT i.id_traspaso,d.estado,i.estado,@actor,CONVERT(nvarchar(500),SESSION_CONTEXT(N'motivo'))
 FROM inserted i LEFT JOIN deleted d ON d.id_traspaso=i.id_traspaso
 WHERE d.id_traspaso IS NULL OR i.estado<>d.estado;
END;
GO

/* 14. Devoluciones: serializar por detalle vendido para proteger el acumulado. */
CREATE OR ALTER TRIGGER dbo.trg_DetalleDevolucion_validar
ON dbo.DetalleDevolucion AFTER INSERT,UPDATE AS
BEGIN
 SET NOCOUNT ON;
 DECLARE @bloqueados bigint;
 SELECT @bloqueados=COUNT_BIG(*) FROM dbo.DetalleVenta v WITH(UPDLOCK,HOLDLOCK)
 JOIN(SELECT id_detalle_venta FROM inserted UNION SELECT id_detalle_venta FROM deleted)x
 ON x.id_detalle_venta=v.id_detalle_venta;
 IF EXISTS(SELECT 1 FROM inserted i JOIN dbo.Devolucion d ON d.id_devolucion=i.id_devolucion
 JOIN dbo.DetalleVenta v ON v.id_detalle_venta=i.id_detalle_venta WHERE v.id_venta<>d.id_venta)
 THROW 50501,'El detalle no pertenece a la venta original.',1;
 IF EXISTS(SELECT 1 FROM dbo.DetalleVenta v JOIN(SELECT DISTINCT id_detalle_venta FROM inserted)x
 ON x.id_detalle_venta=v.id_detalle_venta
 WHERE(SELECT SUM(CONVERT(bigint,dd.cantidad)) FROM dbo.DetalleDevolucion dd WITH(UPDLOCK,HOLDLOCK)
 JOIN dbo.Devolucion d WITH(UPDLOCK,HOLDLOCK) ON d.id_devolucion=dd.id_devolucion
 WHERE dd.id_detalle_venta=v.id_detalle_venta AND d.estado<>'ANULADA')>v.cantidad)
 THROW 50502,'La cantidad acumulada devuelta supera lo vendido.',1;
END;
GO
CREATE OR ALTER TRIGGER dbo.trg_Devolucion_contabilizar ON dbo.Devolucion AFTER UPDATE AS
BEGIN
 SET NOCOUNT ON;
 IF NOT UPDATE(estado) RETURN;
 DECLARE @actor int=TRY_CONVERT(int,SESSION_CONTEXT(N'id_usuario'));
 IF EXISTS(SELECT 1 FROM inserted i JOIN deleted d ON d.id_devolucion=i.id_devolucion WHERE i.estado<>d.estado)
 AND NOT EXISTS(SELECT 1 FROM dbo.Usuario WHERE id_usuario=@actor AND activo=1)
 THROW 50714,'Establezca SESSION_CONTEXT id_usuario con un usuario activo.',1;
 INSERT dbo.MovimientoInventario(id_tienda,id_producto,tipo,cantidad_delta,id_usuario,id_detalle_devolucion)
 SELECT i.id_tienda,dv.id_producto,'DEVOLUCION',dd.cantidad,@actor,dd.id_detalle_devolucion
 FROM inserted i JOIN deleted d ON d.id_devolucion=i.id_devolucion
 JOIN dbo.DetalleDevolucion dd ON dd.id_devolucion=i.id_devolucion
 JOIN dbo.DetalleVenta dv ON dv.id_detalle_venta=dd.id_detalle_venta
 WHERE d.estado='BORRADOR' AND i.estado='CONFIRMADA' AND dd.reintegra_stock=1;
 IF EXISTS(SELECT 1 FROM inserted i JOIN deleted d ON d.id_devolucion=i.id_devolucion
 JOIN dbo.Reembolso r ON r.id_devolucion=i.id_devolucion
 WHERE d.estado='CONFIRMADA' AND i.estado='ANULADA')
 THROW 50720,'Una devolucion reembolsada necesita conciliacion financiera antes de anularse.',1;
 INSERT dbo.MovimientoInventario(id_tienda,id_producto,tipo,cantidad_delta,id_usuario,id_movimiento_revertido,motivo)
 SELECT m.id_tienda,m.id_producto,'REVERSO',-m.cantidad_delta,@actor,m.id_movimiento,i.motivo_anulacion
 FROM inserted i JOIN deleted d ON d.id_devolucion=i.id_devolucion
 JOIN dbo.DetalleDevolucion dd ON dd.id_devolucion=i.id_devolucion
 JOIN dbo.MovimientoInventario m ON m.id_detalle_devolucion=dd.id_detalle_devolucion AND m.tipo='DEVOLUCION'
 WHERE d.estado='CONFIRMADA' AND i.estado='ANULADA';
END;
GO

/* 15. Refuerzo financiero: conservar cobros historicos y limitar devoluciones
   por importe, no solo por el valor del pago original. */
CREATE OR ALTER TRIGGER dbo.trg_Reembolso_concurrencia ON dbo.Reembolso AFTER INSERT,UPDATE,DELETE AS
BEGIN
 SET NOCOUNT ON;
 IF EXISTS(SELECT 1 FROM deleted) THROW 50721,'Los reembolsos son historicos; no se modifican ni eliminan.',1;
 DECLARE @n bigint;
 SELECT @n=COUNT_BIG(*) FROM dbo.PagoVenta p WITH(UPDLOCK,HOLDLOCK)
 JOIN(SELECT DISTINCT id_pago_venta FROM inserted)x ON x.id_pago_venta=p.id_pago_venta;
 SELECT @n=COUNT_BIG(*) FROM dbo.AbonoApartado a WITH(UPDLOCK,HOLDLOCK)
 JOIN(SELECT DISTINCT id_abono FROM inserted)x ON x.id_abono=a.id_abono;
 SELECT @n=COUNT_BIG(*) FROM dbo.Devolucion d WITH(UPDLOCK,HOLDLOCK)
 JOIN(SELECT DISTINCT id_devolucion FROM inserted)x ON x.id_devolucion=d.id_devolucion;
 IF EXISTS(SELECT 1 FROM dbo.PagoVenta p JOIN(SELECT DISTINCT id_pago_venta FROM inserted)x
 ON x.id_pago_venta=p.id_pago_venta WHERE(SELECT SUM(r.monto) FROM dbo.Reembolso r WITH(UPDLOCK,HOLDLOCK)
 WHERE r.id_pago_venta=p.id_pago_venta)>p.monto)
 OR EXISTS(SELECT 1 FROM dbo.AbonoApartado a JOIN(SELECT DISTINCT id_abono FROM inserted)x
 ON x.id_abono=a.id_abono WHERE(SELECT SUM(r.monto) FROM dbo.Reembolso r WITH(UPDLOCK,HOLDLOCK)
 WHERE r.id_abono=a.id_abono)>a.monto)
 THROW 50722,'El reembolso acumulado supera el cobro original.',1;
 IF EXISTS(SELECT 1 FROM inserted i JOIN dbo.Devolucion d ON d.id_devolucion=i.id_devolucion
 WHERE d.estado<>'CONFIRMADA') THROW 50723,'Solo se reembolsa una devolucion confirmada.',1;
 IF EXISTS(SELECT 1 FROM dbo.vw_DevolucionTotal t JOIN(SELECT DISTINCT id_devolucion FROM inserted)x
 ON x.id_devolucion=t.id_devolucion
 WHERE(SELECT SUM(r.monto) FROM dbo.Reembolso r WITH(UPDLOCK,HOLDLOCK)
 WHERE r.id_devolucion=t.id_devolucion)>t.total)
 THROW 50724,'El reembolso acumulado supera el importe de la devolucion.',1;
 IF EXISTS(SELECT 1 FROM inserted i JOIN dbo.PagoVenta p ON p.id_pago_venta=i.id_pago_venta
 JOIN dbo.Venta v ON v.id_venta=p.id_venta WHERE i.id_devolucion IS NULL AND v.estado<>'ANULADA')
 THROW 50725,'Un reembolso sin devolucion requiere una venta anulada.',1;
 IF EXISTS(SELECT 1 FROM inserted i JOIN dbo.AbonoApartado a ON a.id_abono=i.id_abono
 JOIN dbo.Apartado h ON h.id_apartado=a.id_apartado WHERE h.estado NOT IN('CANCELADO','VENCIDO'))
 THROW 50726,'Solo se reembolsa un abono de apartado cancelado o vencido.',1;
END;
GO

CREATE OR ALTER TRIGGER dbo.trg_AbonoApartado_proteger ON dbo.AbonoApartado AFTER INSERT,UPDATE,DELETE AS
BEGIN
 SET NOCOUNT ON;
 IF EXISTS(SELECT 1 FROM deleted d LEFT JOIN inserted i ON i.id_abono=d.id_abono
 WHERE i.id_abono IS NULL OR i.id_apartado<>d.id_apartado OR i.monto<>d.monto
 OR i.id_metodo_pago<>d.id_metodo_pago OR i.id_usuario<>d.id_usuario
 OR i.fecha<>d.fecha OR ISNULL(i.referencia,N'')<>ISNULL(d.referencia,N'')
 OR(d.id_pago_venta IS NOT NULL AND ISNULL(i.id_pago_venta,0)<>d.id_pago_venta))
 THROW 50727,'No altere el cobro original de un abono; solo puede enlazarlo una vez a su pago.',1;
 IF EXISTS(SELECT 1 FROM inserted i JOIN dbo.Apartado a WITH(UPDLOCK,HOLDLOCK) ON a.id_apartado=i.id_apartado
 WHERE a.estado<>'ACTIVO') THROW 50728,'El apartado no permite nuevos abonos o enlaces.',1;
 IF EXISTS(SELECT 1 FROM inserted i JOIN dbo.Apartado a ON a.id_apartado=i.id_apartado
 LEFT JOIN dbo.PagoVenta p ON p.id_pago_venta=i.id_pago_venta
 WHERE i.id_pago_venta IS NOT NULL AND
 (p.id_venta<>a.id_venta OR a.id_venta IS NULL OR p.monto<>i.monto OR p.id_metodo_pago<>i.id_metodo_pago))
 THROW 50729,'El pago enlazado no corresponde al apartado, monto o metodo de pago.',1;
 IF EXISTS(SELECT id_pago_venta FROM dbo.AbonoApartado WITH(UPDLOCK,HOLDLOCK)
 WHERE id_pago_venta IN(SELECT id_pago_venta FROM inserted WHERE id_pago_venta IS NOT NULL)
 GROUP BY id_pago_venta HAVING COUNT(*)>1)
 THROW 50730,'Cada abono debe enlazarse a su propio pago.',1;
 IF EXISTS(SELECT 1 FROM dbo.vw_ApartadoTotal t JOIN(SELECT DISTINCT id_apartado FROM inserted)x
 ON x.id_apartado=t.id_apartado WHERE(SELECT SUM(monto) FROM dbo.AbonoApartado WITH(UPDLOCK,HOLDLOCK)
 WHERE id_apartado=t.id_apartado)>t.total)
 THROW 50731,'Los abonos superan el total del apartado.',1;
END;
GO

/* 16. Auditoria de Categoria: valores antes/despues sin inventar un usuario.
   Para atribuirlo, la aplicacion debe establecer SESSION_CONTEXT id_usuario. */
CREATE OR ALTER TRIGGER dbo.trg_Categoria_auditoria ON dbo.Categoria AFTER INSERT,UPDATE,DELETE AS
BEGIN
 SET NOCOUNT ON;
 -- Evita duplicar el registro por el UPDATE interno del trigger de fecha.
 IF TRIGGER_NESTLEVEL()>1 RETURN;
 INSERT dbo.Auditoria(id_usuario,modulo,accion,entidad,id_registro,descripcion,valores_antes_json,valores_despues_json,equipo)
 SELECT TRY_CONVERT(int,SESSION_CONTEXT(N'id_usuario')),'CATALOGO',
 CASE WHEN d.id_categoria IS NULL THEN 'CREAR' WHEN i.id_categoria IS NULL THEN 'ELIMINAR'
 WHEN d.activo=1 AND i.activo=0 THEN 'DESACTIVAR' WHEN d.activo=0 AND i.activo=1 THEN 'REACTIVAR' ELSE 'MODIFICAR' END,
 'Categoria',CONVERT(nvarchar(100),COALESCE(i.id_categoria,d.id_categoria)),N'Cambio de categoria',
 CASE WHEN d.id_categoria IS NOT NULL THEN(SELECT d.nombre,d.descripcion,d.activo FOR JSON PATH,WITHOUT_ARRAY_WRAPPER,INCLUDE_NULL_VALUES) END,
 CASE WHEN i.id_categoria IS NOT NULL THEN(SELECT i.nombre,i.descripcion,i.activo FOR JSON PATH,WITHOUT_ARRAY_WRAPPER,INCLUDE_NULL_VALUES) END,
 HOST_NAME()
 FROM inserted i FULL JOIN deleted d ON d.id_categoria=i.id_categoria;
END;
GO

/* 17. Tarea periodica explicita. Programar con SQL Server Agent o el programador
   externo: EXEC dbo.usp_ProcesarVencimientos @id_usuario=<usuario de servicio>;
   No se crea un Job ni se asignan destinatarios sin conocer la politica del ERP. */
CREATE OR ALTER PROCEDURE dbo.usp_ProcesarVencimientos @id_usuario int AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 IF @@TRANCOUNT<>0 THROW 50732,'Ejecute esta tarea fuera de una transaccion existente.',1;
 IF NOT EXISTS(SELECT 1 FROM dbo.Usuario WHERE id_usuario=@id_usuario AND activo=1)
 THROW 50714,'Se requiere un usuario de servicio activo.',1;
 DECLARE @antes sql_variant=SESSION_CONTEXT(N'id_usuario');
 BEGIN TRY
  EXEC sys.sp_set_session_context @key=N'id_usuario',@value=@id_usuario;
  BEGIN TRANSACTION;
  UPDATE dbo.Apartado SET estado='VENCIDO' WHERE estado='ACTIVO' AND vence_en<=SYSUTCDATETIME();
  INSERT dbo.EventoNotificacion(clave_evento,tipo,titulo,mensaje)
  SELECT CONCAT('INVERSION:',id_inversion,':',CONVERT(char(8),fecha_vencimiento,112)),
  'INVERSION_VENCIMIENTO',N'Inversion vencida',CONCAT(N'La inversion ',id_inversion,N' ha alcanzado su vencimiento.')
  FROM dbo.Inversion i WHERE i.activo=1 AND i.fecha_vencimiento<=CONVERT(date,SYSUTCDATETIME())
  AND NOT EXISTS(SELECT 1 FROM dbo.EventoNotificacion e WITH(UPDLOCK,HOLDLOCK)
  WHERE e.clave_evento=CONCAT('INVERSION:',i.id_inversion,':',CONVERT(char(8),i.fecha_vencimiento,112)));
  COMMIT;
  EXEC sys.sp_set_session_context @key=N'id_usuario',@value=@antes;
 END TRY
 BEGIN CATCH
  IF XACT_STATE()<>0 ROLLBACK;
  EXEC sys.sp_set_session_context @key=N'id_usuario',@value=@antes;
  THROW;
 END CATCH
END;
GO

/* 18. Avisos de stock y traspasos. Se crean eventos deduplicados; la aplicacion
   asigna Notificacion a usuarios autorizados por tienda/rol. No se difunden a todos. */
CREATE OR ALTER TRIGGER dbo.trg_InventarioTienda_aviso ON dbo.InventarioTienda AFTER INSERT,UPDATE AS
BEGIN
 SET NOCOUNT ON;
 INSERT dbo.EventoNotificacion(clave_evento,tipo,titulo,mensaje)
 SELECT CONCAT('STOCK:',i.id_tienda,':',i.id_producto,':',CONVERT(char(8),SYSUTCDATETIME(),112)),
 'STOCK_BAJO',N'Stock disponible bajo',CONCAT(N'Producto ',i.id_producto,N', tienda ',i.id_tienda,
 N': disponible ',i.existencia-i.reservado,N', minimo ',i.stock_minimo)
 FROM inserted i WHERE i.existencia-i.reservado<i.stock_minimo
 AND NOT EXISTS(SELECT 1 FROM dbo.EventoNotificacion e WITH(UPDLOCK,HOLDLOCK)
 WHERE e.clave_evento=CONCAT('STOCK:',i.id_tienda,':',i.id_producto,':',CONVERT(char(8),SYSUTCDATETIME(),112)));
END;
GO
CREATE OR ALTER TRIGGER dbo.trg_Traspaso_aviso ON dbo.Traspaso AFTER INSERT AS
BEGIN
 SET NOCOUNT ON;
 INSERT dbo.EventoNotificacion(clave_evento,tipo,titulo,mensaje)
 SELECT CONCAT('TRASPASO:',i.id_traspaso),'TRASPASO_PENDIENTE',N'Traspaso pendiente',
 CONCAT(N'Traspaso ',i.id_traspaso,N' de tienda ',i.id_tienda_origen,N' a ',i.id_tienda_destino)
 FROM inserted i WHERE NOT EXISTS(SELECT 1 FROM dbo.EventoNotificacion e WITH(UPDLOCK,HOLDLOCK)
 WHERE e.clave_evento=CONCAT('TRASPASO:',i.id_traspaso));
END;
GO

/* 19. Auditoria de documentos y cambios de estado, incluidos los automaticos.
   No se registran hashes de contrasena ni contenido de sesiones. */
DECLARE @docs TABLE(tabla sysname,pk sysname);
INSERT @docs VALUES('Compra','id_compra'),('Venta','id_venta'),('Devolucion','id_devolucion'),
 ('Traspaso','id_traspaso'),('Apartado','id_apartado');
DECLARE @t sysname,@pk sysname,@sql nvarchar(max);
DECLARE auditar CURSOR LOCAL FAST_FORWARD FOR SELECT tabla,pk FROM @docs;
OPEN auditar; FETCH NEXT FROM auditar INTO @t,@pk;
WHILE @@FETCH_STATUS=0
BEGIN
 SET @sql=N'CREATE OR ALTER TRIGGER dbo.trg_'+@t+N'_auditoria ON dbo.'+QUOTENAME(@t)+N'
 AFTER INSERT,UPDATE AS BEGIN
 SET NOCOUNT ON;
 INSERT dbo.Auditoria(id_usuario,modulo,accion,entidad,id_registro,descripcion,valores_antes_json,valores_despues_json,equipo)
 SELECT TRY_CONVERT(int,SESSION_CONTEXT(N''id_usuario'')),''DOCUMENTOS'',
 CASE WHEN d.'+QUOTENAME(@pk)+N' IS NULL THEN ''CREAR'' ELSE ''MODIFICAR'' END,
 '''+@t+N''',CONVERT(nvarchar(100),i.'+QUOTENAME(@pk)+N'),N''Cambio de documento'',
 CASE WHEN d.'+QUOTENAME(@pk)+N' IS NOT NULL THEN(SELECT d.* FOR JSON PATH,WITHOUT_ARRAY_WRAPPER,INCLUDE_NULL_VALUES) END,
 (SELECT i.* FOR JSON PATH,WITHOUT_ARRAY_WRAPPER,INCLUDE_NULL_VALUES),HOST_NAME()
 FROM inserted i LEFT JOIN deleted d ON d.'+QUOTENAME(@pk)+N'=i.'+QUOTENAME(@pk)+N'; END;';
 EXEC sys.sp_executesql @sql;
 FETCH NEXT FROM auditar INTO @t,@pk;
END
CLOSE auditar; DEALLOCATE auditar;
GO

/* Desactivar o reactivar Rol/Permiso tambien cambia los privilegios efectivos. */
CREATE OR ALTER TRIGGER dbo.trg_Rol_seguridad ON dbo.Rol AFTER UPDATE AS
BEGIN
 SET NOCOUNT ON;
 DECLARE @ids dbo.tt_IdUsuario;
 INSERT @ids SELECT DISTINCT ur.id_usuario FROM dbo.UsuarioRol ur
 JOIN inserted i ON i.id_rol=ur.id_rol JOIN deleted d ON d.id_rol=i.id_rol
 WHERE i.activo<>d.activo;
 IF EXISTS(SELECT 1 FROM @ids) EXEC dbo.usp_InvalidarSesiones @ids;
END;
GO
CREATE OR ALTER TRIGGER dbo.trg_Permiso_seguridad ON dbo.Permiso AFTER UPDATE AS
BEGIN
 SET NOCOUNT ON;
 DECLARE @ids dbo.tt_IdUsuario;
 INSERT @ids SELECT DISTINCT ur.id_usuario FROM dbo.UsuarioRol ur
 JOIN dbo.RolPermiso rp ON rp.id_rol=ur.id_rol
 JOIN inserted i ON i.id_permiso=rp.id_permiso JOIN deleted d ON d.id_permiso=i.id_permiso
 WHERE i.activo<>d.activo OR i.modulo<>d.modulo OR i.accion<>d.accion;
 IF EXISTS(SELECT 1 FROM @ids) EXEC dbo.usp_InvalidarSesiones @ids;
END;
GO

-- Verificacion final: el numero depende de las ampliaciones; no se fija en 31.
SELECT OBJECT_NAME(parent_id) tabla,name trigger_nombre,is_disabled
FROM sys.triggers WHERE parent_class=1 AND is_ms_shipped=0
ORDER BY OBJECT_NAME(parent_id),name;
GO

select * from dbo.Categoria