using System.Data;
using Microsoft.Data.SqlClient;

namespace Sistema_ERP
{
    public sealed partial class UsuariosRolesPermisos : Form
    {
        private TipoSeguridad tipo;
        private DataView? vista;
        private int? idSeleccionado;
        private byte[]? version;
        private bool activo, cargando, disponible;
        private sealed record Opcion(int Id, string Nombre, bool Activo)
        {
            public override string ToString() => Nombre;
        }

        public UsuariosRolesPermisos()
        {
            InitializeComponent();
            foreach (Button boton in acciones.Controls) { ProductoEstilo.Boton(boton, boton == guardar); boton.Width = 110; }
            usuarios.Click += (_, _) => CambiarPestana(TipoSeguridad.Usuarios);
            roles.Click += (_, _) => CambiarPestana(TipoSeguridad.Roles);
            permisos.Click += (_, _) => CambiarPestana(TipoSeguridad.Permisos);
            guardar.Click += (_, _) => Guardar(false);
            actualizar.Click += (_, _) => Guardar(true);
            nuevo.Click += (_, _) => Ejecutar(() => Limpiar());
            recargar.Click += (_, _) => Cargar();
            desactivar.Click += (_, _) => CambiarEstado(false);
            reactivar.Click += (_, _) => CambiarEstado(true);
            buscar.TextChanged += (_, _) => Filtrar();
            inactivos.CheckedChanged += (_, _) => { if (!cargando) Cargar(); };
            tabla.SelectionChanged += (_, _) => { if (!cargando) Ejecutar(Seleccionar); };
            ConfigurarCampos();
            Botones();
            PermisosInterfaz.Configurar(this, Modulos.Seguridad, guardar, actualizar, nuevo, desactivar, reactivar);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!ModoDiseno.Activo(this)) Cargar();
        }

        private void CambiarPestana(TipoSeguridad siguiente)
        {
            cargando = true;
            try
            {
                tipo = siguiente; vista = null; tabla.DataSource = null;
                buscar.Clear(); inactivos.Checked = false;
                ConfigurarCampos();
            }
            finally { cargando = false; }
            Cargar();
        }

        private void Cargar()
        {
            if (ModoDiseno.Activo(this)) return;
            Ejecutar(() =>
            {
                cargando = true;
                try
                {
                    disponible = false;
                    if (tipo == TipoSeguridad.Usuarios)
                    {
                        empleado.DisplayMember = "Nombre"; empleado.ValueMember = "Id";
                        empleado.DataSource = SeguridadDatos.Empleados();
                    }
                    vista = SeguridadDatos.Listar(tipo, inactivos.Checked).DefaultView;
                    tabla.DataSource = vista;
                    disponible = true;
                    Limpiar(); Filtrar();
                }
                catch
                {
                    disponible = false; vista = null; tabla.DataSource = null;
                    idSeleccionado = null; version = null; contrasena.Clear(); confirmar.Clear();
                    contador.Text = "No se pudieron cargar los registros";
                    throw;
                }
                finally { cargando = false; Botones(); }
            });
        }

        private void Limpiar()
        {
            disponible = false;
            tabla.ClearSelection(); idSeleccionado = null; version = null; activo = false;
            primero.Clear(); segundo.Clear(); descripcion.Clear(); contrasena.Clear(); confirmar.Clear();
            empleado.SelectedIndex = -1; estado.Text = "";
            CargarAsignaciones(); disponible = true; Botones();
        }

        private void Seleccionar()
        {
            if (tabla.SelectedRows.Count != 1 || tabla.SelectedRows[0].DataBoundItem is not DataRowView fila)
            {
                if (idSeleccionado.HasValue)
                {
                    disponible = false;
                    contrasena.Clear(); confirmar.Clear();
                    Mensaje("Selecciona un registro o pulsa Nuevo para continuar.");
                }
                idSeleccionado = null; version = null; Botones(); return;
            }
            idSeleccionado = Convert.ToInt32(fila["Id"]);
            version = (byte[])fila["version_fila"]; activo = Convert.ToBoolean(fila["activo"]);
            primero.Text = Convert.ToString(fila["Primero"]); segundo.Text = Convert.ToString(fila["Segundo"]);
            descripcion.Text = Convert.ToString(fila["Descripcion"]);
            contrasena.Clear(); confirmar.Clear();
            if (tipo == TipoSeguridad.Usuarios) empleado.SelectedValue = fila["id_empleado"];
            disponible = false;
            Botones();
            CargarAsignaciones();
            disponible = true;
            estado.Text = ""; Botones();
        }

