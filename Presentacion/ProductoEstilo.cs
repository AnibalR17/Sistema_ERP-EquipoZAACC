namespace Sistema_ERP
{
    internal static class ProductoEstilo
    {
        internal static readonly Color Fondo = Color.FromArgb(253, 253, 253);
        internal static readonly Color Suave = Color.FromArgb(243, 243, 243);
        internal static readonly Color Exito = Color.FromArgb(0, 100, 0);
        internal static readonly Color Error = Color.FromArgb(178, 34, 34);
        internal static readonly Color TextoSecundario = Color.FromArgb(125, 125, 125);
        internal static readonly Color Texto = Color.FromArgb(24, 24, 24);

        internal static void Boton(Button boton, bool principal = false)
        {
            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderSize = principal ? 0 : 1;
            boton.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 220);
            boton.FlatAppearance.MouseOverBackColor = principal ? Color.FromArgb(55, 55, 55) : Suave;
            boton.BackColor = principal ? Texto : Color.White;
            boton.ForeColor = principal ? Color.White : Texto;
            boton.Font = new Font("Segoe UI", 11F);
            boton.Cursor = Cursors.Hand;
            boton.UseVisualStyleBackColor = false;
            boton.Size = new Size(170, 44);
        }
    }
}
