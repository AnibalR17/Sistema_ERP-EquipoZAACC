using System.Data;
using System.Reflection;
using Sistema_ERP;
using Sistema_ERP.Vistas;

internal static partial class Program
{
    private static void ProbarBusquedaCombos()
    {
        var aplicar = app.GetType("Sistema_ERP.TemaAplicacion")!.GetMethod("Aplicar", estaticos)!;
        void Preparar(Control control) => aplicar.Invoke(null, [control]);
        int comprobaciones = 0;
        void Comprobar(bool valor, string contexto) { Igual(true, valor, contexto); comprobaciones++; }
        IEnumerable<Control> Controles(Control control) => new[] { control }.Concat(control.Controls.Cast<Control>().SelectMany(Controles));
        void ComprobarCombo(ComboBox combo, string contexto)
        {
            Comprobar(combo.DropDownStyle == ComboBoxStyle.DropDown && combo.AutoCompleteMode == AutoCompleteMode.SuggestAppend
                && combo.AutoCompleteSource == AutoCompleteSource.ListItems, $"{contexto} (estilo={combo.DropDownStyle}, modo={combo.AutoCompleteMode}, fuente={combo.AutoCompleteSource})");
        }

        using var datos = new DataTable(); datos.Columns.Add("Id", typeof(int)); datos.Columns.Add("Nombre");
        datos.Rows.Add(3, "Cuaderno universitario"); datos.Rows.Add(8, "Lápiz grafito"); datos.Rows.Add(12, "Cuaderno universitario");
        using var ventana = new FormularioTematico { Size = new Size(600, 400), ShowInTaskbar = false,
            StartPosition = FormStartPosition.Manual, Location = new Point(-10000, -10000), Opacity = 0 };
        var combo = new ComboBox { Name = "producto", DropDownStyle = ComboBoxStyle.DropDownList,
            DisplayMember = "Nombre", ValueMember = "Id", DataSource = datos, Width = 320 };
        var libre = new ComboBox { DropDownStyle = ComboBoxStyle.DropDown, Top = 45 };
        libre.Items.AddRange(["CONSULTAR", "GESTIONAR"]);
        ventana.Controls.AddRange([combo, libre]); ventana.Show(); Application.DoEvents();
        ComprobarCombo(combo, "Se puede escribir y desplegar los registros enlazados");
        combo.SelectedValue = 12;
        for (int i = 0; i < 3; i++) Preparar(ventana);
        Comprobar((int)combo.SelectedValue! == 12 && combo.Items.Count == 3, "Reaplicar conserva el cliente/producto elegido entre nombres repetidos");
        combo.Text = "PRODUCTO INEXISTENTE";
        string textoInvalido = combo.Text;
        bool validacion = ventana.ValidateChildren();
        Comprobar(!validacion && datos.Rows.Count == 3, $"El texto ajeno al catálogo no es una selección válida ni crea registros (antes={textoInvalido}, después={combo.Text}, índice={combo.SelectedIndex}, validación={validacion})");
        int guardados = 0;
        var guardar = new Button { Text = "Guardar", Left = 350, Top = 45 };
        guardar.Click += (_, _) => guardados++;
        ventana.Controls.Add(guardar); combo.Focus(); guardar.PerformClick();
        Comprobar(guardados == 0, "Guardar no utiliza una selección anterior cuando el texto de búsqueda es inválido");
        combo.Text = "Lápiz grafito";
        Comprobar(ventana.ValidateChildren() && (int)combo.SelectedValue! == 8, "Un texto exacto selecciona el identificador existente");
        guardar.PerformClick(); Comprobar(guardados == 1, "Guardar sigue disponible después de elegir una opción válida");
        combo.SelectedIndex = -1; combo.Text = "";
        Comprobar(ventana.ValidateChildren() && combo.SelectedValue is null, "Los campos opcionales conservan la selección vacía");
        libre.Text = "ACCION_PERSONALIZADA";
        Comprobar(ventana.ValidateChildren() && libre.Text == "ACCION_PERSONALIZADA", "Los ComboBox que admitían códigos nuevos conservan esa función");
        using var recarga = datos.Copy(); recarga.Rows.Add(20, "Marcador azul"); combo.DataSource = recarga;
        combo.DropDownStyle = ComboBoxStyle.DropDownList;
        ComprobarCombo(combo, "Recargar o restablecer el estilo mantiene la búsqueda");
        combo.Text = "Marcador azul";
        Comprobar(ventana.ValidateChildren() && (int)combo.SelectedValue! == 20 && combo.Items.Count == 4, "Las sugerencias usan el catálogo recargado");
        var dinamico = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Top = 90 };
        dinamico.Items.AddRange(["ACTIVO", "CANCELADO"]); ventana.Controls.Add(dinamico);
        ComprobarCombo(dinamico, "Los ComboBox agregados después de abrir la pantalla también permiten buscar");

