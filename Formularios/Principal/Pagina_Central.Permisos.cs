namespace Sistema_ERP;

public partial class Pagina_Central
{
    private void AplicarPermisos(PermisosUsuario actuales)
    {
        RevisarPermisosBusqueda(actuales);
        permisos = actuales;
        (Button Boton, string Modulo)[] opciones = [
            (salesButton, Modulos.Ventas), (inventoryButton, Modulos.Inventario),
            (comprasButton, Modulos.Compras), (proveedoresButton, Modulos.Proveedores),
            (transferButton, Modulos.Traspasos), (validationButton, Modulos.Validacion),
            (bulkButton, Modulos.CargaMasiva), (catalogButton, Modulos.Productos),
            (returnsButton, Modulos.Apartados), (rolesButton, Modulos.Directorio),
            (seguridadButton, Modulos.Seguridad), (settingsButton, Modulos.Configuracion)
        ];
        int arriba = adminLabel.Bottom + 4;
        foreach (var (boton, modulo) in opciones)
        {
            boton.Visible = permisos.Puede(modulo);
            if (boton == salesButton) continue;
            boton.Top = arriba;
            if (permisos.Puede(modulo)) arriba += boton.Height;
        }
        menuNavegacion.AutoScrollMinSize = new Size(0, arriba + 12);
        notificationButton.Visible = permisos.Puede(Modulos.Inventario);
        if (permisos.Puede(Modulos.Inventario)) actualizarAlertas?.Start(); else actualizarAlertas?.Stop();
        if (!permisos.Puede(Modulos.Inventario))
            foreach (var dialogo in OwnedForms.OfType<Notificaciones>().ToArray()) dialogo.Dispose();
        selectorTienda.Visible = permisos.PuedeAlguno(Modulos.Ventas, Modulos.Apartados, Modulos.Compras,
            Modulos.Inventario, Modulos.Traspasos, Modulos.Validacion, Modulos.Productos,
            Modulos.CargaMasiva, Modulos.Directorio, Modulos.Configuracion);
        cardPanel0.Visible = permisos.Puede(Modulos.Ventas);
        cardPanel1.Visible = permisos.Puede(Modulos.Compras);
        cardPanel2.Visible = permisos.PuedeAlguno(Modulos.Traspasos, Modulos.Validacion);
        cardPanel3.Visible = permisos.Puede(Modulos.Inventario);
        chartPanel.Visible = permisos.PuedeAlguno(Modulos.Ventas, Modulos.Compras);
        btnprobarconexion.Visible = permisos.Puede(Modulos.Configuracion);
        MostrarInicioSinDatos("Consulta los módulos disponibles en el menú.");
        foreach (var (clave, pantalla) in pantallas.ToArray())
        {
            var modulo = AutorizacionDatos.ModuloPantalla(clave);
            if (modulo is not null && !permisos.Puede(modulo)) RetirarPantalla(clave, pantalla);
        }
        if (Visible && ActiveMdiChild is null && !MdiChildren.Any(p => p.Visible)) MostrarInicio();
    }

    private void RetirarPantalla(string clave, Form pantalla)
    {
        pantallas.Remove(clave);
        navegacion.Remove(pantalla);
        pantalla.Dispose();
    }

    private bool validandoPantalla;

    private void ValidarPantallaActiva()
    {
        if (validandoPantalla || IsDisposed || Disposing || ActiveMdiChild is not Form pantalla) return;
        string? clave = pantallas.FirstOrDefault(p => p.Value == pantalla).Key;
        if (clave is null || clave == "Inicio") return;
        validandoPantalla = true;
        try { AutorizacionDatos.ExigirPantalla(clave); }
        catch (Exception ex)
        {
            RetirarPantalla(clave, pantalla);
            MessageBox.Show(this, "No se pudo autorizar el acceso. " + ex.Message, "Acceso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        finally { validandoPantalla = false; }
    }
}
