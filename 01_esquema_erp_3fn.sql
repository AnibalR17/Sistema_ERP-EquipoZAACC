-- Instalacion inicial. No ejecutar sobre tablas existentes.
IF DB_ID(N'EquipoZAACC') IS NULL
    EXEC(N'CREATE DATABASE [EquipoZAACC]');
GO
USE EquipoZAACC;
GO
SET NOCOUNT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO
CREATE TABLE dbo.Categoria (
    id_categoria         int IDENTITY(1,1) NOT NULL,
    nombre               nvarchar(100) NOT NULL,
    descripcion          nvarchar(300) NULL,
    creado_en            datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    creado_por           int NULL,
    actualizado_en       datetime2(3) NULL,
    actualizado_por      int NULL,
    desactivado_en       datetime2(3) NULL,
    desactivado_por      int NULL,
    motivo_desactivacion nvarchar(300) NULL,
    activo               AS (CONVERT(bit, CASE WHEN desactivado_en IS NULL THEN 1 ELSE 0 END)) PERSISTED,
    version_fila         rowversion NOT NULL,
    CONSTRAINT PK_Categoria PRIMARY KEY (id_categoria),
    CONSTRAINT UQ_Categoria_nombre UNIQUE (nombre),
    CONSTRAINT CK_Categoria_desactivacion CHECK (desactivado_en IS NOT NULL OR (desactivado_por IS NULL AND motivo_desactivacion IS NULL))
);
GO
CREATE TABLE dbo.Proveedor (
    id_proveedor         int IDENTITY(1,1) NOT NULL,
    nombre_empresa       nvarchar(150) NOT NULL,
    rtn                  varchar(30) NULL,
    contacto             nvarchar(150) NULL,
    telefono             nvarchar(30) NULL,
    correo               nvarchar(254) NULL,
    direccion            nvarchar(300) NULL,
    creado_en            datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    creado_por           int NULL,
    actualizado_en       datetime2(3) NULL,
    actualizado_por      int NULL,
    desactivado_en       datetime2(3) NULL,
    desactivado_por      int NULL,
    motivo_desactivacion nvarchar(300) NULL,
    activo               AS (CONVERT(bit, CASE WHEN desactivado_en IS NULL THEN 1 ELSE 0 END)) PERSISTED,
    version_fila         rowversion NOT NULL,
    CONSTRAINT PK_Proveedor PRIMARY KEY (id_proveedor),
    CONSTRAINT CK_Proveedor_desactivacion CHECK (desactivado_en IS NOT NULL OR (desactivado_por IS NULL AND motivo_desactivacion IS NULL))
);
GO
CREATE TABLE dbo.Cliente (
    id_cliente           int IDENTITY(1,1) NOT NULL,
    nombre               nvarchar(150) NOT NULL,
    documento_fiscal     varchar(30) NULL,
    telefono             nvarchar(30) NULL,
    correo               nvarchar(254) NULL,
    direccion            nvarchar(300) NULL,
    creado_en            datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    creado_por           int NULL,
    actualizado_en       datetime2(3) NULL,
    actualizado_por      int NULL,
    desactivado_en       datetime2(3) NULL,
    desactivado_por      int NULL,
    motivo_desactivacion nvarchar(300) NULL,
    activo               AS (CONVERT(bit, CASE WHEN desactivado_en IS NULL THEN 1 ELSE 0 END)) PERSISTED,
    version_fila         rowversion NOT NULL,
    CONSTRAINT PK_Cliente PRIMARY KEY (id_cliente),
    CONSTRAINT CK_Cliente_desactivacion CHECK (desactivado_en IS NOT NULL OR (desactivado_por IS NULL AND motivo_desactivacion IS NULL))
);
GO
CREATE TABLE dbo.Tienda (
    id_tienda            int IDENTITY(1,1) NOT NULL,
    codigo               varchar(20) NOT NULL,
    nombre               nvarchar(100) NOT NULL,
    direccion            nvarchar(300) NULL,
    telefono             nvarchar(30) NULL,
    creado_en            datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    creado_por           int NULL,
    actualizado_en       datetime2(3) NULL,
    actualizado_por      int NULL,
    desactivado_en       datetime2(3) NULL,
    desactivado_por      int NULL,
    motivo_desactivacion nvarchar(300) NULL,
    activo               AS (CONVERT(bit, CASE WHEN desactivado_en IS NULL THEN 1 ELSE 0 END)) PERSISTED,
    version_fila         rowversion NOT NULL,
    CONSTRAINT PK_Tienda PRIMARY KEY (id_tienda),
    CONSTRAINT UQ_Tienda_codigo UNIQUE (codigo),
    CONSTRAINT CK_Tienda_desactivacion CHECK (desactivado_en IS NOT NULL OR (desactivado_por IS NULL AND motivo_desactivacion IS NULL))
);
GO
CREATE TABLE dbo.MetodoPago (
    id_metodo_pago       int IDENTITY(1,1) NOT NULL,
    codigo               varchar(30) NOT NULL,
    nombre               nvarchar(80) NOT NULL,
    creado_en            datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    creado_por           int NULL,
    actualizado_en       datetime2(3) NULL,
    actualizado_por      int NULL,
    desactivado_en       datetime2(3) NULL,
    desactivado_por      int NULL,
    motivo_desactivacion nvarchar(300) NULL,
    activo               AS (CONVERT(bit, CASE WHEN desactivado_en IS NULL THEN 1 ELSE 0 END)) PERSISTED,
    version_fila         rowversion NOT NULL,
    CONSTRAINT PK_MetodoPago PRIMARY KEY (id_metodo_pago),
    CONSTRAINT UQ_MetodoPago_codigo UNIQUE (codigo),
    CONSTRAINT CK_MetodoPago_desactivacion CHECK (desactivado_en IS NOT NULL OR (desactivado_por IS NULL AND motivo_desactivacion IS NULL))
);
GO
CREATE TABLE dbo.Empleado (
    id_empleado          int IDENTITY(1,1) NOT NULL,
    codigo               varchar(30) NOT NULL,
    nombre               nvarchar(150) NOT NULL,
    telefono             nvarchar(30) NULL,
    correo               nvarchar(254) NULL,
    id_tienda            int NOT NULL,
    creado_en            datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    creado_por           int NULL,
    actualizado_en       datetime2(3) NULL,
    actualizado_por      int NULL,
    desactivado_en       datetime2(3) NULL,
    desactivado_por      int NULL,
    motivo_desactivacion nvarchar(300) NULL,
    activo               AS (CONVERT(bit, CASE WHEN desactivado_en IS NULL THEN 1 ELSE 0 END)) PERSISTED,
    version_fila         rowversion NOT NULL,
    CONSTRAINT PK_Empleado PRIMARY KEY (id_empleado),
    CONSTRAINT UQ_Empleado_codigo UNIQUE (codigo),
    CONSTRAINT FK_Empleado_Tienda FOREIGN KEY (id_tienda) REFERENCES dbo.Tienda (id_tienda),
    CONSTRAINT CK_Empleado_desactivacion CHECK (desactivado_en IS NOT NULL OR (desactivado_por IS NULL AND motivo_desactivacion IS NULL))
);
GO
CREATE TABLE dbo.Usuario (
    id_usuario                 int IDENTITY(1,1) NOT NULL,
    id_empleado                int NOT NULL,
    nombre_usuario             nvarchar(50) NOT NULL,
    nombre_usuario_normalizado AS (UPPER(LTRIM(RTRIM(nombre_usuario)))) PERSISTED,
    contrasena_hash            nvarchar(512) NOT NULL,
    contrasena_actualizada_en  datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    debe_cambiar_contrasena    bit NOT NULL DEFAULT 1,
    intentos_fallidos          int NOT NULL DEFAULT 0,
    bloqueado_hasta            datetime2(3) NULL,
    ultimo_acceso_en           datetime2(3) NULL,
    version_seguridad          uniqueidentifier NOT NULL DEFAULT NEWID(),
    creado_en                  datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    creado_por                 int NULL,
    actualizado_en             datetime2(3) NULL,
    actualizado_por            int NULL,
    desactivado_en             datetime2(3) NULL,
    desactivado_por            int NULL,
    motivo_desactivacion       nvarchar(300) NULL,
    activo                     AS (CONVERT(bit, CASE WHEN desactivado_en IS NULL THEN 1 ELSE 0 END)) PERSISTED,
    version_fila               rowversion NOT NULL,
    CONSTRAINT PK_Usuario PRIMARY KEY (id_usuario),
    CONSTRAINT UQ_Usuario_empleado UNIQUE (id_empleado),
    CONSTRAINT UQ_Usuario_normalizado UNIQUE (nombre_usuario_normalizado),
    CONSTRAINT FK_Usuario_Empleado FOREIGN KEY (id_empleado) REFERENCES dbo.Empleado (id_empleado),
    CONSTRAINT CK_Usuario_intentos CHECK (intentos_fallidos >= 0),
    CONSTRAINT CK_Usuario_desactivacion CHECK (desactivado_en IS NOT NULL OR (desactivado_por IS NULL AND motivo_desactivacion IS NULL))
);
GO
CREATE TABLE dbo.Rol (
    id_rol               int IDENTITY(1,1) NOT NULL,
    codigo               varchar(30) NOT NULL,
    nombre               nvarchar(80) NOT NULL,
    descripcion          nvarchar(300) NULL,
    creado_en            datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    creado_por           int NULL,
    actualizado_en       datetime2(3) NULL,
    actualizado_por      int NULL,
    desactivado_en       datetime2(3) NULL,
    desactivado_por      int NULL,
    motivo_desactivacion nvarchar(300) NULL,
    activo               AS (CONVERT(bit, CASE WHEN desactivado_en IS NULL THEN 1 ELSE 0 END)) PERSISTED,
    version_fila         rowversion NOT NULL,
    CONSTRAINT PK_Rol PRIMARY KEY (id_rol),
    CONSTRAINT UQ_Rol_codigo UNIQUE (codigo),
    CONSTRAINT CK_Rol_desactivacion CHECK (desactivado_en IS NOT NULL OR (desactivado_por IS NULL AND motivo_desactivacion IS NULL))
);
GO
CREATE TABLE dbo.Permiso (
    id_permiso           int IDENTITY(1,1) NOT NULL,
    modulo               varchar(50) NOT NULL,
    accion               varchar(30) NOT NULL,
    codigo               AS (modulo + '.' + accion) PERSISTED,
    descripcion          nvarchar(300) NOT NULL,
    creado_en            datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    creado_por           int NULL,
    actualizado_en       datetime2(3) NULL,
    actualizado_por      int NULL,
    desactivado_en       datetime2(3) NULL,
    desactivado_por      int NULL,
    motivo_desactivacion nvarchar(300) NULL,
    activo               AS (CONVERT(bit, CASE WHEN desactivado_en IS NULL THEN 1 ELSE 0 END)) PERSISTED,
    version_fila         rowversion NOT NULL,
    CONSTRAINT PK_Permiso PRIMARY KEY (id_permiso),
    CONSTRAINT UQ_Permiso_modulo_accion UNIQUE (modulo, accion),
    CONSTRAINT CK_Permiso_desactivacion CHECK (desactivado_en IS NOT NULL OR (desactivado_por IS NULL AND motivo_desactivacion IS NULL))
);
GO
CREATE TABLE dbo.UsuarioRol (
    id_usuario   int NOT NULL,
    id_rol       int NOT NULL,
    activo       bit NOT NULL DEFAULT 1,
    asignado_en  datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    asignado_por int NULL,
    CONSTRAINT PK_UsuarioRol PRIMARY KEY (id_usuario, id_rol),
    CONSTRAINT FK_UsuarioRol_Usuario FOREIGN KEY (id_usuario) REFERENCES dbo.Usuario (id_usuario),
    CONSTRAINT FK_UsuarioRol_Rol FOREIGN KEY (id_rol) REFERENCES dbo.Rol (id_rol),
    CONSTRAINT FK_UsuarioRol_asignado_por FOREIGN KEY (asignado_por) REFERENCES dbo.Usuario (id_usuario)
);
GO
CREATE TABLE dbo.RolPermiso (
    id_rol       int NOT NULL,
    id_permiso   int NOT NULL,
    activo       bit NOT NULL DEFAULT 1,
    asignado_en  datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    asignado_por int NULL,
    CONSTRAINT PK_RolPermiso PRIMARY KEY (id_rol, id_permiso),
    CONSTRAINT FK_RolPermiso_Rol FOREIGN KEY (id_rol) REFERENCES dbo.Rol (id_rol),
    CONSTRAINT FK_RolPermiso_Permiso FOREIGN KEY (id_permiso) REFERENCES dbo.Permiso (id_permiso),
    CONSTRAINT FK_RolPermiso_asignado_por FOREIGN KEY (asignado_por) REFERENCES dbo.Usuario (id_usuario)
);
GO
CREATE TABLE dbo.UsuarioTienda (
    id_usuario   int NOT NULL,
    id_tienda    int NOT NULL,
    activo       bit NOT NULL DEFAULT 1,
    asignado_en  datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    asignado_por int NULL,
    CONSTRAINT PK_UsuarioTienda PRIMARY KEY (id_usuario, id_tienda),
    CONSTRAINT FK_UsuarioTienda_Usuario FOREIGN KEY (id_usuario) REFERENCES dbo.Usuario (id_usuario),
    CONSTRAINT FK_UsuarioTienda_Tienda FOREIGN KEY (id_tienda) REFERENCES dbo.Tienda (id_tienda),
    CONSTRAINT FK_UsuarioTienda_asignado_por FOREIGN KEY (asignado_por) REFERENCES dbo.Usuario (id_usuario)
);
GO
CREATE TABLE dbo.SesionUsuario (
    id_sesion           bigint IDENTITY(1,1) NOT NULL,
    id_usuario          int NOT NULL,
    token_hash          binary(32) NOT NULL,
    version_seguridad   uniqueidentifier NOT NULL,
    creada_en           datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    expira_en           datetime2(3) NOT NULL,
    ultima_actividad_en datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    revocada_en         datetime2(3) NULL,
    equipo              nvarchar(150) NULL,
    direccion_ip        varchar(45) NULL,
    CONSTRAINT PK_SesionUsuario PRIMARY KEY (id_sesion),
    CONSTRAINT UQ_SesionUsuario_token UNIQUE (token_hash),
    CONSTRAINT FK_SesionUsuario_Usuario FOREIGN KEY (id_usuario) REFERENCES dbo.Usuario (id_usuario),
    CONSTRAINT CK_SesionUsuario_expira CHECK (expira_en > creada_en)
);
GO
CREATE TABLE dbo.EventoAcceso (
    id_evento_acceso bigint IDENTITY(1,1) NOT NULL,
    id_usuario       int NULL,
    nombre_intentado nvarchar(50) NOT NULL,
    fecha            datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    resultado        varchar(30) NOT NULL,
    equipo           nvarchar(150) NULL,
    direccion_ip     varchar(45) NULL,
    CONSTRAINT PK_EventoAcceso PRIMARY KEY (id_evento_acceso),
    CONSTRAINT FK_EventoAcceso_Usuario FOREIGN KEY (id_usuario) REFERENCES dbo.Usuario (id_usuario),
    CONSTRAINT CK_EventoAcceso_resultado CHECK (resultado IN ('EXITOSO','INVALIDO','BLOQUEADO','INACTIVO'))
);
GO
CREATE TABLE dbo.RecuperacionAcceso (
    id_recuperacion bigint IDENTITY(1,1) NOT NULL,
    id_usuario      int NOT NULL,
    token_hash      binary(32) NOT NULL,
    creado_en       datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    expira_en       datetime2(3) NOT NULL,
    utilizado_en    datetime2(3) NULL,
    revocado_en     datetime2(3) NULL,
    CONSTRAINT PK_RecuperacionAcceso PRIMARY KEY (id_recuperacion),
    CONSTRAINT UQ_RecuperacionAcceso_token UNIQUE (token_hash),
    CONSTRAINT FK_RecuperacionAcceso_Usuario FOREIGN KEY (id_usuario) REFERENCES dbo.Usuario (id_usuario),
    CONSTRAINT CK_RecuperacionAcceso_expira CHECK (expira_en > creado_en)
);
GO
CREATE TABLE dbo.Producto ( -- H4: sin stock_actual
    id_producto          int IDENTITY(1,1) NOT NULL,
    codigo_barras        nvarchar(50) NOT NULL,
    nombre               nvarchar(150) NOT NULL,
    precio_unitario      decimal(10,2) NOT NULL,
    id_categoria         int NOT NULL,
    id_proveedor         int NOT NULL,
    creado_en            datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    creado_por           int NULL,
    actualizado_en       datetime2(3) NULL,
    actualizado_por      int NULL,
    desactivado_en       datetime2(3) NULL,
    desactivado_por      int NULL,
    motivo_desactivacion nvarchar(300) NULL,
    activo               AS (CONVERT(bit, CASE WHEN desactivado_en IS NULL THEN 1 ELSE 0 END)) PERSISTED,
    version_fila         rowversion NOT NULL,
    CONSTRAINT PK_Producto PRIMARY KEY (id_producto),
    CONSTRAINT UQ_Producto_codigo_barras UNIQUE (codigo_barras),
    CONSTRAINT FK_Producto_Categoria FOREIGN KEY (id_categoria) REFERENCES dbo.Categoria (id_categoria),
    CONSTRAINT FK_Producto_Proveedor FOREIGN KEY (id_proveedor) REFERENCES dbo.Proveedor (id_proveedor),
    CONSTRAINT CK_Producto_precio CHECK (precio_unitario > 0),
    CONSTRAINT CK_Producto_desactivacion CHECK (desactivado_en IS NOT NULL OR (desactivado_por IS NULL AND motivo_desactivacion IS NULL))
);
GO
CREATE TABLE dbo.InventarioTienda (
    id_tienda    int NOT NULL,
    id_producto  int NOT NULL,
    existencia   int NOT NULL DEFAULT 0,
    reservado    int NOT NULL DEFAULT 0,
    stock_minimo int NOT NULL DEFAULT 0,
    version_fila rowversion NOT NULL,
    CONSTRAINT PK_InventarioTienda PRIMARY KEY (id_tienda, id_producto),
    CONSTRAINT FK_InventarioTienda_Tienda FOREIGN KEY (id_tienda) REFERENCES dbo.Tienda (id_tienda),
    CONSTRAINT FK_InventarioTienda_Producto FOREIGN KEY (id_producto) REFERENCES dbo.Producto (id_producto),
    CONSTRAINT CK_InventarioTienda_existencia CHECK (existencia >= 0),
    CONSTRAINT CK_InventarioTienda_reservado CHECK (reservado >= 0 AND reservado <= existencia),
    CONSTRAINT CK_InventarioTienda_minimo CHECK (stock_minimo >= 0)
);
GO
CREATE TABLE dbo.Compra ( -- H5: sin total
    id_compra          int IDENTITY(1,1) NOT NULL,
    id_proveedor       int NOT NULL,
    id_tienda          int NOT NULL,
    numero_factura     nvarchar(50) NOT NULL,
    fecha_factura      date NOT NULL,
    registrado_en      datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    id_usuario         int NOT NULL,
    estado             varchar(20) NOT NULL DEFAULT 'BORRADOR',
    observaciones      nvarchar(500) NULL,
    anulado_en         datetime2(3) NULL,
    anulado_por        int NULL,
    motivo_anulacion   nvarchar(500) NULL,
    CONSTRAINT PK_Compra PRIMARY KEY (id_compra),
    CONSTRAINT UQ_Compra_proveedor_factura UNIQUE (id_proveedor, numero_factura),
    CONSTRAINT FK_Compra_Proveedor FOREIGN KEY (id_proveedor) REFERENCES dbo.Proveedor (id_proveedor),
    CONSTRAINT FK_Compra_Tienda FOREIGN KEY (id_tienda) REFERENCES dbo.Tienda (id_tienda),
    CONSTRAINT FK_Compra_Usuario FOREIGN KEY (id_usuario) REFERENCES dbo.Usuario (id_usuario),
    CONSTRAINT FK_Compra_anulado_por FOREIGN KEY (anulado_por) REFERENCES dbo.Usuario (id_usuario),
    CONSTRAINT CK_Compra_estado CHECK (estado IN ('BORRADOR','CONFIRMADA','ANULADA')),
    CONSTRAINT CK_Compra_anulacion CHECK (estado <> 'ANULADA' OR (anulado_en IS NOT NULL AND anulado_por IS NOT NULL AND motivo_anulacion IS NOT NULL))
);
GO
CREATE TABLE dbo.DetalleCompra (
    id_detalle_compra int IDENTITY(1,1) NOT NULL,
    id_compra         int NOT NULL,
    id_producto       int NOT NULL,
    cantidad          int NOT NULL,
    costo_unitario    decimal(18,2) NOT NULL,
    subtotal          AS (CONVERT(decimal(18,2), cantidad * costo_unitario)),
    CONSTRAINT PK_DetalleCompra PRIMARY KEY (id_detalle_compra),
    CONSTRAINT FK_DetalleCompra_Compra FOREIGN KEY (id_compra) REFERENCES dbo.Compra (id_compra),
    CONSTRAINT FK_DetalleCompra_Producto FOREIGN KEY (id_producto) REFERENCES dbo.Producto (id_producto),
    CONSTRAINT CK_DetalleCompra_cantidad CHECK (cantidad > 0),
    CONSTRAINT CK_DetalleCompra_costo CHECK (costo_unitario > 0)
);
GO
CREATE TABLE dbo.Venta ( -- H5: sin total
    id_venta         int IDENTITY(1,1) NOT NULL,
    numero           varchar(40) NOT NULL,
    id_tienda        int NOT NULL,
    id_cliente       int NOT NULL,
    id_usuario       int NOT NULL,
    fecha            datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    estado           varchar(20) NOT NULL DEFAULT 'BORRADOR',
    observaciones    nvarchar(500) NULL,
    anulado_en       datetime2(3) NULL,
    anulado_por      int NULL,
    motivo_anulacion nvarchar(500) NULL,
    CONSTRAINT PK_Venta PRIMARY KEY (id_venta),
    CONSTRAINT UQ_Venta_numero UNIQUE (numero),
    CONSTRAINT FK_Venta_Tienda FOREIGN KEY (id_tienda) REFERENCES dbo.Tienda (id_tienda),
    CONSTRAINT FK_Venta_Cliente FOREIGN KEY (id_cliente) REFERENCES dbo.Cliente (id_cliente),
    CONSTRAINT FK_Venta_Usuario FOREIGN KEY (id_usuario) REFERENCES dbo.Usuario (id_usuario),
    CONSTRAINT FK_Venta_anulado_por FOREIGN KEY (anulado_por) REFERENCES dbo.Usuario (id_usuario),
    CONSTRAINT CK_Venta_estado CHECK (estado IN ('BORRADOR','CONFIRMADA','ANULADA')),
    CONSTRAINT CK_Venta_anulacion CHECK (estado <> 'ANULADA' OR (anulado_en IS NOT NULL AND anulado_por IS NOT NULL AND motivo_anulacion IS NOT NULL))
);
GO
CREATE TABLE dbo.DetalleVenta (
    id_detalle_venta int IDENTITY(1,1) NOT NULL,
    id_venta         int NOT NULL,
    id_producto      int NOT NULL,
    cantidad         int NOT NULL,
    precio_unitario  decimal(18,2) NOT NULL,
    nombre_producto  nvarchar(150) NOT NULL,
    subtotal         AS (CONVERT(decimal(18,2), cantidad * precio_unitario)),
    CONSTRAINT PK_DetalleVenta PRIMARY KEY (id_detalle_venta),
    CONSTRAINT FK_DetalleVenta_Venta FOREIGN KEY (id_venta) REFERENCES dbo.Venta (id_venta),
    CONSTRAINT FK_DetalleVenta_Producto FOREIGN KEY (id_producto) REFERENCES dbo.Producto (id_producto),
    CONSTRAINT CK_DetalleVenta_cantidad CHECK (cantidad > 0),
    CONSTRAINT CK_DetalleVenta_precio CHECK (precio_unitario > 0)
);
GO
CREATE TABLE dbo.PagoVenta (
    id_pago_venta  int IDENTITY(1,1) NOT NULL,
    id_venta       int NOT NULL,
    id_metodo_pago int NOT NULL,
    monto          decimal(18,2) NOT NULL,
    monto_recibido decimal(18,2) NULL,
    referencia     nvarchar(100) NULL,
    fecha          datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT PK_PagoVenta PRIMARY KEY (id_pago_venta),
    CONSTRAINT FK_PagoVenta_Venta FOREIGN KEY (id_venta) REFERENCES dbo.Venta (id_venta),
    CONSTRAINT FK_PagoVenta_MetodoPago FOREIGN KEY (id_metodo_pago) REFERENCES dbo.MetodoPago (id_metodo_pago),
    CONSTRAINT CK_PagoVenta_monto CHECK (monto > 0),
    CONSTRAINT CK_PagoVenta_recibido CHECK (monto_recibido IS NULL OR monto_recibido >= monto)
);
GO
CREATE TABLE dbo.Traspaso ( -- H1: sin columnas de pasos
    id_traspaso       int IDENTITY(1,1) NOT NULL,
    id_tienda_origen  int NOT NULL,
    id_tienda_destino int NOT NULL,
    estado            varchar(20) NOT NULL DEFAULT 'PENDIENTE',
    solicitado_por    int NOT NULL,
    solicitado_en     datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    observaciones     nvarchar(500) NULL,
    version_fila      rowversion NOT NULL,
    CONSTRAINT PK_Traspaso PRIMARY KEY (id_traspaso),
    CONSTRAINT FK_Traspaso_Origen FOREIGN KEY (id_tienda_origen) REFERENCES dbo.Tienda (id_tienda),
    CONSTRAINT FK_Traspaso_Destino FOREIGN KEY (id_tienda_destino) REFERENCES dbo.Tienda (id_tienda),
    CONSTRAINT FK_Traspaso_solicitado_por FOREIGN KEY (solicitado_por) REFERENCES dbo.Usuario (id_usuario),
    CONSTRAINT CK_Traspaso_tiendas CHECK (id_tienda_origen <> id_tienda_destino),
    CONSTRAINT CK_Traspaso_estado CHECK (estado IN ('PENDIENTE','APROBADO','EN_TRANSITO','RECIBIDO','RECHAZADO','CANCELADO'))
);
GO
CREATE TABLE dbo.DetalleTraspaso (
    id_detalle_traspaso int IDENTITY(1,1) NOT NULL,
    id_traspaso         int NOT NULL,
    id_producto         int NOT NULL,
    cantidad            int NOT NULL,
    precio_referencia   decimal(18,2) NOT NULL,
    CONSTRAINT PK_DetalleTraspaso PRIMARY KEY (id_detalle_traspaso),
    CONSTRAINT UQ_DetalleTraspaso_producto UNIQUE (id_traspaso, id_producto),
    CONSTRAINT FK_DetalleTraspaso_Traspaso FOREIGN KEY (id_traspaso) REFERENCES dbo.Traspaso (id_traspaso),
    CONSTRAINT FK_DetalleTraspaso_Producto FOREIGN KEY (id_producto) REFERENCES dbo.Producto (id_producto),
    CONSTRAINT CK_DetalleTraspaso_cantidad CHECK (cantidad > 0),
    CONSTRAINT CK_DetalleTraspaso_precio CHECK (precio_referencia >= 0)
);
GO
CREATE TABLE dbo.EventoTraspaso (
    id_evento_traspaso bigint IDENTITY(1,1) NOT NULL,
    id_traspaso        int NOT NULL,
    estado_anterior    varchar(20) NULL,
    estado_nuevo       varchar(20) NOT NULL,
    id_usuario         int NOT NULL,
    fecha              datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    motivo             nvarchar(500) NULL,
    CONSTRAINT PK_EventoTraspaso PRIMARY KEY (id_evento_traspaso),
    CONSTRAINT FK_EventoTraspaso_Traspaso FOREIGN KEY (id_traspaso) REFERENCES dbo.Traspaso (id_traspaso),
    CONSTRAINT FK_EventoTraspaso_Usuario FOREIGN KEY (id_usuario) REFERENCES dbo.Usuario (id_usuario),
    CONSTRAINT CK_EventoTraspaso_anterior CHECK (estado_anterior IS NULL OR estado_anterior IN ('PENDIENTE','APROBADO','EN_TRANSITO','RECIBIDO','RECHAZADO','CANCELADO')),
    CONSTRAINT CK_EventoTraspaso_nuevo CHECK (estado_nuevo IN ('PENDIENTE','APROBADO','EN_TRANSITO','RECIBIDO','RECHAZADO','CANCELADO')),
    CONSTRAINT CK_EventoTraspaso_motivo CHECK (estado_nuevo NOT IN ('RECHAZADO','CANCELADO') OR motivo IS NOT NULL)
);
GO
  -- 6. APARTADOS, DEVOLUCIONES Y REEMBOLSOS
