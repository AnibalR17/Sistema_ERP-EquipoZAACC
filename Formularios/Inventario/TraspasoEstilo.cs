namespace Sistema_ERP
{
    internal static class TraspasoEstilo
    {
        internal static void Superficie(Control control)
        {
            control.Font = new Font("Segoe UI", 11F);
            control.ForeColor = ProductoEstilo.Texto;
            control.BackColor = ProductoEstilo.Fondo;
            control.Padding = new Padding(32, 34, 32, 36);
            TemaAplicacion.Aplicar(control);
            if (control is Form ventana)
            {
                ventana.MinimumSize = new Size(560, 360);
                ventana.ShowInTaskbar = false;
            }
        }

        internal static Control Cabecera(string titulo, string descripcion)
        {
            var panel = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 1, Margin = Padding.Empty };
            panel.Controls.Add(new Label { Text = titulo, Font = new Font("Segoe UI", 27F, FontStyle.Bold), AutoSize = true, Margin = new Padding(0, 0, 0, 8) });
            panel.Controls.Add(new Label { Text = descripcion, ForeColor = ProductoEstilo.TextoSecundario, AutoSize = true, MaximumSize = new Size(1000, 0), Margin = new Padding(0, 0, 0, 20) });
            return panel;
        }

        internal static Control Campo(string texto, Control campo)
        {
            var panel = new TableLayoutPanel { AutoSize = true, ColumnCount = 1, RowCount = 2, Margin = new Padding(0, 0, 20, 12) };
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            campo.Margin = Padding.Empty;
            campo.BackColor = Color.White;
            campo.ForeColor = ProductoEstilo.Texto;
            if (campo is TextBox entrada)
                entrada.BorderStyle = BorderStyle.FixedSingle;
            if (campo is ComboBox combo)
                combo.FlatStyle = FlatStyle.Flat;
            panel.Controls.Add(new Label { Text = texto, AutoSize = true, Margin = new Padding(0, 0, 0, 8) });
            panel.Controls.Add(campo);
            return panel;
        }

        internal static void Boton(Button boton, bool principal = false)
        {
            ProductoEstilo.Boton(boton, principal);
            boton.AutoSize = true;
            boton.MinimumSize = new Size(110, 44);
            boton.Margin = new Padding(0, 4, 12, 8);
            boton.Padding = new Padding(12, 0, 12, 0);
        }

        internal static Control Tarjeta(Control contenido)
        {
            var panel = new PanelDisenoInventario { Dock = DockStyle.Fill, Padding = new Padding(24, 18, 24, 28), Radio = 30, MostrarSombra = false, ColorBorde = Color.FromArgb(247, 247, 247) };
            panel.Controls.Add(contenido);
            return panel;
        }

        internal static void Tabla(DataGridView tabla)
        {
            tabla.BackgroundColor = Color.White;
            tabla.BorderStyle = BorderStyle.None;
            tabla.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            tabla.GridColor = ProductoEstilo.Suave;
            tabla.EnableHeadersVisualStyles = false;
            tabla.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            tabla.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = ProductoEstilo.Suave,
                ForeColor = ProductoEstilo.Texto,
                SelectionBackColor = ProductoEstilo.Suave,
                SelectionForeColor = ProductoEstilo.Texto,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Padding = new Padding(8)
            };
            tabla.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            tabla.ColumnHeadersHeight = 48;
            tabla.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.White,
                ForeColor = ProductoEstilo.Texto,
                SelectionBackColor = ProductoEstilo.Suave,
                SelectionForeColor = ProductoEstilo.Texto,
                Padding = new Padding(8, 4, 8, 4),
                NullValue = ""
            };
            tabla.RowTemplate.Height = 42;
            tabla.AllowUserToResizeRows = false;
            tabla.DataBindingComplete += (_, _) =>
            {
                foreach (DataGridViewColumn columna in tabla.Columns)
                    columna.HeaderText = columna.DataPropertyName switch
                    {
                        "id_traspaso" => "Solicitud",
                        "id_producto" => "Código de producto",
                        "estado" => "Estado",
                        "solicitado_en" => "Fecha de solicitud",
                        "observaciones" => "Observaciones",
                        "cantidad" => "Cantidad",
                        "precio_referencia" => "Precio de referencia",
                        "fecha" => "Fecha",
                        "estado_anterior" => "Estado anterior",
                        "estado_nuevo" => "Nuevo estado",
                        "motivo" => "Motivo",
                        "existencia" => "Existencia actual",
                        "reservado" => "Reservado actual",
                        "disponible" => "Disponible actual",
                        _ => columna.HeaderText
                    };
            };
        }

        internal static Control Pie(Form ventana, Button principal)
        {
            Boton(principal, true);
            principal.Dock = DockStyle.None;
            var cancelar = new Button { Text = "Cerrar", DialogResult = DialogResult.Cancel };
            Boton(cancelar);
            var acciones = new FlowLayoutPanel { Dock = DockStyle.Bottom, AutoSize = true, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(0, 12, 0, 0) };
            acciones.Controls.Add(principal);
            acciones.Controls.Add(cancelar);
            ventana.CancelButton = cancelar;
            return acciones;
        }
    }

}
