using System.Data;
using Sistema_ERP;

internal static partial class Program
{
    private static void ProbarCatalogoInicialPermisos()
    {
        int comprobaciones = 0;
        void Comprobar(bool condicion, string contexto) { Igual(true, condicion, contexto); comprobaciones++; }
        DataTable Preparar(int? idRol = null) => (DataTable)Invocar("SeguridadDatos", "CatalogoPermisosParaRol", idRol)!;
        if (!System.Text.RegularExpressions.Regex.IsMatch(new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(cadena).InitialCatalog,
            "^CodexERP_Prueba_[a-f0-9]{32}$")) throw new InvalidOperationException("Solo se vacía el catálogo en la base temporal de pruebas.");
        Ejecutar("DELETE FROM dbo.Permiso");
        using (var vacio = SeguridadDatos.Asignaciones(TipoSeguridad.Roles, null))
            Comprobar(vacio.Rows.Count == 0, "Se reproduce el catálogo vacío de una base existente");
        using (var catalogo = Preparar())
        {
            Comprobar(catalogo.Rows.Count == 24 && catalogo.AsEnumerable().Select(f => f.Field<string>("Modulo")).Distinct().Count() == 12,
                "La preparación crea consulta y gestión para las 12 pantallas sin migraciones manuales");
            Comprobar(catalogo.AsEnumerable().All(f => !f.Field<bool>("Asignado")), "Preparar el catálogo no concede permisos a ningún rol");
            using var dialogo = new AsignarPermisosRol("Cajero", catalogo, []);
            dialogo.ShowInTaskbar = false; dialogo.Opacity = 0;
            dialogo.StartPosition = FormStartPosition.Manual; dialogo.Location = new Point(-5000, -5000); dialogo.Show();
            var presets = dialogo.Controls.Find("listaPresets", true).OfType<ComboBox>().Single();
            presets.SelectedIndex = 1;
            var aplicar = dialogo.Controls.Find("aplicarPreset", true).OfType<Button>().Single();
            Comprobar(aplicar.Enabled && dialogo.Controls.Find("listaModulos", true).OfType<ListBox>().Single().Items.Count == 12,
                "El selector muestra las pantallas y habilita Aplicar preset tras preparar un catálogo vacío");
            aplicar.PerformClick();
            Comprobar(dialogo.PermisosSeleccionados.Length == 6, "Cajero funciona inmediatamente con los permisos preparados");
        }
        using var primera = Tabla("SELECT id_permiso,version_fila FROM dbo.Permiso ORDER BY id_permiso");
        using (var repetido = Preparar())
        using (var segunda = Tabla("SELECT id_permiso,version_fila FROM dbo.Permiso ORDER BY id_permiso"))
            Comprobar(repetido.Rows.Count == 24 && primera.AsEnumerable().Zip(segunda.AsEnumerable()).All(f =>
                f.First.Field<int>("id_permiso") == f.Second.Field<int>("id_permiso") && f.First.Field<byte[]>("version_fila")!.SequenceEqual(f.Second.Field<byte[]>("version_fila")!)),
                "Reabrir el selector no duplica ni modifica los permisos existentes");
        Ejecutar("DELETE FROM dbo.Permiso WHERE modulo='PRODUCTOS' AND accion='GESTIONAR'");
        using (var parcial = Preparar())
            Comprobar(parcial.Rows.Count == 24, "También se completa un catálogo parcialmente instalado");
        var datos = Tabla("""
            SET NOCOUNT ON;
            INSERT dbo.Rol(codigo,nombre) VALUES('CATALOGO_CONSERVAR',N'Conservar permisos');
            DECLARE @rol int=SCOPE_IDENTITY();
            INSERT dbo.RolPermiso(id_rol,id_permiso)
                SELECT @rol,id_permiso FROM dbo.Permiso WHERE modulo='COMPRAS' AND accion='CONSULTAR';
            UPDATE dbo.Permiso SET descripcion=N'Descripción personalizada',desactivado_en=SYSUTCDATETIME(),motivo_desactivacion=N'Inactivo deliberado'
                WHERE modulo='COMPRAS' AND accion='CONSULTAR';
            SELECT @rol AS Rol,id_permiso,version_fila FROM dbo.Permiso WHERE modulo='COMPRAS' AND accion='CONSULTAR';
            """).Rows[0];
        using (var conservado = Preparar((int)datos["Rol"]))
        {
            var inactivo = conservado.AsEnumerable().Single(f => f.Field<int>("Id") == (int)datos["id_permiso"]);
            Comprobar(!inactivo.Field<bool>("activo") && inactivo.Field<bool>("Asignado") && inactivo.Field<string>("Descripcion") == "Descripción personalizada",
                "La preparación respeta permisos inactivos, descripciones y asignaciones anteriores");
        }
        using (var estado = Tabla("SELECT version_fila FROM dbo.Permiso WHERE modulo='COMPRAS' AND accion='CONSULTAR'"))
            Comprobar(((byte[])estado.Rows[0][0]).SequenceEqual((byte[])datos["version_fila"]), "Un permiso desactivado conserva su versión sin reactivarse");
        Comprobar((int)Tabla("SELECT COUNT(*) FROM dbo.RolPermiso").Rows[0][0] == 1, "Completar el catálogo no agrega asignaciones de rol");
        Ejecutar("UPDATE dbo.Permiso SET desactivado_en=NULL,motivo_desactivacion=NULL WHERE modulo='COMPRAS' AND accion='CONSULTAR'");
        Console.WriteLine($"OK: {comprobaciones} comprobaciones de catálogo vacío, reparación, presets, idempotencia e inactivos.");
    }
}
