using System.Data;

namespace Sistema_ERP
{
    public sealed partial class Notificaciones : Form
    {
        private readonly Func<Task<DataTable>> cargar;
        private bool cargando;

        internal event Action<DataTable>? Actualizadas;
        internal event Action? ErrorCarga;

        public Notificaciones() : this(
            () => Task.Run(() => InventarioDatos.AlertasStock()), InventarioDatos.DescripcionAlerta) { }

        internal Notificaciones(Func<Task<DataTable>> cargar, string criterio)
        {
            this.cargar = cargar ?? throw new ArgumentNullException(nameof(cargar));
            InitializeComponent();
            descripcion.Text = criterio;
            if (ModoDiseno.Activo(this)) return;
            ProductoEstilo.Boton(actualizar);
            ProductoEstilo.Boton(cerrar, true);
            actualizar.Width = cerrar.Width = 120;
            actualizar.Click += async (_, _) => await CargarAlertas();
            Shown += async (_, _) => await CargarAlertas();
        }

        private async Task CargarAlertas()
        {
            if (cargando || ModoDiseno.Activo(this)) return;
            cargando = true;
            actualizar.Enabled = false;
            estado.ForeColor = ProductoEstilo.TextoSecundario;
            estado.Text = "Consultando existencias por tienda...";
            resumen.Text = "Consultando alertas...";
            tablaAlertas.DataSource = null;
            try
            {
                DataTable datos = await cargar();
                if (IsDisposed || Disposing) return;
                tablaAlertas.DataSource = datos;
                resumen.Text = datos.Rows.Count == 0
                    ? "No hay alertas de stock con el criterio indicado."
                    : $"{datos.Rows.Count} {(datos.Rows.Count == 1 ? "producto por tienda requiere" : "productos por tienda requieren")} revisión.";
                estado.Text = $"Datos consultados: {DateTime.Now:dd/MM/yyyy HH:mm:ss}. Actualizar vuelve a consultar las existencias actuales.";
                Actualizadas?.Invoke(datos);
            }
            catch (Exception ex)
            {
                if (IsDisposed || Disposing) return;
                tablaAlertas.DataSource = null;
                resumen.Text = "No se pudieron consultar las alertas.";
                estado.ForeColor = ProductoEstilo.Error;
                estado.Text = ex.Message;
                ErrorCarga?.Invoke();
            }
            finally
            {
                if (!IsDisposed && !Disposing)
                {
                    cargando = false;
                    actualizar.Enabled = true;
                }
            }
        }
    }
}
