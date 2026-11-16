namespace Sistema_ERP;

public partial class Pagina_Central
{
    private void AplicarPermisos(PermisosUsuario actuales)
    {
        RevisarPermisosBusqueda(actuales);
        permisos = actuales;
        (Button Boton, string Modulo)[] opciones = [
            (salesButton, Modulos.ventas), (inventoryButton, Modulos.inventario),
            (comprasButton, Modulos.compras), (proveedoresButton, Modulos.proveedores),
            (transferButton, Modulos.traspasos), (validationButton, Modulos.validacion),
            (bulkButton, Modulos.cargaMasiva), (catalogButton, Modulos.productos),
            (returnsButton, Modulos.apartados), (rolesButton, Modulos.directorio),
            (seguridadButton, Modulos.seguridad), (settingsButton, Modulos.configuracion)
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
        notificationButton.Visible = permisos.Puede(Modulos.inventario);
        if (permisos.Puede(Modulos.inventario)) actualizarAlertas?.Start(); else actualizarAlertas?.Stop();
        if (!permisos.Puede(Modulos.inventario))
            foreach (var dialogo in OwnedForms.OfType<Notificaciones>().ToArray()) dialogo.Dispose();
        selectorTienda.Visible = permisos.PuedeAlguno(Modulos.ventas, Modulos.apartados, Modulos.compras,
            Modulos.inventario, Modulos.traspasos, Modulos.validacion, Modulos.productos,
            Modulos.cargaMasiva, Modulos.directorio, Modulos.configuracion);
        cardPanel0.Visible = permisos.Puede(Modulos.ventas);
        cardPanel1.Visible = permisos.Puede(Modulos.compras);
        cardPanel2.Visible = permisos.PuedeAlguno(Modulos.traspasos, Modulos.validacion);
        cardPanel3.Visible = permisos.Puede(Modulos.inventario);
        chartPanel.Visible = permisos.PuedeAlguno(Modulos.ventas, Modulos.compras);
        btnprobarconexion.Visible = permisos.Puede(Modulos.configuracion);
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
