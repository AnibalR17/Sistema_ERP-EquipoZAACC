-- Datos ficticios: agrega 2 filas en cada una de las 38 tablas.
-- Orden: 01_esquema_erp_3fn.sql -> poblar_bdd.sql -> Triggers2.sql.
-- Los datos iniciales del esquema se conservan, por lo que algunas tablas tendran mas de 2 filas.
-- Ejecutar una sola vez en una base de pruebas. Los hashes de usuario son marcadores sin login real.
USE EquipoZAACC;
GO
SET NOCOUNT ON;
SET XACT_ABORT ON;

IF EXISTS (SELECT 1 FROM sys.triggers WHERE parent_class = 1 AND is_disabled = 0)
    THROW 51000, 'Ejecute esta carga antes de instalar Triggers2.sql, en una base de pruebas.', 1;
IF EXISTS (SELECT 1 FROM dbo.Tienda WHERE codigo IN ('DEMO-T1','DEMO-T2'))
    THROW 51001, 'Los datos DEMO ya existen. Esta carga se ejecuta una sola vez.', 1;

BEGIN TRY
    BEGIN TRANSACTION;

    -- Dos tiendas: tambien permiten relacionar los traspasos entre origen y destino.
    DECLARE @tienda1 int, @tienda2 int;
    INSERT dbo.Tienda (codigo, nombre) VALUES ('DEMO-T1', N'Tienda demo central');
    SET @tienda1 = SCOPE_IDENTITY();
    INSERT dbo.Tienda (codigo, nombre) VALUES ('DEMO-T2', N'Tienda demo sucursal');
    SET @tienda2 = SCOPE_IDENTITY();

    -- Una vuelta crea un registro por tabla; dos vueltas crean dos registros.
    DECLARE @i int = 1, @s varchar(1), @tienda int, @destino int;
    DECLARE @categoria int, @proveedor int, @cliente int, @metodo int,
            @empleado int, @usuario int, @rol int, @permiso int, @producto int,
            @compra int, @venta int, @detalleVenta int, @traspaso int,
            @apartado int, @abono int, @devolucion int, @importacion int;
    DECLARE @evento bigint, @ahora datetime2(3) = SYSUTCDATETIME();

    WHILE @i <= 2
    BEGIN
        SET @s = CONVERT(varchar(1), @i);
        SET @tienda = CASE WHEN @i = 1 THEN @tienda1 ELSE @tienda2 END;
        SET @destino = CASE WHEN @i = 1 THEN @tienda2 ELSE @tienda1 END;

        -- Catalogos y seguridad. Se capturan los ID reales, sin asumir que empiezan en 1.
        INSERT dbo.Categoria (nombre, descripcion)
        VALUES (N'Categoria demo ' + @s, N'Categoria de ejemplo');
        SET @categoria = SCOPE_IDENTITY();

        INSERT dbo.Proveedor (nombre_empresa, contacto)
        VALUES (N'Proveedor demo ' + @s, N'Contacto demo ' + @s);
        SET @proveedor = SCOPE_IDENTITY();

        INSERT dbo.Cliente (nombre, telefono)
        VALUES (N'Cliente demo ' + @s, N'5555000' + @s);
        SET @cliente = SCOPE_IDENTITY();

        INSERT dbo.MetodoPago (codigo, nombre)
        VALUES ('DEMO-P' + @s, N'Metodo demo ' + @s);
        SET @metodo = SCOPE_IDENTITY();

        INSERT dbo.Empleado (codigo, nombre, id_tienda)
        VALUES ('DEMO-E' + @s, N'Empleado demo ' + @s, @tienda);
        SET @empleado = SCOPE_IDENTITY();

        INSERT dbo.Usuario (id_empleado, nombre_usuario, contrasena_hash)
        VALUES (@empleado, N'demo_usuario_' + @s, N'NO_ES_UN_HASH_VALIDO_DEMO_' + @s);
        SET @usuario = SCOPE_IDENTITY();

        INSERT dbo.Rol (codigo, nombre)
        VALUES ('DEMO-R' + @s, N'Rol demo ' + @s);
        SET @rol = SCOPE_IDENTITY();

        INSERT dbo.Permiso (modulo, accion, descripcion)
        VALUES ('DEMO', 'CONSULTAR' + @s, N'Permiso de ejemplo');
        SET @permiso = SCOPE_IDENTITY();

        INSERT dbo.UsuarioRol (id_usuario, id_rol, asignado_por)
        VALUES (@usuario, @rol, @usuario);
        INSERT dbo.RolPermiso (id_rol, id_permiso, asignado_por)
        VALUES (@rol, @permiso, @usuario);
        INSERT dbo.UsuarioTienda (id_usuario, id_tienda, asignado_por)
        VALUES (@usuario, @tienda, @usuario);

        INSERT dbo.SesionUsuario (id_usuario, token_hash, version_seguridad, creada_en, expira_en)
        SELECT @usuario, HASHBYTES('SHA2_256', 'DEMO-SESION-' + @s),
               version_seguridad, @ahora, DATEADD(hour, 8, @ahora)
        FROM dbo.Usuario WHERE id_usuario = @usuario;

        INSERT dbo.EventoAcceso (id_usuario, nombre_intentado, resultado, equipo)
        VALUES (@usuario, N'demo_usuario_' + @s, 'EXITOSO', N'Equipo demo');

        INSERT dbo.RecuperacionAcceso (id_usuario, token_hash, creado_en, expira_en)
        VALUES (@usuario, HASHBYTES('SHA2_256', 'DEMO-RECUPERACION-' + @s),
                @ahora, DATEADD(hour, 1, @ahora));

        -- Existencia inicial respaldada por un movimiento de ajuste.
        INSERT dbo.Producto (codigo_barras, nombre, precio_unitario, id_categoria, id_proveedor)
        VALUES (N'DEMO-PROD-' + @s, N'Producto demo ' + @s, 100, @categoria, @proveedor);
        SET @producto = SCOPE_IDENTITY();

        INSERT dbo.InventarioTienda (id_tienda, id_producto, existencia, reservado, stock_minimo)
        VALUES (@tienda, @producto, 20, 0, 5);
        INSERT dbo.MovimientoInventario (id_tienda, id_producto, tipo, cantidad_delta, id_usuario, motivo)
        VALUES (@tienda, @producto, 'AJUSTE', 20, @usuario, N'Existencia inicial demo');

        -- Documentos en borrador: no afectan existencias.
        INSERT dbo.Compra (id_proveedor, id_tienda, numero_factura, fecha_factura, id_usuario)
        VALUES (@proveedor, @tienda, N'DEMO-FAC-' + @s, CONVERT(date, @ahora), @usuario);
        SET @compra = SCOPE_IDENTITY();
        INSERT dbo.DetalleCompra (id_compra, id_producto, cantidad, costo_unitario)
        VALUES (@compra, @producto, 10, 60);

        INSERT dbo.Venta (numero, id_tienda, id_cliente, id_usuario)
        VALUES ('DEMO-V-' + @s, @tienda, @cliente, @usuario);
        SET @venta = SCOPE_IDENTITY();
        INSERT dbo.DetalleVenta (id_venta, id_producto, cantidad, precio_unitario, nombre_producto)
        VALUES (@venta, @producto, 1, 100, N'Producto demo ' + @s);
        SET @detalleVenta = SCOPE_IDENTITY();
        INSERT dbo.PagoVenta (id_venta, id_metodo_pago, monto, monto_recibido)
        VALUES (@venta, @metodo, 100, 100);

        INSERT dbo.Traspaso (id_tienda_origen, id_tienda_destino, solicitado_por)
        VALUES (@tienda, @destino, @usuario);
        SET @traspaso = SCOPE_IDENTITY();
        INSERT dbo.DetalleTraspaso (id_traspaso, id_producto, cantidad, precio_referencia)
        VALUES (@traspaso, @producto, 1, 100);
        INSERT dbo.EventoTraspaso (id_traspaso, estado_anterior, estado_nuevo, id_usuario)
        VALUES (@traspaso, NULL, 'PENDIENTE', @usuario);

        -- Apartado cancelado con su abono devuelto: no mantiene reservas.
        INSERT dbo.Apartado (id_cliente, id_tienda, id_usuario, fecha, vence_en, estado)
        VALUES (@cliente, @tienda, @usuario, @ahora, DATEADD(day, 15, @ahora), 'CANCELADO');
        SET @apartado = SCOPE_IDENTITY();
        INSERT dbo.DetalleApartado (id_apartado, id_producto, cantidad, precio_unitario)
        VALUES (@apartado, @producto, 1, 100);
        INSERT dbo.AbonoApartado (id_apartado, id_metodo_pago, monto, id_usuario)
        VALUES (@apartado, @metodo, 25, @usuario);
        SET @abono = SCOPE_IDENTITY();
        INSERT dbo.Reembolso (id_abono, monto, id_metodo_pago, id_usuario, motivo)
        VALUES (@abono, 25, @metodo, @usuario, N'Cancelacion del apartado demo');

        INSERT dbo.Devolucion (id_venta, id_tienda, id_usuario, motivo)
        VALUES (@venta, @tienda, @usuario, N'Devolucion demo pendiente de confirmar la venta');
        SET @devolucion = SCOPE_IDENTITY();
        INSERT dbo.DetalleDevolucion (id_devolucion, id_detalle_venta, cantidad, reintegra_stock)
        VALUES (@devolucion, @detalleVenta, 1, 0);

        -- Inversiones, importaciones y registros de ejemplo.
        INSERT dbo.Inversion (institucion, tipo, monto, tasa_interes, fecha_inicio, fecha_vencimiento, estado)
        VALUES (N'Banco demo ' + @s, 'PLAZO_FIJO', 1000, 5,
                CONVERT(date, @ahora), CONVERT(date, DATEADD(year, 1, @ahora)), 'VIGENTE');

        INSERT dbo.Importacion (nombre_archivo, archivo_hash, id_usuario, estado)
        VALUES (N'demo_' + @s + N'.csv', HASHBYTES('SHA2_256', 'DEMO-ARCHIVO-' + @s), @usuario, 'LISTA');
        SET @importacion = SCOPE_IDENTITY();
        INSERT dbo.DetalleImportacion (id_importacion, numero_fila, datos_originales_json, estado, id_producto)
        VALUES (@importacion, 1, N'{"origen":"demo"}', 'VALIDA', @producto);

        INSERT dbo.Auditoria (id_usuario, modulo, accion, entidad, id_registro, descripcion)
        VALUES (@usuario, 'DEMO', 'CARGA', 'Producto', CONVERT(nvarchar(100), @producto), N'Carga de ejemplo');

        INSERT dbo.EventoNotificacion (clave_evento, tipo, titulo, mensaje)
        VALUES ('DEMO-TRASPASO-' + @s, 'TRASPASO_PENDIENTE', N'Traspaso demo pendiente', N'Revisar traspaso demo ' + @s);
        SET @evento = SCOPE_IDENTITY();
        INSERT dbo.Notificacion (id_evento_notificacion, id_usuario)
        VALUES (@evento, @usuario);

        SET @i += 1;
    END;

    COMMIT TRANSACTION;
    PRINT 'Carga completada: se agregaron 76 filas (2 por cada una de las 38 tablas).';
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
