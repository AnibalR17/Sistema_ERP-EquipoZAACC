using System.ComponentModel;
using System.Reflection;
using Sistema_ERP;
using Sistema_ERP.Vistas;

internal static partial class Program
{
    private sealed class SitioDiseno(IComponent componente) : ISite
    {
        public IComponent Component => componente;
        public IContainer? Container => null;
        public bool DesignMode => true;
        public string? Name { get; set; }
        public object? GetService(Type tipo) => null;
    }

    private static void ProbarDisposicion(bool rapida = false, bool capturar = false)
    {
        ProbarNavegacionLateral(capturar);
        var preparar = app.GetType("Sistema_ERP.DisposicionControles")!.GetMethod("Preparar", estaticos)!;
        var errores = new List<string>();
        int comprobaciones = 0;
        IEnumerable<Control> Descendientes(Control control) => control.Controls.Cast<Control>()
            .SelectMany(hijo => new[] { hijo }.Concat(Descendientes(hijo)));
        void Distribuir(Control control)
        {
            control.PerformLayout();
            foreach (Control hijo in control.Controls) Distribuir(hijo);
            control.PerformLayout();
        }
        void Comprobar(bool condicion, string detalle)
        {
            comprobaciones++;
            if (!condicion) errores.Add(detalle);
        }
        Form Envolver(UserControl contenido)
        {
            var ventana = new FormularioTematico { Name = contenido.GetType().Name, Text = contenido.GetType().Name };
            ventana.Site = new SitioDiseno(ventana);
            contenido.Dock = DockStyle.Fill;
            ventana.Controls.Add(contenido);
            return ventana;
        }

        foreach (float escala in rapida ? new[] { 1f } : new[] { 1f, 1.25f, 1.5f, 2f })
        foreach (int ancho in rapida ? new[] { 900 } : new[] { 900, 1366 })
        foreach (Func<Form> crear in new Func<Form>[]
        {
            () => new PantallaProductos(), () => new PantallaProveedores(), () => new Nuevo_Producto(),
            () => new PantallaCompras(), () => new PuntoVenta(), () => new ApartadosDevoluciones(),
            () => new UsuariosRolesPermisos(), () => new DirectorioRoles(), () => new ConfiguracionAuditoria(),
            () => new CargaMasiva(), () => Envolver(new Catalogo_Productos()), () => Envolver(new Inventario()),
            () => Envolver(new Traspaso_Tiendas()), () => Envolver(new Validacion_Traspasos())
        })
        {
            using var ventana = crear();
            ventana.Site = new SitioDiseno(ventana);
            ventana.AutoScaleMode = AutoScaleMode.None;
            var controles = new[] { ventana }.Concat(Descendientes(ventana)).ToArray();
            // Simula el cambio de métricas sin cambiar la configuración de Windows.
            // Todos los controles se construyen de nuevo para cada escala.
            if (escala != 1)
            {
                ventana.Scale(new SizeF(escala, escala));
                var fuentes = controles.Select(control => control.Font).ToArray();
                for (int i = 0; i < controles.Length; i++)
                    controles[i].Font = new Font(fuentes[i].FontFamily, fuentes[i].Size * escala, fuentes[i].Style);
            }
            ventana.ClientSize = new Size((int)(ancho * escala), (int)(850 * escala));
            preparar.Invoke(null, [ventana]);
            Distribuir(ventana);

            if (capturar && escala == 1 && ancho == 900
                && (ventana is PuntoVenta or PantallaCompras or ConfiguracionAuditoria or UsuariosRolesPermisos
                    || ventana.Name == nameof(Catalogo_Productos)))
            {
                app.GetType("Sistema_ERP.TemaAplicacion")!.GetMethod("Aplicar", estaticos)!.Invoke(null, [ventana]);
                ventana.ShowInTaskbar = false;
                ventana.Opacity = 0;
                ventana.StartPosition = FormStartPosition.Manual;
                ventana.Location = new Point(-10000, -10000);
                ventana.Show();
                Distribuir(ventana);
                string directorio = Path.Combine(AppContext.BaseDirectory, "capturas-disposicion");
                Directory.CreateDirectory(directorio);
                using var captura = new Bitmap(ventana.Width, ventana.Height);
                ventana.DrawToBitmap(captura, new Rectangle(Point.Empty, captura.Size));
                captura.Save(Path.Combine(directorio, (ventana.Name.Length > 0 ? ventana.Name : ventana.GetType().Name) + ".png"));
            }

            foreach (var tabs in controles.OfType<TabControl>())
                foreach (TabPage pagina in tabs.TabPages)
                {
                    tabs.SelectedTab = pagina;
                    Distribuir(ventana);
                    Verificar(pagina);
                }
            Verificar(ventana);

            // Reaplicar no debe acumular tamaños ni perder valores de los campos.
            var medidas = controles.OfType<Button>().Select(b => b.MinimumSize).ToArray();
            preparar.Invoke(null, [ventana]);
            Distribuir(ventana);
            Comprobar(controles.OfType<Button>().Select(b => b.MinimumSize).SequenceEqual(medidas),
                $"{ventana.GetType().Name}: reaplicar no acumula tamaños ({escala}, {ancho})");

            void Verificar(Control raiz)
            {
                bool Oculto(Control control) => Ancestros(control).Prepend(control).TakeWhile(c => c != ventana)
                    .Any(c => TypeDescriptor.GetProperties(c)[nameof(Control.Visible)]!.ShouldSerializeValue(c));
                foreach (var boton in Descendientes(raiz).OfType<Button>()
                    .Where(b => b.Parent is FlowLayoutPanel or TableLayoutPanel))
                {
                    if (Oculto(boton)) continue;
                    // Solo se verifica la página que tiene asignado el tamaño del tab.
                    if (Ancestros(boton).OfType<TabPage>().Any(p => ((TabControl)p.Parent!).SelectedTab != p)) continue;
                    string contexto = $"{ventana.GetType().Name}: {boton.Text} ({escala}, {ancho})";
                    Size texto = TextRenderer.MeasureText(boton.Text, boton.Font, Size.Empty, TextFormatFlags.SingleLine);
                    Comprobar(boton.ClientSize.Width >= texto.Width + boton.Padding.Horizontal,
                        contexto + $" texto horizontal {boton.Size}, requiere {texto.Width + boton.Padding.Horizontal}");
                    Comprobar(boton.ClientSize.Height >= texto.Height + boton.Padding.Vertical,
                        contexto + $" texto vertical {boton.Size}, requiere {texto.Height + boton.Padding.Vertical}");
                    if (boton.Parent is FlowLayoutPanel barra)
                        Comprobar(boton.Bottom + boton.Margin.Bottom <= barra.ClientSize.Height - barra.Padding.Bottom,
                            contexto + $" barra recorta botón: {boton.Bounds}, barra={barra.ClientSize}");
                    if (boton.Parent is TableLayoutPanel tabla && tabla.GetRowSpan(boton) == 1)
                    {
                        int fila = tabla.GetPositionFromControl(boton).Row;
                        int alto = tabla.GetRowHeights()[fila];
                        Comprobar(alto >= boton.Height + boton.Margin.Vertical,
                            contexto + $" fila recorta botón: {alto} < {boton.Height + boton.Margin.Vertical}");
                    }
                }
                foreach (var campo in Descendientes(raiz).Where(c => c is TextBox or ComboBox or NumericUpDown))
                {
                    if (Oculto(campo)) continue;
                    if (campo.Parent is not TableLayoutPanel tabla || tabla.GetRowSpan(campo) != 1) continue;
                    if (Ancestros(campo).OfType<TabPage>().Any(p => ((TabControl)p.Parent!).SelectedTab != p)) continue;
                    int fila = tabla.GetPositionFromControl(campo).Row;
                    int alto = tabla.GetRowHeights()[fila];
                    Comprobar(alto >= campo.Height + campo.Margin.Vertical,
                        $"{ventana.GetType().Name}: {campo.GetType().Name} {campo.Name} recortado ({escala}, {ancho}): {alto} < {campo.Height + campo.Margin.Vertical}, mínimo={campo.MinimumSize}, preferido={campo.PreferredSize}");
                }
                foreach (var etiqueta in Descendientes(raiz).OfType<Label>()
                    .Where(e => e.Parent is TableLayoutPanel && !string.IsNullOrWhiteSpace(e.Text) && !Oculto(e)))
                {
                    if (Ancestros(etiqueta).OfType<TabPage>().Any(p => ((TabControl)p.Parent!).SelectedTab != p)) continue;
                    int alto = etiqueta.GetPreferredSize(new Size(Math.Max(1, etiqueta.Width), 0)).Height;
                    Comprobar(etiqueta.Height >= alto,
                        $"{ventana.GetType().Name}: etiqueta {etiqueta.Text} recortada ({escala}, {ancho}): {etiqueta.Height} < {alto}");
                }
            }
        }
        if (errores.Count > 0)
            throw new InvalidOperationException(string.Join(Environment.NewLine, errores.Take(45)) + $"\nTotal: {errores.Count} fallos de {comprobaciones} comprobaciones.");
        string escenarios = rapida ? "900 px y escala 100 %" : "dos anchos y escalas 100/125/150/200 %";
        Console.WriteLine($"OK: {comprobaciones} comprobaciones de botones, etiquetas y campos en 14 pantallas, {escenarios}, sin SQL.");
    }

