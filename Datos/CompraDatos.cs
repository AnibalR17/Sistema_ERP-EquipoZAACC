using System.Data;
using Microsoft.Data.SqlClient;
using Sistema_ERP.Clases;

namespace Sistema_ERP
{
    public record LineaCompra(int Producto, int Cantidad, decimal Costo);

    public static class CompraDatos
    {
        private const decimal MaximoImporte = 9999999999999999.99m;

        private const string VersionSql = """
            HASHBYTES('SHA2_256',(
                SELECT c.id_compra,c.id_proveedor,c.id_tienda,c.numero_factura,c.fecha_factura,
                       c.registrado_en,c.id_usuario,c.estado,c.observaciones,
                       c.anulado_en,c.anulado_por,c.motivo_anulacion,
                       (SELECT dc.id_detalle_compra,dc.id_producto,dc.cantidad,dc.costo_unitario
                        FROM dbo.DetalleCompra dc WHERE dc.id_compra=c.id_compra
                        ORDER BY dc.id_detalle_compra FOR JSON PATH) AS detalle
                FOR JSON PATH,WITHOUT_ARRAY_WRAPPER,INCLUDE_NULL_VALUES))
            """;

        private static readonly string CabeceraSql = $"""
            SELECT c.id_compra,p.nombre_empresa AS Proveedor,t.nombre AS Tienda,
                   c.numero_factura,c.fecha_factura,c.registrado_en,u.nombre_usuario AS Usuario,
                   c.estado,c.observaciones,c.motivo_anulacion,c.id_proveedor,c.id_tienda,
                   total.total AS Total,{VersionSql} AS version_fila
            FROM dbo.Compra c
            JOIN dbo.Proveedor p ON p.id_proveedor=c.id_proveedor
            JOIN dbo.Tienda t ON t.id_tienda=c.id_tienda
            JOIN dbo.Usuario u ON u.id_usuario=c.id_usuario
            JOIN dbo.vw_CompraTotal total ON total.id_compra=c.id_compra
            """;

        private const string DetalleSql = """
            SELECT d.id_detalle_compra,d.id_producto,p.codigo_barras AS Codigo,
                   p.nombre AS Producto,d.cantidad,d.costo_unitario,d.subtotal,p.activo AS Activo
            FROM dbo.DetalleCompra d JOIN dbo.Producto p ON p.id_producto=d.id_producto
            WHERE d.id_compra=@id ORDER BY d.id_detalle_compra
            """;

        public static DataTable Proveedores() => Consultar("""
            SELECT id_proveedor,nombre_empresa AS nombre FROM dbo.Proveedor
            WHERE activo=1 ORDER BY nombre_empresa
            """);

        public static DataTable Tiendas() => Consultar("""
            SELECT id_tienda,nombre FROM dbo.Tienda WHERE activo=1 ORDER BY nombre
            """);

        public static DataTable Productos() => Consultar("""
            SELECT id_producto,codigo_barras,nombre,precio_unitario FROM dbo.Producto
            WHERE activo=1 ORDER BY nombre
            """);

        public static DataTable Listar(string buscar = "", string estado = "", DateTime? desde = null, DateTime? hasta = null)
        {
            estado = estado.Trim().ToUpperInvariant();
            if (estado.Length > 0 && estado is not ("BORRADOR" or "CONFIRMADA" or "ANULADA"))
                throw new ArgumentException("Selecciona un estado de compra válido.");
            if (desde.HasValue && hasta.HasValue && desde.Value.Date > hasta.Value.Date)
                throw new ArgumentException("La fecha inicial no puede ser posterior a la final.");
            using var conexion = Conexion.AbrirConexion();
            AutorizacionDatos.Exigir(conexion, Modulos.Compras);
            using var comando = new SqlCommand(CabeceraSql + """

                WHERE (@estado='' OR c.estado=@estado)
                AND (@desde IS NULL OR c.fecha_factura>=@desde)
                AND (@hasta IS NULL OR c.fecha_factura<=@hasta)
                AND (@buscar=N'' OR CHARINDEX(@buscar,c.numero_factura)>0
                    OR CHARINDEX(@buscar,p.nombre_empresa)>0 OR CHARINDEX(@buscar,t.nombre)>0
                    OR CHARINDEX(@buscar,ISNULL(c.observaciones,N''))>0
                    OR CHARINDEX(@buscar,CONVERT(nvarchar(20),c.id_compra))>0)
                ORDER BY c.id_compra DESC
                """, conexion);
            comando.Parameters.Add("@estado", SqlDbType.VarChar, 20).Value = estado;
            comando.Parameters.Add("@buscar", SqlDbType.NVarChar, 150).Value = buscar.Trim();
            comando.Parameters.Add("@desde", SqlDbType.Date).Value = (object?)desde?.Date ?? DBNull.Value;
            comando.Parameters.Add("@hasta", SqlDbType.Date).Value = (object?)hasta?.Date ?? DBNull.Value;
            return Llenar(comando);
        }

