using System.Reflection;
using Sistema_ERP;
using Sistema_ERP.Vistas;

internal static partial class Program
{
    private sealed class PrincipalSinConsultas : Pagina_Central
    {
        protected override void OnShown(EventArgs e) { }
    }

    private sealed class ComprasSinConsultas : PantallaCompras
    {
        internal int AvisosCierrePendiente { get; private set; }
        protected override void OnLoad(EventArgs e) { }
        protected override void MostrarCierrePendiente() => AvisosCierrePendiente++;
    }

    private static void ProbarCierreSesion()
    {
        const BindingFlags instancia = BindingFlags.Instance | BindingFlags.NonPublic;
        foreach (bool cerrarSesion in new[] { false, true })
        foreach (bool consultaEnCurso in new[] { false, true })
        {
            using var principal = new PrincipalSinConsultas
            {
                ShowInTaskbar = false, Opacity = 0, StartPosition = FormStartPosition.Manual,
                Location = new Point(-10000, -10000)
            };
            principal.Show();
            using var compras = new ComprasSinConsultas { MdiParent = principal };
            compras.Show();
            typeof(PantallaCompras).GetField("ocupado", instancia)!.SetValue(compras, consultaEnCurso);
            using var otroModulo = new FormularioTematico { MdiParent = principal, Text = "Otro módulo" };
            otroModulo.Show();
            bool cerrada = false;
            principal.FormClosed += (_, _) => cerrada = true;
            if (cerrarSesion) principal.Controls.Find("logoutButton", true).OfType<Button>().Single().PerformClick();
            else principal.Close();
            Igual(true, cerrada, $"{(cerrarSesion ? "Cerrar sesión" : "Cerrar ventana")} funciona con consulta de Compras {(consultaEnCurso ? "pendiente" : "inactiva")}");
            if (cerrarSesion)
                Igual(true, (bool)typeof(Pagina_Central).GetProperty("VolverAlAcceso", instancia)!.GetValue(principal)!,
                    "Cerrar sesión solicita volver al acceso");
        }
        using (var principal = new PrincipalSinConsultas
        {
            ShowInTaskbar = false, Opacity = 0, StartPosition = FormStartPosition.Manual,
            Location = new Point(-10000, -10000)
        })
        {
            principal.Show();
            using var compras = new ComprasSinConsultas { MdiParent = principal };
            compras.Show();
            using var terminar = new ManualResetEventSlim();
            Action escritura = () =>
            {
                if (!terminar.Wait(TimeSpan.FromSeconds(10))) throw new TimeoutException("La prueba no liberó la escritura.");
                throw new InvalidOperationException("Fallo simulado al guardar una compra.");
            };
            var tarea = (Task)typeof(PantallaCompras).GetMethod("CambiarEstado", instancia)!
                .Invoke(compras, [escritura, "CONFIRMADA", "Compra confirmada."])!;
            var cerrar = principal.Controls.Find("logoutButton", true).OfType<Button>().Single();
            cerrar.PerformClick();
            Igual(false, principal.IsDisposed, "Se espera a que termine una escritura de Compras");
            Igual(1, compras.AvisosCierrePendiente, "El cierre bloqueado por una escritura informa al usuario");
            Igual(false, (bool)typeof(Pagina_Central).GetProperty("VolverAlAcceso", instancia)!.GetValue(principal)!,
                "Un cierre cancelado conserva la sesión actual");
            terminar.Set();
            DateTime limite = DateTime.UtcNow.AddSeconds(5);
            while (!tarea.IsCompleted && DateTime.UtcNow < limite) { Application.DoEvents(); Thread.Sleep(10); }
            Igual(true, tarea.IsCompleted, "La operación termina y libera el cierre incluso si falla");
            tarea.GetAwaiter().GetResult();
            cerrar.PerformClick();
            Igual(true, principal.IsDisposed, "Al terminar la escritura se puede volver a cerrar sesión");
        }
        Console.WriteLine("OK: cierre por X y por botón, consultas en segundo plano, escrituras pendientes y solicitud de regreso al acceso, sin SQL.");
    }
}
