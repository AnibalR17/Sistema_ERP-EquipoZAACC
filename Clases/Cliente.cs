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
        { GuardarClienteValidacion.Validar(this); }
    }
}
