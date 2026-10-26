namespace Sistema_ERP
{
    public sealed partial class AgregarDirectorio
    {
        private void ConfigurarPresentacion()
        {
            Text = tipo switch { TipoDirectorio.Clientes => "Agregar cliente", TipoDirectorio.Proveedores => "Agregar proveedor", _ => "Agregar empleado" };
            lblNombre.Text = tipo == TipoDirectorio.Proveedores ? "Empresa *" : "Nombre *";
            lblIdentificador.Text = tipo switch { TipoDirectorio.Clientes => "Documento fiscal", TipoDirectorio.Proveedores => "RTN", _ => "Codigo *" };
            if (tipo == TipoDirectorio.Empleados)
            {
                campos.Controls.Remove(direccion); lblDireccion.Text = "Tienda *";
                campos.Controls.Add(tienda, 0, campos.GetRow(lblDireccion) + 1);
            }
            if (tipo == TipoDirectorio.Proveedores)
            {
                var fila = campos.GetRow(estado);
                campos.RowCount = fila + 3;
                campos.SetRow(estado, fila + 2);
                // El control y su etiqueta estan declarados en el disenador.
                campos.Controls.Add(lblContacto, 0, fila);
                campos.Controls.Add(contacto, 0, fila + 1);
            }
        }
    }
}
