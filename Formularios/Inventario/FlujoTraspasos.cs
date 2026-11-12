using System.Data;
using Sistema_ERP.Clases;

namespace Sistema_ERP
{
    internal sealed partial class FlujoTraspasos : UserControl
    {
        private readonly DataGridView lista = Tabla();
        private readonly TextBox usuario = new() { Width = 240, ReadOnly = true };
        private readonly Dictionary<string, Button> botones = new();
        private readonly ComboBox filtro = new() { Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
        private bool cargado;
        private readonly bool crearSolicitud;
        private readonly Panel contenidoSolicitud = new() { Dock = DockStyle.Fill };

        internal FlujoTraspasos(bool validacion)
        {
            crearSolicitud = !validacion;
            if (crearSolicitud)
                ConfigurarSolicitud();
            else
                ConfigurarPresentacion();
            filtro.SelectedIndexChanged += (_, _) => Ejecutar(Cargar);
            Load += FlujoTraspasos_Load;
            lista.SelectionChanged += (_, _) => ActualizarAcciones();
            PermisosInterfaz.Configurar(this, validacion ? Modulos.Validacion : Modulos.Traspasos);
            PermisosInterfaz.ConfigurarAcciones(this, botones.Where(b => b.Key is not ("Actualizar" or "Detalle / historial"))
                .Select(b => ((Control)b.Value, b.Key is "Aprobar" or "Rechazar" ? Modulos.Validacion : Modulos.Traspasos)).ToArray());
        }

        private void ConfigurarSolicitud()
        {
            Dock = DockStyle.Fill;
            TraspasoEstilo.Superficie(this);
            var campos = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true };
            campos.Controls.Add(TraspasoEstilo.Campo("Usuario solicitante", usuario));
            contenidoSolicitud.Controls.Add(new EditorTraspaso(null, Usuario, false));
            Controls.Add(contenidoSolicitud);
            Controls.Add(campos);
            Controls.Add(TraspasoEstilo.Cabecera("Traspaso entre tiendas",
                "Crea una solicitud de traspaso. Quedará pendiente de aprobación en Validación de Traspasos."));
        }

