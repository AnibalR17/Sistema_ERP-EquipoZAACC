namespace Sistema_ERP
{
    public sealed partial class UsuariosRolesPermisos
    {
        private void ConfigurarCampos()
        {
            bool usuario = tipo == TipoSeguridad.Usuarios;
            bool rol = tipo == TipoSeguridad.Roles;
            foreach (var (boton, valor) in new[] { (usuarios, TipoSeguridad.Usuarios), (roles, TipoSeguridad.Roles), (permisos, TipoSeguridad.Permisos) })
            {
                ProductoEstilo.Boton(boton, tipo == valor); boton.Width = 140;
            }
            etiquetaPrimero.Text = usuario ? "Usuario *" : rol ? "Código *" : "Módulo *";
            etiquetaSegundo.Text = rol ? "Nombre *" : "Acción *";
            etiquetaDescripcion.Text = rol ? "Descripción" : "Descripción *";
            etiquetaEmpleado.Text = "Empleado *";
            etiquetaContrasena.Text = "Contraseña * (nueva: 12–128 caracteres)";
            etiquetaConfirmar.Text = "Confirmar contraseña *";
            primero.MaxLength = rol ? 30 : 50; segundo.MaxLength = rol ? 80 : 30;
            segundo.Visible = etiquetaSegundo.Visible = descripcion.Visible = etiquetaDescripcion.Visible = !usuario;
            empleado.Visible = etiquetaEmpleado.Visible = contrasena.Visible = etiquetaContrasena.Visible = confirmar.Visible = etiquetaConfirmar.Visible = usuario;
            campos.RowStyles[2].Height = usuario ? 26 : 0; campos.RowStyles[3].Height = usuario ? 46 : 0;
            etiquetaAsignaciones.Visible = asignaciones.Visible = tipo != TipoSeguridad.Permisos;
            etiquetaAsignaciones.Text = usuario ? "Roles asignados" : "Permisos asignados";
            aviso.Text = usuario
                ? "Selecciona un empleado. Al actualizar, deja la contraseña vacía para conservarla. Las cuentas nuevas deben cambiarla al ingresar."
                : "Selecciona una fila para editar. Desactivar conserva el registro y sus relaciones.";
            tabla.Columns.Clear();
            void Columna(string propiedad, string texto) => tabla.Columns.Add(new DataGridViewTextBoxColumn { Name = propiedad, DataPropertyName = propiedad, HeaderText = texto, MinimumWidth = 100 });
            Columna("Primero", usuario ? "Usuario" : rol ? "Código" : "Módulo");
            Columna("Segundo", usuario ? "ID empleado" : rol ? "Nombre" : "Acción");
            if (!usuario) Columna("Descripcion", "Descripción");
            tabla.Columns.Add(new DataGridViewCheckBoxColumn { Name = "activo", DataPropertyName = "activo", HeaderText = "Activo", FillWeight = 45 });
            Columna("motivo_desactivacion", "Motivo de desactivación");
        }
    }
}
