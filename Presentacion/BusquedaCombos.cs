using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Sistema_ERP;

internal static class BusquedaCombos
{
    private static readonly ConditionalWeakTable<Control, object> preparados = new();

    internal static void Preparar(ComboBox combo)
    {
        if (combo.IsDisposed || ModoDiseno.Activo(combo)) return;
        if (preparados.TryGetValue(combo, out var existente)) { ((Busqueda)existente).Configurar(); return; }
        var busqueda = new Busqueda(combo);
        preparados.Add(combo, busqueda);
    }

    internal static void Preparar(DataGridView tabla)
    {
        if (tabla.IsDisposed || ModoDiseno.Activo(tabla) || preparados.TryGetValue(tabla, out _)) return;
        preparados.Add(tabla, new object());
        tabla.EditingControlShowing += (_, e) =>
        {
            if (e.Control is ComboBox combo) Preparar(combo);
        };
        tabla.CellValidating += (_, e) =>
        {
            if (tabla.EditingControl is not ComboBox combo || tabla.CurrentCell is not DataGridViewComboBoxCell celda) return;
            try
            {
                SeleccionarOpcionValidacion.Validar(combo.Text, combo.Items.Cast<object>().Select(o => combo.GetItemText(o) ?? "").ToArray(), combo.SelectedIndex);
                celda.ErrorText = "";
            }
            catch (ArgumentException ex) { e.Cancel = true; celda.ErrorText = ex.Message; }
        };
    }

    private sealed class Busqueda
    {
        private readonly ComboBox combo;
        private bool soloOpciones, configurando;
        private ErrorProvider? errores;

        internal Busqueda(ComboBox combo)
        {
            this.combo = combo;
            combo.DropDownStyleChanged += (_, _) => Configurar();
            combo.Validating += Validar;
            combo.TextUpdate += (_, _) =>
            {
                errores?.SetError(combo, "");
                if (combo is DataGridViewComboBoxEditingControl editor && editor.EditingControlDataGridView is { } tabla)
                {
                    editor.EditingControlValueChanged = true;
                    tabla.NotifyCurrentCellDirty(true);
                }
            };
            combo.SelectionChangeCommitted += (_, _) => errores?.SetError(combo, "");
            combo.Disposed += (_, _) => errores?.Dispose();
            combo.AccessibleDescription = (combo.AccessibleDescription + " Escribe para buscar o abre la lista para elegir una opción.").Trim();
            Configurar();
        }

        internal void Configurar()
        {
            if (configurando || combo.IsDisposed) return;
            configurando = true;
            try
            {
                soloOpciones |= combo.DropDownStyle == ComboBoxStyle.DropDownList || combo is DataGridViewComboBoxEditingControl;
                combo.FormattingEnabled = true;
                combo.DropDownStyle = ComboBoxStyle.DropDown;
                combo.AutoCompleteSource = AutoCompleteSource.ListItems;
                combo.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            }
            finally { configurando = false; }
        }

        private void Validar(object? sender, CancelEventArgs e)
        {
            if (!soloOpciones || !combo.Enabled) return;
            try
            {
                int indice = SeleccionarOpcionValidacion.Validar(combo.Text, combo.Items.Cast<object>().Select(o => combo.GetItemText(o) ?? "").ToArray(), combo.SelectedIndex);
                if (combo.SelectedIndex != indice) combo.SelectedIndex = indice;
                errores?.SetError(combo, "");
            }
            catch (ArgumentException ex)
            {
                e.Cancel = true;
                if (combo.FindForm() is { } formulario)
                {
                    errores ??= new ErrorProvider(formulario) { BlinkStyle = ErrorBlinkStyle.NeverBlink };
                    errores.SetError(combo, ex.Message);
                }
            }
        }
    }
}