CREATE TABLE dbo.Apartado ( -- H5: sin total
    id_apartado  int IDENTITY(1,1) NOT NULL,
    id_cliente   int NOT NULL,
    id_tienda    int NOT NULL,
    id_usuario   int NOT NULL,
    fecha        datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    vence_en     datetime2(3) NOT NULL,
    estado       varchar(20) NOT NULL DEFAULT 'ACTIVO',
    id_venta     int NULL,
    version_fila rowversion NOT NULL,
    CONSTRAINT PK_Apartado PRIMARY KEY (id_apartado),
    CONSTRAINT FK_Apartado_Cliente FOREIGN KEY (id_cliente) REFERENCES dbo.Cliente (id_cliente),
    CONSTRAINT FK_Apartado_Tienda FOREIGN KEY (id_tienda) REFERENCES dbo.Tienda (id_tienda),
    CONSTRAINT FK_Apartado_Usuario FOREIGN KEY (id_usuario) REFERENCES dbo.Usuario (id_usuario),
    CONSTRAINT FK_Apartado_Venta FOREIGN KEY (id_venta) REFERENCES dbo.Venta (id_venta),
    CONSTRAINT CK_Apartado_vence CHECK (vence_en > fecha),
    CONSTRAINT CK_Apartado_estado CHECK (estado IN ('ACTIVO','ENTREGADO','CANCELADO','VENCIDO')),
    CONSTRAINT CK_Apartado_entrega CHECK (estado <> 'ENTREGADO' OR id_venta IS NOT NULL)
);
GO
CREATE TABLE dbo.DetalleApartado (
    id_detalle_apartado int IDENTITY(1,1) NOT NULL,
    id_apartado         int NOT NULL,
    id_producto         int NOT NULL,
    cantidad            int NOT NULL,
    precio_unitario     decimal(18,2) NOT NULL,
    subtotal            AS (CONVERT(decimal(18,2), cantidad * precio_unitario)),
    CONSTRAINT PK_DetalleApartado PRIMARY KEY (id_detalle_apartado),
    CONSTRAINT FK_DetalleApartado_Apartado FOREIGN KEY (id_apartado) REFERENCES dbo.Apartado (id_apartado),
    CONSTRAINT FK_DetalleApartado_Producto FOREIGN KEY (id_producto) REFERENCES dbo.Producto (id_producto),
    CONSTRAINT CK_DetalleApartado_cantidad CHECK (cantidad > 0),
    CONSTRAINT CK_DetalleApartado_precio CHECK (precio_unitario > 0)
);
GO
CREATE TABLE dbo.AbonoApartado (
    id_abono       int IDENTITY(1,1) NOT NULL,
    id_apartado    int NOT NULL,
    id_metodo_pago int NOT NULL,
    monto          decimal(18,2) NOT NULL,
    fecha          datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    id_usuario     int NOT NULL,
    referencia     nvarchar(100) NULL,
    id_pago_venta  int NULL,
    CONSTRAINT PK_AbonoApartado PRIMARY KEY (id_abono),
    CONSTRAINT FK_AbonoApartado_Apartado FOREIGN KEY (id_apartado) REFERENCES dbo.Apartado (id_apartado),
    CONSTRAINT FK_AbonoApartado_MetodoPago FOREIGN KEY (id_metodo_pago) REFERENCES dbo.MetodoPago (id_metodo_pago),
    CONSTRAINT FK_AbonoApartado_Usuario FOREIGN KEY (id_usuario) REFERENCES dbo.Usuario (id_usuario),
    CONSTRAINT FK_AbonoApartado_PagoVenta FOREIGN KEY (id_pago_venta) REFERENCES dbo.PagoVenta (id_pago_venta),
    CONSTRAINT CK_AbonoApartado_monto CHECK (monto > 0)
);
GO
CREATE TABLE dbo.Devolucion ( -- H5: sin total
    id_devolucion    int IDENTITY(1,1) NOT NULL,
    id_venta         int NOT NULL,
    id_tienda        int NOT NULL,
    id_usuario       int NOT NULL,
    fecha            datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    motivo           nvarchar(500) NOT NULL,
    estado           varchar(20) NOT NULL DEFAULT 'BORRADOR',
    anulado_en       datetime2(3) NULL,
    anulado_por      int NULL,
    motivo_anulacion nvarchar(500) NULL,
    CONSTRAINT PK_Devolucion PRIMARY KEY (id_devolucion),
    CONSTRAINT FK_Devolucion_Venta FOREIGN KEY (id_venta) REFERENCES dbo.Venta (id_venta),
    CONSTRAINT FK_Devolucion_Tienda FOREIGN KEY (id_tienda) REFERENCES dbo.Tienda (id_tienda),
    CONSTRAINT FK_Devolucion_Usuario FOREIGN KEY (id_usuario) REFERENCES dbo.Usuario (id_usuario),
    CONSTRAINT FK_Devolucion_anulado_por FOREIGN KEY (anulado_por) REFERENCES dbo.Usuario (id_usuario),
    CONSTRAINT CK_Devolucion_estado CHECK (estado IN ('BORRADOR','CONFIRMADA','ANULADA')),
    CONSTRAINT CK_Devolucion_anulacion CHECK (estado <> 'ANULADA' OR (anulado_en IS NOT NULL AND anulado_por IS NOT NULL AND motivo_anulacion IS NOT NULL))
);
GO
CREATE TABLE dbo.DetalleDevolucion ( -- H6: sin precio_unitario ni subtotal
    id_detalle_devolucion int IDENTITY(1,1) NOT NULL,
    id_devolucion         int NOT NULL,
    id_detalle_venta      int NOT NULL,
    cantidad              int NOT NULL,
    reintegra_stock       bit NOT NULL,
    CONSTRAINT PK_DetalleDevolucion PRIMARY KEY (id_detalle_devolucion),
    CONSTRAINT FK_DetalleDevolucion_Devolucion FOREIGN KEY (id_devolucion) REFERENCES dbo.Devolucion (id_devolucion),
    CONSTRAINT FK_DetalleDevolucion_DetalleVenta FOREIGN KEY (id_detalle_venta) REFERENCES dbo.DetalleVenta (id_detalle_venta),
    CONSTRAINT CK_DetalleDevolucion_cantidad CHECK (cantidad > 0)
);
GO
CREATE TABLE dbo.Reembolso (
    id_reembolso   int IDENTITY(1,1) NOT NULL,
    id_pago_venta  int NULL,
    id_abono       int NULL,
    id_devolucion  int NULL,
    monto          decimal(18,2) NOT NULL,
    id_metodo_pago int NOT NULL,
    id_usuario     int NOT NULL,
    fecha          datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    referencia     nvarchar(100) NULL,
    motivo         nvarchar(500) NOT NULL,
    CONSTRAINT PK_Reembolso PRIMARY KEY (id_reembolso),
    CONSTRAINT FK_Reembolso_PagoVenta FOREIGN KEY (id_pago_venta) REFERENCES dbo.PagoVenta (id_pago_venta),
    CONSTRAINT FK_Reembolso_AbonoApartado FOREIGN KEY (id_abono) REFERENCES dbo.AbonoApartado (id_abono),
    CONSTRAINT FK_Reembolso_Devolucion FOREIGN KEY (id_devolucion) REFERENCES dbo.Devolucion (id_devolucion),
    CONSTRAINT FK_Reembolso_MetodoPago FOREIGN KEY (id_metodo_pago) REFERENCES dbo.MetodoPago (id_metodo_pago),
    CONSTRAINT FK_Reembolso_Usuario FOREIGN KEY (id_usuario) REFERENCES dbo.Usuario (id_usuario),
    CONSTRAINT CK_Reembolso_monto CHECK (monto > 0),
    CONSTRAINT CK_Reembolso_cobro CHECK ((id_pago_venta IS NOT NULL AND id_abono IS NULL) OR (id_pago_venta IS NULL AND id_abono IS NOT NULL))
);
GO
CREATE TABLE dbo.MovimientoInventario (
    id_movimiento           bigint IDENTITY(1,1) NOT NULL,
    id_tienda               int NOT NULL,
    id_producto             int NOT NULL,
    fecha                   datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    tipo                    varchar(30) NOT NULL,
    cantidad_delta          int NOT NULL,
    id_usuario              int NOT NULL,
    id_detalle_compra       int NULL,
    id_detalle_venta        int NULL,
    id_detalle_traspaso     int NULL,
    id_detalle_devolucion   int NULL,
    id_movimiento_revertido bigint NULL,
    motivo                  nvarchar(500) NULL,
    CONSTRAINT PK_MovimientoInventario PRIMARY KEY (id_movimiento),
    CONSTRAINT FK_Movimiento_Tienda FOREIGN KEY (id_tienda) REFERENCES dbo.Tienda (id_tienda),
    CONSTRAINT FK_Movimiento_Producto FOREIGN KEY (id_producto) REFERENCES dbo.Producto (id_producto),
    CONSTRAINT FK_Movimiento_Usuario FOREIGN KEY (id_usuario) REFERENCES dbo.Usuario (id_usuario),
    CONSTRAINT FK_Movimiento_DetalleCompra FOREIGN KEY (id_detalle_compra) REFERENCES dbo.DetalleCompra (id_detalle_compra),
    CONSTRAINT FK_Movimiento_DetalleVenta FOREIGN KEY (id_detalle_venta) REFERENCES dbo.DetalleVenta (id_detalle_venta),
    CONSTRAINT FK_Movimiento_DetalleTraspaso FOREIGN KEY (id_detalle_traspaso) REFERENCES dbo.DetalleTraspaso (id_detalle_traspaso),
    CONSTRAINT FK_Movimiento_DetalleDevolucion FOREIGN KEY (id_detalle_devolucion) REFERENCES dbo.DetalleDevolucion (id_detalle_devolucion),
    CONSTRAINT FK_Movimiento_Revertido FOREIGN KEY (id_movimiento_revertido) REFERENCES dbo.MovimientoInventario (id_movimiento),
    CONSTRAINT CK_Movimiento_tipo CHECK (tipo IN ('COMPRA','VENTA','TRASPASO_SALIDA','TRASPASO_ENTRADA','DEVOLUCION','AJUSTE','REVERSO')),
    CONSTRAINT CK_Movimiento_delta CHECK (cantidad_delta <> 0),
    CONSTRAINT CK_Movimiento_origen CHECK (
           (tipo = 'COMPRA' AND id_detalle_compra IS NOT NULL AND id_detalle_venta IS NULL AND id_detalle_traspaso IS NULL AND id_detalle_devolucion IS NULL AND id_movimiento_revertido IS NULL)
        OR (tipo = 'VENTA' AND id_detalle_venta IS NOT NULL AND id_detalle_compra IS NULL AND id_detalle_traspaso IS NULL AND id_detalle_devolucion IS NULL AND id_movimiento_revertido IS NULL)
        OR (tipo IN ('TRASPASO_SALIDA','TRASPASO_ENTRADA') AND id_detalle_traspaso IS NOT NULL AND id_detalle_compra IS NULL AND id_detalle_venta IS NULL AND id_detalle_devolucion IS NULL AND id_movimiento_revertido IS NULL)
        OR (tipo = 'DEVOLUCION' AND id_detalle_devolucion IS NOT NULL AND id_detalle_compra IS NULL AND id_detalle_venta IS NULL AND id_detalle_traspaso IS NULL AND id_movimiento_revertido IS NULL)
        OR (tipo = 'AJUSTE' AND id_detalle_compra IS NULL AND id_detalle_venta IS NULL AND id_detalle_traspaso IS NULL AND id_detalle_devolucion IS NULL AND id_movimiento_revertido IS NULL AND motivo IS NOT NULL)
        OR (tipo = 'REVERSO' AND id_movimiento_revertido IS NOT NULL AND id_detalle_compra IS NULL AND id_detalle_venta IS NULL AND id_detalle_traspaso IS NULL AND id_detalle_devolucion IS NULL AND motivo IS NOT NULL)
    )
);
GO
-- 8. INVERSIONES, IMPORTACIONES, AUDITORIA Y AVISOS
CREATE TABLE dbo.Inversion (
    id_inversion         int IDENTITY(1,1) NOT NULL,
    institucion          nvarchar(150) NOT NULL,
    tipo                 varchar(20) NOT NULL,
    monto                decimal(18,2) NOT NULL,
    tasa_interes         decimal(5,2) NOT NULL,
    fecha_inicio         date NOT NULL,
    fecha_vencimiento    date NOT NULL,
    estado               varchar(20) NOT NULL,
    creado_en            datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    creado_por           int NULL,
    actualizado_en       datetime2(3) NULL,
    actualizado_por      int NULL,
    desactivado_en       datetime2(3) NULL,
    desactivado_por      int NULL,
    motivo_desactivacion nvarchar(300) NULL,
    activo               AS (CONVERT(bit, CASE WHEN desactivado_en IS NULL THEN 1 ELSE 0 END)) PERSISTED,
    version_fila         rowversion NOT NULL,
    CONSTRAINT PK_Inversion PRIMARY KEY (id_inversion),
    CONSTRAINT CK_Inversion_tipo CHECK (tipo IN ('PLAZO_FIJO','AHORRO','OTRO')),
    CONSTRAINT CK_Inversion_monto CHECK (monto > 0),
    CONSTRAINT CK_Inversion_tasa CHECK (tasa_interes BETWEEN 0 AND 100),
    CONSTRAINT CK_Inversion_fechas CHECK (fecha_vencimiento >= fecha_inicio),
    CONSTRAINT CK_Inversion_estado CHECK (estado IN ('VIGENTE','VENCIDA','LIQUIDADA','CANCELADA')),
    CONSTRAINT CK_Inversion_desactivacion CHECK (desactivado_en IS NOT NULL OR (desactivado_por IS NULL AND motivo_desactivacion IS NULL))
);
GO
CREATE TABLE dbo.Importacion (
    id_importacion int IDENTITY(1,1) NOT NULL,
    nombre_archivo nvarchar(260) NOT NULL,
    archivo_hash   binary(32) NOT NULL,
    id_usuario     int NOT NULL,
    fecha          datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    estado         varchar(20) NOT NULL,
    CONSTRAINT PK_Importacion PRIMARY KEY (id_importacion),
    CONSTRAINT FK_Importacion_Usuario FOREIGN KEY (id_usuario) REFERENCES dbo.Usuario (id_usuario),
    CONSTRAINT CK_Importacion_estado CHECK (estado IN ('VALIDANDO','CON_ERRORES','LISTA','COMPLETADA','CANCELADA'))
);
GO
CREATE TABLE dbo.DetalleImportacion (
    id_detalle_importacion bigint IDENTITY(1,1) NOT NULL,
    id_importacion         int NOT NULL,
    numero_fila            int NOT NULL,
    datos_originales_json  nvarchar(max) NOT NULL,
    estado                 varchar(20) NOT NULL,
    error                  nvarchar(1000) NULL,
    id_producto            int NULL,
    CONSTRAINT PK_DetalleImportacion PRIMARY KEY (id_detalle_importacion),
    CONSTRAINT UQ_DetalleImportacion_fila UNIQUE (id_importacion, numero_fila),
    CONSTRAINT FK_DetalleImportacion_Importacion FOREIGN KEY (id_importacion) REFERENCES dbo.Importacion (id_importacion),
    CONSTRAINT FK_DetalleImportacion_Producto FOREIGN KEY (id_producto) REFERENCES dbo.Producto (id_producto),
    CONSTRAINT CK_DetalleImportacion_fila CHECK (numero_fila > 0),
    CONSTRAINT CK_DetalleImportacion_json CHECK (ISJSON(datos_originales_json) = 1),
    CONSTRAINT CK_DetalleImportacion_estado CHECK (estado IN ('VALIDA','ERROR','DUPLICADA','IMPORTADA','OMITIDA'))
);
GO
CREATE TABLE dbo.Auditoria (
    id_auditoria          bigint IDENTITY(1,1) NOT NULL,
    fecha                 datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    id_usuario            int NULL,
    modulo                varchar(50) NOT NULL,
    accion                varchar(50) NOT NULL,
    entidad               varchar(80) NOT NULL,
    id_registro           nvarchar(100) NOT NULL,
    descripcion           nvarchar(1000) NOT NULL,
    valores_antes_json    nvarchar(max) NULL,
    valores_despues_json  nvarchar(max) NULL,
    equipo                nvarchar(150) NULL,
    CONSTRAINT PK_Auditoria PRIMARY KEY (id_auditoria),
    CONSTRAINT FK_Auditoria_Usuario FOREIGN KEY (id_usuario) REFERENCES dbo.Usuario (id_usuario),
    CONSTRAINT CK_Auditoria_antes_json CHECK (valores_antes_json IS NULL OR ISJSON(valores_antes_json) = 1),
    CONSTRAINT CK_Auditoria_despues_json CHECK (valores_despues_json IS NULL OR ISJSON(valores_despues_json) = 1)
);
GO
CREATE TABLE dbo.EventoNotificacion ( -- H3
    id_evento_notificacion bigint IDENTITY(1,1) NOT NULL,
    clave_evento           varchar(150) NOT NULL,
    tipo                   varchar(40) NOT NULL,
    titulo                 nvarchar(150) NOT NULL,
    mensaje                nvarchar(500) NOT NULL,
    creada_en              datetime2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT PK_EventoNotificacion PRIMARY KEY (id_evento_notificacion),
    CONSTRAINT UQ_EventoNotificacion_clave UNIQUE (clave_evento),
    CONSTRAINT CK_EventoNotificacion_tipo CHECK (tipo IN ('STOCK_BAJO','TRASPASO_PENDIENTE','INVERSION_VENCIMIENTO'))
);
GO
CREATE TABLE dbo.Notificacion ( -- H3
    id_notificacion        bigint IDENTITY(1,1) NOT NULL,
    id_evento_notificacion bigint NOT NULL,
    id_usuario             int NOT NULL,
    leida_en               datetime2(3) NULL,
    CONSTRAINT PK_Notificacion PRIMARY KEY (id_notificacion),
    CONSTRAINT UQ_Notificacion_evento_usuario UNIQUE (id_evento_notificacion, id_usuario),
    CONSTRAINT FK_Notificacion_Evento FOREIGN KEY (id_evento_notificacion) REFERENCES dbo.EventoNotificacion (id_evento_notificacion),
    CONSTRAINT FK_Notificacion_Usuario FOREIGN KEY (id_usuario) REFERENCES dbo.Usuario (id_usuario)
);
GO
-- (se agregan al final para resolver la dependencia circular con Usuario)
ALTER TABLE dbo.Categoria  ADD CONSTRAINT FK_Categoria_creado_por       FOREIGN KEY (creado_por)      REFERENCES dbo.Usuario (id_usuario),
                               CONSTRAINT FK_Categoria_actualizado_por  FOREIGN KEY (actualizado_por) REFERENCES dbo.Usuario (id_usuario),
                               CONSTRAINT FK_Categoria_desactivado_por  FOREIGN KEY (desactivado_por) REFERENCES dbo.Usuario (id_usuario);