        public static DataSet Obtener(int id)
        {
            if (id <= 0) throw new ArgumentException("Selecciona una compra.");
            using var conexion = Conexion.AbrirConexion();
            using var transaccion = conexion.BeginTransaction(IsolationLevel.Serializable);
            AutorizacionDatos.Exigir(conexion, Modulos.Compras, transaccion: transaccion);
            using var cabecera = new SqlCommand(CabeceraSql + " WHERE c.id_compra=@id", conexion, transaccion);
            cabecera.Parameters.Add("@id", SqlDbType.Int).Value = id;
            var datos = new DataSet();
            DataTable tabla = Llenar(cabecera);
            tabla.TableName = "Compra";
            if (tabla.Rows.Count != 1) throw new InvalidOperationException("La compra ya no existe. Recarga el listado.");
            datos.Tables.Add(tabla);
            using var detalle = new SqlCommand(DetalleSql, conexion, transaccion);
            detalle.Parameters.Add("@id", SqlDbType.Int).Value = id;
            tabla = Llenar(detalle);
            tabla.TableName = "Detalle";
            datos.Tables.Add(tabla);
            transaccion.Commit();
            return datos;
        }

        public static DataTable Detalle(int id)
        {
            using var datos = Obtener(id);
            return datos.Tables[1].Copy();
        }

