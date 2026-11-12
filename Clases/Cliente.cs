namespace Sistema_ERP.Clases
{

    public class Cliente
    {
        public int IdCliente { get; set; }
        public string Nombre { get; set; } = "";
        public string? Telefono { get; set; }
        public string? Correo { get; set; }

        public DateTime? DesactivadoEn { get; set; }
        public int? DesactivadoPor { get; set; }
        public string? MotivoDesactivacion { get; set; }
        public DateTime? ActualizadoEn { get; set; }

      
        public bool Activo => DesactivadoEn is null;


        public void Validar()
        {
            if (string.IsNullOrWhiteSpace(Nombre))
                throw new ArgumentException("El nombre del cliente es obligatorio.", nameof(Nombre));

            Nombre = Nombre.Trim();
            Telefono = string.IsNullOrWhiteSpace(Telefono) ? null : Telefono.Trim();
            Correo = string.IsNullOrWhiteSpace(Correo) ? null : Correo.Trim();
            ValidacionDatos.NoSoloCeros(Nombre, "Nombre del cliente");
            ValidacionDatos.NoSoloCeros(Telefono, "Teléfono");
            ValidacionDatos.NoSoloCeros(Correo, "Correo");
            ValidacionDatos.Nombre(Nombre, "Nombre del cliente");
            ValidacionDatos.Correo(Correo, maximo: 150);
        }
    }
}