ALTER TABLE dbo.Proveedor  ADD CONSTRAINT FK_Proveedor_creado_por       FOREIGN KEY (creado_por)      REFERENCES dbo.Usuario (id_usuario),
                               CONSTRAINT FK_Proveedor_actualizado_por  FOREIGN KEY (actualizado_por) REFERENCES dbo.Usuario (id_usuario),
                               CONSTRAINT FK_Proveedor_desactivado_por  FOREIGN KEY (desactivado_por) REFERENCES dbo.Usuario (id_usuario);
ALTER TABLE dbo.Cliente    ADD CONSTRAINT FK_Cliente_creado_por         FOREIGN KEY (creado_por)      REFERENCES dbo.Usuario (id_usuario),
                               CONSTRAINT FK_Cliente_actualizado_por    FOREIGN KEY (actualizado_por) REFERENCES dbo.Usuario (id_usuario),
                               CONSTRAINT FK_Cliente_desactivado_por    FOREIGN KEY (desactivado_por) REFERENCES dbo.Usuario (id_usuario);
ALTER TABLE dbo.Tienda     ADD CONSTRAINT FK_Tienda_creado_por          FOREIGN KEY (creado_por)      REFERENCES dbo.Usuario (id_usuario),
                               CONSTRAINT FK_Tienda_actualizado_por     FOREIGN KEY (actualizado_por) REFERENCES dbo.Usuario (id_usuario),
                               CONSTRAINT FK_Tienda_desactivado_por     FOREIGN KEY (desactivado_por) REFERENCES dbo.Usuario (id_usuario);
