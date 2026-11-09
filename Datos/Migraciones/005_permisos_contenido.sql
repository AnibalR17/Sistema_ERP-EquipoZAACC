SET XACT_ABORT ON;
BEGIN TRANSACTION;

INSERT dbo.Permiso(modulo,accion,descripcion)
SELECT m.modulo,a.accion,CONCAT(a.descripcion,N' ',m.descripcion)
FROM (VALUES
    ('VENTAS',N'ventas'),
    ('INVENTARIO',N'inventario y notificaciones de stock'),
    ('COMPRAS',N'compras'),
    ('PROVEEDORES',N'proveedores'),
    ('TRASPASOS',N'solicitudes, envíos y recepciones de traspasos'),
    ('VALIDACION_TRASPASOS',N'aprobación y rechazo de traspasos'),
    ('CARGA_MASIVA',N'carga masiva de productos'),
    ('PRODUCTOS',N'catálogo de productos y categorías'),
    ('APARTADOS_DEVOLUCIONES',N'apartados y devoluciones'),
    ('DIRECTORIO',N'directorio de clientes, proveedores y empleados'),
    ('SEGURIDAD',N'usuarios, roles y permisos'),
    ('CONFIGURACION',N'tiendas, facturación y auditoría')
) m(modulo,descripcion)
CROSS JOIN (VALUES ('CONSULTAR',N'Consultar'),('GESTIONAR',N'Modificar')) a(accion,descripcion)
WHERE NOT EXISTS(SELECT 1 FROM dbo.Permiso p WITH(UPDLOCK,HOLDLOCK)
    WHERE p.modulo=m.modulo AND p.accion=a.accion);

COMMIT;
GO
