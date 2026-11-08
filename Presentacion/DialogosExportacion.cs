namespace Sistema_ERP;

internal static class DialogosExportacion
{
    internal static async Task ReciboPdf(Form propietario, int venta, string numero)
    {
        string nombre = string.Concat(numero.Select(c => Path.GetInvalidFileNameChars().Contains(c) ? '_' : c));
        using var guardar = new SaveFileDialog { Filter = "Documento PDF (*.pdf)|*.pdf", DefaultExt = "pdf", AddExtension = true, FileName = "Recibo_" + nombre + ".pdf" };
        if (guardar.ShowDialog(propietario) != DialogResult.OK) return;
        bool generado = false;
        try
        {
            await Task.Run(() =>
            {
                string contenido = VentaDatos.Comprobante(venta);
                ExportacionArchivos.ReciboPdf(guardar.FileName, numero, contenido);
            });
            generado = true;
            await Task.Run(() => AuditoriaDatos.RegistrarExportacion("VENTAS", "Venta", venta.ToString(), "Recibo de la factura " + numero + " exportado a PDF."));
            if (!propietario.IsDisposed) MessageBox.Show(propietario, "Recibo guardado en PDF.", "Exportación", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            if (!propietario.IsDisposed) MessageBox.Show(propietario,
                generado ? "El PDF se guardó, pero no se pudo registrar la exportación en la bitácora. " + ex.Message : "No se pudo exportar el recibo a PDF. " + ex.Message,
                "Exportación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