ALTER TABLE dbo.MetodoPago ADD CONSTRAINT FK_MetodoPago_creado_por      FOREIGN KEY (creado_por)      REFERENCES dbo.Usuario (id_usuario),
                               CONSTRAINT FK_MetodoPago_actualizado_por FOREIGN KEY (actualizado_por) REFERENCES dbo.Usuario (id_usuario),
                               CONSTRAINT FK_MetodoPago_desactivado_por FOREIGN KEY (desactivado_por) REFERENCES dbo.Usuario (id_usuario);
ALTER TABLE dbo.Empleado   ADD CONSTRAINT FK_Empleado_creado_por        FOREIGN KEY (creado_por)      REFERENCES dbo.Usuario (id_usuario),
                               CONSTRAINT FK_Empleado_actualizado_por   FOREIGN KEY (actualizado_por) REFERENCES dbo.Usuario (id_usuario),
                               CONSTRAINT FK_Empleado_desactivado_por   FOREIGN KEY (desactivado_por) REFERENCES dbo.Usuario (id_usuario);
ALTER TABLE dbo.Usuario    ADD CONSTRAINT FK_Usuario_creado_por         FOREIGN KEY (creado_por)      REFERENCES dbo.Usuario (id_usuario),
                               CONSTRAINT FK_Usuario_actualizado_por    FOREIGN KEY (actualizado_por) REFERENCES dbo.Usuario (id_usuario),
                               CONSTRAINT FK_Usuario_desactivado_por    FOREIGN KEY (desactivado_por) REFERENCES dbo.Usuario (id_usuario);