        private void ConfigurarPresentacion()
        {
            Dock = DockStyle.Fill;
            TraspasoEstilo.Superficie(this);
            var cabecera = TraspasoEstilo.Cabecera("Validación de traspasos",
                "Consulta y revisa las solicitudes. Aprobar reserva las unidades disponibles en origen.");
            var barra = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, WrapContents = true };
            var filtros = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, WrapContents = true };
            filtros.Controls.Add(TraspasoEstilo.Campo("Usuario que realiza la operación", usuario));
            filtro.Items.AddRange(new object[] { "TODOS", "PENDIENTE", "APROBADO", "RECHAZADO", "CANCELADO", "EN_TRANSITO", "RECIBIDO" });
            filtro.SelectedIndex = 1;
            filtros.Controls.Add(TraspasoEstilo.Campo("Estado de la solicitud", filtro));
            void Boton(string texto, Action accion)
            {
                var boton = new Button { Text = texto, AutoSize = true };
                TraspasoEstilo.Boton(boton, texto is "Solicitar" or "Aprobar");
                boton.Click += (_, _) => Ejecutar(accion);
                botones[texto] = boton;
                barra.Controls.Add(boton);
            }
            Boton("Actualizar", Cargar);
            RecargaManual.Preparar(botones["Actualizar"]);
            botones["Actualizar"].Name = "btnReiniciar";
            Boton("Detalle / historial", () => Abrir(false));
            Boton("Aprobar", () => Resolver("APROBADO"));
            Boton("Rechazar", () => Resolver("RECHAZADO"));
            Boton("Editar pendiente", () => Abrir(true));
            Boton("Cancelar solicitud", () => Resolver("CANCELADO"));
            Boton("Enviar", () => Resolver("EN_TRANSITO"));
            Boton("Confirmar recepción", () => Resolver("RECIBIDO"));
            ActualizarAcciones();
            Controls.Add(TraspasoEstilo.Tarjeta(lista));
            Controls.Add(barra);
            Controls.Add(filtros);
            Controls.Add(cabecera);
        }

        private void FlujoTraspasos_Load(object? sender, EventArgs e)
        {
            if (cargado)
                return;

            Ejecutar(() =>
            {
                usuario.Text = SesionActual.Exigir().Nombre;
                if (!crearSolicitud)
                {
                    Cargar();
                }
                cargado = true;
            });
        }

        private static void Ejecutar(Action accion)
        {
            try
            {
                accion();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Traspasos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        internal static DataGridView Tabla()
        {
            var tabla = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false,
                AutoGenerateColumns = true
            };
            TraspasoEstilo.Tabla(tabla);
            return tabla;
        }

        internal static void Enlazar(ComboBox combo, DataTable datos, string valor, string texto)
        {
            combo.DisplayMember = texto;
            combo.ValueMember = valor;
            combo.DataSource = datos;
            combo.SelectedIndex = -1;
        }

        private int Usuario()
        {
            return SesionActual.Exigir().Id;
        }

        private void ActualizarAcciones()
        {
            string? estado = (lista.CurrentRow?.DataBoundItem as DataRowView)?["estado"] as string;
            foreach (var (texto, boton) in botones)
                boton.Enabled = texto switch
                {
                    "Actualizar" => true,
                    "Detalle / historial" => estado != null,
                    "Cancelar solicitud" => estado is "PENDIENTE" or "APROBADO",
                    "Enviar" => estado == "APROBADO",
                    "Confirmar recepción" => estado == "EN_TRANSITO",
                    _ => estado == "PENDIENTE"
                };
        }

        private DataRow Fila()
        {
            if (lista.CurrentRow?.DataBoundItem is not DataRowView seleccion)
                throw new InvalidOperationException("Seleccione una solicitud.");

            return seleccion.Row;
        }

        private static void Pendiente(DataRow fila)
        {
            if ((string)fila["estado"] != "PENDIENTE")
                throw new InvalidOperationException("Solo se pueden modificar solicitudes pendientes.");
        }

        private void Cargar()
        {
            lista.DataSource = null;
            ActualizarAcciones();
            var datos = TraspasoDatos.Listar();
            var vista = datos.DefaultView;
            if (filtro.SelectedIndex > 0)
                vista.RowFilter = $"estado = '{filtro.SelectedItem}'";
            lista.DataSource = vista;
            foreach (string columna in new[] { "id_tienda_origen", "id_tienda_destino", "version_fila" })
                if (lista.Columns[columna] is DataGridViewColumn encontrada)
                    encontrada.Visible = false;
            ActualizarAcciones();
        }

        private void Abrir(bool editar)
        {
            var fila = Fila();
            if (editar)
            {
                Pendiente(fila);
                Editar(fila);
                return;
            }
            using var ventana = new FormularioTematico { Text = $"Traspaso #{fila["id_traspaso"]} — {fila["estado"]}", Size = new Size(900, 600), StartPosition = FormStartPosition.CenterParent };
            TraspasoEstilo.Superficie(ventana);
            var tabs = new Sistema_ERP.PestanasTematicas { Dock = DockStyle.Fill, Padding = new Point(18, 10) };
            void CargarDetalle()
            {
                int seleccion = tabs.SelectedIndex;
                var detalle = TraspasoDatos.Detalle((int)fila["id_traspaso"]);
                var historial = TraspasoDatos.Historial((int)fila["id_traspaso"]);
                foreach (TabPage anterior in tabs.TabPages.Cast<TabPage>().ToArray()) anterior.Dispose();
                tabs.TabPages.Clear();
                foreach (var par in new[] { ("Productos", detalle), ("Historial", historial) })
                {
                    var pagina = new TabPage(par.Item1) { BackColor = ProductoEstilo.Fondo, Padding = new Padding(8) };
                    var tabla = Tabla(); tabla.DataSource = par.Item2;
                    pagina.Controls.Add(TraspasoEstilo.Tarjeta(tabla)); tabs.TabPages.Add(pagina);
                }
                if (seleccion >= 0 && seleccion < tabs.TabCount) tabs.SelectedIndex = seleccion;
            }
            CargarDetalle();
            ventana.Controls.Add(tabs);
            ventana.Controls.Add(TraspasoEstilo.Cabecera($"Traspaso #{fila["id_traspaso"]}", $"{fila["Origen"]} → {fila["Destino"]} · {fila["estado"]}\nSolicitante: {fila["Solicitante"]}\nObservaciones: {fila["observaciones"]}"));
            var cerrar = new Button { Text = "Cerrar", DialogResult = DialogResult.OK };
            TraspasoEstilo.Boton(cerrar);
            var pie = new FlowLayoutPanel { Dock = DockStyle.Bottom, AutoSize = true, FlowDirection = FlowDirection.RightToLeft };
            pie.Controls.Add(cerrar);
            RecargaManual.Agregar(pie, CargarDetalle);
            ventana.Controls.Add(pie);
            ventana.CancelButton = cerrar;
            ventana.ShowDialog(this);
        }

        private void Editar(DataRow? fila)
        {
            Usuario();
            using var ventana = new FormularioTematico
            {
                Text = "Editar solicitud pendiente", Size = new Size(950, 650),
                MinimumSize = new Size(760, 560), StartPosition = FormStartPosition.CenterParent
            };
            var editor = new EditorTraspaso(fila, Usuario);
            editor.Guardado += (_, _) => ventana.DialogResult = DialogResult.OK;
            ventana.Controls.Add(editor);
            if (ventana.ShowDialog(this) == DialogResult.OK)
                Cargar();
        }

        private void Resolver(string estado)
        {
            var fila = Fila();
            string anterior = (string)fila["estado"];
            bool permitido = estado switch
            {
                "EN_TRANSITO" => anterior == "APROBADO",
                "RECIBIDO" => anterior == "EN_TRANSITO",
                "CANCELADO" => anterior is "PENDIENTE" or "APROBADO",
                _ => anterior == "PENDIENTE"
            };
            if (!permitido) throw new InvalidOperationException("El estado actual no permite esta operación.");
            int actor = Usuario();
            using var dialogo = new FormularioTematico { Text = $"{estado}: traspaso #{fila["id_traspaso"]}", Size = new Size(680, 450), StartPosition = FormStartPosition.CenterParent };
            TraspasoEstilo.Superficie(dialogo);
            var motivo = new TextBox { Dock = DockStyle.Fill, Multiline = true, MaxLength = 500 };
            var aceptar = new Button { Text = "Confirmar", Dock = DockStyle.Bottom, Height = 36 };
            aceptar.Click += (_, _) => Ejecutar(() =>
            {
                Modificar.ResolverTraspaso((int)fila["id_traspaso"], (byte[])fila["version_fila"], actor, estado, motivo.Text);
                dialogo.DialogResult = DialogResult.OK;
            });
            motivo.BorderStyle = BorderStyle.FixedSingle;
            dialogo.Controls.Add(TraspasoEstilo.Tarjeta(motivo));
            dialogo.Controls.Add(TraspasoEstilo.Cabecera(estado switch
            {
                "APROBADO" => "Aprobar traspaso",
                "RECHAZADO" => "Rechazar traspaso",
                "EN_TRANSITO" => "Enviar traspaso",
                "RECIBIDO" => "Confirmar recepción",
                _ => "Cancelar traspaso"
            },
                $"Solicitud #{fila["id_traspaso"]} · {fila["Origen"]} → {fila["Destino"]}\n" +
                (estado switch
                {
                    "APROBADO" => "Se reservarán las unidades en origen. Motivo opcional.",
                    "EN_TRANSITO" => "Se descontarán las unidades del origen y se liberará su reserva. Motivo opcional.",
                    "RECIBIDO" => "Se ingresarán las unidades en la tienda de destino. Motivo opcional.",
                    _ => "Indica el motivo de la decisión. Cancelar un aprobado libera su reserva."
                })));
            dialogo.Controls.Add(TraspasoEstilo.Pie(dialogo, aceptar));
            if (dialogo.ShowDialog(this) == DialogResult.OK)
                Cargar();
        }

    }

}
