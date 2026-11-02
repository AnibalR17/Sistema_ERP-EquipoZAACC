#nullable enable
namespace Sistema_ERP
{
    partial class Notificaciones
    {
        private System.ComponentModel.IContainer? components;
        private TableLayoutPanel contenido = null!;
        private Label titulo = null!, descripcion = null!, resumen = null!, estado = null!;
        private PanelDisenoInventario tarjeta = null!;
        private DataGridView tablaAlertas = null!;
        private DataGridViewTextBoxColumn columnaTienda = null!, columnaCodigo = null!, columnaProducto = null!, columnaExistencia = null!, columnaReservado = null!, columnaDisponible = null!, columnaMinimo = null!;
        private FlowLayoutPanel acciones = null!;
        private Button actualizar = null!, cerrar = null!;

        protected override void Dispose(bool disposing)
        {
            if (disposing) components?.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            contenido = new TableLayoutPanel();
            titulo = new Label(); descripcion = new Label(); resumen = new Label(); estado = new Label();
            tarjeta = new PanelDisenoInventario(); tablaAlertas = new DataGridView();
            columnaTienda = new DataGridViewTextBoxColumn(); columnaCodigo = new DataGridViewTextBoxColumn(); columnaProducto = new DataGridViewTextBoxColumn();
            columnaExistencia = new DataGridViewTextBoxColumn(); columnaReservado = new DataGridViewTextBoxColumn(); columnaDisponible = new DataGridViewTextBoxColumn(); columnaMinimo = new DataGridViewTextBoxColumn();
            acciones = new FlowLayoutPanel(); actualizar = new Button(); cerrar = new Button();
            SuspendLayout();
            contenido.SuspendLayout();
            tarjeta.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)tablaAlertas).BeginInit();
            acciones.SuspendLayout();
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Font = new Font("Segoe UI", 11F);
            BackColor = Color.White; ForeColor = Color.FromArgb(24, 24, 24);
            Name = "Notificaciones"; Text = "Notificaciones de inventario";
            ClientSize = new Size(940, 560); MinimumSize = new Size(720, 440);
            StartPosition = FormStartPosition.CenterParent; ShowInTaskbar = false; ShowIcon = false;
            MinimizeBox = false; MaximizeBox = false; FormBorderStyle = FormBorderStyle.Sizable;
            Padding = new Padding(28);
            contenido.Name = "contenido"; contenido.Dock = DockStyle.Fill;
            contenido.ColumnCount = 1; contenido.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            contenido.RowCount = 6;
            contenido.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
            contenido.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
            contenido.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
            contenido.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            contenido.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));
            contenido.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
            titulo.Name = "titulo"; titulo.Text = "Alertas de inventario";
            titulo.Font = new Font("Segoe UI", 20F, FontStyle.Bold); titulo.Dock = DockStyle.Fill;
            titulo.Margin = Padding.Empty; titulo.TextAlign = ContentAlignment.MiddleLeft; titulo.TabStop = false;
            descripcion.Name = "descripcion"; descripcion.Text = "Las alertas se consultan a partir de las existencias actuales por tienda.";
            descripcion.ForeColor = Color.FromArgb(125, 125, 125); descripcion.Dock = DockStyle.Fill;
            descripcion.Margin = Padding.Empty; descripcion.TextAlign = ContentAlignment.MiddleLeft; descripcion.TabStop = false;
            resumen.Name = "resumen"; resumen.Text = "Pulsa Actualizar para consultar las alertas.";
            resumen.Dock = DockStyle.Fill; resumen.Margin = Padding.Empty; resumen.TextAlign = ContentAlignment.MiddleLeft;
            resumen.Font = new Font("Segoe UI", 10F, FontStyle.Bold); resumen.TabStop = false;
            tarjeta.Name = "tarjeta"; tarjeta.Dock = DockStyle.Fill; tarjeta.Radio = 30; tarjeta.MostrarSombra = false;
            tarjeta.ColorBorde = Color.FromArgb(247, 247, 247); tarjeta.Padding = new Padding(16); tarjeta.Margin = Padding.Empty;
            tablaAlertas.Name = "tablaAlertas"; tablaAlertas.Dock = DockStyle.Fill;
            tablaAlertas.ReadOnly = true; tablaAlertas.AllowUserToAddRows = false; tablaAlertas.AllowUserToDeleteRows = false;
            tablaAlertas.AllowUserToOrderColumns = false; tablaAlertas.MultiSelect = false;
            tablaAlertas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            tablaAlertas.RowHeadersVisible = false; tablaAlertas.AutoGenerateColumns = false;
            tablaAlertas.BorderStyle = BorderStyle.None; tablaAlertas.BackgroundColor = Color.White;
            tablaAlertas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            tablaAlertas.RowTemplate.Height = 42; tablaAlertas.ColumnHeadersHeight = 42;
            tablaAlertas.EnableHeadersVisualStyles = false;
            tablaAlertas.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal; tablaAlertas.GridColor = Color.FromArgb(247, 247, 247);
            tablaAlertas.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            tablaAlertas.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.White, ForeColor = Color.FromArgb(24, 24, 24), Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            tablaAlertas.DefaultCellStyle.SelectionBackColor = Color.FromArgb(243, 243, 243);
            tablaAlertas.DefaultCellStyle.SelectionForeColor = Color.FromArgb(24, 24, 24);
            columnaTienda.Name = columnaTienda.DataPropertyName = "Tienda"; columnaTienda.HeaderText = "Tienda"; columnaTienda.MinimumWidth = 100; columnaTienda.FillWeight = 140;
            columnaCodigo.Name = columnaCodigo.DataPropertyName = "Codigo"; columnaCodigo.HeaderText = "Código"; columnaCodigo.MinimumWidth = 85; columnaCodigo.FillWeight = 110;
            columnaProducto.Name = columnaProducto.DataPropertyName = "Producto"; columnaProducto.HeaderText = "Producto"; columnaProducto.MinimumWidth = 150; columnaProducto.FillWeight = 200;
            columnaExistencia.Name = columnaExistencia.DataPropertyName = "Existencia"; columnaExistencia.HeaderText = "Existencia"; columnaExistencia.MinimumWidth = 85; columnaExistencia.FillWeight = 85;
            columnaReservado.Name = columnaReservado.DataPropertyName = "Reservado"; columnaReservado.HeaderText = "Reservado"; columnaReservado.MinimumWidth = 85; columnaReservado.FillWeight = 85;
            columnaDisponible.Name = columnaDisponible.DataPropertyName = "Disponible"; columnaDisponible.HeaderText = "Disponible"; columnaDisponible.MinimumWidth = 85; columnaDisponible.FillWeight = 85;
            columnaMinimo.Name = columnaMinimo.DataPropertyName = "Minimo"; columnaMinimo.HeaderText = "Mínimo"; columnaMinimo.MinimumWidth = 70; columnaMinimo.FillWeight = 70;
            foreach (var columna in new[] { columnaExistencia, columnaReservado, columnaDisponible, columnaMinimo })
            {
                columna.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                columna.DefaultCellStyle.Format = "N0";
            }
            tablaAlertas.Columns.AddRange(new DataGridViewColumn[] { columnaTienda, columnaCodigo, columnaProducto, columnaExistencia, columnaReservado, columnaDisponible, columnaMinimo });
            estado.Name = "estado"; estado.Text = "Las alertas reflejan el inventario al momento de consultar.";
            estado.Dock = DockStyle.Fill; estado.Margin = new Padding(0, 8, 0, 0);
            estado.Font = new Font("Segoe UI", 10F); estado.ForeColor = Color.FromArgb(125, 125, 125);
            estado.TextAlign = ContentAlignment.MiddleLeft; estado.TabStop = false;
            acciones.Name = "acciones"; acciones.Dock = DockStyle.Fill;
            acciones.FlowDirection = FlowDirection.RightToLeft; acciones.WrapContents = false; acciones.Margin = Padding.Empty;
            cerrar.Name = "cerrar"; cerrar.Text = "Cerrar"; cerrar.DialogResult = DialogResult.Cancel;
            actualizar.Name = "actualizar"; actualizar.Text = "Actualizar";
            foreach (var boton in new[] { cerrar, actualizar })
            {
                boton.Size = new Size(120, 44); boton.Margin = new Padding(8, 4, 0, 0);
                boton.FlatStyle = FlatStyle.Flat; boton.BackColor = Color.White;
                boton.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 220);
                boton.UseVisualStyleBackColor = false; boton.Cursor = Cursors.Hand;
            }
            cerrar.BackColor = Color.FromArgb(24, 24, 24); cerrar.ForeColor = Color.White;
            tablaAlertas.TabIndex = 0; acciones.TabIndex = 1; actualizar.TabIndex = 0; cerrar.TabIndex = 1;
            acciones.Controls.Add(cerrar); acciones.Controls.Add(actualizar);
            tarjeta.Controls.Add(tablaAlertas);
            contenido.Controls.Add(titulo, 0, 0); contenido.Controls.Add(descripcion, 0, 1);
            contenido.Controls.Add(resumen, 0, 2); contenido.Controls.Add(tarjeta, 0, 3);
            contenido.Controls.Add(estado, 0, 4); contenido.Controls.Add(acciones, 0, 5);
            Controls.Add(contenido); CancelButton = cerrar;
            acciones.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)tablaAlertas).EndInit();
            tarjeta.ResumeLayout(false);
            contenido.ResumeLayout(false);
            ResumeLayout(true);
        }
    }
}
