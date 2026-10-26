using System.Net.Mail;

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
        {
            if (string.IsNullOrWhiteSpace(NombreEmpresa))
                throw new ArgumentException("Introduzca el nombre de la empresa.");
            foreach (var (valor, limite, campo) in new[]
            {
                (NombreEmpresa, 150, "Nombre de empresa"), (Rtn, 30, "RTN"),
                (Contacto, 150, "Contacto"), (Telefono, 30, "Teléfono"),
                (Correo, 254, "Correo"), (Direccion, 300, "Dirección")
            })
                if (valor.Trim().Length > limite)
                    throw new ArgumentException($"{campo} admite hasta {limite} caracteres.");
            if (Rtn.Any(c => c > 127))
                throw new ArgumentException("El RTN debe contener caracteres ASCII.");
            if (!string.IsNullOrWhiteSpace(Correo) &&
                (!MailAddress.TryCreate(Correo.Trim(), out var correo) || correo.Address != Correo.Trim()))
                throw new ArgumentException("Introduzca un correo electrónico válido.");
        }
    }
}