ALTER TABLE dbo.Rol        ADD CONSTRAINT FK_Rol_creado_por             FOREIGN KEY (creado_por)      REFERENCES dbo.Usuario (id_usuario),
                               CONSTRAINT FK_Rol_actualizado_por        FOREIGN KEY (actualizado_por) REFERENCES dbo.Usuario (id_usuario),
                               CONSTRAINT FK_Rol_desactivado_por        FOREIGN KEY (desactivado_por) REFERENCES dbo.Usuario (id_usuario);
ALTER TABLE dbo.Permiso    ADD CONSTRAINT FK_Permiso_creado_por         FOREIGN KEY (creado_por)      REFERENCES dbo.Usuario (id_usuario),
                               CONSTRAINT FK_Permiso_actualizado_por    FOREIGN KEY (actualizado_por) REFERENCES dbo.Usuario (id_usuario),
                               CONSTRAINT FK_Permiso_desactivado_por    FOREIGN KEY (desactivado_por) REFERENCES dbo.Usuario (id_usuario);
ALTER TABLE dbo.Producto   ADD CONSTRAINT FK_Producto_creado_por        FOREIGN KEY (creado_por)      REFERENCES dbo.Usuario (id_usuario),
                               CONSTRAINT FK_Producto_actualizado_por   FOREIGN KEY (actualizado_por) REFERENCES dbo.Usuario (id_usuario),
                               CONSTRAINT FK_Producto_desactivado_por   FOREIGN KEY (desactivado_por) REFERENCES dbo.Usuario (id_usuario);
