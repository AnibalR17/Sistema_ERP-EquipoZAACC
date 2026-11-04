using System.Data;

namespace Sistema_ERP
{
    public partial class Pagina_Central
    {
        private Task<DataTable>? consultaAlertas;
        private System.Windows.Forms.Timer? actualizarAlertas;
        private ToolTip? ayudaNotificaciones;
        private bool notificacionesAbiertas;

        private void ConfigurarNotificaciones()
        {
            components ??= new System.ComponentModel.Container();
            ayudaNotificaciones = new ToolTip(components);
            actualizarAlertas = new System.Windows.Forms.Timer(components) { Interval = 60000 };
            notificationButton.AccessibleName = "Notificaciones de inventario";
            MostrarIndicadorDesconocido("Las alertas de inventario todavía no se han consultado.");
            Shown += async (_, _) =>
            {
                if (acceso == null || IsDisposed) return;
                await ActualizarAlertasAutomaticamente();
                if (!IsDisposed) actualizarAlertas.Start();
            };
            actualizarAlertas.Tick += async (_, _) => await ActualizarAlertasAutomaticamente();
            FormClosed += (_, _) => actualizarAlertas.Stop();
        }

        private async Task ActualizarAlertasAutomaticamente()
        {
            if (IsDisposed || Disposing || acceso == null || notificacionesAbiertas || consultaAlertas != null) return;
            try { await ConsultarAlertas(); }
            catch (Exception) {  }
        }

        private async Task<DataTable> ConsultarAlertas()
        {
            if (consultaAlertas != null) return await consultaAlertas;
            consultaAlertas = Task.Run(InventarioDatos.AlertasStock);
            try
            {
                var alertas = await consultaAlertas;
                if (!IsDisposed && !Disposing) MostrarIndicadorAlertas(alertas.Rows.Count);
                return alertas;
            }
            catch
            {
                if (!IsDisposed && !Disposing)
                    MostrarIndicadorDesconocido("No se pudo consultar el inventario. Abre Notificaciones para ver el error y reintentar.");
                throw;
            }
            finally { consultaAlertas = null; }
        }

        private void MostrarIndicadorAlertas(int cantidad)
        {
            notificationButton.Text = cantidad > 9 ? "🔔 9+" : $"🔔 {cantidad}";
            string descripcion = cantidad == 0 ? "Sin alertas de stock mínimo."
                : $"{cantidad} alertas de stock mínimo por producto y tienda.";
            notificationButton.AccessibleDescription = descripcion;
            ayudaNotificaciones?.SetToolTip(notificationButton, descripcion);
        }

        private void MostrarIndicadorDesconocido(string descripcion)
        {
            notificationButton.Text = "🔔 ?";
            notificationButton.AccessibleDescription = descripcion;
            ayudaNotificaciones?.SetToolTip(notificationButton, descripcion);
        }

        private void notificationButton_Click(object? sender, EventArgs e)
        {
            if (notificacionesAbiertas || IsDisposed || Disposing) return;
            notificacionesAbiertas = true;
            try
            {
                DialogosMaqueta.Mostrar(this, new Notificaciones(ConsultarAlertas, InventarioDatos.DescripcionAlerta));
            }
            finally { notificacionesAbiertas = false; }
        }
    }
}