    private static IEnumerable<Control> Ancestros(Control control)
    {
        for (Control? actual = control.Parent; actual != null; actual = actual.Parent) yield return actual;
    }

    private static void ProbarNavegacionLateral(bool capturar)
    {
        const BindingFlags instancia = BindingFlags.Instance | BindingFlags.NonPublic;
        using var principal = new Pagina_Central();
        var menu = (Panel)typeof(Pagina_Central).GetField("menuNavegacion", instancia)!.GetValue(principal)!;
        var botones = menu.Controls.OfType<Button>().ToArray();
        Igual(13, botones.Length, "Se comprueban todas las opciones de navegación, incluido Proveedores");
        using var ventana = new FormularioTematico
        {
            Font = principal.Font, ShowInTaskbar = false, Opacity = 0,
            StartPosition = FormStartPosition.Manual, Location = new Point(-10000, -10000)
        };
        ventana.Site = new SitioDiseno(ventana);
        ventana.Controls.Add(menu);
        ventana.Show();
        var estilo = app.GetType("Sistema_ERP.ProductoEstilo")!.GetMethod("Navegacion", estaticos)!;
        var catalogo = botones.Single(b => b.Name == "catalogButton");
        var medida = botones.Single(b => b.Name == "inventoryButton").Size;
        foreach (int ancho in new[] { 265, 320, 400 })
        foreach (int alto in new[] { 300, 900 })
        {
            ventana.ClientSize = new Size(ancho, alto);
            ventana.PerformLayout(); menu.PerformLayout();
            foreach (bool seleccionado in new[] { true, false })
            {
                catalogo.Font = new Font(catalogo.Font, seleccionado ? FontStyle.Bold : FontStyle.Regular);
                estilo.Invoke(null, [catalogo, seleccionado]);
                menu.PerformLayout();
                Igual(true, botones.All(b => b.Size == medida), "El menú conserva el mismo tamaño con selección y desplazamiento");
                Igual(1, botones.Select(b => b.Left).Distinct().Count(), "Todas las opciones tienen el mismo margen izquierdo");
            }
        }
        if (capturar)
        {
            ventana.ClientSize = new Size(265, 760);
            catalogo.Font = new Font(catalogo.Font, FontStyle.Bold);
            estilo.Invoke(null, [catalogo, true]);
            string directorio = Path.Combine(AppContext.BaseDirectory, "capturas-disposicion");
            Directory.CreateDirectory(directorio);
            using var captura = new Bitmap(menu.Width, menu.Height);
            menu.DrawToBitmap(captura, new Rectangle(Point.Empty, captura.Size));
            captura.Save(Path.Combine(directorio, "NavegacionLateral.png"));
        }
        Console.WriteLine("OK: navegación lateral uniforme con selección, tres anchos y desplazamiento vertical, sin SQL.");
    }
}
