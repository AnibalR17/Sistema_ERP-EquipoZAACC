using System.Data;
using System.Reflection;
using Sistema_ERP;

internal static partial class Program
{
    private static void ProbarFiltrosFechas()
    {
        using var datos = new DataTable(); datos.Columns.Add("Nombre"); datos.Columns.Add("Fecha UTC", typeof(DateTime));
        datos.Columns.Add("Vencimiento", typeof(DateTime));
        DateTime dia = new(2024, 2, 29);
        datos.Rows.Add("Inicio", dia, dia.AddDays(2));
        datos.Rows.Add("Final", dia.AddDays(1).AddTicks(-1), dia.AddDays(2));
        datos.Rows.Add("Otro", dia.AddDays(1), dia);
        datos.Rows.Add("Anterior", dia.AddDays(-1), dia.AddDays(3));
        datos.Rows.Add("Sin fecha", DBNull.Value, DBNull.Value);
        using var tabla = new DataGridView { DataSource = datos, AllowUserToAddRows = false };
        using var filtro = (Control)app.GetType("Sistema_ERP.BusquedaTabla")!.GetMethod("Crear", estaticos)!.Invoke(null, [tabla, true])!;
        using var ventana = new FormularioTematico { Size = new Size(1000, 500), ShowInTaskbar = false,
            StartPosition = FormStartPosition.Manual, Location = new Point(-10000, -10000), Opacity = 0 };
        ventana.Controls.Add(filtro); ventana.Show(); Application.DoEvents();
        int comprobaciones = 0;
        void Comprobar(bool valor, string detalle) { Igual(true, valor, detalle); comprobaciones++; }
        var desde = filtro.Controls.Find("desdeFecha", true).OfType<DateTimePicker>().Single();
        var hasta = filtro.Controls.Find("hastaFecha", true).OfType<DateTimePicker>().Single();
        var campo = filtro.Controls.Find("campoFecha", true).OfType<ComboBox>().Single();
        var texto = filtro.Controls.Find("buscarDatos", true).OfType<TextBox>().Single();
        var limpiar = filtro.Controls.Find("limpiarFechas", true).OfType<Button>().Single();
        void Activar(DateTimePicker control, bool activo)
        {
            control.Checked = activo;
            typeof(DateTimePicker).GetMethod("OnValueChanged", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(control, [EventArgs.Empty]);
        }
        Comprobar(!desde.Checked && !hasta.Checked && tabla.Rows.Count == 5, "El historial comienza sin restringir fechas");
        desde.Value = hasta.Value = dia; Activar(desde, true); Activar(hasta, true);
        Comprobar(tabla.Rows.Count == 2 && datos.Rows.Count == 5, "Mismo día incluye medianoche y el último instante, sin modificar registros");
        texto.Text = "Final"; Comprobar(tabla.Rows.Count == 1, "La búsqueda de texto se combina con las fechas");
        texto.Clear(); campo.SelectedIndex = 1;
        Comprobar(tabla.Rows.Count == 1 && Convert.ToString(tabla.Rows[0].Cells["Nombre"].Value) == "Otro", "Se puede elegir la columna de fecha del historial");
        campo.SelectedIndex = 0; Activar(desde, false);
        Comprobar(tabla.Rows.Count == 3, "Solo fecha hasta incluye el día completo y excluye valores nulos");
        desde.Value = dia.AddDays(1); Activar(desde, true);
        Comprobar(tabla.Rows.Count == 0, "Un intervalo invertido muestra un resultado vacío");
        limpiar.PerformClick(); Comprobar(tabla.Rows.Count == 5 && !desde.Checked && !hasta.Checked, "Quitar fechas recupera todos los registros");
        Activar(hasta, false); desde.Value = dia.AddDays(1); Activar(desde, true);
        Comprobar(tabla.Rows.Count == 1, "Solo fecha desde incluye el límite inicial");
        using var recarga = datos.Copy(); recarga.Rows.Add("Nuevo", dia.AddDays(2), dia.AddDays(3)); tabla.DataSource = recarga;
        Comprobar(tabla.Rows.Count == 2, "Recargar conserva el intervalo elegido");
        Fallar(() => Invocar("VentaDatos", "Historial", dia.AddDays(1), dia));
        Fallar(() => Invocar("AuditoriaDatos", "Listar", "", long.MaxValue, dia.AddDays(1), dia));
        Console.WriteLine($"OK: {comprobaciones} comprobaciones de fechas opcionales, días completos, intervalos, columnas y recarga, sin SQL.");
    }

    private static void ProbarHistorialFechasSql()
    {
        Ejecutar($"""
            INSERT dbo.Venta(numero,id_tienda,id_cliente,id_usuario,fecha) VALUES
                ('FECHAS-INICIO',{tienda},{cliente},{usuario},'2020-02-29T00:00:00'),
                ('FECHAS-FINAL',{tienda},{cliente},{usuario},'2020-02-29T23:59:59.999'),
                ('FECHAS-FUERA',{tienda},{cliente},{usuario},'2020-03-01T00:00:00');
            WITH n AS (SELECT TOP(501) ROW_NUMBER() OVER(ORDER BY object_id) AS i FROM sys.all_objects)
            INSERT dbo.Venta(numero,id_tienda,id_cliente,id_usuario,fecha)
                SELECT CONCAT('FECHAS-NUEVA-',i),{tienda},{cliente},{usuario},'2024-01-01' FROM n;
            INSERT dbo.Auditoria(fecha,id_usuario,modulo,accion,entidad,id_registro,descripcion) VALUES
                ('2020-02-29T00:00:00',{usuario},'VENTAS','CONSULTAR','Venta','INICIO','PRUEBA_FECHAS_HISTORIAL'),
                ('2020-02-29T23:59:59.999',{usuario},'VENTAS','CONSULTAR','Venta','FINAL','PRUEBA_FECHAS_HISTORIAL'),
                ('2020-03-01T00:00:00',{usuario},'VENTAS','CONSULTAR','Venta','FUERA','PRUEBA_FECHAS_HISTORIAL');
            WITH n AS (SELECT TOP(205) ROW_NUMBER() OVER(ORDER BY object_id) AS i FROM sys.all_objects)
            INSERT dbo.Auditoria(fecha,id_usuario,modulo,accion,entidad,id_registro,descripcion)
                SELECT '2024-01-01',{usuario},'VENTAS','CONSULTAR','Venta',CONVERT(nvarchar(100),i),'PRUEBA_FECHAS_HISTORIAL' FROM n;
            """);
        DateTime dia = new(2020, 2, 29);
        using var recientes = (DataTable)Invocar("VentaDatos", "Historial")!;
        Igual(500, recientes.Rows.Count, "Historial de facturas conserva su límite por consulta");
        Igual(false, recientes.AsEnumerable().Any(r => Convert.ToString(r["Numero"]) == "FECHAS-INICIO"), "La factura antigua está fuera de las últimas 500");
        using var facturas = (DataTable)Invocar("VentaDatos", "Historial", dia, dia)!;
        Igual(2, facturas.Rows.Count, "El filtro SQL encuentra facturas antiguas antes de limitar los resultados e incluye el día completo");
        using var vacio = (DataTable)Invocar("VentaDatos", "Historial", dia.AddDays(-1), dia.AddDays(-1))!;
        Igual(0, vacio.Rows.Count, "La búsqueda de facturas admite días sin registros");
        using var antiguos = (DataTable)Invocar("AuditoriaDatos", "Listar", "PRUEBA_FECHAS_HISTORIAL", long.MaxValue, dia, dia)!;
        Igual(2, antiguos.Rows.Count, "Auditoría aplica texto y fechas antes de limitar a 200");
        using var primera = (DataTable)Invocar("AuditoriaDatos", "Listar", "PRUEBA_FECHAS_HISTORIAL", long.MaxValue, new DateTime(2024, 1, 1), new DateTime(2024, 1, 1))!;
        Igual(200, primera.Rows.Count, "Primera página de auditoría por fecha");
        using var segunda = (DataTable)Invocar("AuditoriaDatos", "Listar", "PRUEBA_FECHAS_HISTORIAL", (long)primera.Rows[^1]["Id"], new DateTime(2024, 1, 1), new DateTime(2024, 1, 1))!;
        Igual(5, segunda.Rows.Count, "La página siguiente conserva el rango y no mezcla eventos de otras fechas");
        Console.WriteLine("OK: filtros SQL de historiales, límites de días completos, facturas antiguas y paginación de auditoría.");
    }
}
