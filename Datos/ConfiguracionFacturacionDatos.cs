using System.Data;
using System.Text.RegularExpressions;
using Sistema_ERP.Clases;
using static Sistema_ERP.FlujoVentaDatos;

namespace Sistema_ERP;

internal static class ConfiguracionFacturacionDatos
{
    internal static string? NormalizarCai(string? cai)
    { return GuardarCaiValidacion.Normalizar(cai); }

    internal static DataTable Obtener(int tienda)
    {
        using var cn = Conexion.AbrirConexion(); AutorizacionDatos.Exigir(cn, Modulos.configuracion);
        return Tabla(cn, null, "SELECT cai,version_fila FROM dbo.ConfiguracionFacturacion WHERE id_tienda=@tienda", ("@tienda", tienda));
    }

    internal static void Guardar(int tienda, string? cai, byte[]? version)
    {
        cai = NormalizarCai(cai);
        using var cn = Conexion.AbrirConexion(); using var tx = cn.BeginTransaction(IsolationLevel.Serializable);
        int usuario = AutorizacionDatos.Exigir(cn, Modulos.configuracion, gestionar: true, transaccion: tx);
        var anterior = Tabla(cn, tx, "SELECT version_fila FROM dbo.ConfiguracionFacturacion WITH(UPDLOCK,HOLDLOCK) WHERE id_tienda=@tienda", ("@tienda", tienda));
        GuardarCaiValidacion.VersionActual(anterior, version);
        Ejecutar(cn, tx, """
            EXEC sys.sp_set_session_context @key=N'id_usuario',@value=@usuario;
            IF NOT EXISTS(SELECT 1 FROM dbo.Tienda WITH(HOLDLOCK) WHERE id_tienda=@tienda AND activo=1)
                THROW 50001,'Selecciona una tienda activa.',1;
            IF EXISTS(SELECT 1 FROM dbo.ConfiguracionFacturacion WHERE id_tienda=@tienda)
                UPDATE dbo.ConfiguracionFacturacion SET cai=@cai,actualizado_en=SYSUTCDATETIME(),actualizado_por=@usuario WHERE id_tienda=@tienda;
            ELSE INSERT dbo.ConfiguracionFacturacion(id_tienda,cai,actualizado_por) VALUES(@tienda,@cai,@usuario);
            """, ("@tienda", tienda), ("@cai", cai), ("@usuario", usuario));
        tx.Commit();
    }
}
