namespace Sistema_ERP.Clases
{
    public sealed class Proveedor
    {
        public int IdProveedor { get; set; }
        public string NombreEmpresa { get; set; } = "";
        public string Rtn { get; set; } = "";
        public string Contacto { get; set; } = "";
        public string Telefono { get; set; } = "";
        public string Correo { get; set; } = "";
        public string Direccion { get; set; } = "";

        public void Validar()
        { GuardarProveedorValidacion.Validar(this); }
    }
}
