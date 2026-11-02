using System.Data;
using Microsoft.Data.SqlClient;

namespace Sistema_ERP
{
    internal static class SesionActual
    {
        internal static AccesoUsuario? Usuario { get; set; }
        internal static AccesoUsuario Exigir() => Usuario is { Sesion: not null, CambiarClave: false } acceso
            ? acceso : throw new InvalidOperationException("Inicia sesión para realizar esta operación.");

        internal static int Validar(SqlConnection conexion, SqlTransaction? transaccion = null)
        {
            var acceso = Exigir();
            using var comando = new SqlCommand("""
                SELECT COUNT(*) FROM dbo.SesionUsuario s WITH(HOLDLOCK)
                JOIN dbo.Usuario u WITH(HOLDLOCK) ON u.id_usuario=s.id_usuario
                JOIN dbo.Empleado e WITH(HOLDLOCK) ON e.id_empleado=u.id_empleado
                WHERE s.id_sesion=@sesion AND s.id_usuario=@usuario AND s.revocada_en IS NULL
                AND s.expira_en>SYSUTCDATETIME() AND s.version_seguridad=u.version_seguridad
                AND u.activo=1 AND e.activo=1 AND u.debe_cambiar_contrasena=0
                """, conexion, transaccion);
            comando.Parameters.Add("@sesion", SqlDbType.BigInt).Value = acceso.Sesion!.Value;
            comando.Parameters.Add("@usuario", SqlDbType.Int).Value = acceso.Id;
            if (Convert.ToInt32(comando.ExecuteScalar()) != 1)
                throw new InvalidOperationException("La sesión ya no está vigente. Inicia sesión nuevamente.");
            return acceso.Id;
        }
    }
}
