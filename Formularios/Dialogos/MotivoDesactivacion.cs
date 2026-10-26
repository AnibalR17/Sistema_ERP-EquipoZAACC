namespace Sistema_ERP
{
    public sealed class MotivoDesactivacion : DialogoMaqueta
    {
        private readonly TextBox txtMotivo = new()
        {
            Name = "txtMotivo",
            Dock = DockStyle.Fill,
            Multiline = true,
            MaxLength = 300,
            ScrollBars = ScrollBars.Vertical,
            AcceptsReturn = true,
            BorderStyle = BorderStyle.FixedSingle,
            PlaceholderText = "Escribe el motivo de desactivación..."
        };

        public string Motivo { get; private set; } = "";

        public MotivoDesactivacion(string nombre, string entidad = "categoría") : base($"Desactivar {entidad}", 410)
        {
            var campos = PantallaMaqueta.Filas(64, 32, -100, 36);
            var descripcion = PantallaMaqueta.Etiqueta(
                $"Se desactivará «{nombre}». Se conservarán sus registros asociados e historial.", 10);
            descripcion.AutoEllipsis = true;
            campos.Controls.Add(descripcion, 0, 0);
            campos.Controls.Add(PantallaMaqueta.Etiqueta("Motivo (obligatorio, máximo 300 caracteres)", 10), 0, 1);
            campos.Controls.Add(txtMotivo, 0, 2);
            var error = PantallaMaqueta.Etiqueta("", 10);
            error.ForeColor = Color.Firebrick;
            campos.Controls.Add(error, 0, 3);
            Contenido.Controls.Add(campos, 0, 1);

            var acciones = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false
            };
            var confirmar = PantallaMaqueta.Boton("Desactivar", true);
            var cancelar = PantallaMaqueta.Boton("Cancelar");
            cancelar.DialogResult = DialogResult.Cancel;
            confirmar.Click += (_, _) =>
            {
                string motivo = txtMotivo.Text.Trim();
                if (motivo.Length == 0)
                {
                    error.Text = "Introduce el motivo para continuar.";
                    txtMotivo.Focus();
                    return;
                }
                Motivo = motivo;
                DialogResult = DialogResult.OK;
            };
            acciones.Controls.Add(confirmar);
            acciones.Controls.Add(cancelar);
            Contenido.Controls.Add(acciones, 0, 2);
            AcceptButton = confirmar;
            CancelButton = cancelar;
            Shown += (_, _) => txtMotivo.Focus();
        }
    }
}
