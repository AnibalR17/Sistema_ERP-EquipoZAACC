using System.Data;
using System.Text.RegularExpressions;
using Sistema_ERP.Clases;
using static Sistema_ERP.FlujoVentaDatos;

namespace Sistema_ERP;

internal static class ConfiguracionFacturacionDatos
{
    internal static string? NormalizarCai(string? cai)
    {
        string valor = Regex.Replace(cai ?? "", @"\s", "").ToUpperInvariant();
        if (valor.Length == 0) return null;
        if (Regex.IsMatch(valor, "^[A-Z0-9]{32}$")) valor = string.Join("-", Enumerable.Range(0, 5).Select(i => valor.Substring(i * 6, 6)).Append(valor[30..]));
        if (!Regex.IsMatch(valor, "^[A-Z0-9]{6}(?:-[A-Z0-9]{6}){4}-[A-Z0-9]{2}$"))
            throw new ArgumentException("Introduce el CAI autorizado con formato XXXXXX-XXXXXX-XXXXXX-XXXXXX-XXXXXX-XX (37 caracteres).");
        return valor;
    }

    internal static DataTable Obtener(int tienda)
    {
        using var cn = Conexion.AbrirConexion(); SesionActual.Validar(cn);
        return Tabla(cn, null, "SELECT cai,version_fila FROM dbo.ConfiguracionFacturacion WHERE id_tienda=@tienda", ("@tienda", tienda));
    }

    internal static void Guardar(int tienda, string? cai, byte[]? version)
    {
        cai = NormalizarCai(cai);
        using var cn = Conexion.AbrirConexion(); using var tx = cn.BeginTransaction(IsolationLevel.Serializable);
        int usuario = SesionActual.Validar(cn, tx);
        var anterior = Tabla(cn, tx, "SELECT version_fila FROM dbo.ConfiguracionFacturacion WITH(UPDLOCK,HOLDLOCK) WHERE id_tienda=@tienda", ("@tienda", tienda));
        if ((anterior.Rows.Count == 0 && version != null) || (anterior.Rows.Count == 1 && (version is null || !((byte[])anterior.Rows[0][0]).SequenceEqual(version))))
            throw new InvalidOperationException("El CAI cambió en otra ventana. Actualiza la configuración antes de guardar.");
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
