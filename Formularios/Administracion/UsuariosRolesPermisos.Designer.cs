#nullable enable
namespace Sistema_ERP
{
    public sealed partial class UsuariosRolesPermisos
    {
        private System.ComponentModel.IContainer? components;
        private TableLayoutPanel contenido = null!, campos = null!;
        private FlowLayoutPanel pestanas = null!, acciones = null!, filtros = null!, opcionesAsignaciones = null!, accionesAsignaciones = null!;
        private Button usuarios = null!, roles = null!;
        private Button guardar = null!, actualizar = null!, desactivar = null!, reactivar = null!, nuevo = null!, recargar = null!;
        private Button generarPermisos = null!, asignarPermisosModulo = null!, editarPermisosRol = null!;
        private TextBox primero = null!, segundo = null!, descripcion = null!, contrasena = null!, confirmar = null!, buscar = null!;
        private ComboBox empleado = null!;
        private CheckedListBox asignaciones = null!;
        private SelectorRoles selectorRoles = null!;
        private Panel contenedorAsignaciones = null!;
        private Label titulo = null!, aviso = null!, estado = null!, contador = null!;
        private Label etiquetaPrimero = null!, etiquetaSegundo = null!, etiquetaDescripcion = null!, etiquetaEmpleado = null!, etiquetaContrasena = null!, etiquetaConfirmar = null!, etiquetaAsignaciones = null!;
        private CheckBox inactivos = null!;
        private DataGridView tabla = null!;
        private PanelDisenoInventario tarjeta = null!;

