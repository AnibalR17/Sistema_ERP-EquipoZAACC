namespace Sistema_ERP
{
    public sealed partial class UsuariosRolesPermisos
    {
        private void ConfigurarCampos()
        {
            bool usuario = tipo == TipoSeguridad.Usuarios;
            bool rol = tipo == TipoSeguridad.Roles;
            foreach (var (boton, valor) in new[] { (usuarios, TipoSeguridad.Usuarios), (roles, TipoSeguridad.Roles) })
            {
                ProductoEstilo.Boton(boton, tipo == valor); boton.Width = 140;
            }
            etiquetaPrimero.Text = usuario ? "Usuario *" : "Código *";
            etiquetaSegundo.Text = "Nombre *";
            etiquetaDescripcion.Text = "Descripción";
            etiquetaEmpleado.Text = "Empleado *";
            etiquetaContrasena.Text = "Contraseña * (nueva: 12–128 caracteres)";
            etiquetaConfirmar.Text = "Confirmar contraseña *";
            primero.MaxLength = rol ? 30 : 50; segundo.MaxLength = rol ? 80 : 30;
            segundo.Visible = etiquetaSegundo.Visible = descripcion.Visible = etiquetaDescripcion.Visible = !usuario;
            empleado.Visible = etiquetaEmpleado.Visible = contrasena.Visible = etiquetaContrasena.Visible = confirmar.Visible = etiquetaConfirmar.Visible = usuario;
            campos.RowStyles[2].Height = usuario ? 26 : 0; campos.RowStyles[3].Height = usuario ? 46 : 0;
            generarPermisos.Visible = usuario;
            asignarPermisosModulo.Visible = rol;
            editarPermisosRol.Visible = rol;
            etiquetaAsignaciones.Text = usuario ? "Roles asignados" : "Permisos asignados";
            selectorRoles.Visible = usuario;
            asignaciones.Visible = rol;
            contenedorAsignaciones.Height = usuario ? 140 : 90;
            aviso.Text = usuario
                ? "Selecciona un empleado y agrega sus roles desde el desplegable. Generar permisos muestra los accesos combinados. Al actualizar, deja la contraseña vacía para conservarla."
                : "Usa Pantallas y presets para elegir accesos. Crea y modifica permisos desde Crear / editar permisos. Guarda o actualiza el rol para conservar su asignación.";
            tabla.Columns.Clear();
            void Columna(string propiedad, string texto) => tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = propiedad, DataPropertyName = propiedad, HeaderText = texto, MinimumWidth = 100 });
            Columna("Primero", usuario ? "Usuario" : "Código");
            Columna("Segundo", usuario ? "ID empleado" : "Nombre");
            if (!usuario) Columna("Descripcion", "Descripción");
            tabla.Columns.Add(new DataGridViewCheckBoxColumn { Name = "activo", DataPropertyName = "activo", HeaderText = "Activo", FillWeight = 45 });
            Columna("motivo_desactivacion", "Motivo de desactivación");
        }
    }
}
