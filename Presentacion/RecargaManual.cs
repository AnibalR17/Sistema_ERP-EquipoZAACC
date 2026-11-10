namespace Sistema_ERP;

internal static class RecargaManual
{
    internal static void Actualizar(ComboBox combo, System.Data.DataTable datos)
    {
        object? anterior = combo.SelectedValue;
        combo.DataSource = datos;
        combo.SelectedIndex = -1;
        if (anterior is int id && datos.Rows.Cast<System.Data.DataRow>().Any(f => Equals(f[combo.ValueMember], id)))
            combo.SelectedValue = id;
    }

    internal static void Preparar(Button boton)
    {
        boton.Text = "Reiniciar";
        boton.AccessibleName = "Reiniciar: recargar datos";
        boton.AccessibleDescription = "Vuelve a consultar los datos actuales de esta pantalla.";
    }

    internal static Button Agregar(Control barra, Action recargar, string nombre = "btnReiniciar")
        => Agregar(barra, () => { recargar(); return Task.CompletedTask; }, nombre);

    internal static Button Agregar(Control barra, Func<Task> recargar, string nombre = "btnReiniciar")
    {
        var boton = PantallaMaqueta.Boton("Reiniciar");
        boton.Name = nombre;
        boton.Width = 120;
        Preparar(boton);
        boton.Click += async (_, _) =>
        {
            boton.Enabled = false;
            try { await recargar(); }
            catch (Exception ex)
            {
                if (!boton.IsDisposed)
                    MessageBox.Show(boton.FindForm(), "No se pudieron recargar los datos. " + ex.Message,
                        "Reiniciar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally { if (!boton.IsDisposed) boton.Enabled = true; }
        };
        barra.Controls.Add(boton);
        return boton;
    }
}