ALTER TABLE dbo.Inversion  ADD CONSTRAINT FK_Inversion_creado_por       FOREIGN KEY (creado_por)      REFERENCES dbo.Usuario (id_usuario),
                               CONSTRAINT FK_Inversion_actualizado_por  FOREIGN KEY (actualizado_por) REFERENCES dbo.Usuario (id_usuario),
                               CONSTRAINT FK_Inversion_desactivado_por  FOREIGN KEY (desactivado_por) REFERENCES dbo.Usuario (id_usuario);
GO
CREATE UNIQUE INDEX UX_Proveedor_rtn                    ON dbo.Proveedor (rtn)                          WHERE rtn IS NOT NULL;
CREATE UNIQUE INDEX UX_Cliente_documento_fiscal         ON dbo.Cliente (documento_fiscal)               WHERE documento_fiscal IS NOT NULL;
CREATE UNIQUE INDEX UX_Apartado_venta                   ON dbo.Apartado (id_venta)                      WHERE id_venta IS NOT NULL;
CREATE UNIQUE INDEX UX_AbonoApartado_pago_venta         ON dbo.AbonoApartado (id_pago_venta)            WHERE id_pago_venta IS NOT NULL;
CREATE UNIQUE INDEX UX_EventoTraspaso_estado            ON dbo.EventoTraspaso (id_traspaso, estado_nuevo);
CREATE UNIQUE INDEX UX_Movimiento_detalle_compra        ON dbo.MovimientoInventario (id_detalle_compra)                 WHERE id_detalle_compra IS NOT NULL;
CREATE UNIQUE INDEX UX_Movimiento_detalle_venta         ON dbo.MovimientoInventario (id_detalle_venta)                  WHERE id_detalle_venta IS NOT NULL;
CREATE UNIQUE INDEX UX_Movimiento_detalle_traspaso_tipo ON dbo.MovimientoInventario (id_detalle_traspaso, tipo)         WHERE id_detalle_traspaso IS NOT NULL;
CREATE UNIQUE INDEX UX_Movimiento_detalle_devolucion    ON dbo.MovimientoInventario (id_detalle_devolucion)             WHERE id_detalle_devolucion IS NOT NULL;
CREATE UNIQUE INDEX UX_Movimiento_revertido             ON dbo.MovimientoInventario (id_movimiento_revertido)           WHERE id_movimiento_revertido IS NOT NULL;
GO
CREATE INDEX IX_Empleado_tienda               ON dbo.Empleado (id_tienda);
CREATE INDEX IX_Producto_categoria            ON dbo.Producto (id_categoria);
CREATE INDEX IX_Producto_proveedor            ON dbo.Producto (id_proveedor);
CREATE INDEX IX_InventarioTienda_producto     ON dbo.InventarioTienda (id_producto);
CREATE INDEX IX_UsuarioRol_rol                ON dbo.UsuarioRol (id_rol);
CREATE INDEX IX_RolPermiso_permiso            ON dbo.RolPermiso (id_permiso);
CREATE INDEX IX_UsuarioTienda_tienda          ON dbo.UsuarioTienda (id_tienda);
CREATE INDEX IX_SesionUsuario_usuario         ON dbo.SesionUsuario (id_usuario);
CREATE INDEX IX_EventoAcceso_usuario          ON dbo.EventoAcceso (id_usuario);
CREATE INDEX IX_EventoAcceso_nombre_fecha     ON dbo.EventoAcceso (nombre_intentado, fecha);
CREATE INDEX IX_RecuperacionAcceso_usuario    ON dbo.RecuperacionAcceso (id_usuario);
CREATE INDEX IX_Compra_tienda                 ON dbo.Compra (id_tienda);
CREATE INDEX IX_Compra_usuario                ON dbo.Compra (id_usuario);
CREATE INDEX IX_DetalleCompra_compra          ON dbo.DetalleCompra (id_compra);
CREATE INDEX IX_DetalleCompra_producto        ON dbo.DetalleCompra (id_producto);
CREATE INDEX IX_Venta_tienda_fecha            ON dbo.Venta (id_tienda, fecha);
CREATE INDEX IX_Venta_cliente                 ON dbo.Venta (id_cliente);
CREATE INDEX IX_Venta_usuario                 ON dbo.Venta (id_usuario);
CREATE INDEX IX_DetalleVenta_venta            ON dbo.DetalleVenta (id_venta);
CREATE INDEX IX_DetalleVenta_producto         ON dbo.DetalleVenta (id_producto);
CREATE INDEX IX_PagoVenta_venta               ON dbo.PagoVenta (id_venta);
CREATE INDEX IX_PagoVenta_metodo_pago         ON dbo.PagoVenta (id_metodo_pago);
CREATE INDEX IX_Traspaso_estado_destino       ON dbo.Traspaso (estado, id_tienda_destino);
CREATE INDEX IX_Traspaso_origen               ON dbo.Traspaso (id_tienda_origen);
CREATE INDEX IX_Traspaso_solicitado_por       ON dbo.Traspaso (solicitado_por);
CREATE INDEX IX_DetalleTraspaso_producto      ON dbo.DetalleTraspaso (id_producto);
CREATE INDEX IX_EventoTraspaso_usuario        ON dbo.EventoTraspaso (id_usuario);
CREATE INDEX IX_Apartado_cliente              ON dbo.Apartado (id_cliente);
CREATE INDEX IX_Apartado_tienda               ON dbo.Apartado (id_tienda);
CREATE INDEX IX_Apartado_usuario              ON dbo.Apartado (id_usuario);
CREATE INDEX IX_DetalleApartado_apartado      ON dbo.DetalleApartado (id_apartado);
CREATE INDEX IX_DetalleApartado_producto      ON dbo.DetalleApartado (id_producto);
CREATE INDEX IX_AbonoApartado_apartado        ON dbo.AbonoApartado (id_apartado);
CREATE INDEX IX_AbonoApartado_metodo_pago     ON dbo.AbonoApartado (id_metodo_pago);
CREATE INDEX IX_AbonoApartado_usuario         ON dbo.AbonoApartado (id_usuario);
CREATE INDEX IX_Devolucion_venta              ON dbo.Devolucion (id_venta);
CREATE INDEX IX_Devolucion_tienda             ON dbo.Devolucion (id_tienda);
CREATE INDEX IX_Devolucion_usuario            ON dbo.Devolucion (id_usuario);
CREATE INDEX IX_DetalleDevolucion_devolucion  ON dbo.DetalleDevolucion (id_devolucion);
CREATE INDEX IX_DetalleDevolucion_detalle_venta ON dbo.DetalleDevolucion (id_detalle_venta);
CREATE INDEX IX_Reembolso_pago_venta          ON dbo.Reembolso (id_pago_venta);
CREATE INDEX IX_Reembolso_abono               ON dbo.Reembolso (id_abono);
CREATE INDEX IX_Reembolso_devolucion          ON dbo.Reembolso (id_devolucion);
CREATE INDEX IX_Reembolso_metodo_pago         ON dbo.Reembolso (id_metodo_pago);
CREATE INDEX IX_Reembolso_usuario             ON dbo.Reembolso (id_usuario);
CREATE INDEX IX_Movimiento_tienda_producto_fecha ON dbo.MovimientoInventario (id_tienda, id_producto, fecha);
CREATE INDEX IX_Movimiento_producto           ON dbo.MovimientoInventario (id_producto);
CREATE INDEX IX_Movimiento_usuario            ON dbo.MovimientoInventario (id_usuario);
CREATE INDEX IX_Importacion_usuario           ON dbo.Importacion (id_usuario);
CREATE INDEX IX_DetalleImportacion_producto   ON dbo.DetalleImportacion (id_producto);
CREATE INDEX IX_Auditoria_fecha_usuario       ON dbo.Auditoria (fecha, id_usuario);
CREATE INDEX IX_Auditoria_usuario             ON dbo.Auditoria (id_usuario);
CREATE INDEX IX_Notificacion_usuario          ON dbo.Notificacion (id_usuario);
GO
-- 11. VISTAS (reemplazan los datos derivados eliminados en la normalizacion)
CREATE VIEW dbo.vw_ProductoStock AS
SELECT p.id_producto, p.codigo_barras, p.nombre, p.precio_unitario,
       p.id_categoria, p.id_proveedor,
       ISNULL(s.stock_actual, 0) AS stock_actual,
       p.creado_en, p.creado_por, p.actualizado_en, p.actualizado_por,
       p.desactivado_en, p.desactivado_por, p.motivo_desactivacion,
       p.activo, p.version_fila
