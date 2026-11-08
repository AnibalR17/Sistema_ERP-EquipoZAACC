-- Ejecutar después de Triggers2.sql y Migraciones/002_bitacora_sistema.sql.
-- Todas las operaciones usan registros temporales y se revierten al terminar.
SET NOCOUNT ON;
SET XACT_ABORT ON;
DECLARE @contexto sql_variant=SESSION_CONTEXT(N'id_usuario'),@motivo sql_variant=SESSION_CONTEXT(N'motivo');
BEGIN TRY
    IF EXISTS(SELECT 1 FROM sys.tables t WHERE t.schema_id=SCHEMA_ID('dbo') AND t.name<>'Auditoria'
        AND NOT EXISTS(SELECT 1 FROM sys.triggers tr WHERE tr.parent_id=t.object_id AND tr.is_disabled=0
            AND (tr.name LIKE 'trg_Bitacora_%' OR tr.name LIKE '%_auditoria')))
        THROW 51000,'Hay tablas sin captura de auditoría habilitada.',1;
    BEGIN TRANSACTION;
    DECLARE @actor int=(SELECT TOP(1) id_usuario FROM dbo.Usuario WHERE activo=1 ORDER BY id_usuario);
    IF @actor IS NULL THROW 51000,'La prueba necesita un usuario activo existente.',1;
    EXEC sys.sp_set_session_context @key=N'id_usuario',@value=@actor;
    EXEC sys.sp_set_session_context @key=N'motivo',@value=N'Prueba integral de bitácora (rollback)';
    DECLARE @inicio bigint=ISNULL((SELECT MAX(id_auditoria) FROM dbo.Auditoria),0),
        @tag varchar(20)=CONCAT('AUD-',LEFT(CONVERT(varchar(36),NEWID()),12)),
        @tienda int,@destino int,@categoria int,@proveedor int,@cliente int,@empleado int,@usuario int,
        @rol int,@permiso int,@producto int,@metodo int,@compra int,@venta int,@detalleVenta int,
        @pago int,@traspaso int,@apartado int,@devolucion int,@importacion int,@evento bigint;
    INSERT dbo.Tienda(codigo,nombre) VALUES(@tag,N'Prueba auditoría origen'); SET @tienda=SCOPE_IDENTITY();
    INSERT dbo.ConfiguracionFacturacion(id_tienda,cai,actualizado_por) VALUES(@tienda,'ABC123-DEF456-ABC123-DEF456-ABC123-AA',@actor);
    INSERT dbo.Tienda(codigo,nombre) VALUES(CONCAT(@tag,'-D'),N'Prueba auditoría destino'); SET @destino=SCOPE_IDENTITY();
    INSERT dbo.Categoria(nombre) VALUES(@tag); SET @categoria=SCOPE_IDENTITY();
    INSERT dbo.Proveedor(nombre_empresa) VALUES(@tag); SET @proveedor=SCOPE_IDENTITY();
    INSERT dbo.Cliente(nombre) VALUES(@tag); SET @cliente=SCOPE_IDENTITY();
    INSERT dbo.Empleado(codigo,nombre,id_tienda) VALUES(@tag,@tag,@tienda); SET @empleado=SCOPE_IDENTITY();
    INSERT dbo.Usuario(id_empleado,nombre_usuario,contrasena_hash) VALUES(@empleado,@tag,N'SENTINELA-NO-AUTENTICABLE-PRUEBA'); SET @usuario=SCOPE_IDENTITY();
    INSERT dbo.Rol(codigo,nombre) VALUES(@tag,@tag); SET @rol=SCOPE_IDENTITY();
    INSERT dbo.Permiso(modulo,accion,descripcion) VALUES(@tag,'PRUEBA',N'Prueba'); SET @permiso=SCOPE_IDENTITY();
    INSERT dbo.UsuarioRol(id_usuario,id_rol,asignado_por) VALUES(@usuario,@rol,@actor);
    INSERT dbo.RolPermiso(id_rol,id_permiso,asignado_por) VALUES(@rol,@permiso,@actor);
    INSERT dbo.UsuarioTienda(id_usuario,id_tienda,asignado_por) VALUES(@usuario,@tienda,@actor);
    INSERT dbo.SesionUsuario(id_usuario,token_hash,version_seguridad,expira_en)
        SELECT @usuario,CRYPT_GEN_RANDOM(32),version_seguridad,DATEADD(hour,1,SYSUTCDATETIME()) FROM dbo.Usuario WHERE id_usuario=@usuario;
    INSERT dbo.EventoAcceso(id_usuario,nombre_intentado,resultado)
        SELECT @usuario,@tag,resultado FROM (VALUES('EXITOSO'),('INVALIDO'),('BLOQUEADO'),('INACTIVO')) r(resultado);
    INSERT dbo.RecuperacionAcceso(id_usuario,token_hash,expira_en) VALUES(@usuario,CRYPT_GEN_RANDOM(32),DATEADD(hour,1,SYSUTCDATETIME()));
    INSERT dbo.MetodoPago(codigo,nombre) VALUES(@tag,N'Pago prueba'); SET @metodo=SCOPE_IDENTITY();
    INSERT dbo.Producto(codigo_barras,nombre,precio_unitario,id_categoria,id_proveedor)
        VALUES(@tag,@tag,10,@categoria,@proveedor); SET @producto=SCOPE_IDENTITY();
    INSERT dbo.Compra(id_proveedor,id_tienda,numero_factura,fecha_factura,id_usuario)
        VALUES(@proveedor,@tienda,@tag,CONVERT(date,SYSUTCDATETIME()),@actor); SET @compra=SCOPE_IDENTITY();
    INSERT dbo.DetalleCompra(id_compra,id_producto,cantidad,costo_unitario) VALUES(@compra,@producto,20,5);
    UPDATE dbo.Compra SET estado='CONFIRMADA' WHERE id_compra=@compra;
    UPDATE dbo.InventarioTienda SET stock_minimo=3 WHERE id_tienda=@tienda AND id_producto=@producto;
    INSERT dbo.Venta(numero,id_tienda,id_cliente,id_usuario) VALUES(@tag,@tienda,@cliente,@actor); SET @venta=SCOPE_IDENTITY();
    INSERT dbo.DetalleVenta(id_venta,id_producto,cantidad,precio_unitario,nombre_producto,tasa_isv)
        VALUES(@venta,@producto,2,10,@tag,0); SET @detalleVenta=SCOPE_IDENTITY();
    INSERT dbo.PagoVenta(id_venta,id_metodo_pago,monto,monto_recibido) VALUES(@venta,@metodo,20,20); SET @pago=SCOPE_IDENTITY();
    UPDATE dbo.Venta SET estado='CONFIRMADA' WHERE id_venta=@venta;
    INSERT dbo.Traspaso(id_tienda_origen,id_tienda_destino,solicitado_por) VALUES(@tienda,@destino,@actor); SET @traspaso=SCOPE_IDENTITY();
    INSERT dbo.DetalleTraspaso(id_traspaso,id_producto,cantidad,precio_referencia) VALUES(@traspaso,@producto,1,10);
    UPDATE dbo.Traspaso SET estado='APROBADO' WHERE id_traspaso=@traspaso;
    UPDATE dbo.Traspaso SET estado='EN_TRANSITO' WHERE id_traspaso=@traspaso;
    UPDATE dbo.Traspaso SET estado='RECIBIDO' WHERE id_traspaso=@traspaso;
    INSERT dbo.Apartado(id_cliente,id_tienda,id_usuario,vence_en) VALUES(@cliente,@tienda,@actor,DATEADD(day,2,SYSUTCDATETIME())); SET @apartado=SCOPE_IDENTITY();
    INSERT dbo.DetalleApartado(id_apartado,id_producto,cantidad,precio_unitario,tasa_isv) VALUES(@apartado,@producto,1,10,0);
    INSERT dbo.AbonoApartado(id_apartado,id_metodo_pago,monto,id_usuario) VALUES(@apartado,@metodo,5,@actor);
    INSERT dbo.Devolucion(id_venta,id_tienda,id_usuario,motivo) VALUES(@venta,@tienda,@actor,N'Prueba'); SET @devolucion=SCOPE_IDENTITY();
    INSERT dbo.DetalleDevolucion(id_devolucion,id_detalle_venta,cantidad,reintegra_stock) VALUES(@devolucion,@detalleVenta,1,1);
    UPDATE dbo.Devolucion SET estado='CONFIRMADA' WHERE id_devolucion=@devolucion;
    INSERT dbo.Reembolso(id_pago_venta,id_devolucion,monto,id_metodo_pago,id_usuario,motivo) VALUES(@pago,@devolucion,10,@metodo,@actor,N'Prueba');
    INSERT dbo.Inversion(institucion,tipo,monto,tasa_interes,fecha_inicio,fecha_vencimiento,estado)
        VALUES(@tag,'AHORRO',100,1,CONVERT(date,SYSUTCDATETIME()),CONVERT(date,DATEADD(day,10,SYSUTCDATETIME())),'VIGENTE');
    INSERT dbo.Importacion(nombre_archivo,archivo_hash,id_usuario,estado) VALUES(@tag,CRYPT_GEN_RANDOM(32),@actor,'VALIDANDO'); SET @importacion=SCOPE_IDENTITY();
    INSERT dbo.DetalleImportacion(id_importacion,numero_fila,datos_originales_json,estado) VALUES(@importacion,1,N'{}','VALIDA');
    INSERT dbo.EventoNotificacion(clave_evento,tipo,titulo,mensaje) VALUES(@tag,'STOCK_BAJO',N'Prueba',N'Prueba'); SET @evento=SCOPE_IDENTITY();
    INSERT dbo.Notificacion(id_evento_notificacion,id_usuario) VALUES(@evento,@usuario);
    UPDATE dbo.Notificacion SET leida_en=SYSUTCDATETIME() WHERE id_evento_notificacion=@evento AND id_usuario=@usuario;

    -- El administrador responsable debe conservarse aunque el registro afecte a otro usuario.
    UPDATE dbo.Usuario SET desactivado_en=SYSUTCDATETIME(),motivo_desactivacion=N'Prueba' WHERE id_usuario=@usuario;
    IF NOT EXISTS(SELECT 1 FROM dbo.Auditoria WHERE id_auditoria>@inicio AND entidad='Usuario' AND accion='MODIFICAR'
        AND id_registro=CONCAT('{"id_usuario":',@usuario,'}') AND id_usuario=@actor)
        THROW 51000,'La modificación de usuario perdió el administrador responsable.',1;
    IF NOT EXISTS(SELECT 1 FROM dbo.Auditoria WHERE id_auditoria>@inicio AND entidad='SesionUsuario'
        AND descripcion=N'Sesión cerrada o revocada' AND id_usuario=@actor)
        THROW 51000,'La revocación de sesión no se auditó.',1;
    -- Alta, modificación y eliminación de una entidad sin referencias.
    INSERT dbo.Categoria(nombre) VALUES(CONCAT(@tag,'-BORRAR'));
    DELETE dbo.Categoria WHERE nombre=CONCAT(@tag,'-BORRAR');
    UPDATE dbo.Proveedor SET telefono=N'0000' WHERE id_proveedor=@proveedor;
    INSERT dbo.Cliente(nombre) VALUES(CONCAT(@tag,'-BORRAR'));
    DELETE dbo.Cliente WHERE nombre=CONCAT(@tag,'-BORRAR');
    IF NOT EXISTS(SELECT 1 FROM dbo.Auditoria WHERE id_auditoria>@inicio AND entidad='Cliente' AND accion='ELIMINAR')
        OR NOT EXISTS(SELECT 1 FROM dbo.Auditoria WHERE id_auditoria>@inicio AND entidad='Proveedor' AND accion='MODIFICAR')
        THROW 51000,'Falta auditoría de modificaciones o eliminaciones fuera de ventas.',1;

    -- Cada tabla operativa debe haber generado un evento real, incluidos efectos de triggers anidados.
    IF EXISTS(SELECT 1 FROM sys.tables t WHERE t.schema_id=SCHEMA_ID('dbo') AND t.name<>'Auditoria'
        AND NOT EXISTS(SELECT 1 FROM dbo.Auditoria a WHERE a.id_auditoria>@inicio AND a.entidad=t.name AND a.id_usuario=@actor))
    BEGIN
        SELECT t.name AS EntidadSinEvento FROM sys.tables t WHERE t.schema_id=SCHEMA_ID('dbo') AND t.name<>'Auditoria'
            AND NOT EXISTS(SELECT 1 FROM dbo.Auditoria a WHERE a.id_auditoria>@inicio AND a.entidad=t.name AND a.id_usuario=@actor);
        THROW 51000,'Hay entidades sin un evento de prueba auditado.',1;
    END;
    IF EXISTS(SELECT 1 FROM dbo.Auditoria WHERE id_auditoria>@inicio
        AND (CHARINDEX('SENTINELA-NO-AUTENTICABLE-PRUEBA',ISNULL(valores_antes_json,N'')+ISNULL(valores_despues_json,N''))>0))
        THROW 51000,'La auditoría expuso credenciales de prueba.',1;
    SELECT entidad AS Entidad,COUNT(*) AS Eventos FROM dbo.Auditoria WHERE id_auditoria>@inicio AND id_usuario=@actor GROUP BY entidad ORDER BY entidad;
    ROLLBACK TRANSACTION;
    EXEC sys.sp_set_session_context @key=N'id_usuario',@value=@contexto;
    EXEC sys.sp_set_session_context @key=N'motivo',@value=@motivo;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT>0 ROLLBACK TRANSACTION;
    EXEC sys.sp_set_session_context @key=N'id_usuario',@value=@contexto;
    EXEC sys.sp_set_session_context @key=N'motivo',@value=@motivo;
    THROW;
END CATCH;
