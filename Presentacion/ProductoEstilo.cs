namespace Sistema_ERP
{
    internal static class ProductoEstilo
    {
        internal static bool Oscuro { get; set; }
        // El tema oscuro toma sus colores base de VS Code Dark Modern.
        internal static Color Fondo => Oscuro ? Color.FromArgb(31, 31, 31) : Color.FromArgb(244, 247, 251);
        internal static Color Superficie => Oscuro ? Color.FromArgb(37, 37, 38) : Color.White;
        internal static Color Lateral => Oscuro ? Color.FromArgb(24, 24, 24) : Color.White;
        internal static Color Campo => Oscuro ? Color.FromArgb(49, 49, 49) : Color.White;
        internal static Color Suave => Oscuro ? Color.FromArgb(43, 43, 43) : Color.FromArgb(232, 238, 245);
        internal static Color Acento => Oscuro ? Color.FromArgb(0, 120, 212) : Color.FromArgb(29, 78, 216);
        internal static Color TextoAcento => Oscuro ? Color.FromArgb(77, 170, 252) : Acento;
        internal static Color AcentoHover => Oscuro ? Color.FromArgb(2, 110, 193) : Color.FromArgb(30, 64, 175);
        internal static Color Seleccion => Oscuro ? Color.FromArgb(38, 79, 120) : Color.FromArgb(219, 234, 254);
        internal static Color TextoSeleccion => Oscuro ? Color.White : Color.FromArgb(30, 58, 138);
        internal static Color Borde => Oscuro ? Color.FromArgb(60, 60, 60) : Color.FromArgb(184, 197, 214);
        internal static Color FondoDeshabilitado => Oscuro ? Color.FromArgb(45, 45, 45) : Color.FromArgb(226, 232, 240);
        internal static Color TextoDeshabilitado => Oscuro ? Color.FromArgb(160, 160, 160) : Color.FromArgb(71, 85, 105);
        internal static Color Exito => Oscuro ? Color.FromArgb(137, 209, 133) : Color.FromArgb(0, 100, 0);
        internal static Color Error => Oscuro ? Color.FromArgb(248, 81, 73) : Color.FromArgb(178, 34, 34);
        internal static Color TextoSecundario => Oscuro ? Color.FromArgb(157, 157, 157) : Color.FromArgb(71, 85, 105);
        internal static Color Texto => Oscuro ? Color.FromArgb(204, 204, 204) : Color.FromArgb(31, 41, 55);

        // Solo cambia los colores; conserva las medidas y la distribución del diseñador.
        internal static void ColoresBoton(Button boton, bool principal = false) =>
            EstiloBotones.Aplicar(boton, principal ? EstiloBotones.Tipo.Principal : EstiloBotones.Tipo.Secundario);

        internal static void Navegacion(Button boton, bool seleccionado) =>
            EstiloBotones.Aplicar(boton, seleccionado ? EstiloBotones.Tipo.NavegacionSeleccionada : EstiloBotones.Tipo.Navegacion);

        internal static void Boton(Button boton, bool principal = false)
        {
            ColoresBoton(boton, principal);
            boton.Font = new Font("Segoe UI", 11F);
            DisposicionControles.Boton(boton);
        }
    }
}
