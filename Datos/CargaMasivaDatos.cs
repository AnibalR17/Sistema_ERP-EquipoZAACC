using Microsoft.Data.SqlClient;
using Sistema_ERP.Clases;
using System.Data;

namespace Sistema_ERP;


public record ProductoImportado(
    string CodigoBarras, string Nombre, decimal Precio, int StockInicial, int IdCategoria, int IdProveedor, int FilaExcel = 0);


public static class CargaMasivaDatos
{
    private const int uniqueIndex = 2601;
    private const int uniqueConstraint = 2627;


    public static string NormalizarRtn(string? rtn) =>
        (rtn ?? "").Replace("-", "").Replace(" ", "").Trim();

    public static HashSet<string> CodigosExistentes()
    {
        var codigos = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        using SqlConnection conexion = Conexion.AbrirConexion();
        AutorizacionDatos.Exigir(conexion, Modulos.cargaMasiva);
        using var comando = new SqlCommand("SELECT codigo_barras FROM Producto", conexion);
        using SqlDataReader lector = comando.ExecuteReader();
        while (lector.Read()) codigos.Add(lector.GetString(0).Trim());
        return codigos;
    }


    public static Dictionary<string, int> MapaCategorias()
    {
        var mapa = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        using SqlConnection conexion = Conexion.AbrirConexion();
        AutorizacionDatos.Exigir(conexion, Modulos.cargaMasiva);
        using var comando = new SqlCommand("SELECT id_categoria, nombre FROM dbo.Categoria WHERE activo=1", conexion);
        using SqlDataReader lector = comando.ExecuteReader();
        while (lector.Read()) mapa[lector.GetString(1).Trim()] = lector.GetInt32(0);
        return mapa;
    }


    public static Dictionary<string, int> MapaProveedoresPorRtn()
    {
        var mapa = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        using SqlConnection conexion = Conexion.AbrirConexion();
        AutorizacionDatos.Exigir(conexion, Modulos.cargaMasiva);
        using var comando = new SqlCommand("SELECT id_proveedor, rtn FROM dbo.Proveedor WHERE activo=1 AND rtn IS NOT NULL", conexion);
        using SqlDataReader lector = comando.ExecuteReader();
        while (lector.Read())
        {
            string rtn = NormalizarRtn(lector.GetString(1));
            if (rtn.Length == 0) continue;
            ImportarProductosValidacion.ProveedorUnico(mapa.TryAdd(rtn, lector.GetInt32(0)));
        }
        return mapa;
    }


