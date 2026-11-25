using System.Security.Authentication;
using Microsoft.Data.SqlClient;

namespace Sistema_ERP.Clases;

internal static class DiagnosticoConexion
{
    internal static string Describir(Exception error)
    {
        if (Contiene<AuthenticationException>(error) || ErrorDeCertificado(error))
            return "No se pudo validar el certificado de SQL Server. Revisa el certificado o, solo para pruebas locales, activa la opción de confiar en el certificado.";

        if (Buscar<SqlException>(error) is SqlException sql)
        {
            int[] numeros = sql.Errors.Cast<SqlError>().Select(errorSql => errorSql.Number).ToArray();
            if (numeros.Contains(-2))
                return "Se agotó el tiempo de espera al conectar con SQL Server. Revisa el servidor, la instancia y la red.";
            if (numeros.Any(numero => numero is 18456 or 18452 or 18470))
                return "SQL Server rechazó la autenticación. Revisa el método de acceso, el usuario y la contraseña.";
            if (numeros.Any(numero => numero is 4060 or 911 or 916))
                return "No se pudo abrir la base de datos indicada. Revisa el nombre de la base y los permisos de esa cuenta.";
            if (numeros.Any(numero => numero is 229 or 262 or 15247))
                return "La cuenta pudo llegar a SQL Server, pero no tiene permisos suficientes para verificar la conexión.";
            if (numeros.Any(numero => numero is 2 or 26 or 53 or 40 or 233 or 258 or 10060 or 10061 or 11001))
                return "No se encontró SQL Server o no responde. Revisa el nombre de servidor e instancia, que el servicio esté iniciado y que la red permita el acceso.";

            int numeroError = numeros.FirstOrDefault();
            return numeroError == 0
                ? "SQL Server no pudo completar la conexión. Revisa el servidor, la base de datos, la autenticación y los permisos."
                : $"SQL Server no pudo completar la conexión (error {numeroError}). Revisa el servidor, la base de datos, la autenticación y los permisos.";
        }

        if (error is ArgumentException or FormatException)
            return "Los datos de conexión no tienen un formato válido. Revisa el servidor, la base de datos y la autenticación.";

        if (error is InvalidOperationException)
            return error.Message;

        return "No fue posible validar la conexión con SQL Server. Revisa los datos e inténtalo de nuevo.";
    }

    private static T? Buscar<T>(Exception error) where T : Exception
    {
        for (Exception? actual = error; actual is not null; actual = actual.InnerException)
            if (actual is T coincidencia) return coincidencia;
        return null;
    }

    private static bool Contiene<T>(Exception error) where T : Exception => Buscar<T>(error) is not null;

    private static bool ErrorDeCertificado(Exception error)
    {
        for (Exception? actual = error; actual is not null; actual = actual.InnerException)
        {
            string mensaje = actual.Message;
            if (mensaje.Contains("certificate", StringComparison.OrdinalIgnoreCase)
                || mensaje.Contains("certificado", StringComparison.OrdinalIgnoreCase)
                || mensaje.Contains("SSL Provider", StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }
}