FROM dbo.Producto p
OUTER APPLY (SELECT SUM(i.existencia) AS stock_actual
             FROM dbo.InventarioTienda i
             WHERE i.id_producto = p.id_producto) s;
GO
CREATE VIEW dbo.vw_CompraTotal AS
SELECT c.id_compra,
       CONVERT(decimal(18,2), ISNULL(SUM(d.cantidad * d.costo_unitario), 0)) AS total
FROM dbo.Compra c
LEFT JOIN dbo.DetalleCompra d ON d.id_compra = c.id_compra
GROUP BY c.id_compra;
GO
CREATE VIEW dbo.vw_VentaTotal AS
SELECT v.id_venta,
       CONVERT(decimal(18,2), ISNULL(SUM(d.cantidad * d.precio_unitario), 0)) AS total
FROM dbo.Venta v
LEFT JOIN dbo.DetalleVenta d ON d.id_venta = v.id_venta
GROUP BY v.id_venta;
GO
CREATE VIEW dbo.vw_ApartadoTotal AS
SELECT a.id_apartado,
       CONVERT(decimal(18,2), ISNULL(SUM(d.cantidad * d.precio_unitario), 0)) AS total
FROM dbo.Apartado a
LEFT JOIN dbo.DetalleApartado d ON d.id_apartado = a.id_apartado
GROUP BY a.id_apartado;
GO
CREATE VIEW dbo.vw_DevolucionDetalle AS
SELECT dd.id_detalle_devolucion, dd.id_devolucion, dd.id_detalle_venta,
       dd.cantidad, dv.precio_unitario,
       CONVERT(decimal(18,2), dd.cantidad * dv.precio_unitario) AS subtotal,
       dd.reintegra_stock
