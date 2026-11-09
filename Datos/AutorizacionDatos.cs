using System.Data;
using Microsoft.Data.SqlClient;
using Sistema_ERP.Clases;

namespace Sistema_ERP;

internal static class AutorizacionDatos
{
    internal static PermisosUsuario Obtener()
    {
        using var conexion = Conexion.AbrirConexion();
        return Obtener(conexion);
    }

    internal static PermisosUsuario Obtener(SqlConnection conexion, SqlTransaction? transaccion = null)
    {
        int usuario = SesionActual.Validar(conexion, transaccion);
        using var comando = new SqlCommand("""
            SELECT r.codigo,p.codigo
            FROM dbo.UsuarioRol ur WITH(HOLDLOCK)
            JOIN dbo.Rol r WITH(HOLDLOCK) ON r.id_rol=ur.id_rol AND r.activo=1
            LEFT JOIN dbo.RolPermiso rp WITH(HOLDLOCK) ON rp.id_rol=r.id_rol AND rp.activo=1
            LEFT JOIN dbo.Permiso p WITH(HOLDLOCK) ON p.id_permiso=rp.id_permiso AND p.activo=1
            WHERE ur.id_usuario=@usuario AND ur.activo=1
            """, conexion, transaccion);
        comando.Parameters.Add("@usuario", SqlDbType.Int).Value = usuario;
        using var lector = comando.ExecuteReader();
        bool administrador = false;
        var permisos = new List<string>();
        while (lector.Read())
        {
            administrador |= string.Equals(lector.GetString(0), "ADMINISTRADOR", StringComparison.OrdinalIgnoreCase);
            if (!lector.IsDBNull(1)) permisos.Add(lector.GetString(1));
        }
        return new(administrador, permisos);
    }

    internal static int Exigir(SqlConnection conexion, string modulo, bool gestionar = false, SqlTransaction? transaccion = null)
    {
        if (!Obtener(conexion, transaccion).Puede(modulo, gestionar))
            throw new UnauthorizedAccessException($"No tienes permiso para {(gestionar ? "modificar" : "consultar")} este contenido ({modulo}).");
        return SesionActual.Exigir().Id;
    }

    internal static void ExigirAlguno(SqlConnection conexion, params string[] modulos)
        => ExigirAlguno(conexion, false, modulos);

    internal static void ExigirAlguno(SqlConnection conexion, bool gestionar, params string[] modulos)
    {
        var permisos = Obtener(conexion);
        if (!modulos.Any(m => permisos.Puede(m, gestionar)))
            throw new UnauthorizedAccessException("No tienes permiso para consultar este contenido.");
    }

    internal static void Exigir(string modulo, bool gestionar = false)
    {
        using var conexion = Conexion.AbrirConexion();
        Exigir(conexion, modulo, gestionar);
    }

    internal static void ExigirPantalla(string pantalla)
    {
        using var conexion = Conexion.AbrirConexion();
        string? modulo = ModuloPantalla(pantalla);
        if (modulo is null) SesionActual.Validar(conexion);
        else Exigir(conexion, modulo);
    }

    internal static string? ModuloPantalla(string pantalla) => pantalla switch
    {
        "Inicio" or "Búsqueda global" => null,
        "Punto de Venta" => Modulos.Ventas,
        "Inventario y movimientos" or "Notificaciones" => Modulos.Inventario,
        "Compras" => Modulos.Compras,
        "Proveedores" => Modulos.Proveedores,
        "Traspasos" => Modulos.Traspasos,
        "Validacion" => Modulos.Validacion,
        "Carga masiva" => Modulos.CargaMasiva,
        "Catalogo" => Modulos.Productos,
        "Apartados y devoluciones" => Modulos.Apartados,
        "Directorio y roles" => Modulos.Directorio,
        "Usuarios, roles y permisos" => Modulos.Seguridad,
        "Configuración y auditoría" => Modulos.Configuracion,
        _ => throw new UnauthorizedAccessException("La pantalla no tiene un permiso de acceso definido.")
    };
}
