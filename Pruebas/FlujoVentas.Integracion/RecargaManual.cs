using System.Data;
using System.Reflection;
using Sistema_ERP;
using Sistema_ERP.Vistas;

internal static partial class Program
{
    private static void ProbarRecargaManual()
    {
        const BindingFlags instancia = BindingFlags.Instance | BindingFlags.NonPublic;
        int comprobaciones = 0;
        void Comprobar(bool condicion, string contexto) { Igual(true, condicion, contexto); comprobaciones++; }
        T Campo<T>(object objeto, string nombre) => (T)objeto.GetType().GetField(nombre, instancia)!.GetValue(objeto)!;
        IEnumerable<Control> Controles(Control control) => new[] { control }.Concat(control.Controls.Cast<Control>().SelectMany(Controles));
        Button Reiniciar(Control pantalla) => Controles(pantalla).OfType<Button>().Single(b => b.Visible && b.Text == "Reiniciar");
        void Esperar(Func<bool> completada)
        {
            DateTime limite = DateTime.UtcNow.AddSeconds(30);
            while (!completada() && DateTime.UtcNow < limite) { Application.DoEvents(); Thread.Sleep(10); }
            if (!completada()) throw new TimeoutException("La recarga manual no terminó.");
        }
        Form Mostrar(Control pantalla)
        {
            var ventana = pantalla as Form ?? new Form();
            if (pantalla is not Form) { pantalla.Dock = DockStyle.Fill; ventana.Controls.Add(pantalla); }
            ventana.Size = new Size(1400, 950); ventana.ShowInTaskbar = false; ventana.Opacity = 0;
            ventana.StartPosition = FormStartPosition.Manual; ventana.Location = new Point(-5000, -5000); ventana.Show();
            return ventana;
        }
        bool Tiene(DataGridView tabla, string columna, string texto) => tabla.Rows.Cast<DataGridViewRow>()
            .Any(r => r.DataBoundItem is DataRowView fila && Convert.ToString(fila[columna]) == texto);

        using (var pantalla = new PantallaProveedores())
        using (Mostrar(pantalla))
        {
            var buscar = Campo<TextBox>(pantalla, "txtBuscar"); buscar.Text = "Recarga";
            Ejecutar("INSERT dbo.Proveedor(nombre_empresa) VALUES(N'Recarga proveedor')");
            Reiniciar(pantalla).PerformClick();
            Comprobar(Tiene(Campo<DataGridView>(pantalla, "dgvProveedores"), "nombre_empresa", "Recarga proveedor") && buscar.Text == "Recarga",
                "Reiniciar proveedores consulta las altas externas y conserva la búsqueda");
        }
        using (var pantalla = new PantallaClientes())
        using (Mostrar(pantalla))
        {
            Ejecutar("INSERT dbo.Cliente(nombre) VALUES(N'Recarga cliente')");
            Reiniciar(pantalla).PerformClick();
            Comprobar(Tiene(Campo<DataGridView>(pantalla, "dgvClientes"), "nombre", "Recarga cliente"), "Reiniciar clientes consulta los datos actuales");
        }
        using (var pantalla = new Catalogo_Productos())
        using (Mostrar(pantalla))
        {
            Ejecutar($"INSERT dbo.Producto(codigo_barras,nombre,precio_unitario,id_categoria,id_proveedor) VALUES('RECARGA-PRODUCTO',N'Recarga producto',15,{categoria},{proveedor})");
            Reiniciar(pantalla).PerformClick();
            Comprobar(Tiene(Campo<DataGridView>(pantalla, "dgvProductos"), "codigo_barras", "RECARGA-PRODUCTO"), "El catálogo incorpora productos creados desde otra pantalla");
            Controles(pantalla).OfType<Button>().Single(b => b.Text == "Categorías").PerformClick();
            Ejecutar("INSERT dbo.Categoria(nombre) VALUES(N'Recarga categoría')");
            Reiniciar(pantalla).PerformClick();
            Comprobar(Tiene(Campo<DataGridView>(pantalla, "dgvCategorias"), "nombre", "Recarga categoría"), "La pestaña Categorías tiene su propia recarga");
        }
        using (var pantalla = new PantallaProductos())
        using (Mostrar(pantalla))
        {
            Ejecutar("UPDATE dbo.Producto SET nombre=N'Recarga producto actualizado' WHERE codigo_barras='RECARGA-PRODUCTO'");
            Reiniciar(pantalla).PerformClick();
            Comprobar(Tiene(Campo<DataGridView>(pantalla, "dgvProductos"), "nombre", "Recarga producto actualizado"), "La pantalla alternativa de productos también recarga");
        }
        using (var pantalla = new Nuevo_Producto())
        using (Mostrar(pantalla))
        {
            Campo<TextBox>(pantalla, "txtNombre").Text = "Captura pendiente";
            Ejecutar("INSERT dbo.Categoria(nombre) VALUES(N'Recarga categoría nueva')");
            Reiniciar(pantalla).PerformClick();
            Comprobar(Campo<ComboBox>(pantalla, "cmbCategoria").Items.Cast<DataRowView>().Any(f => (string)f["nombre"] == "Recarga categoría nueva") &&
                Campo<TextBox>(pantalla, "txtNombre").Text == "Captura pendiente", "Nuevo producto actualiza sus catálogos sin borrar la captura");
        }
        using (var pantalla = new DirectorioRoles())
        using (Mostrar(pantalla))
        {
            Ejecutar("INSERT dbo.Cliente(nombre) VALUES(N'Recarga directorio')");
            Reiniciar(pantalla).PerformClick();
            Comprobar(Tiene(Campo<DataGridView>(pantalla, "tabla"), "Nombre", "Recarga directorio"), "El directorio recarga la pestaña seleccionada");
        }
        using (var pantalla = new UsuariosRolesPermisos())
        using (Mostrar(pantalla))
        {
            foreach (TipoSeguridad tipo in new[] { TipoSeguridad.Usuarios, TipoSeguridad.Roles })
            {
                typeof(UsuariosRolesPermisos).GetMethod("CambiarPestana", instancia)!.Invoke(pantalla, [tipo]);
                var tabla = Campo<DataGridView>(pantalla, "tabla"); object? anterior = tabla.DataSource;
                Reiniciar(pantalla).PerformClick();
                Comprobar(tabla.DataSource is not null && !ReferenceEquals(anterior, tabla.DataSource), "Reiniciar está operativo en " + tipo);
            }
        }
        using (var pantalla = new EditorPermisosRol())
        using (Mostrar(pantalla))
        {
            var tabla = Campo<DataGridView>(pantalla, "tabla"); object? anterior = tabla.DataSource;
            Reiniciar(pantalla).PerformClick();
            Comprobar(tabla.DataSource is not null && !ReferenceEquals(anterior, tabla.DataSource), "El editor de permisos desde Roles recarga su catálogo");
        }
        using (var pantalla = new ConfiguracionAuditoria())
        using (Mostrar(pantalla))
        {
            Ejecutar("INSERT dbo.Tienda(codigo,nombre) VALUES('RECARGA',N'Recarga tienda')");
            Reiniciar(pantalla).PerformClick();
            Comprobar(Tiene(Campo<DataGridView>(pantalla, "tiendas"), "Nombre", "Recarga tienda"), "Configuración recarga las tiendas");
            var tabs = pantalla.Controls.Find("pestanasConfiguracion", true).OfType<TabControl>().Single();
            tabs.SelectedIndex = 1; Reiniciar(pantalla).PerformClick();
            Comprobar(pantalla.Controls.Find("tiendaCAI", true).OfType<ComboBox>().Single().Items.Cast<DataRowView>().Any(f => (string)f["nombre"] == "Recarga tienda"), "Facturación tiene recarga de tiendas y CAI");
            tabs.SelectedIndex = 2; Reiniciar(pantalla).PerformClick();
            Comprobar(Campo<DataGridView>(pantalla, "tablaAuditoria").Rows.Count > 0, "Auditoría recarga desde la primera página");
        }
        using (var pantalla = new PantallaCompras())
        using (Mostrar(pantalla))
        {
            Esperar(() => !Campo<bool>(pantalla, "ocupado"));
            Ejecutar("INSERT dbo.Proveedor(nombre_empresa) VALUES(N'Recarga compras')");
            Reiniciar(pantalla).PerformClick();
            Esperar(() => !Campo<bool>(pantalla, "ocupado"));
            Comprobar(Campo<ComboBox>(pantalla, "cmbProveedor").Items.Cast<DataRowView>().Any(f => (string)f["nombre"] == "Recarga compras"), "Compras vuelve a cargar sus catálogos");
        }
        using (var pantalla = new Inventario())
        using (Mostrar(pantalla))
        {
            Esperar(() => !Campo<bool>(pantalla, "cargandoInventario"));
            Reiniciar(pantalla).PerformClick();
            Esperar(() => !Campo<bool>(pantalla, "cargandoInventario"));
            Comprobar(Campo<DataGridView>(pantalla, "existenciasInventario").DataSource is not null && Campo<DataGridView>(pantalla, "historialInventario").DataSource is not null,
                "Inventario recarga existencias, movimientos y tiendas");
        }
        using (var pantalla = new PuntoVenta())
        using (Mostrar(pantalla))
        {
            Campo<ComboBox>(pantalla, "tienda").SelectedValue = tienda;
            Campo<ComboBox>(pantalla, "cliente").SelectedValue = cliente;
            var carrito = Campo<DataTable>(pantalla, "detalle"); carrito.Rows.Add(producto, "Captura", 1, 10m);
            Campo<TextBox>(pantalla, "observaciones").Text = "Pendiente";
            Ejecutar("INSERT dbo.Cliente(nombre) VALUES(N'Recarga facturación')");
            Reiniciar(pantalla).PerformClick();
            Comprobar(carrito.Rows.Count == 1 && (int)Campo<ComboBox>(pantalla, "tienda").SelectedValue! == tienda && Campo<TextBox>(pantalla, "observaciones").Text == "Pendiente",
                "Facturación conserva el carrito, la tienda y la captura al recargar");
            Comprobar(Campo<ComboBox>(pantalla, "cliente").Items.Cast<DataRowView>().Any(f => (string)f["nombre"] == "Recarga facturación"), "Facturación incorpora los clientes actuales");
        }
        using (var pantalla = new ApartadosDevoluciones())
        using (Mostrar(pantalla))
        {
            var tabs = Controles(pantalla).OfType<TabControl>().Single();
            for (int i = 0; i < tabs.TabPages.Count; i++)
            {
                tabs.SelectedIndex = i;
                var tabla = Campo<DataGridView>(pantalla, i == 0 ? "apartados" : "devoluciones"); object? anterior = tabla.DataSource;
                Reiniciar(pantalla).PerformClick();
                Comprobar(tabla.DataSource is not null && !ReferenceEquals(anterior, tabla.DataSource), "La pestaña " + tabs.SelectedTab!.Text + " permite recarga manual");
            }
        }
        using (var pantalla = new CargaMasiva())
        using (Mostrar(pantalla))
        {
            Ejecutar("INSERT dbo.Tienda(codigo,nombre) VALUES('RECARGA-CARGA',N'Recarga importación')");
            Reiniciar(pantalla).PerformClick();
            Comprobar(Campo<ComboBox>(pantalla, "cmbTiendaImportacion").Items.Cast<DataRowView>().Any(f => (string)f["nombre"] == "Recarga importación"), "Carga masiva actualiza las tiendas");
        }
        using var roles = SeguridadDatos.Asignaciones(TipoSeguridad.Usuarios, null);
        using var permisos = SeguridadDatos.PermisosPorRol();
        int seleccionado = (int)roles.Rows[0]["Id"];
        using (var pantalla = new GenerarPermisosUsuario("Recarga", roles, permisos, [seleccionado]))
        using (Mostrar(pantalla))
        {
            Ejecutar("INSERT dbo.Rol(codigo,nombre) VALUES('RECARGA_ROL',N'Recarga rol')");
            Reiniciar(pantalla).PerformClick();
            Comprobar(Campo<CheckedListBox>(pantalla, "listaRoles").Items.Cast<object>().Any(r => r.ToString() == "Recarga rol") && pantalla.RolesSeleccionados.Contains(seleccionado),
                "La ventana de permisos de usuario actualiza los roles sin perder la selección");
        }
        using var catalogo = SeguridadDatos.Asignaciones(TipoSeguridad.Roles, null);
        int permisoSeleccionado = (int)catalogo.Rows[0]["Id"];
        using (var pantalla = new AsignarPermisosRol("Recarga", catalogo, [permisoSeleccionado]))
        using (Mostrar(pantalla))
        {
            Ejecutar("INSERT dbo.Permiso(modulo,accion,descripcion) VALUES('RECARGA_MODULO','CONSULTAR',N'Recarga permiso')");
            Reiniciar(pantalla).PerformClick();
            Comprobar(Campo<ListBox>(pantalla, "listaModulos").Items.Contains("RECARGA_MODULO") && pantalla.PermisosSeleccionados.Contains(permisoSeleccionado),
                "La ventana de permisos de rol actualiza los módulos sin perder la selección");
        }
        using (var pantalla = new AgregarDirectorio(TipoDirectorio.Empleados))
        using (Mostrar(pantalla))
        {
            Reiniciar(pantalla).PerformClick();
            Comprobar(Campo<ComboBox>(pantalla, "tienda").Items.Cast<DataRowView>().Any(f => (string)f["nombre"] == "Recarga importación"), "Agregar empleado permite recargar sus tiendas");
        }
        foreach (UserControl pantalla in new UserControl[] { new Traspaso_Tiendas(), new Validacion_Traspasos() })
        using (pantalla)
        using (Mostrar(pantalla))
        {
            var boton = Reiniciar(pantalla); boton.PerformClick();
            Comprobar(boton.Enabled, pantalla.GetType().Name + " permite recargar sin cerrar la pantalla");
        }
        Console.WriteLine($"OK: {comprobaciones} comprobaciones de recarga manual, pestañas, catálogos y conservación de captura.");
    }
}
