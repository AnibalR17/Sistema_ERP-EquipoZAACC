namespace Sistema_ERP;

public partial class Pagina_Central
{
    private readonly System.Windows.Forms.Timer esperaBusqueda = new() { Interval = 400 };
    private CancellationTokenSource? cancelacionBusqueda;
    private PantallaBusquedaGlobal? pantallaBusqueda;
    private int versionBusqueda;
    private int? tiendaBusqueda;
    private bool abriendoDetalleBusqueda;

    private void ConfigurarBusquedaGlobal()
    {
        searchBox.MaxLength = 150;
        searchBox.PlaceholderText = "Buscar en todo el ERP… (Ctrl+K)";
        searchBox.AccessibleName = "Búsqueda global del ERP";
        searchBox.TextChanged += (_, _) => PrepararBusqueda();
        searchBox.KeyDown += async (_, e) =>
        {
            if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; await BuscarGlobal(); }
        };
        esperaBusqueda.Tick += async (_, _) => await BuscarGlobal();
        TiendaSeleccionadaChanged += (_, _) =>
        {
            int? seleccion = IdTiendaSeleccionada;
            if (seleccion == tiendaBusqueda) return;
            tiendaBusqueda = seleccion;
            CancelarBusqueda();
            if (pantallaBusqueda is { IsDisposed: false }) pantallaBusqueda.MostrarEstado("Cambió la tienda. Vuelve a buscar desde la barra superior.");
            if (ActiveMdiChild == pantallaBusqueda || searchBox.Focused) PrepararBusqueda();
        };
        FormClosed += (_, _) => { CancelarBusqueda(); esperaBusqueda.Dispose(); };
    }

    private void CancelarBusqueda()
    {
        versionBusqueda++; esperaBusqueda.Stop(); cancelacionBusqueda?.Cancel();
    }

    private void PrepararBusqueda()
    {
        CancelarBusqueda();
        if (pantallaBusqueda is { IsDisposed: false })
            pantallaBusqueda.MostrarEstado(searchBox.Text.Trim().Length < 2
                ? "Escribe al menos 2 caracteres en la barra superior para buscar." : "Preparando búsqueda…");
        if (acceso is not null && searchBox.Text.Trim().Length >= 2) esperaBusqueda.Start();
    }

    private async Task BuscarGlobal()
    {
        CancelarBusqueda();
        if (IsDisposed || Disposing || acceso is null || searchBox.Text.Trim().Length < 2) return;
        int version = versionBusqueda;
        string texto = searchBox.Text.Trim(), alcance = NombreTiendaSeleccionada;
        int? tienda = IdTiendaSeleccionada;
        tiendaBusqueda = tienda;
        var cancelacion = new CancellationTokenSource(); cancelacionBusqueda = cancelacion;
        try
        {
            MostrarPantalla("Búsqueda global", null, () =>
            {
                pantallaBusqueda = new PantallaBusquedaGlobal();
                pantallaBusqueda.VerDetalle += async resultado => await VerDetalleBusqueda(resultado);
                pantallaBusqueda.AbrirModulo += AbrirModuloBusqueda;
                pantallaBusqueda.FormClosed += (_, _) => CancelarBusqueda();
                return pantallaBusqueda;
            });
            if (pantallaBusqueda is null || pantallaBusqueda.IsDisposed) return;
            pantallaBusqueda.MostrarEstado($"Buscando «{texto}»…");
            searchBox.Focus(); searchBox.SelectionStart = searchBox.TextLength;
            var resultados = await Task.Run(() => BusquedaGlobalDatos.Buscar(texto, tienda, cancelacion.Token), cancelacion.Token);
            if (IsDisposed || Disposing || version != versionBusqueda || pantallaBusqueda.IsDisposed) return;
            pantallaBusqueda.MostrarResultados(texto, alcance, resultados);
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            if (!IsDisposed && version == versionBusqueda && pantallaBusqueda is { IsDisposed: false })
                pantallaBusqueda.MostrarEstado("No se pudo completar la búsqueda. Pulsa Enter para reintentar. " + ex.Message);
        }
        finally
        {
            if (ReferenceEquals(cancelacionBusqueda, cancelacion)) cancelacionBusqueda = null;
            cancelacion.Dispose();
        }
    }

    private async Task VerDetalleBusqueda(ResultadoBusqueda resultado)
    {
        if (abriendoDetalleBusqueda) return;
        abriendoDetalleBusqueda = true;
        pantallaBusqueda?.MarcarOcupado(true);
        try
        {
            using var datos = await Task.Run(() => BusquedaGlobalDatos.ObtenerDetalle(resultado));
            if (IsDisposed || Disposing) return;
            using var dialogo = new DetalleBusquedaGlobal(resultado, datos);
            if (dialogo.ShowDialog(this) == DialogResult.OK) AbrirModuloBusqueda(resultado);
        }
        catch (Exception ex)
        {
            if (!IsDisposed) MessageBox.Show(this, ex.Message, "Resultado de búsqueda", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        finally
        {
            abriendoDetalleBusqueda = false;
            if (pantallaBusqueda is { IsDisposed: false }) pantallaBusqueda.MarcarOcupado(false);
        }
    }

    private void AbrirModuloBusqueda(ResultadoBusqueda resultado)
    {
        try
        {
            AutorizacionDatos.ExigirPantalla(resultado.Pantalla);
            switch (resultado.Pantalla)
            {
                case "Inicio": MostrarInicio(); break;
                case "Punto de Venta": salesButton_Click(this, EventArgs.Empty); break;
                case "Inventario y movimientos": MostrarInventario(); break;
                case "Compras": comprasButton_Click(this, EventArgs.Empty); break;
                case "Proveedores": MostrarPantalla("Proveedores", proveedoresButton, () => new Vistas.PantallaProveedores()); break;
                case "Traspasos": MostrarTraspaso(); break;
                case "Validacion": MostrarValidacion(); break;
                case "Carga masiva": MostrarPantalla("Carga masiva", bulkButton, () => new CargaMasiva()); break;
                case "Catalogo": catalogButton_Click(this, EventArgs.Empty); break;
                case "Apartados y devoluciones": returnsButton_Click(this, EventArgs.Empty); break;
                case "Directorio y roles": rolesButton_Click(this, EventArgs.Empty); break;
                case "Usuarios, roles y permisos": seguridadButton_Click(this, EventArgs.Empty); break;
                case "Configuración y auditoría": settingsButton_Click(this, EventArgs.Empty); break;
                default: throw new UnauthorizedAccessException("El destino de búsqueda no está disponible.");
            }
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "Búsqueda global", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
    }

    private void RevisarPermisosBusqueda(PermisosUsuario actuales)
    {
        if (BusquedaGlobalDatos.Destinos.Any(d => BusquedaGlobalDatos.PuedeVer(permisos, d.Pantalla) != BusquedaGlobalDatos.PuedeVer(actuales, d.Pantalla)))
        {
            CancelarBusqueda();
            if (pantallaBusqueda is { IsDisposed: false }) pantallaBusqueda.MostrarEstado("Tus permisos cambiaron. Vuelve a buscar.");
        }
        foreach (var dialogo in OwnedForms.OfType<DetalleBusquedaGlobal>().ToArray())
            if (!BusquedaGlobalDatos.PuedeVer(actuales, dialogo.Resultado.Pantalla)) dialogo.Dispose();
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == (Keys.Control | Keys.K)) { searchBox.Focus(); searchBox.SelectAll(); return true; }
        if (keyData == Keys.Escape && searchBox.Focused) { searchBox.Clear(); return true; }
        return base.ProcessCmdKey(ref msg, keyData);
    }
}
