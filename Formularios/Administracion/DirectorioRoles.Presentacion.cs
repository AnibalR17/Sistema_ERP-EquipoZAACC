namespace Sistema_ERP
{
    public sealed partial class DirectorioRoles
    {
        private readonly Dictionary<Control, TableLayoutPanel> bloques = new();

        private void ConfigurarPresentacion()
        {
            bloques.Add(nombre, bloqueNombre);
            bloques.Add(identificador, bloqueIdentificador);
            bloques.Add(telefono, bloqueTelefono);
            bloques.Add(correo, bloqueCorreo);
            bloques.Add(direccion, bloqueDireccion);
            bloques.Add(contacto, bloqueContacto);
            bloques.Add(tienda, bloqueTienda);
            ConfigurarCampos();
        }

        private void ConfigurarCampos()
        {
            campos.SuspendLayout(); campos.Controls.Clear();
            void Campo(Control control, string titulo, int columna, int fila, int span = 1)
            {
                var bloque = bloques[control]; bloque.Controls[0].Text = titulo;
                bloque.Margin = new Padding(0, 0, columna + span == 3 ? 0 : 18, 0);
                campos.Controls.Add(bloque, columna, fila); campos.SetColumnSpan(bloque, span);
            }
            Campo(nombre, tipoActual == TipoDirectorio.Proveedores ? "Empresa *" : "Nombre *", 0, 0);
            Campo(identificador, tipoActual switch { TipoDirectorio.Clientes => "Documento fiscal (opcional)", TipoDirectorio.Proveedores => "RTN (opcional)", _ => "Código *" }, 1, 0);
            if (tipoActual == TipoDirectorio.Proveedores)
            {
                Campo(contacto, "Contacto", 2, 0); Campo(telefono, "Teléfono", 0, 1); Campo(correo, "Correo", 1, 1); Campo(direccion, "Dirección", 2, 1);
            }
            else if (tipoActual == TipoDirectorio.Empleados)
            {
                Campo(tienda, "Tienda *", 2, 0); Campo(telefono, "Teléfono", 0, 1); Campo(correo, "Correo", 1, 1, 2);
            }
            else
            {
                Campo(telefono, "Teléfono", 2, 0); Campo(correo, "Correo", 0, 1); Campo(direccion, "Dirección", 1, 1, 2);
            }
            campos.ResumeLayout(true);
            aviso.Text = tipoActual == TipoDirectorio.Empleados
                ? "Nombre, código y tienda son obligatorios. Selecciona una fila para actualizar sus datos."
                : "El nombre es obligatorio. Selecciona una fila para actualizar sus datos.";
            inactivos.Text = "Mostrar también " + tipoActual.ToString().ToLowerInvariant() + " inactivos";
            buscar.PlaceholderText = tipoActual switch { TipoDirectorio.Clientes => "Buscar cliente por nombre o documento...", TipoDirectorio.Proveedores => "Buscar proveedor por nombre o RTN...", _ => "Buscar empleado por nombre o código..." };
            tabla.Columns.Clear();
            void Columna(string campo, string titulo) => tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = campo, DataPropertyName = campo, HeaderText = titulo, MinimumWidth = 90 });
            Columna("Nombre", tipoActual == TipoDirectorio.Proveedores ? "Empresa" : "Nombre");
            Columna("Identificador", tipoActual switch { TipoDirectorio.Clientes => "Documento", TipoDirectorio.Proveedores => "RTN", _ => "Código" });
            if (tipoActual == TipoDirectorio.Proveedores) Columna("Contacto", "Contacto");
            Columna("Telefono", "Teléfono"); Columna("Correo", "Correo");
            if (tipoActual == TipoDirectorio.Empleados) Columna("Tienda", "Tienda"); else Columna("Direccion", "Dirección");
            tabla.Columns.Add(new DataGridViewCheckBoxColumn { Name = "activo", DataPropertyName = "activo", HeaderText = "Activo", FillWeight = 50 });
            Columna("motivo_desactivacion", "Motivo de desactivación");
        }

    }
}
