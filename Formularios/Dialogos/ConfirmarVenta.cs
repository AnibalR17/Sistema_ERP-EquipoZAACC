using System.Drawing.Drawing2D;

namespace Sistema_ERP
{
    public sealed class ConfirmarVenta : DialogoMaqueta
    {
        public ConfirmarVenta() : base("¿Procesar venta?", 210)
        {
            Contenido.Controls.Add(PantallaMaqueta.Etiqueta("Total: L. 0.00\nCambio: L. 0.00"), 0, 1);
            var acciones = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
            var confirmar = PantallaMaqueta.Boton("Confirmar", true); confirmar.Width = 125; confirmar.DialogResult = DialogResult.OK;
            var cancelar = PantallaMaqueta.Boton("Cancelar"); cancelar.Width = 115; cancelar.DialogResult = DialogResult.Cancel;
            acciones.Controls.Add(confirmar); acciones.Controls.Add(cancelar); Contenido.Controls.Add(acciones, 0, 2); CancelButton = cancelar;
        }
    }
}
