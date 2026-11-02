#nullable enable
namespace Sistema_ERP
{
    public sealed partial class AjusteInventario
    {
        private Label titulo = null!, indicacion = null!, error = null!, etiquetaCantidad = null!;
        private NumericUpDown cantidad = null!;
        private TextBox motivo = null!;
        private Button guardar = null!, cancelar = null!;

        private void InitializeComponent()
        {
            titulo = new Label(); indicacion = new Label(); error = new Label(); etiquetaCantidad = new Label();
            cantidad = new NumericUpDown(); motivo = new TextBox(); guardar = new Button(); cancelar = new Button();
            Font = new Font("Segoe UI", 11F); BackColor = Color.White;
            AutoScaleMode = AutoScaleMode.Font; ClientSize = new Size(620, 470);
            FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false; MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent; ShowInTaskbar = false;
            titulo.Location = new Point(24, 24); titulo.Size = new Size(572, 40); titulo.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            indicacion.Location = new Point(24, 70); indicacion.Size = new Size(572, 52);
            indicacion.Text = "Indica una cantidad positiva para entrada o negativa para salida. El motivo es obligatorio.";
            etiquetaCantidad.Text = "Cantidad *"; etiquetaCantidad.Location = new Point(24, 126); etiquetaCantidad.AutoSize = true;
            cantidad.Location = new Point(24, 152); cantidad.Width = 220; cantidad.Minimum = -int.MaxValue; cantidad.Maximum = int.MaxValue;
            var etiquetaMotivo = new Label { Text = "Motivo * (hasta 500 caracteres)", Location = new Point(24, 195), AutoSize = true };
            motivo.Location = new Point(24, 224); motivo.Size = new Size(572, 100); motivo.Multiline = true; motivo.MaxLength = 500;
            error.Location = new Point(24, 334); error.Size = new Size(572, 60); error.ForeColor = ProductoEstilo.Error;
            guardar.Text = "Confirmar"; cancelar.Text = "Cancelar";
            ProductoEstilo.Boton(guardar, true); ProductoEstilo.Boton(cancelar);
            guardar.Location = new Point(242, 402); cancelar.Location = new Point(426, 402);
            cancelar.DialogResult = DialogResult.Cancel; AcceptButton = guardar; CancelButton = cancelar;
            Controls.AddRange(new Control[] { titulo, indicacion, etiquetaCantidad, cantidad, etiquetaMotivo, motivo, error, guardar, cancelar });
        }
    }
}