FROM dbo.DetalleDevolucion dd
JOIN dbo.DetalleVenta dv ON dv.id_detalle_venta = dd.id_detalle_venta;
GO
CREATE VIEW dbo.vw_DevolucionTotal AS
SELECT d.id_devolucion,
       CONVERT(decimal(18,2), ISNULL(SUM(x.subtotal), 0)) AS total
FROM dbo.Devolucion d
LEFT JOIN dbo.vw_DevolucionDetalle x ON x.id_devolucion = d.id_devolucion
GROUP BY d.id_devolucion;
GO
CREATE VIEW dbo.vw_TraspasoPasos AS
SELECT t.id_traspaso,
       MAX(CASE WHEN e.estado_nuevo = 'APROBADO'    THEN e.id_usuario END) AS aprobado_por,
       MAX(CASE WHEN e.estado_nuevo = 'APROBADO'    THEN e.fecha END)      AS aprobado_en,
       MAX(CASE WHEN e.estado_nuevo = 'EN_TRANSITO' THEN e.id_usuario END) AS enviado_por,
       MAX(CASE WHEN e.estado_nuevo = 'EN_TRANSITO' THEN e.fecha END)      AS enviado_en,
       MAX(CASE WHEN e.estado_nuevo = 'RECIBIDO'    THEN e.id_usuario END) AS recibido_por,
       MAX(CASE WHEN e.estado_nuevo = 'RECIBIDO'    THEN e.fecha END)      AS recibido_en
FROM dbo.Traspaso t
LEFT JOIN dbo.EventoTraspaso e ON e.id_traspaso = t.id_traspaso
GROUP BY t.id_traspaso;
GO
-- 12. DATOS INICIALES (opcional, se puede ejecutar varias veces)
       -- No se siembran claves ni usuarios: el primer administrador se crea con la
       -- herramienta de arranque. Tampoco se siembran permisos ni tienda inicial,
       -- porque dependen de los modulos que implemente la aplicacion.
INSERT INTO dbo.Rol (codigo, nombre, descripcion)
SELECT v.codigo, v.nombre, v.descripcion
FROM (VALUES
    ('ADMINISTRADOR', N'Administrador', N'Acceso total al sistema'),
    ('GERENTE',       N'Gerente',       N'Gestion de catalogo, compras, ventas y traspasos de sus tiendas'),
    ('CAJERO',        N'Cajero',        N'Ventas, apartados y consulta de disponibilidad en sus tiendas')
) AS v (codigo, nombre, descripcion)
WHERE NOT EXISTS (SELECT 1 FROM dbo.Rol r WHERE r.codigo = v.codigo);
INSERT INTO dbo.MetodoPago (codigo, nombre)
SELECT v.codigo, v.nombre
FROM (VALUES
    ('EFECTIVO',      N'Efectivo'),
    ('TARJETA',       N'Tarjeta'),
    ('TRANSFERENCIA', N'Transferencia')
) AS v (codigo, nombre)
WHERE NOT EXISTS (SELECT 1 FROM dbo.MetodoPago m WHERE m.codigo = v.codigo);
IF NOT EXISTS (SELECT 1 FROM dbo.Cliente WHERE nombre = N'Cliente general')
    INSERT INTO dbo.Cliente (nombre) VALUES (N'Cliente general');
GO
-- 13. VERIFICACION (esperado: 38 tablas y 7 vistas)
SELECT (SELECT COUNT(*) FROM sys.tables  WHERE schema_id = SCHEMA_ID('dbo')) AS tablas,
       (SELECT COUNT(*) FROM sys.views   WHERE schema_id = SCHEMA_ID('dbo')) AS vistas,
       (SELECT COUNT(*) FROM sys.foreign_keys WHERE schema_id = SCHEMA_ID('dbo')) AS llaves_foraneas;
GO
