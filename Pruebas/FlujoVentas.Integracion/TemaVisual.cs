using System.Reflection;
using Sistema_ERP;

internal static partial class Program
{
    private static void ProbarTemasVisuales()
    {
        var tema = app.GetType("Sistema_ERP.TemaAplicacion")!;
        var preferencias = app.GetType("Sistema_ERP.PreferenciasApariencia")!;
        var estilo = app.GetType("Sistema_ERP.ProductoEstilo")!;
        var tipoTema = app.GetType("Sistema_ERP.TemaVisual")!;
        object claro = Enum.Parse(tipoTema, "Claro"), oscuro = Enum.Parse(tipoTema, "Oscuro");
        object anterior = tema.GetProperty("Actual", estaticos)!.GetValue(null)!;
        int comprobaciones = 0;
        void Comprobar(bool valor, string detalle) { Igual(true, valor, detalle); comprobaciones++; }
        void Cambiar(object valor) => tema.GetMethod("Cambiar", estaticos)!.Invoke(null, [valor, false]);
        void Aplicar(Control control) => tema.GetMethod("Aplicar", estaticos)!.Invoke(null, [control]);
        Color ColorTema(string nombre) => (Color)estilo.GetProperty(nombre, estaticos)!.GetValue(null)!;
        void ComprobarContraste(Color texto, Color fondo, string detalle) =>
            Comprobar(ContrasteTema(texto, fondo) >= 4.5, detalle);

        string ruta = Path.Combine(Path.GetTempPath(), "ERP-apariencia-" + Guid.NewGuid().ToString("N") + ".json");
        try
        {
            using var ventana = new FormularioTematico();
            using var datos = new System.Data.DataTable();
            datos.Columns.Add("Producto"); datos.Rows.Add("Ejemplo");
            var contenedor = new FlowLayoutPanel { Dock = DockStyle.Fill };
            var principal = new Button { Text = "Guardar", Size = new Size(180, 44) };
            var secundario = new Button { Text = "Consultar" };
            var inactivo = new Button { Text = "Confirmar", Enabled = false, Size = principal.Size };
            var entrada = new TextBox { Text = "Datos sin guardar" };
            var soloLectura = new TextBox { Text = "Consulta", ReadOnly = true };
            var seleccion = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            seleccion.Items.AddRange(["Primero", "Segundo"]); seleccion.SelectedIndex = 1;
            var tabla = new DataGridView { DataSource = datos, AllowUserToAddRows = false };
            var aviso = new Label { Text = "Error de validación", ForeColor = Color.FromArgb(178, 34, 34) };
            estilo.GetMethod("ColoresBoton", estaticos)!.Invoke(null, [principal, true]);
            contenedor.Controls.AddRange([principal, secundario, inactivo, entrada, soloLectura, seleccion, tabla, aviso]);
            ventana.Controls.Add(contenedor);
            Aplicar(ventana);
            var medidas = principal.Size; var fuente = principal.Font;
            int clics = 0; inactivo.Click += (_, _) => clics++;
            foreach (object modo in new[] { oscuro, claro, oscuro, claro })
            {
                Cambiar(modo);
                bool esOscuro = modo.Equals(oscuro);
                Comprobar(esOscuro ? ventana.BackColor.GetBrightness() < .2f : ventana.BackColor.GetBrightness() > .9f, "Fondo del tema seleccionado");
                Comprobar(entrada.Text == "Datos sin guardar" && seleccion.SelectedIndex == 1 && tabla.DataSource == datos, "Cambiar tema conserva datos y selección");
                Comprobar(principal.Size == medidas && principal.Font == fuente, "Cambiar tema conserva distribución y tipografía");
                Comprobar(!inactivo.Enabled && soloLectura.ReadOnly, "Cambiar tema conserva restricciones de controles");
                inactivo.PerformClick(); Comprobar(clics == 0, "Cambiar tema no habilita acciones no disponibles");
                using (var captura = new Bitmap(inactivo.Width, inactivo.Height))
                {
                    inactivo.DrawToBitmap(captura, new Rectangle(Point.Empty, captura.Size));
                    int pixelesTexto = 0;
                    for (int y = 4; y < captura.Height - 4; y++)
                        for (int x = 4; x < captura.Width - 4; x++)
                            if (captura.GetPixel(x, y).ToArgb() == inactivo.ForeColor.ToArgb()) pixelesTexto++;
                    Comprobar(pixelesTexto > 20, "El texto deshabilitado se dibuja con el contraste del tema");
                }
                ComprobarContraste(principal.ForeColor, principal.BackColor, "Contraste de acción principal");
                ComprobarContraste(secundario.ForeColor, secundario.BackColor, "Contraste de acción secundaria");
                ComprobarContraste(inactivo.ForeColor, inactivo.BackColor, "Contraste de acción deshabilitada");
                ComprobarContraste(entrada.ForeColor, entrada.BackColor, "Contraste de campos");
                ComprobarContraste(tabla.DefaultCellStyle.ForeColor, tabla.DefaultCellStyle.BackColor, "Contraste de filas");
                ComprobarContraste(tabla.DefaultCellStyle.SelectionForeColor, tabla.DefaultCellStyle.SelectionBackColor, "Contraste de filas seleccionadas");
                ComprobarContraste(aviso.ForeColor, ventana.BackColor, "Contraste de mensajes de error");
                principal.Enabled = false; principal.Enabled = true;
                Comprobar(principal.BackColor.ToArgb() == ColorTema("Acento").ToArgb(), "Botón recupera el color del tema actual");
                var nuevo = new TextBox(); contenedor.Controls.Add(nuevo);
                Comprobar(nuevo.BackColor.ToArgb() == ColorTema("Campo").ToArgb(), "Los controles nuevos reciben el tema actual");
            }

            Cambiar(oscuro);
            using (var modal = new FormularioTematico())
            {
                var campo = new TextBox(); modal.Controls.Add(campo);
                bool temaAntesDeCargar = false;
                modal.Load += (_, _) => temaAntesDeCargar = campo.BackColor.ToArgb() == ColorTema("Campo").ToArgb();
                typeof(FormularioTematico).GetMethod("OnLoad", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(modal, [EventArgs.Empty]);
                Comprobar(temaAntesDeCargar, "Los diálogos reciben el tema antes de sus eventos de carga");
            }
            using (var configuracion = new ConfiguracionAuditoria())
            {
                Aplicar(configuracion);
                var tabs = configuracion.Controls.Find("pestanasConfiguracion", true).OfType<TabControl>().Single();
                Comprobar(tabs.TabPages.Count == 4 && tabs.TabPages[3].Name == "tabApariencia", "Apariencia conserva las pestañas de configuración anteriores");
                var selector = configuracion.Controls.Find("selectorTema", true).OfType<ComboBox>().Single();
                Comprobar(selector.SelectedIndex == 1 && selector.Items.Count == 2, "Configuración refleja el tema actual");
                Comprobar(configuracion.Controls.Find("btnAplicarTema", true).Single() is Button, "Configuración permite aplicar el tema");
                var mensajeTema = configuracion.Controls.Find("estadoTema", true).Single();
                Comprobar(mensajeTema.Parent!.Height >= mensajeTema.Bottom && mensajeTema.Parent.Height >= 300, "La opción Apariencia tiene espacio para mostrar sus controles");
            }

            using (var mdi = new FormularioTematico { IsMdiContainer = true })
            using (var hijo = new FormularioTematico())
            {
                Aplicar(mdi);
                var campo = new TextBox(); hijo.Controls.Add(campo); hijo.MdiParent = mdi;
                Comprobar(campo.BackColor.ToArgb() == ColorTema("Campo").ToArgb(), "Los módulos MDI nuevos reciben el tema");
            }

            object Cargar() => preferencias.GetMethod("Cargar", estaticos)!.Invoke(null, [ruta])!;
            void Guardar(object valor) => preferencias.GetMethod("Guardar", estaticos)!.Invoke(null, [ruta, valor]);
            Comprobar(Cargar().Equals(claro), "Sin preferencia se conserva el tema claro");
            Guardar(oscuro); Comprobar(Cargar().Equals(oscuro), "Se guarda y recupera el tema oscuro");
            tema.GetMethod("Inicializar", estaticos)!.Invoke(null, [ruta]);
            Comprobar(tema.GetProperty("Actual", estaticos)!.GetValue(null)!.Equals(oscuro), "El arranque restaura el tema guardado");
            Guardar(claro); Comprobar(Cargar().Equals(claro), "Se reemplaza la preferencia por el tema claro");
            foreach (string contenido in new[] { "{", "[]", "{\"tema\":42}", "{\"tema\":\"desconocido\"}" })
            {
                File.WriteAllText(ruta, contenido);
                Comprobar(Cargar().Equals(claro), "Una preferencia inválida no impide iniciar");
            }
            bool rechazado = false;
            try { preferencias.GetMethod("Guardar", estaticos)!.Invoke(null, [Path.Combine(ruta, "apariencia.json"), oscuro]); }
            catch (TargetInvocationException ex) when (ex.InnerException is IOException) { rechazado = true; }
            Comprobar(rechazado, "El fallo de guardado se comunica a la configuración");
        }
        finally { Cambiar(anterior); File.Delete(ruta); }
        Console.WriteLine($"OK: {comprobaciones} comprobaciones de temas, contraste, configuración y preferencias sin SQL.");
    }

    private static double ContrasteTema(Color texto, Color fondo)
    {
        double Luminancia(Color color)
        {
            double Canal(byte valor) { double c = valor / 255d; return c <= .04045 ? c / 12.92 : Math.Pow((c + .055) / 1.055, 2.4); }
            return .2126 * Canal(color.R) + .7152 * Canal(color.G) + .0722 * Canal(color.B);
        }
        double a = Luminancia(texto), b = Luminancia(fondo);
        return (Math.Max(a, b) + .05) / (Math.Min(a, b) + .05);
    }
}