        public static int Guardar(int? id, byte[]? version, int proveedor, int tienda, string factura,
            DateTime fecha, string observaciones, IReadOnlyCollection<LineaCompra> lineas)
        {
            ValidarCompra(proveedor, tienda, factura, observaciones, lineas);
            if (id.HasValue && (id.Value <= 0 || version is not { Length: 32 }))
                throw new ArgumentException("Recarga la compra antes de actualizarla.");
            using var conexion = Conexion.AbrirConexion();
            using var transaccion = conexion.BeginTransaction(IsolationLevel.Serializable);
            int usuario = Preparar(conexion, transaccion, "Guardar borrador de compra");
            if (id.HasValue)
            {
                if (ComprobarVersion(conexion, transaccion, id.Value, version!) != "BORRADOR")
                    throw new InvalidOperationException("Solo se pueden editar compras en BORRADOR.");
            }
            ValidarEntidades(conexion, transaccion, proveedor, tienda, lineas.Select(l => l.Producto));
            int numero;
            try
            {
                using var guardar = new SqlCommand(id.HasValue ? """
                    UPDATE dbo.Compra SET numero_factura=@factura,fecha_factura=@fecha,observaciones=@texto
                    WHERE id_compra=@id AND estado='BORRADOR' AND id_proveedor=@proveedor AND id_tienda=@tienda;
                    IF @@ROWCOUNT=0 THROW 50001, 'No se permite cambiar proveedor ni tienda de una compra existente.', 1;
                    SELECT @id;
                    """ : """
                    INSERT dbo.Compra(id_proveedor,id_tienda,numero_factura,fecha_factura,id_usuario,observaciones)
                    VALUES(@proveedor,@tienda,@factura,@fecha,@usuario,@texto);
                    SELECT CAST(SCOPE_IDENTITY() AS int);
                    """, conexion, transaccion);
                guardar.Parameters.Add("@id", SqlDbType.Int).Value = (object?)id ?? DBNull.Value;
                guardar.Parameters.Add("@proveedor", SqlDbType.Int).Value = proveedor;
                guardar.Parameters.Add("@tienda", SqlDbType.Int).Value = tienda;
                guardar.Parameters.Add("@factura", SqlDbType.NVarChar, 50).Value = factura.Trim();
                guardar.Parameters.Add("@fecha", SqlDbType.Date).Value = fecha.Date;
                guardar.Parameters.Add("@usuario", SqlDbType.Int).Value = usuario;
                guardar.Parameters.Add("@texto", SqlDbType.NVarChar, 500).Value = TextoOpcional(observaciones);
                numero = Convert.ToInt32(guardar.ExecuteScalar());
                using (var borrar = new SqlCommand("DELETE dbo.DetalleCompra WHERE id_compra=@id", conexion, transaccion))
                {
                    borrar.Parameters.Add("@id", SqlDbType.Int).Value = numero;
                    borrar.ExecuteNonQuery();
                }
                foreach (var linea in lineas)
                {
                    using var detalle = new SqlCommand("""
                        INSERT dbo.DetalleCompra(id_compra,id_producto,cantidad,costo_unitario)
                        VALUES(@id,@producto,@cantidad,@costo)
                        """, conexion, transaccion);
                    detalle.Parameters.Add("@id", SqlDbType.Int).Value = numero;
                    detalle.Parameters.Add("@producto", SqlDbType.Int).Value = linea.Producto;
                    detalle.Parameters.Add("@cantidad", SqlDbType.Int).Value = linea.Cantidad;
                    var costo = detalle.Parameters.Add("@costo", SqlDbType.Decimal);
                    costo.Precision = 18;
                    costo.Scale = 2;
                    costo.Value = linea.Costo;
                    detalle.ExecuteNonQuery();
                }
                transaccion.Commit();
            }
            catch (SqlException ex) when (ex.Number is 2601 or 2627)
            {
                throw new InvalidOperationException("Ya existe una compra con ese proveedor y número de factura, incluso si está anulada.", ex);
            }
            return numero;
        }

        public static void Confirmar(int id, byte[] version)
        {
            using var conexion = Conexion.AbrirConexion();
            using var transaccion = conexion.BeginTransaction(IsolationLevel.Serializable);
            Preparar(conexion, transaccion, "Confirmar compra");
            if (ComprobarVersion(conexion, transaccion, id, version) != "BORRADOR")
                throw new InvalidOperationException("Solo se puede confirmar una compra en BORRADOR.");
            using (var validar = new SqlCommand("""
                IF NOT EXISTS(SELECT 1 FROM dbo.DetalleCompra WHERE id_compra=@id)
                    THROW 50001, 'Agrega al menos un producto antes de confirmar.', 1;
                IF EXISTS(SELECT 1 FROM dbo.Compra c JOIN dbo.Proveedor p WITH(HOLDLOCK) ON p.id_proveedor=c.id_proveedor
                    JOIN dbo.Tienda t WITH(HOLDLOCK) ON t.id_tienda=c.id_tienda
                    WHERE c.id_compra=@id AND (p.activo=0 OR t.activo=0))
                    THROW 50001, 'El proveedor y la tienda deben estar activos para confirmar.', 1;
                IF EXISTS(SELECT 1 FROM dbo.DetalleCompra d JOIN dbo.Producto p WITH(HOLDLOCK) ON p.id_producto=d.id_producto
                    WHERE d.id_compra=@id AND p.activo=0)
                    THROW 50001, 'La compra contiene productos inactivos. Actualiza su detalle antes de confirmar.', 1;
                """, conexion, transaccion))
            {
                validar.Parameters.Add("@id", SqlDbType.Int).Value = id;
                validar.ExecuteNonQuery();
            }
            using var confirmar = new SqlCommand("UPDATE dbo.Compra SET estado='CONFIRMADA' WHERE id_compra=@id", conexion, transaccion);
            confirmar.Parameters.Add("@id", SqlDbType.Int).Value = id;
            confirmar.ExecuteNonQuery();
            transaccion.Commit();
        }