        using var lineas = new DataTable(); lineas.Columns.Add("Producto", typeof(int)); lineas.Rows.Add(3);
        var tabla = new DataGridView { Top = 140, Width = 500, Height = 180, AutoGenerateColumns = false,
            AllowUserToAddRows = false, DataSource = lineas };
        tabla.Columns.Add(new DataGridViewComboBoxColumn { DataPropertyName = "Producto", DataSource = datos,
            DisplayMember = "Nombre", ValueMember = "Id", Width = 300 });
        ventana.Controls.Add(tabla); tabla.CurrentCell = tabla.Rows[0].Cells[0]; tabla.BeginEdit(true);
        Comprobar(tabla.EditingControl is ComboBox, "La tabla usa un editor de productos desplegable");
        var editor = (ComboBox)tabla.EditingControl!;
        ComprobarCombo(editor, "El producto dentro de la tabla de traspasos también permite escribir y buscar");
        editor.SelectedValue = 8; tabla.EndEdit();
        Comprobar((int)lineas.Rows[0]["Producto"] == 8, "Elegir la sugerencia de la tabla conserva el identificador del producto");
        tabla.BeginEdit(true); editor = (ComboBox)tabla.EditingControl!;
        ComprobarCombo(editor, "El editor reutilizado conserva el autocompletado");
        bool error = false; tabla.DataError += (_, e) => { error = true; e.ThrowException = false; };
        editor.Text = "NO EXISTE";
        typeof(ComboBox).GetMethod("OnTextUpdate", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(editor, [EventArgs.Empty]);
        bool finEdicion = tabla.EndEdit();
        Comprobar(!finEdicion && (int)lineas.Rows[0]["Producto"] == 8, $"La tabla rechaza productos escritos que no existen (fin={finEdicion}, error={error}, texto={editor.Text}, valor={lineas.Rows[0]["Producto"]})");
        tabla.CancelEdit();

        foreach (Func<Control> crear in new Func<Control>[] { () => new PantallaCompras(), () => new ApartadosDevoluciones(),
            () => new Nuevo_Producto(), () => new Traspaso_Tiendas(), () => new PuntoVenta(), () => new ConfiguracionAuditoria(),
            () => new GenerarPermisosUsuario(), () => new AsignarPermisosRol() })
        {
            using var pantalla = crear(); Preparar(pantalla);
            foreach (var selector in Controles(pantalla).OfType<ComboBox>()) ComprobarCombo(selector, $"Búsqueda en {pantalla.GetType().Name}");
        }
        var cancelar = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel, Top = 330 };
        ventana.Controls.Add(cancelar); ventana.CancelButton = cancelar; Preparar(ventana);
        combo.Focus(); combo.Text = "BÚSQUEDA PENDIENTE"; cancelar.PerformClick();
        Comprobar(!cancelar.CausesValidation && ventana.DialogResult == DialogResult.Cancel,
            "Cancelar sigue disponible aunque el texto de búsqueda no coincida con una opción");
        Console.WriteLine($"OK: {comprobaciones} comprobaciones de escritura, listas, catálogos, selección válida y ComboBox en tablas, sin SQL.");
    }
}
