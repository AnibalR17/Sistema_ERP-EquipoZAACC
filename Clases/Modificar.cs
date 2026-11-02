using System.Data;
using Microsoft.Data.SqlClient;

namespace Sistema_ERP.Clases
{
    public static class Modificar
    {
        public static int Traspaso(int id, byte[] version, int origen, int destino, int usuario,
            string observaciones, IReadOnlyCollection<LineaTraspaso> lineas)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id));
            ArgumentNullException.ThrowIfNull(version);
            return TraspasoDatos.Guardar(id, version, origen, destino, usuario, observaciones, lineas);
        }

        public static void ResolverTraspaso(int id, byte[] version, int usuario, string estado, string motivo)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            ArgumentNullException.ThrowIfNull(version);
            if (estado is not ("CANCELADO" or "APROBADO" or "RECHAZADO"))
                throw new ArgumentException("Estado inválido.");
            if (motivo.Length > 500 || (estado != "APROBADO" && string.IsNullOrWhiteSpace(motivo)))
                throw new ArgumentException("Indique un motivo de hasta 500 caracteres.");
            using var conexion = Conexion.AbrirConexion();
            using var transaccion = conexion.BeginTransaction();
            TraspasoDatos.PrepararOperacion(conexion, transaccion, usuario, motivo.Trim());
            using var comando = new SqlCommand(@"UPDATE dbo.Traspaso SET estado=@estado
                WHERE id_traspaso=@id AND estado='PENDIENTE' AND version_fila=@version;
                IF @@ROWCOUNT=0 THROW 50003, 'La solicitud cambió o ya no está pendiente. Actualice la lista.', 1;", conexion, transaccion);
            comando.Parameters.AddWithValue("@id", id);
            comando.Parameters.AddWithValue("@estado", estado);
            comando.Parameters.Add("@version", SqlDbType.Timestamp).Value = version;
            comando.ExecuteNonQuery();
            // El trigger reserva el inventario al aprobar y registra la decisión.
            transaccion.Commit();
        }

        public static bool ReactivarProducto(int idProducto)
            => Reactivar("Producto", "id_producto", idProducto);

        public static bool ReactivarProveedor(int idProveedor)
            => Reactivar("Proveedor", "id_proveedor", idProveedor);

        public static bool ReactivarCategoria(int idCategoria)
            => Reactivar("Categoria", "id_categoria", idCategoria);

        private static bool Reactivar(string tabla, string columnaId, int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id));

            // Los identificadores provienen exclusivamente de los métodos de esta clase.
            string sql = $@"UPDATE dbo.{tabla}
                            SET desactivado_en = NULL, desactivado_por = NULL,
                                motivo_desactivacion = NULL, actualizado_en = SYSUTCDATETIME()
                            WHERE {columnaId} = @id AND activo = 0;
                            SELECT CONVERT(int, @@ROWCOUNT);";

            using SqlConnection conexion = Conexion.AbrirConexion();
            using var comando = new SqlCommand(sql, conexion);
            comando.Parameters.Add("@id", SqlDbType.Int).Value = id;
            return (int)comando.ExecuteScalar() > 0;
        }

        public static bool Proveedor(Proveedor proveedor)
        {
            proveedor.Validar();
            if (proveedor.IdProveedor <= 0)
                throw new ArgumentOutOfRangeException(nameof(proveedor.IdProveedor));
            const string sql = @"UPDATE dbo.Proveedor
                                 SET nombre_empresa = @nombre, rtn = @rtn, contacto = @contacto,
                                     telefono = @telefono, correo = @correo, direccion = @direccion,
                                     actualizado_en = SYSUTCDATETIME()
                                 WHERE id_proveedor = @id AND activo = 1;
                                 SELECT CONVERT(int, @@ROWCOUNT);";
            using SqlConnection conexion = Conexion.AbrirConexion();
            using var comando = new SqlCommand(sql, conexion);
            ProveedorDatos.AgregarParametros(comando, proveedor);
            comando.Parameters.Add("@id", SqlDbType.Int).Value = proveedor.IdProveedor;
            return (int)comando.ExecuteScalar() > 0;
        }

        public static bool Categoria(int idCategoria, string nombre, string? descripcion)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nombre);
            if (idCategoria <= 0)
                throw new ArgumentOutOfRangeException(nameof(idCategoria));
            if (nombre.Trim().Length > 100)
                throw new ArgumentException("El nombre admite hasta 100 caracteres.", nameof(nombre));
            if (descripcion?.Trim().Length > 300)
                throw new ArgumentException("La descripción admite hasta 300 caracteres.", nameof(descripcion));

            const string sql = @"UPDATE dbo.Categoria SET nombre = @nombre, descripcion = @descripcion,
                                 actualizado_en = SYSUTCDATETIME()
                                 WHERE id_categoria = @id AND activo = 1;
                                 SELECT CONVERT(int, @@ROWCOUNT);";

            using SqlConnection conexion = Conexion.AbrirConexion();
            using var comando = new SqlCommand(sql, conexion);
            comando.Parameters.Add("@nombre", SqlDbType.NVarChar, 100).Value = nombre.Trim();
            comando.Parameters.Add("@id", SqlDbType.Int).Value = idCategoria;
            comando.Parameters.Add("@descripcion", SqlDbType.NVarChar, 300).Value =
                string.IsNullOrWhiteSpace(descripcion) ? DBNull.Value : descripcion.Trim();
            return (int)comando.ExecuteScalar() > 0;
        }
    }
}