        private void CargarAsignaciones()
        {
            asignaciones.Items.Clear();
            if (tipo == TipoSeguridad.Permisos) return;
            foreach (DataRow fila in SeguridadDatos.Asignaciones(tipo, idSeleccionado).Rows)
                asignaciones.Items.Add(new Opcion((int)fila["Id"], (string)fila["Nombre"], (bool)fila["activo"]), (bool)fila["Asignado"]);
        }

        private void Guardar(bool editar)
        {
            if (!disponible || (editar && (!idSeleccionado.HasValue || !activo))) return;
            Ejecutar(() =>
            {
                if (tipo == TipoSeguridad.Usuarios && contrasena.Text != confirmar.Text)
                    throw new ArgumentException("Las contraseñas no coinciden.");
                SeguridadDatos.Guardar(tipo, editar ? idSeleccionado : null, editar ? version : null,
                    primero.Text, segundo.Text, descripcion.Text, empleado.SelectedValue as int?, contrasena.Text,
                    asignaciones.CheckedItems.Cast<Opcion>().Select(o => o.Id).ToArray());
                contrasena.Clear(); confirmar.Clear();
                Cargar();
                if (disponible) Mensaje(editar ? "Registro actualizado correctamente." : "Registro guardado correctamente.");
            });
        }

        private void CambiarEstado(bool activar)
        {
            if (!disponible || idSeleccionado is null || version is null) return;
            string motivo = "";
            if (!activar)
            {
                using var dialogo = new MotivoDesactivacion(primero.Text, tipo.ToString().ToLowerInvariant());
                if (dialogo.ShowDialog(this) != DialogResult.OK) return;
                motivo = dialogo.Motivo;
            }
            Ejecutar(() =>
            {
                SeguridadDatos.CambiarEstado(tipo, idSeleccionado.Value, version, activar, motivo);
                Cargar();
                if (disponible) Mensaje(activar ? "Registro reactivado correctamente." : "Registro desactivado correctamente.");
            });
        }

        private void Filtrar()
        {
            if (vista is null) return;
            string texto = buscar.Text.Trim();
            string literal = texto.Replace("'", "''").Replace("[", "[[]").Replace("%", "[%]").Replace("*", "[*]");
            vista.RowFilter = $"Primero LIKE '%{literal}%' OR Segundo LIKE '%{literal}%' OR Descripcion LIKE '%{literal}%'";
            contador.Text = $"{vista.Count} registros";
        }

        private void Botones()
        {
            guardar.Enabled = disponible && !idSeleccionado.HasValue;
            actualizar.Enabled = desactivar.Enabled = disponible && idSeleccionado.HasValue && activo;
            reactivar.Enabled = disponible && idSeleccionado.HasValue && !activo;
            bool editable = disponible && (!idSeleccionado.HasValue || activo);
            foreach (var campo in new[] { primero, segundo, descripcion, contrasena, confirmar }) campo.ReadOnly = !editable;
            empleado.Enabled = asignaciones.Enabled = editable;
        }

        private void Ejecutar(Action accion)
        {
            try { accion(); }
            catch (SqlException ex) when (ex.Number is 2601 or 2627) { Mensaje("El usuario, empleado, código o combinación de módulo y acción ya está registrado, incluso entre los inactivos.", true); }
            catch (Exception ex) { Mensaje(ex.Message, true); }
            finally { Botones(); }
        }

        private void Mensaje(string texto, bool error = false)
        {
            estado.Text = texto;
            estado.ForeColor = error ? ProductoEstilo.Error : ProductoEstilo.Exito;
        }
    }
}
