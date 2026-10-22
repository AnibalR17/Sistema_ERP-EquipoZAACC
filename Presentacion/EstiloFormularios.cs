namespace Sistema_ERP.Vistas
{
    internal static class EstiloFormularios
    {
        internal static void Aplicar(Form formulario, TableLayoutPanel layout, Label titulo)
        {
            formulario.SuspendLayout();
            formulario.Font = new Font("Segoe UI", 11F);
            formulario.ForeColor = ProductoEstilo.Texto;
            formulario.BackColor = ProductoEstilo.Fondo;
            formulario.Padding = new Padding(32, 20, 32, 28);
            formulario.AutoScroll = true;
            layout.MinimumSize = new Size(680, 540);
            titulo.Font = new Font("Segoe UI", 27F, FontStyle.Bold);
            titulo.Dock = DockStyle.Fill;
            layout.RowStyles[0].Height = 72;
            formulario.ResumeLayout(true);
        }

        internal static void Acciones(FlowLayoutPanel acciones, TextBox buscar, params Button[] botones)
        {
            acciones.WrapContents = false;
            acciones.AutoScroll = true;
            foreach (var boton in botones)
            {
                ProductoEstilo.Boton(boton, boton == botones[0]);
                boton.Width = 110;
                boton.Margin = new Padding(0, 4, 8, 0);
            }
            buscar.Width = 260;
            buscar.Margin = new Padding(0, 8, 0, 0);
        }

        internal static void Campos(Panel panel, Label titulo, params (Label etiqueta, Control campo)[] campos)
        {
            var tabla = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, ColumnCount = campos.Length, RowCount = 3,
                Padding = new Padding(16, 8, 16, 8)
            };
            tabla.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
            tabla.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));
            tabla.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            for (int i = 0; i < campos.Length; i++)
            {
                tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / campos.Length));
                var (etiqueta, campo) = campos[i];
                etiqueta.Dock = DockStyle.Fill;
                etiqueta.AutoSize = false;
                etiqueta.Margin = new Padding(0, 0, 12, 0);
                campo.Dock = DockStyle.Top;
                campo.Margin = new Padding(0, 0, 12, 0);
                tabla.Controls.Add(etiqueta, i, 1);
                tabla.Controls.Add(campo, i, 2);
            }
            titulo.Dock = DockStyle.Fill;
            tabla.Controls.Add(titulo, 0, 0);
            tabla.SetColumnSpan(titulo, campos.Length);
            panel.Controls.Add(tabla);
        }
    }
}