        public static void Anular(int id, byte[] version, string motivo)
        {
            if (string.IsNullOrWhiteSpace(motivo) || motivo.Trim().Length > 500)
                throw new ArgumentException("Introduce un motivo de anulación de hasta 500 caracteres.");
            using var conexion = Conexion.AbrirConexion();
            using var transaccion = conexion.BeginTransaction(IsolationLevel.Serializable);
            int usuario = Preparar(conexion, transaccion, motivo.Trim());
            string estado = ComprobarVersion(conexion, transaccion, id, version);
            if (estado is not ("BORRADOR" or "CONFIRMADA"))
                throw new InvalidOperationException("La compra ya está anulada.");
            using var anular = new SqlCommand("""
                UPDATE dbo.Compra SET estado='ANULADA',anulado_en=SYSUTCDATETIME(),
                    anulado_por=@usuario,motivo_anulacion=@motivo WHERE id_compra=@id
                """, conexion, transaccion);
            anular.Parameters.Add("@id", SqlDbType.Int).Value = id;
            anular.Parameters.Add("@usuario", SqlDbType.Int).Value = usuario;
            anular.Parameters.Add("@motivo", SqlDbType.NVarChar, 500).Value = motivo.Trim();
            anular.ExecuteNonQuery();
            transaccion.Commit();
        }

        private static void ValidarCompra(int proveedor, int tienda, string factura, string observaciones,
            IReadOnlyCollection<LineaCompra> lineas)
        {
            if (proveedor <= 0 || tienda <= 0) throw new ArgumentException("Selecciona proveedor y tienda.");
            if (string.IsNullOrWhiteSpace(factura) || factura.Trim().Length > 50)
                throw new ArgumentException("Introduce un número de factura de hasta 50 caracteres.");
            if (observaciones.Length > 500) throw new ArgumentException("Las observaciones admiten hasta 500 caracteres.");
            if (lineas.Count == 0 || lineas.Any(l => l.Producto <= 0 || l.Cantidad <= 0 || l.Costo <= 0
                || l.Costo > MaximoImporte || decimal.Round(l.Costo, 2) != l.Costo)
                || lineas.Select(l => l.Producto).Distinct().Count() != lineas.Count)
                throw new ArgumentException("Agrega productos sin repetir, cantidades enteras positivas y costos positivos con hasta dos decimales.");
            decimal total = 0;
            foreach (var linea in lineas)
            {
                decimal subtotal = linea.Cantidad * linea.Costo;
                if (subtotal > MaximoImporte || total > MaximoImporte - subtotal)
                    throw new ArgumentException("El subtotal o total supera el importe permitido (decimal 18,2).");
                total += subtotal;
            }
        }

        private static void ValidarEntidades(SqlConnection conexion, SqlTransaction transaccion,
            int proveedor, int tienda, IEnumerable<int> productos)
        {
            using (var validar = new SqlCommand("""
                IF NOT EXISTS(SELECT 1 FROM dbo.Proveedor WITH(HOLDLOCK) WHERE id_proveedor=@proveedor AND activo=1)
                    OR NOT EXISTS(SELECT 1 FROM dbo.Tienda WITH(HOLDLOCK) WHERE id_tienda=@tienda AND activo=1)
                    THROW 50001, 'El proveedor y la tienda deben estar activos.', 1;
                """, conexion, transaccion))
            {
                validar.Parameters.Add("@proveedor", SqlDbType.Int).Value = proveedor;
                validar.Parameters.Add("@tienda", SqlDbType.Int).Value = tienda;
                validar.ExecuteNonQuery();
            }
            foreach (int producto in productos)
            {
                using var validar = new SqlCommand("""
                    IF NOT EXISTS(SELECT 1 FROM dbo.Producto WITH(HOLDLOCK) WHERE id_producto=@producto AND activo=1)
                        THROW 50001, 'Un producto ya no está activo. Recarga los catálogos y revisa el detalle.', 1;
                    """, conexion, transaccion);
                validar.Parameters.Add("@producto", SqlDbType.Int).Value = producto;
                validar.ExecuteNonQuery();
            }
        }

