using Microsoft.Data.SqlClient;
using Sistema_ERP.Clases;
using System.Data;

namespace Sistema_ERP;


public record ProductoImportado(
    string CodigoBarras, string Nombre, decimal Precio, int StockInicial, int IdCategoria, int IdProveedor);


public static class CargaMasivaDatos
{
    private const int UniqueIndex = 2601;
    private const int UniqueConstraint = 2627;


    public static string NormalizarRtn(string? rtn) =>
        (rtn ?? "").Replace("-", "").Replace(" ", "").Trim();

    public static HashSet<string> CodigosExistentes()
    {
        var codigos = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        using SqlConnection conexion = ConexionDB.AbrirConexion();
        using var comando = new SqlCommand("SELECT codigo_barras FROM Producto", conexion);
        using SqlDataReader lector = comando.ExecuteReader();
        while (lector.Read()) codigos.Add(lector.GetString(0).Trim());
        return codigos;
    }


    public static Dictionary<string, int> MapaCategorias()
    {
        var mapa = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        using SqlConnection conexion = ConexionDB.AbrirConexion();
        using var comando = new SqlCommand("SELECT id_categoria, nombre FROM Categoria", conexion);
        using SqlDataReader lector = comando.ExecuteReader();
        while (lector.Read()) mapa[lector.GetString(1).Trim()] = lector.GetInt32(0);
        return mapa;
    }


    public static Dictionary<string, int> MapaProveedoresPorRtn()
    {
        var mapa = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        using SqlConnection conexion = ConexionDB.AbrirConexion();
        using var comando = new SqlCommand("SELECT id_proveedor, DNI_RTN FROM Proveedor WHERE DNI_RTN IS NOT NULL", conexion);
        using SqlDataReader lector = comando.ExecuteReader();
        while (lector.Read()) mapa[NormalizarRtn(lector.GetString(1))] = lector.GetInt32(0);
        return mapa;
    }


    public static int ImportarProductos(IReadOnlyList<ProductoImportado> productos)
    {
        if (productos.Count == 0) return 0;

        const string sql = @"INSERT INTO Producto
                                    (codigo_barras, nombre, precio_unitario, stock_actual, id_categoria, id_proveedor)
                                 VALUES (@codigo, @nombre, @precio, @stock, @idCategoria, @idProveedor)";

        using SqlConnection conexion = ConexionDB.AbrirConexion();
        using SqlTransaction transaccion = conexion.BeginTransaction();
        try
        {
            using var comando = new SqlCommand(sql, conexion, transaccion);
            var codigo = comando.Parameters.Add("@codigo", SqlDbType.NVarChar, 50);
            var nombre = comando.Parameters.Add("@nombre", SqlDbType.NVarChar, 150);
            var precio = comando.Parameters.Add("@precio", SqlDbType.Decimal);
            precio.Precision = 10;
            precio.Scale = 2;
            var stock = comando.Parameters.Add("@stock", SqlDbType.Int);
            var cat = comando.Parameters.Add("@idCategoria", SqlDbType.Int);
            var prov = comando.Parameters.Add("@idProveedor", SqlDbType.Int);

            foreach (ProductoImportado p in productos)
            {
                codigo.Value = p.CodigoBarras;
                nombre.Value = p.Nombre;
                precio.Value = p.Precio;
                stock.Value = p.StockInicial;
                cat.Value = p.IdCategoria;
                prov.Value = p.IdProveedor;
                comando.ExecuteNonQuery();
            }

            transaccion.Commit();
            return productos.Count;
        }
        catch (SqlException ex) when (ex.Number is UniqueIndex or UniqueConstraint)
        {
            transaccion.Rollback();
            throw new InvalidOperationException(
                "Se encontró un código de barras duplicado durante la importación. No se guardó ningún producto.", ex);
        }
        catch
        {
            transaccion.Rollback();
            throw;
        }
    }
}