    public static int ImportarProductos(IReadOnlyList<ProductoImportado> productos, int? tienda = null, string? archivo = null, byte[]? hash = null)
    {
        if (productos.Count == 0) return 0;
        ImportarProductosValidacion.Validar(productos, tienda);
        const string sql = """
            IF NOT EXISTS(SELECT 1 FROM dbo.Categoria WITH(HOLDLOCK) WHERE id_categoria=@idCategoria AND activo=1)
                OR NOT EXISTS(SELECT 1 FROM dbo.Proveedor WITH(HOLDLOCK) WHERE id_proveedor=@idProveedor AND activo=1)
                THROW 50001,'La categoría o el proveedor ya no están activos. Vuelve a cargar el archivo.',1;
            INSERT dbo.Producto(codigo_barras,nombre,precio_unitario,id_categoria,id_proveedor,creado_por)
            VALUES(@codigo,@nombre,@precio,@idCategoria,@idProveedor,@usuario);
            SELECT CONVERT(int,SCOPE_IDENTITY());
            """;

        using SqlConnection conexion = Conexion.AbrirConexion();
        using SqlTransaction transaccion = conexion.BeginTransaction(IsolationLevel.Serializable);
        try
        {
            int usuario = AutorizacionDatos.Exigir(conexion, Modulos.cargaMasiva, gestionar: true, transaccion: transaccion);
            FlujoVentaDatos.Ejecutar(conexion, transaccion, """
                EXEC sys.sp_set_session_context @key=N'id_usuario',@value=@usuario;
                EXEC sys.sp_set_session_context @key=N'motivo',@value=N'Importación de productos desde Excel';
                IF @tienda>0 AND NOT EXISTS(SELECT 1 FROM dbo.Tienda WITH(HOLDLOCK) WHERE id_tienda=@tienda AND activo=1)
                    THROW 50001,'Selecciona una tienda activa para el stock inicial.',1;
                IF @conStock=1 AND EXISTS(SELECT 1 FROM (VALUES('trg_MovimientoInventario_aplicar'),('trg_MovimientoInventario_validar'),
                    ('trg_MovimientoInventario_documento'),('trg_MovimientoInventario_inmutable')) r(nombre)
                    WHERE NOT EXISTS(SELECT 1 FROM sys.triggers WHERE object_id=OBJECT_ID('dbo.'+r.nombre) AND is_disabled=0))
                    THROW 50001,'Instala Triggers2.sql antes de importar stock inicial.',1;
                """, ("@usuario", usuario), ("@tienda", tienda ?? 0), ("@conStock", productos.Any(p => p.StockInicial > 0)));
            int? importacion = archivo != null && hash is { Length: 32 } ? FlujoVentaDatos.Id(conexion, transaccion, """
                INSERT dbo.Importacion(nombre_archivo,archivo_hash,id_usuario,estado) VALUES(@archivo,@hash,@usuario,'VALIDANDO');
                SELECT CONVERT(int,SCOPE_IDENTITY());
                """, ("@archivo", Path.GetFileName(archivo)), ("@hash", hash), ("@usuario", usuario)) : null;
            using var comando = new SqlCommand(sql, conexion, transaccion);
            comando.Parameters.Add("@usuario", SqlDbType.Int).Value = usuario;
            var codigo = comando.Parameters.Add("@codigo", SqlDbType.NVarChar, 50);
            var nombre = comando.Parameters.Add("@nombre", SqlDbType.NVarChar, 150);
            var precio = comando.Parameters.Add("@precio", SqlDbType.Decimal);
            precio.Precision = 10;
            precio.Scale = 2;
            var cat = comando.Parameters.Add("@idCategoria", SqlDbType.Int);
            var prov = comando.Parameters.Add("@idProveedor", SqlDbType.Int);

            int fila = 1;
            foreach (ProductoImportado p in productos)
            {
                codigo.Value = p.CodigoBarras;
                nombre.Value = p.Nombre;
                precio.Value = p.Precio;
                cat.Value = p.IdCategoria;
                prov.Value = p.IdProveedor;
                int producto = Convert.ToInt32(comando.ExecuteScalar());
                if (p.StockInicial > 0) FlujoVentaDatos.Ejecutar(conexion, transaccion, """
                    INSERT dbo.MovimientoInventario(id_tienda,id_producto,tipo,cantidad_delta,id_usuario,motivo)
                    VALUES(@tienda,@producto,'AJUSTE',@cantidad,@usuario,N'Stock inicial de carga masiva');
                    """, ("@tienda", tienda!.Value), ("@producto", producto), ("@cantidad", p.StockInicial), ("@usuario", usuario));
                if (importacion.HasValue) FlujoVentaDatos.Ejecutar(conexion, transaccion, """
                    INSERT dbo.DetalleImportacion(id_importacion,numero_fila,datos_originales_json,estado,id_producto)
                    VALUES(@id,@fila,@json,'IMPORTADA',@producto);
                    """, ("@id", importacion.Value), ("@fila", p.FilaExcel > 1 ? p.FilaExcel : ++fila), ("@json", System.Text.Json.JsonSerializer.Serialize(p)), ("@producto", producto));
            }
            if (importacion.HasValue) FlujoVentaDatos.Ejecutar(conexion, transaccion, "UPDATE dbo.Importacion SET estado='COMPLETADA' WHERE id_importacion=@id", ("@id", importacion.Value));
            transaccion.Commit();
            return productos.Count;
        }
        catch (SqlException ex) when (ex.Number is uniqueIndex or uniqueConstraint)
        {
            throw new InvalidOperationException(
                "Se encontró un código de barras duplicado durante la importación. No se guardó ningún producto.", ex);
        }
    }
}
