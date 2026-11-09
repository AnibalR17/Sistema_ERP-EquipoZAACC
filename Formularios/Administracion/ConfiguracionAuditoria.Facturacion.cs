using System.Data;

namespace Sistema_ERP;

public sealed partial class ConfiguracionAuditoria
{
    private readonly ComboBox tiendaCai = new() { Name = "tiendaCAI", DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill };
    private readonly TextBox valorCai = new() { Name = "valorCAI", MaxLength = 37, CharacterCasing = CharacterCasing.Upper, Dock = DockStyle.Fill, PlaceholderText = "XXXXXX-XXXXXX-XXXXXX-XXXXXX-XXXXXX-XX" };
    private readonly Label mensajeCai = new() { Dock = DockStyle.Fill };
    private byte[]? versionCai;

    private void PrepararFacturacion()
    {
        var tabs = new TabControl { Name = "pestanasConfiguracion", Dock = DockStyle.Fill };
        var tabTiendas = new TabPage("Tiendas") { AutoScroll = true, BackColor = Color.White };
        var tabFiscal = new TabPage("Facturación / CAI") { Padding = new Padding(24), BackColor = Color.White, AutoScroll = true };
        var tabAuditoria = new TabPage("Auditoría") { Padding = new Padding(12), BackColor = Color.White };
        layoutConfiguracion.Controls.Remove(layoutAuditoria); Cuerpo.Controls.Remove(layoutConfiguracion);
        layoutConfiguracion.RowStyles.RemoveAt(8); layoutConfiguracion.RowCount = 8;
        layoutConfiguracion.MinimumSize = new Size(0, 550); layoutConfiguracion.RowStyles[6].Height = 100;
        tabTiendas.Controls.Add(layoutConfiguracion); tabAuditoria.Controls.Add(layoutAuditoria);
        tabs.TabPages.AddRange([tabTiendas, tabFiscal, tabAuditoria]); Cuerpo.Controls.Add(tabs);
        var campos = PantallaMaqueta.Filas(48, 28, 40, 28, 42, 64, 48, 76);
        campos.Dock = DockStyle.Top; campos.Height = 474;
        campos.Controls.Add(PantallaMaqueta.Etiqueta("Configuración de facturación", 19, true), 0, 0);
        campos.Controls.Add(PantallaMaqueta.Etiqueta("Tienda"), 0, 1); campos.Controls.Add(tiendaCai, 0, 2);
        campos.Controls.Add(PantallaMaqueta.Etiqueta("CAI autorizado"), 0, 3); campos.Controls.Add(valorCai, 0, 4);
        campos.Controls.Add(PantallaMaqueta.Etiqueta("Ingresa el CAI entregado por el SAR para esta tienda. Se guardará en cada nueva factura y se mostrará en el recibo y sus reimpresiones.", 10), 0, 5);
        var botones = new FlowLayoutPanel { Dock = DockStyle.Fill };
        var guardarCai = PantallaMaqueta.Boton("Guardar CAI", true); var recargarCai = PantallaMaqueta.Boton("Actualizar");
        PermisosInterfaz.ConfigurarAcciones(this, (guardarCai, Modulos.Configuracion));
        botones.Controls.AddRange([guardarCai, recargarCai]); campos.Controls.Add(botones, 0, 6); campos.Controls.Add(mensajeCai, 0, 7); tabFiscal.Controls.Add(campos);
        tiendaCai.SelectedIndexChanged += (_, _) => CargarCai();
        recargarCai.Click += (_, _) => CargarTiendasCai();
        guardarCai.Click += (_, _) =>
        {
            if (tiendaCai.SelectedValue is not int tienda) { mensajeCai.Text = "Selecciona una tienda."; return; }
            guardarCai.Enabled = false;
            try
            {
                ConfiguracionFacturacionDatos.Guardar(tienda, valorCai.Text, versionCai);
                CargarCai(); mensajeCai.ForeColor = ProductoEstilo.Exito; mensajeCai.Text = "CAI guardado para las nuevas facturas de esta tienda."; ReiniciarAuditoria();
            }
            catch (Exception ex) { mensajeCai.ForeColor = ProductoEstilo.Error; mensajeCai.Text = ex.Message; }
            finally { guardarCai.Enabled = true; }
        };
        tabs.SelectedIndexChanged += (_, _) => { if (tabs.SelectedTab == tabFiscal) CargarTiendasCai(); else if (tabs.SelectedTab == tabAuditoria) ReiniciarAuditoria(); };
    }

    private void CargarTiendasCai()
    {
        if (ModoDiseno.Activo(this)) return;
        try
        {
            object? seleccion = tiendaCai.SelectedValue;
            tiendaCai.DisplayMember = "nombre"; tiendaCai.ValueMember = "id_tienda";
            var tiendas = VentaDatos.Consultar("SELECT id_tienda,nombre FROM dbo.Tienda WHERE activo=1 ORDER BY nombre", modulo: Modulos.Configuracion);
            tiendaCai.DataSource = tiendas;
            if (seleccion is int id && tiendas.AsEnumerable().Any(t => t.Field<int>("id_tienda") == id)) tiendaCai.SelectedValue = id;
            CargarCai();
        }
        catch (Exception ex) { tiendaCai.DataSource = null; valorCai.Clear(); mensajeCai.ForeColor = ProductoEstilo.Error; mensajeCai.Text = ex.Message; }
    }

    private void CargarCai()
    {
        versionCai = null; valorCai.Clear(); mensajeCai.Text = "";
        if (tiendaCai.SelectedValue is not int id || ModoDiseno.Activo(this)) return;
        try
        {
            var datos = ConfiguracionFacturacionDatos.Obtener(id);
            if (datos.Rows.Count == 1) { valorCai.Text = Convert.ToString(datos.Rows[0]["cai"]); versionCai = (byte[])datos.Rows[0]["version_fila"]; }
            mensajeCai.Text = valorCai.Text.Length == 0 ? "No hay CAI configurado para esta tienda." : "CAI actual cargado.";
        }
        catch (Exception ex) { mensajeCai.ForeColor = ProductoEstilo.Error; mensajeCai.Text = ex.Message; }
    }
}
