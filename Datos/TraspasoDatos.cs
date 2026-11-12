using System.Data;
using Microsoft.Data.SqlClient;
using Sistema_ERP.Clases;

namespace Sistema_ERP
{
    public record LineaTraspaso(int Producto, int Cantidad, decimal Precio);

    public static class TraspasoDatos
    {
        private static DataTable Consultar(string sql, int? id = null)
        {
            using var conexion = Conexion.AbrirConexion();
            AutorizacionDatos.ExigirAlguno(conexion, Modulos.Traspasos, Modulos.Validacion);
            using var adaptador = new SqlDataAdapter(sql, conexion);
            if (id.HasValue)
                adaptador.SelectCommand.Parameters.Add("@id", SqlDbType.Int).Value = id.Value;
            var tabla = new DataTable();
            adaptador.Fill(tabla);
            return tabla;
        }

        public static DataTable Usuarios()
        {
            const string sql = @"SELECT id_usuario, nombre_usuario FROM dbo.Usuario
                                 WHERE activo = 1 ORDER BY nombre_usuario";
            return Consultar(sql);
        }

        public static DataTable Listar()
        {
            const string sql = @"SELECT t.id_traspaso, o.nombre AS Origen, d.nombre AS Destino,
                                        t.estado, u.nombre_usuario AS Solicitante, t.solicitado_en,
                                        t.observaciones, t.id_tienda_origen, t.id_tienda_destino, t.version_fila
                                 FROM dbo.Traspaso t
                                 JOIN dbo.Tienda o ON o.id_tienda = t.id_tienda_origen
                                 JOIN dbo.Tienda d ON d.id_tienda = t.id_tienda_destino
                                 JOIN dbo.Usuario u ON u.id_usuario = t.solicitado_por
                                 ORDER BY t.id_traspaso DESC";
            return Consultar(sql);
        }

        public static DataTable Detalle(int id)
        {
            const string sql = @"SELECT d.id_producto, p.nombre AS Producto, d.cantidad, d.precio_referencia,
                                        ISNULL(i.existencia, 0) AS existencia,
                                        ISNULL(i.reservado, 0) AS reservado,
                                        ISNULL(i.existencia - i.reservado, 0) AS disponible
                                 FROM dbo.DetalleTraspaso d
                                 JOIN dbo.Producto p ON p.id_producto = d.id_producto
                                 JOIN dbo.Traspaso t ON t.id_traspaso = d.id_traspaso
                                 LEFT JOIN dbo.InventarioTienda i ON i.id_producto = d.id_producto
                                     AND i.id_tienda = t.id_tienda_origen
                                 WHERE d.id_traspaso = @id";
            return Consultar(sql, id);
        }

        public static DataTable Productos(int idTienda, bool incluirInactivos = false)
        {
            const string sql = @"SELECT p.id_producto, p.nombre, p.precio_unitario,
                                        ISNULL(i.existencia, 0) AS existencia,
                                        ISNULL(i.reservado, 0) AS reservado,
                                        ISNULL(i.existencia - i.reservado, 0) AS disponible
                                 FROM dbo.Producto p
                                 LEFT JOIN dbo.InventarioTienda i ON i.id_producto = p.id_producto
                                     AND i.id_tienda = @id
                                 WHERE p.activo = 1 OR @incluirInactivos = 1
                                 ORDER BY p.nombre";
            using var conexion = Conexion.AbrirConexion();
            AutorizacionDatos.ExigirAlguno(conexion, Modulos.Traspasos, Modulos.Validacion);
            using var adaptador = new SqlDataAdapter(sql, conexion);
            adaptador.SelectCommand.Parameters.Add("@id", SqlDbType.Int).Value = idTienda;
            adaptador.SelectCommand.Parameters.Add("@incluirInactivos", SqlDbType.Bit).Value = incluirInactivos;
            var tabla = new DataTable();
            adaptador.Fill(tabla);
            return tabla;
        }

        public static DataTable Historial(int id)
        {
            const string sql = @"SELECT e.fecha, e.estado_anterior, e.estado_nuevo,
                                        u.nombre_usuario AS Usuario, e.motivo
                                 FROM dbo.EventoTraspaso e
                                 JOIN dbo.Usuario u ON u.id_usuario = e.id_usuario
                                 WHERE e.id_traspaso = @id ORDER BY e.id_evento_traspaso";
            return Consultar(sql, id);
        }

