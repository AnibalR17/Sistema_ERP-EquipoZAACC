using System.Data;
using Microsoft.Data.SqlClient;
using Sistema_ERP.Clases;

namespace Sistema_ERP;

internal static class AuditoriaDatos
{
    internal static void RegistrarNavegacion(string pantalla)
    {
        using var conexion = Conexion.AbrirConexion();
        int usuario = SesionActual.Validar(conexion);
        using var comando = new SqlCommand("""
            INSERT dbo.Auditoria(id_usuario,modulo,accion,entidad,id_registro,descripcion,equipo)
            VALUES(@usuario,'NAVEGACION','ABRIR','Pantalla',@pantalla,@descripcion,@equipo)
            """, conexion);
        comando.Parameters.AddWithValue("@usuario", usuario);
        comando.Parameters.Add("@pantalla", SqlDbType.NVarChar, 100).Value = pantalla;
        comando.Parameters.Add("@descripcion", SqlDbType.NVarChar, 1000).Value = "Acceso a " + pantalla;
        comando.Parameters.Add("@equipo", SqlDbType.NVarChar, 150).Value = Environment.MachineName;
        comando.ExecuteNonQuery();
    }

    internal static DataTable Listar(string filtro, long antes = long.MaxValue)
    {
        using var conexion = Conexion.AbrirConexion();
        SesionActual.Validar(conexion);
        using var comando = new SqlCommand("""
            SELECT TOP(200) a.id_auditoria AS Id,a.fecha AS [Fecha UTC],
            COALESCE(u.nombre_usuario,N'Sistema / acceso sin autenticar') AS Usuario,
            m.modulo AS Modulo,a.accion AS Accion,a.entidad AS Entidad,a.id_registro AS Registro,
            a.descripcion AS Descripcion,a.equipo AS Equipo
            FROM dbo.Auditoria a LEFT JOIN dbo.Usuario u ON u.id_usuario=a.id_usuario
            CROSS APPLY (VALUES(CASE WHEN a.modulo IN ('DOCUMENTOS','CATALOGO') THEN
                CASE a.entidad WHEN 'Compra' THEN 'COMPRAS' WHEN 'Venta' THEN 'VENTAS'
                WHEN 'Traspaso' THEN 'TRASPASOS' WHEN 'Apartado' THEN 'APARTADOS'
                WHEN 'Devolucion' THEN 'DEVOLUCIONES' WHEN 'Categoria' THEN 'PRODUCTOS' ELSE a.modulo END
                ELSE a.modulo END)) m(modulo)
            WHERE a.id_auditoria<@antes AND (@buscar=N'' OR CHARINDEX(@buscar,a.descripcion)>0
                OR CHARINDEX(@buscar,m.modulo)>0 OR CHARINDEX(@buscar,a.entidad)>0
                OR CHARINDEX(@buscar,a.id_registro)>0 OR CHARINDEX(@buscar,a.equipo)>0
                OR CHARINDEX(@buscar,a.accion)>0 OR CHARINDEX(@buscar,u.nombre_usuario)>0)
            ORDER BY a.id_auditoria DESC
            """, conexion);
        comando.Parameters.Add("@antes", SqlDbType.BigInt).Value = antes;
        comando.Parameters.Add("@buscar", SqlDbType.NVarChar, 150).Value = filtro.Trim();
        using var adaptador = new SqlDataAdapter(comando);
        var tabla = new DataTable(); adaptador.Fill(tabla); return tabla;
    }
}
