namespace Sistema_ERP;

public sealed partial class ConfiguracionAuditoria
{
    private void PrepararApariencia(TabControl tabs)
    {
        var pagina = new TabPage("Apariencia") { Name = "tabApariencia", Padding = new Padding(24), AutoScroll = true };
        var contenido = PantallaMaqueta.Filas(56, 50, 30, 44, 64, 70);
        contenido.Dock = DockStyle.Top;
        contenido.Height = 314;
        contenido.MaximumSize = new Size(850, 400);
        contenido.Controls.Add(PantallaMaqueta.Etiqueta("Apariencia del programa", 19, true), 0, 0);
        contenido.Controls.Add(PantallaMaqueta.Etiqueta("Elige el tema con el que prefieres trabajar. Puedes cambiarlo en cualquier momento."), 0, 1);
        contenido.Controls.Add(PantallaMaqueta.Etiqueta("Tema de colores"), 0, 2);
        var selector = new ComboBox
        {
            Name = "selectorTema", DropDownStyle = ComboBoxStyle.DropDownList,
            Dock = DockStyle.Left, Width = 340, AccessibleName = "Tema de colores"
        };
        selector.Items.AddRange(["Claro (azul y gris)", "Oscuro (estilo VS Code)"]);
        selector.SelectedIndex = TemaAplicacion.Actual == TemaVisual.Oscuro ? 1 : 0;
        contenido.Controls.Add(selector, 0, 3);
        var acciones = new FlowLayoutPanel { Dock = DockStyle.Fill };
        var aplicar = PantallaMaqueta.Boton("Aplicar tema", true);
        aplicar.Name = "btnAplicarTema";
        var mensaje = PantallaMaqueta.Etiqueta("Se aplica a las ventanas abiertas y se recuerda para tu usuario en este equipo.");
        mensaje.Name = "estadoTema";
        acciones.Controls.Add(aplicar);
        contenido.Controls.Add(acciones, 0, 4);
        contenido.Controls.Add(mensaje, 0, 5);
        pagina.Controls.Add(contenido);
        tabs.TabPages.Add(pagina);
        aplicar.Click += (_, _) =>
        {
            aplicar.Enabled = false;
            try
            {
                TemaAplicacion.Cambiar(selector.SelectedIndex == 1 ? TemaVisual.Oscuro : TemaVisual.Claro);
                mensaje.ForeColor = ProductoEstilo.Exito;
                mensaje.Text = "Tema aplicado. Se conservará al volver a abrir el programa.";
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                selector.SelectedIndex = TemaAplicacion.Actual == TemaVisual.Oscuro ? 1 : 0;
                mensaje.ForeColor = ProductoEstilo.Error;
                mensaje.Text = "No se pudo guardar el tema. Inténtalo nuevamente.";
            }
            finally { aplicar.Enabled = true; }
        };
    }
}