        internal static int Guardar(int? id, byte[]? version, int origen, int destino, int usuario,
            string observaciones, IReadOnlyCollection<LineaTraspaso> lineas)
        {
            if (origen <= 0 || destino <= 0 || origen == destino)
                throw new ArgumentException("Seleccione dos tiendas distintas.");
            if (observaciones.Length > 500)
                throw new ArgumentException("Las observaciones admiten hasta 500 caracteres.");
            ValidacionDatos.NoSoloCeros(observaciones, "Observaciones");
            if (lineas.Count == 0 || lineas.Any(l => l.Producto <= 0 || l.Cantidad <= 0 || l.Precio < 0 || l.Precio > 9999999999999999.99m)
                || lineas.Select(l => l.Producto).Distinct().Count() != lineas.Count)
                throw new ArgumentException("Agregue productos sin repetir, con cantidades enteras positivas y precios válidos.");
            if (SesionActual.Exigir().Id != usuario) throw new InvalidOperationException("El operador debe ser el usuario autenticado.");
            using var conexion = Conexion.AbrirConexion();
            using var transaccion = conexion.BeginTransaction(IsolationLevel.Serializable);
            AutorizacionDatos.Exigir(conexion, Modulos.Traspasos, gestionar: true, transaccion: transaccion);
            SqlCommand Comando(string sql) => new(sql, conexion, transaccion);
            using (var validar = Comando(@"SELECT COUNT(*) FROM dbo.Tienda WITH (HOLDLOCK) WHERE activo=1 AND id_tienda IN (@origen,@destino)"))
            {
                validar.Parameters.AddWithValue("@origen", origen);
                validar.Parameters.AddWithValue("@destino", destino);
                if ((int)validar.ExecuteScalar()! != 2)
                    throw new InvalidOperationException("Las tiendas deben estar activas.");
            }
            PrepararOperacion(conexion, transaccion, usuario, "Solicitud creada");
            int numero;
            using (var guardar = Comando(id.HasValue
                ? @"UPDATE dbo.Traspaso SET observaciones = @texto
                WHERE id_traspaso=@id AND estado='PENDIENTE' AND version_fila=@version
                    AND id_tienda_origen=@origen AND id_tienda_destino=@destino;
                IF @@ROWCOUNT=0 THROW 50001, 'La solicitud cambió o ya no está pendiente. Actualice la lista.', 1;
                SELECT @id;"
                : @"INSERT dbo.Traspaso(id_tienda_origen,id_tienda_destino,solicitado_por,observaciones)
                VALUES(@origen,@destino,@usuario,@texto); SELECT CAST(SCOPE_IDENTITY() AS int);"))
            {
                guardar.Parameters.AddWithValue("@origen", origen);
                guardar.Parameters.AddWithValue("@destino", destino);
                guardar.Parameters.AddWithValue("@usuario", usuario);
                guardar.Parameters.Add("@texto", SqlDbType.NVarChar, 500).Value = observaciones;
                guardar.Parameters.AddWithValue("@id", (object?)id ?? DBNull.Value);
                guardar.Parameters.Add("@version", SqlDbType.Timestamp).Value = (object?)version ?? DBNull.Value;
                numero = (int)guardar.ExecuteScalar()!;
            }
            using (var borrar = Comando("DELETE dbo.DetalleTraspaso WHERE id_traspaso=@id"))
            {
                borrar.Parameters.AddWithValue("@id", numero);
                borrar.ExecuteNonQuery();
            }
            foreach (var linea in lineas)
            {
                using var detalle = Comando(@"IF NOT EXISTS(SELECT 1 FROM dbo.Producto WITH(HOLDLOCK) WHERE id_producto=@producto AND activo=1)
                THROW 50002, 'El producto ya no está activo.', 1;
                INSERT dbo.DetalleTraspaso(id_traspaso,id_producto,cantidad,precio_referencia) VALUES(@id,@producto,@cantidad,@precio)");
                detalle.Parameters.AddWithValue("@id", numero);
                detalle.Parameters.AddWithValue("@producto", linea.Producto);
                detalle.Parameters.AddWithValue("@cantidad", linea.Cantidad);
                var precio = detalle.Parameters.Add("@precio", SqlDbType.Decimal);
                precio.Precision = 18;
                precio.Scale = 2;
                precio.Value = linea.Precio;
                detalle.ExecuteNonQuery();
            }
            transaccion.Commit();
            return numero;
        }

        internal static void ValidarUsuario(SqlConnection conexion, SqlTransaction transaccion, int usuario)
        {
            using var comando = new SqlCommand("SELECT COUNT(*) FROM dbo.Usuario WITH(HOLDLOCK) WHERE id_usuario=@usuario AND activo=1", conexion, transaccion);
            comando.Parameters.AddWithValue("@usuario", usuario);
            if ((int)comando.ExecuteScalar()! != 1)
                throw new ArgumentException("Seleccione un usuario activo.");
        }

        internal static void PrepararOperacion(SqlConnection conexion, SqlTransaction transaccion, int usuario, string motivo)
        {
            if (SesionActual.Validar(conexion, transaccion) != usuario)
                throw new InvalidOperationException("El operador debe ser el usuario autenticado.");
            ValidarUsuario(conexion, transaccion, usuario);
            const string sql = @"IF NOT EXISTS (SELECT 1 FROM sys.triggers
                                     WHERE object_id = OBJECT_ID(N'dbo.trg_Traspaso_operar') AND is_disabled = 0)
                                     THROW 50004, 'Instale Triggers2.sql antes de operar traspasos.', 1;
                                 IF EXISTS(SELECT requerido.nombre FROM (VALUES
                                     (N'trg_Traspaso_estado'),(N'trg_MovimientoInventario_aplicar'),
                                     (N'trg_MovimientoInventario_validar'),(N'trg_MovimientoInventario_documento')) requerido(nombre)
                                     WHERE NOT EXISTS(SELECT 1 FROM sys.triggers t WHERE t.object_id=OBJECT_ID(N'dbo.'+requerido.nombre) AND t.is_disabled=0))
                                     THROW 50004, 'Instale todos los triggers de inventario y traspasos de Triggers2.sql.', 1;
                                 EXEC sys.sp_set_session_context @key=N'id_usuario', @value=@usuario;
                                 EXEC sys.sp_set_session_context @key=N'motivo', @value=@motivo;";
            using var comando = new SqlCommand(sql, conexion, transaccion);
            comando.Parameters.Add("@usuario", SqlDbType.Int).Value = usuario;
            comando.Parameters.Add("@motivo", SqlDbType.NVarChar, 500).Value = motivo;
            comando.ExecuteNonQuery();
        }
    }

}
