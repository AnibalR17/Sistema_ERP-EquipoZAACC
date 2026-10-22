namespace Sistema_ERP
{
    public sealed class ControlInversores : PantallaMaqueta
    {
        public ControlInversores() : base("Control de inversores", "Solo Administradores",
            "Rojo: vencida o vence en menos de 30 días. Amarillo: menos de 60 días.")
        {
            // Mantiene accesibles todos los campos mediante desplazamiento en ventanas pequeñas.
            Pagina.MinimumSize = new Size(680, 810);
            var contenido = Filas(120, 360, 174);
            var resumen = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, Margin = Padding.Empty };
            resumen.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            resumen.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            resumen.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            var total = Tarjeta(120);
            total.Margin = new Padding(0, 0, 10, 0);
            var datosTotal = Filas(28, -100);
            var etiquetaTotal = Etiqueta("Total invertido", 10); etiquetaTotal.ForeColor = Color.Gray;
            datosTotal.Controls.Add(etiquetaTotal, 0, 0);
            datosTotal.Controls.Add(Etiqueta("L. 0.00", 24, true), 0, 1);
            total.Controls.Add(datosTotal);
            var activas = Tarjeta(120);
            activas.Margin = new Padding(10, 0, 0, 0);
            var datosActivas = Filas(28, -100);
            var etiquetaActivas = Etiqueta("Inversiones activas", 10); etiquetaActivas.ForeColor = Color.Gray;
            datosActivas.Controls.Add(etiquetaActivas, 0, 0);
            datosActivas.Controls.Add(Etiqueta("0", 24, true), 0, 1);
            activas.Controls.Add(datosActivas);
            resumen.Controls.Add(total, 0, 0); resumen.Controls.Add(activas, 1, 0);

            var registro = Tarjeta(360);
            var campos = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 8, Margin = Padding.Empty };
            campos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            campos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            foreach (float altura in new[] { 38F, 24F, 48F, 24F, 48F, 24F, 48F, 56F })
                campos.RowStyles.Add(new RowStyle(SizeType.Absolute, altura));
            var titulo = Etiqueta("Registrar inversión");
            campos.Controls.Add(titulo, 0, 0); campos.SetColumnSpan(titulo, 2);
            void Campo(string texto, Control control, int columna, int fila)
            {
                var etiqueta = Etiqueta(texto, 10); etiqueta.ForeColor = Color.Gray;
                etiqueta.Margin = new Padding(0, 0, 16, 0);
                control.Dock = DockStyle.Top; control.Margin = new Padding(0, 4, 16, 10);
                control.Name = texto;
                campos.Controls.Add(etiqueta, columna, fila);
                campos.Controls.Add(control, columna, fila + 1);
            }
            Campo("Institución", new TextBox(), 0, 1);
            Campo("Monto", new NumericUpDown { DecimalPlaces = 2, Maximum = 1000000000, ThousandsSeparator = true }, 1, 1);
            var tipo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            tipo.Items.AddRange(new object[] { "Plazo fijo", "Ahorro", "Otro" }); tipo.SelectedIndex = 0;
            Campo("Tipo", tipo, 0, 3);
            Campo("Tasa de interés %", new NumericUpDown { DecimalPlaces = 2, Maximum = 100 }, 1, 3);
            Campo("Inicio", new DateTimePicker { Format = DateTimePickerFormat.Short }, 0, 5);
            Campo("Vencimiento", new DateTimePicker { Format = DateTimePickerFormat.Short }, 1, 5);
            var guardar = Boton("Guardar", true); guardar.Name = "guardarInversion"; guardar.Width = 120;
            // Esta pantalla conserva el alcance de diseño, sin guardar inversiones reales.
            guardar.Click += (_, _) => MessageBox.Show(this, "El registro de inversiones aún no está disponible.",
                "Control de inversores", MessageBoxButtons.OK, MessageBoxIcon.Information);
            campos.Controls.Add(guardar, 0, 7); registro.Controls.Add(campos);

            var listado = Tarjeta(174); var filas = Filas(44, -100, 24);
            var buscar = Buscar(); buscar.Dock = DockStyle.Left;
            filas.Controls.Add(buscar, 0, 0);
            filas.Controls.Add(Tabla("Institución", "Tipo", "Monto", "Tasa", "Inicio", "Días"), 0, 1);
            filas.Controls.Add(Etiqueta("Mostrando 0 registros", 9), 0, 2); listado.Controls.Add(filas);
            contenido.Controls.Add(resumen, 0, 0); contenido.Controls.Add(registro, 0, 1);
            contenido.Controls.Add(listado, 0, 2); Cuerpo.Controls.Add(contenido);
        }
    }
}
