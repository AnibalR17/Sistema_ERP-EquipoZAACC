using System.Security.Cryptography;

namespace Sistema_ERP
{
    internal static class ClaveSegura
    {
        internal static string Crear(string clave)
        {
            if (clave.Length is < 12 or > 128) throw new ArgumentException("La contraseña debe tener entre 12 y 128 caracteres.");
            return Derivar(clave);
        }

        // La clave corta solo se permite para el arranque; exige cambio antes de crear una sesion.
        internal static string CrearTemporalAdministrador() => Derivar("admin");

        private static string Derivar(string clave)
        {
            byte[] sal = RandomNumberGenerator.GetBytes(16);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(clave, sal, 600000, HashAlgorithmName.SHA256, 32);
            return $"PBKDF2-SHA256$600000${Convert.ToBase64String(sal)}${Convert.ToBase64String(hash)}";
        }

        internal static bool Verificar(string clave, string almacenado)
        {
            if (clave.Length is < 1 or > 128) return false;
            string[] partes = almacenado.Split('$');
            if (partes.Length != 4 || partes[0] != "PBKDF2-SHA256" || partes[1] != "600000") return false;
            try
            {
                byte[] sal = Convert.FromBase64String(partes[2]), esperado = Convert.FromBase64String(partes[3]);
                if (sal.Length != 16 || esperado.Length != 32) return false;
                byte[] actual = Rfc2898DeriveBytes.Pbkdf2(clave, sal, 600000, HashAlgorithmName.SHA256, 32);
                return CryptographicOperations.FixedTimeEquals(actual, esperado);
            }
            catch (FormatException) { return false; }
        }
    }
}
