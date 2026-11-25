using System.Security.Cryptography;
using System.Text;

namespace Sistema_ERP.Clases;

internal static class ConfiguracionConexion
{
    private static string Ruta => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "Sistema_ERP", "conexion.dpapi");

    internal static string? Leer()
    {
        string ruta = Ruta;
        if (!File.Exists(ruta)) return null;

        try
        {
            byte[] cifrado = File.ReadAllBytes(ruta);
            byte[] claro = ProtectedData.Unprotect(cifrado, optionalEntropy: null, DataProtectionScope.CurrentUser);
            try { return Encoding.UTF8.GetString(claro); }
            finally { CryptographicOperations.ZeroMemory(claro); }
        }
        catch (CryptographicException ex)
        {
            throw new InvalidOperationException(
                "La configuración de conexión no se puede descifrar con esta cuenta de Windows. Vuelve a configurarla.", ex);
        }
    }

    internal static void Guardar(string cadena)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(cadena);

        string ruta = Ruta;
        string? carpeta = Path.GetDirectoryName(ruta);
        if (string.IsNullOrWhiteSpace(carpeta))
            throw new InvalidOperationException("No se encontró la carpeta de configuración del usuario.");
        Directory.CreateDirectory(carpeta);

        byte[] claro = Encoding.UTF8.GetBytes(cadena);
        byte[] cifrado;
        try { cifrado = ProtectedData.Protect(claro, optionalEntropy: null, DataProtectionScope.CurrentUser); }
        finally { CryptographicOperations.ZeroMemory(claro); }

        string temporal = ruta + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            File.WriteAllBytes(temporal, cifrado);
            File.Move(temporal, ruta, overwrite: true);
        }
        finally
        {
            CryptographicOperations.ZeroMemory(cifrado);
            if (File.Exists(temporal)) File.Delete(temporal);
        }
    }
}