        protected override void Dispose(bool disposing)
        {
            if (disposing) components?.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            contenido = new TableLayoutPanel(); campos = new TableLayoutPanel();
            pestanas = new FlowLayoutPanel(); acciones = new FlowLayoutPanel(); filtros = new FlowLayoutPanel();
            opcionesAsignaciones = new FlowLayoutPanel(); accionesAsignaciones = new FlowLayoutPanel();
            generarPermisos = new Button(); asignarPermisosModulo = new Button(); editarPermisosRol = new Button();
            usuarios = new Button(); roles = new Button();
            guardar = new Button(); actualizar = new Button(); desactivar = new Button(); reactivar = new Button(); nuevo = new Button(); recargar = new Button();
            primero = new TextBox(); segundo = new TextBox(); descripcion = new TextBox(); contrasena = new TextBox(); confirmar = new TextBox(); buscar = new TextBox();
            empleado = new ComboBox(); asignaciones = new CheckedListBox();
            selectorRoles = new SelectorRoles();
            contenedorAsignaciones = new Panel();
            titulo = new Label(); aviso = new Label(); estado = new Label(); contador = new Label();
            etiquetaPrimero = new Label(); etiquetaSegundo = new Label(); etiquetaDescripcion = new Label();
            etiquetaEmpleado = new Label(); etiquetaContrasena = new Label(); etiquetaConfirmar = new Label(); etiquetaAsignaciones = new Label();
            inactivos = new CheckBox(); tabla = new DataGridView(); tarjeta = new PanelDisenoInventario();
            SuspendLayout();
            AutoScaleMode = AutoScaleMode.Font;
            Font = new Font("Segoe UI", 11F);
            BackColor = Color.White;
            Text = "Usuarios, roles y permisos";
            ClientSize = new Size(1180, 850);
            AutoScroll = true;
            contenido.Dock = DockStyle.Top;
            contenido.AutoSize = true;
            contenido.Padding = new Padding(32, 34, 32, 36);
            contenido.ColumnCount = 1;
            contenido.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            contenido.RowCount = 10;
            titulo.Text = "Usuarios, roles y permisos";
            titulo.Font = new Font("Segoe UI", 27F, FontStyle.Bold);
            titulo.AutoSize = true;
            titulo.Margin = new Padding(0, 0, 0, 14);
            contenido.Controls.Add(titulo, 0, 0);
            pestanas.AutoSize = true; pestanas.Dock = DockStyle.Fill;
            usuarios.Text = "Usuarios"; roles.Text = "Roles";
            pestanas.Controls.AddRange(new Control[] { usuarios, roles });
            contenido.Controls.Add(pestanas, 0, 1);
            aviso.AutoSize = true;
            aviso.ForeColor = ProductoEstilo.TextoSecundario;
            aviso.Margin = new Padding(0, 14, 0, 14);
            contenido.Controls.Add(aviso, 0, 2);
            campos.Dock = DockStyle.Fill; campos.AutoSize = true;
            campos.ColumnCount = 3; campos.RowCount = 4;
            for (int i = 0; i < 3; i++) campos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333F));
            for (int i = 0; i < 4; i++) campos.RowStyles.Add(new RowStyle(SizeType.Absolute, i % 2 == 0 ? 26 : 46));
            foreach (Label etiqueta in new[] { etiquetaPrimero, etiquetaSegundo, etiquetaDescripcion, etiquetaEmpleado, etiquetaContrasena, etiquetaConfirmar })
            {
                etiqueta.Font = new Font("Segoe UI", 10F);
                etiqueta.AutoSize = true;
            }
            foreach (TextBox campo in new[] { primero, segundo, descripcion, contrasena, confirmar })
            {
                campo.Dock = DockStyle.Fill;
                campo.Margin = new Padding(0, 0, 18, 12);
            }
            descripcion.MaxLength = 300;
            contrasena.MaxLength = confirmar.MaxLength = 128;
            contrasena.UseSystemPasswordChar = confirmar.UseSystemPasswordChar = true;
            empleado.DropDownStyle = ComboBoxStyle.DropDownList;
            empleado.Dock = DockStyle.Fill; empleado.Margin = new Padding(0, 0, 18, 12);
            campos.Controls.Add(etiquetaPrimero, 0, 0); campos.Controls.Add(primero, 0, 1);
            campos.Controls.Add(etiquetaSegundo, 1, 0); campos.Controls.Add(segundo, 1, 1);
            campos.Controls.Add(etiquetaDescripcion, 2, 0); campos.Controls.Add(descripcion, 2, 1);
            campos.Controls.Add(etiquetaEmpleado, 1, 2); campos.Controls.Add(empleado, 1, 3);
            campos.Controls.Add(etiquetaContrasena, 0, 2); campos.Controls.Add(contrasena, 0, 3);
            campos.Controls.Add(etiquetaConfirmar, 2, 2); campos.Controls.Add(confirmar, 2, 3);
            contenido.Controls.Add(campos, 0, 3);
            etiquetaAsignaciones.AutoSize = true;
            etiquetaAsignaciones.Font = new Font("Segoe UI", 10F);
            etiquetaAsignaciones.Margin = new Padding(0, 10, 18, 0);
            opcionesAsignaciones.AutoSize = true; opcionesAsignaciones.Dock = DockStyle.Fill;
            opcionesAsignaciones.Margin = Padding.Empty;
            generarPermisos.Name = "generarPermisos";
            generarPermisos.Text = "Generar permisos…";
            ProductoEstilo.Boton(generarPermisos);
            generarPermisos.Size = new Size(200, 36);
            asignarPermisosModulo.Name = "asignarPermisosModulo";
            asignarPermisosModulo.Text = "Pantallas y presets…";
            ProductoEstilo.Boton(asignarPermisosModulo);
            asignarPermisosModulo.Size = new Size(230, 36);
            editarPermisosRol.Name = "editarPermisosRol";
            editarPermisosRol.Text = "Crear / editar permisos…";
            ProductoEstilo.Boton(editarPermisosRol);
            editarPermisosRol.Size = new Size(260, 36);
            accionesAsignaciones.AutoSize = true; accionesAsignaciones.Margin = Padding.Empty;
            accionesAsignaciones.Controls.AddRange(new Control[] { generarPermisos, asignarPermisosModulo, editarPermisosRol });
            opcionesAsignaciones.Controls.AddRange(new Control[] { etiquetaAsignaciones, accionesAsignaciones });
            contenido.Controls.Add(opcionesAsignaciones, 0, 4);
            asignaciones.Dock = DockStyle.Fill; asignaciones.Height = 90;
            asignaciones.CheckOnClick = true; asignaciones.IntegralHeight = false;
            asignaciones.Margin = new Padding(0, 4, 0, 14);
            selectorRoles.Name = "selectorRoles";
            selectorRoles.Dock = DockStyle.Fill; selectorRoles.Height = 140;
            contenedorAsignaciones.Dock = DockStyle.Fill; contenedorAsignaciones.Height = 140;
            contenedorAsignaciones.Margin = asignaciones.Margin;
            asignaciones.Margin = selectorRoles.Margin = Padding.Empty;
            contenedorAsignaciones.Controls.Add(asignaciones);
            contenedorAsignaciones.Controls.Add(selectorRoles);
            contenido.Controls.Add(contenedorAsignaciones, 0, 5);
            acciones.AutoSize = true; acciones.Dock = DockStyle.Fill;
            guardar.Text = "Guardar"; actualizar.Text = "Actualizar"; desactivar.Text = "Desactivar";
            reactivar.Text = "Reactivar"; nuevo.Text = "Nuevo"; recargar.Text = "Recargar";
            acciones.Controls.AddRange(new Control[] { guardar, actualizar, desactivar, reactivar, nuevo, recargar });
            contenido.Controls.Add(acciones, 0, 6);
            estado.AutoSize = true; estado.Margin = new Padding(0, 8, 0, 8);
            contenido.Controls.Add(estado, 0, 7);
            filtros.AutoSize = true; filtros.Dock = DockStyle.Fill;
            buscar.Width = 360; buscar.PlaceholderText = "Buscar...";
            inactivos.Text = "Incluir inactivos"; inactivos.AutoSize = true;
            contador.AutoSize = true;
            filtros.Controls.AddRange(new Control[] { buscar, inactivos, contador });
            contenido.Controls.Add(filtros, 0, 8);
            tarjeta.Dock = DockStyle.Fill; tarjeta.Height = 280; tarjeta.Padding = new Padding(16);
            tarjeta.Radio = 30; tarjeta.MostrarSombra = false; tarjeta.ColorBorde = Color.FromArgb(247, 247, 247);
            tabla.Dock = DockStyle.Fill; tabla.ReadOnly = true; tabla.AllowUserToAddRows = false;
            tabla.AllowUserToDeleteRows = false; tabla.MultiSelect = false;
            tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            tabla.AutoGenerateColumns = false; tabla.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            tabla.RowHeadersVisible = false; tabla.BackgroundColor = Color.White; tabla.BorderStyle = BorderStyle.None;
            tabla.RowTemplate.Height = 42; tabla.ColumnHeadersHeight = 42;
            tabla.EnableHeadersVisualStyles = false;
            tabla.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.White, ForeColor = ProductoEstilo.Texto, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            tabla.DefaultCellStyle.SelectionBackColor = ProductoEstilo.Suave;
            tabla.DefaultCellStyle.SelectionForeColor = ProductoEstilo.Texto;
            tarjeta.Controls.Add(tabla); contenido.Controls.Add(tarjeta, 0, 9);
            Controls.Add(contenido);
            ResumeLayout(true);
        }
    }
}