        private static string ComprobarVersion(SqlConnection conexion, SqlTransaction transaccion, int id, byte[] version)
        {
            if (id <= 0 || version is not { Length: 32 }) throw new ArgumentException("Selecciona una compra y recárgala antes de operar.");
            using (var bloquear = new SqlCommand("""
                SELECT id_compra FROM dbo.Compra WITH(UPDLOCK,HOLDLOCK) WHERE id_compra=@id;
                SELECT id_detalle_compra FROM dbo.DetalleCompra WITH(UPDLOCK,HOLDLOCK) WHERE id_compra=@id;
                """, conexion, transaccion))
            {
                bloquear.Parameters.Add("@id", SqlDbType.Int).Value = id;
                bloquear.ExecuteNonQuery();
            }
            using var comando = new SqlCommand($"SELECT c.estado,{VersionSql} AS version_fila FROM dbo.Compra c WHERE c.id_compra=@id", conexion, transaccion);
            comando.Parameters.Add("@id", SqlDbType.Int).Value = id;
            using var lector = comando.ExecuteReader();
            if (!lector.Read() || !((byte[])lector["version_fila"]).SequenceEqual(version))
                throw new InvalidOperationException("La compra cambió en otra ventana. Recárgala antes de guardar o cambiar su estado.");
            return lector.GetString(0);
        }

        private static int Preparar(SqlConnection conexion, SqlTransaction transaccion, string motivo)
        {
            int usuario = AutorizacionDatos.Exigir(conexion, Modulos.Compras, gestionar: true, transaccion: transaccion);
            using var comando = new SqlCommand("""
                IF (SELECT value_in_use FROM sys.configurations WHERE name='nested triggers')<>1
                    OR DATABASEPROPERTYEX(DB_NAME(),'IsRecursiveTriggersEnabled')=1
                    THROW 50004, 'Compras requiere nested triggers habilitado y RECURSIVE_TRIGGERS OFF, como indica Triggers2.sql.', 1;
                IF EXISTS(SELECT requerido.nombre FROM (VALUES
                    (N'trg_Compra_alta_baja'),(N'trg_Compra_cabecera'),(N'trg_Compra_estado'),
                    (N'trg_DetalleCompra_bloqueo'),(N'trg_Compra_contabilizar'),(N'trg_Compra_auditoria'),
                    (N'trg_MovimientoInventario_validar'),(N'trg_MovimientoInventario_documento'),
                    (N'trg_MovimientoInventario_aplicar'),(N'trg_MovimientoInventario_inmutable')) requerido(nombre)
                    WHERE NOT EXISTS(SELECT 1 FROM sys.triggers t
                        WHERE t.object_id=OBJECT_ID(N'dbo.'+requerido.nombre) AND t.is_disabled=0))
                    THROW 50004, 'Instale todos los triggers de compras e inventario de Triggers2.sql antes de operar.', 1;
                EXEC sys.sp_set_session_context @key=N'id_usuario',@value=@usuario;
                EXEC sys.sp_set_session_context @key=N'motivo',@value=@motivo;
                """, conexion, transaccion);
            comando.Parameters.Add("@usuario", SqlDbType.Int).Value = usuario;
            comando.Parameters.Add("@motivo", SqlDbType.NVarChar, 500).Value = motivo;
            comando.ExecuteNonQuery();
            return usuario;
        }

        private static object TextoOpcional(string texto) => string.IsNullOrWhiteSpace(texto) ? DBNull.Value : texto.Trim();

        private static DataTable Consultar(string sql)
        {
            using var conexion = Conexion.AbrirConexion();
            AutorizacionDatos.Exigir(conexion, Modulos.Compras);
            using var comando = new SqlCommand(sql, conexion);
            return Llenar(comando);
        }

        private static DataTable Llenar(SqlCommand comando)
        {
            using var adaptador = new SqlDataAdapter(comando);
            var tabla = new DataTable();
            adaptador.Fill(tabla);
            return tabla;
        }
    }
}
