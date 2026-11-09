using System.Globalization;

namespace Sistema_ERP;

public partial class Pagina_Central
{
    private readonly Button actualizarInicio = new() { Name = "actualizarInicio", Text = "Actualizar resumen", Size = new Size(180, 40), Top = 16, Anchor = AnchorStyles.Top | AnchorStyles.Right };
    private readonly System.Windows.Forms.Timer refrescoInicio = new() { Interval = 60000 };
    private DiaInicio[] diasInicio = [];
    private bool consultaInicioActiva, repetirConsultaInicio;

    private void ConfigurarInicio()
    {
        cardTitle0.Text = "Ventas del día"; cardTitle1.Text = "Compras del día";
        cardTitle2.Text = "Traspasos pendientes"; cardTitle3.Text = "Alertas de stock";
        cardDetail0.Text = cardDetail1.Text = "Facturas confirmadas";
        cardDetail2.Text = "Esperan aprobación"; cardDetail3.Text = "Disponible bajo mínimo";
        chartTitleLabel.Text = "Ventas y compras · últimos 7 días";
        chartTitleLabel.Width = 420;
        MostrarInicioSinDatos("Cargando resumen de ventas y compras…");
        headingPanel.Controls.Add(actualizarInicio);
        void ColocarActualizar() => actualizarInicio.Left = Math.Max(0, btnprobarconexion.Left - actualizarInicio.Width - 10);
        headingPanel.Resize += (_, _) => ColocarActualizar(); ColocarActualizar();
        actualizarInicio.Click += async (_, _) => await ActualizarInicio();
        inicio!.Activated += async (_, _) => { refrescoInicio.Start(); await ActualizarInicio(); };
        inicio.Deactivate += (_, _) => refrescoInicio.Stop();
        TiendaSeleccionadaChanged += async (_, _) => await ActualizarInicio();
        Shown += async (_, _) => await ActualizarInicio();
        refrescoInicio.Tick += async (_, _) => await ActualizarInicio();
        FormClosed += (_, _) => refrescoInicio.Dispose();

        void EnlazarTarjeta(Panel tarjeta, Action abrir)
        {
            foreach (Control control in tarjeta.Controls.Cast<Control>().Append(tarjeta))
            { control.Cursor = Cursors.Hand; control.Click += (_, _) => abrir(); }
        }
        EnlazarTarjeta(cardPanel0, () => salesButton_Click(this, EventArgs.Empty));
        EnlazarTarjeta(cardPanel1, () => comprasButton_Click(this, EventArgs.Empty));
        EnlazarTarjeta(cardPanel2, () => { if (permisos.Puede(Modulos.Validacion)) MostrarValidacion(); else MostrarTraspaso(); });
        EnlazarTarjeta(cardPanel3, () => notificationButton_Click(this, EventArgs.Empty));
    }

    private async Task ActualizarInicio()
    {
        if (IsDisposed || Disposing || acceso is null || ActiveMdiChild != inicio) return;
        repetirConsultaInicio = true;
        if (consultaInicioActiva) return;
        consultaInicioActiva = true; actualizarInicio.Enabled = false;
        try
        {
            do
            {
                repetirConsultaInicio = false;
                int? tienda = IdTiendaSeleccionada;
                DateTime hoy = InicioDatos.AhoraHonduras.Date;
                infoLabel.Text = "Actualizando ventas y compras…";
                try
                {
                    var resumen = await Task.Run(() => InicioDatos.Obtener(tienda, hoy));
                    if (IsDisposed || Disposing) return;
                    if (tienda != IdTiendaSeleccionada || hoy != InicioDatos.AhoraHonduras.Date)
                    { repetirConsultaInicio = true; continue; }
                    AplicarPermisos(resumen.Permisos);
                    diasInicio = resumen.Dias;
                    var dia = diasInicio[^1];
                    cardValue0.Text = $"L {dia.Ventas:N2}"; cardDetail0.Text = $"{dia.FacturasVenta:N0} facturas confirmadas";
                    cardValue1.Text = $"L {dia.Compras:N2}"; cardDetail1.Text = $"{dia.FacturasCompra:N0} facturas confirmadas";
                    cardValue2.Text = resumen.TraspasosPendientes.ToString("N0");
                    cardValue3.Text = resumen.AlertasStock.ToString("N0");
                    string alcance = tienda.HasValue ? selectorTienda.Text : "Todas las tiendas";
                    infoLabel.ForeColor = ProductoEstilo.TextoSecundario;
                    infoLabel.Text = $"{alcance} · Confirmadas · {hoy:dd/MM/yyyy} (Honduras) · Actualizado {InicioDatos.AhoraHonduras:HH:mm:ss}";
                    chartPanel.Invalidate();
                }
                catch (Exception ex)
                {
                    if (IsDisposed || Disposing) return;
                    MostrarInicioSinDatos("No se pudo cargar el resumen. Usa Actualizar resumen para reintentar. " + ex.Message);
                }
            } while (repetirConsultaInicio && ActiveMdiChild == inicio);
        }
        finally
        {
            consultaInicioActiva = false;
            if (!IsDisposed && !Disposing) actualizarInicio.Enabled = true;
        }
    }

    private void MostrarInicioSinDatos(string mensaje)
    {
        foreach (var etiqueta in new[] { cardValue0, cardValue1, cardValue2, cardValue3 }) etiqueta.Text = "—";
        cardDetail0.Text = cardDetail1.Text = "Sin datos cargados";
        diasInicio = [];
        infoLabel.Text = mensaje;
        chartPanel.Invalidate();
    }

    private void DibujarResumenInicio(Graphics g)
    {
        if (chartPanel.Width < 240 || chartPanel.Height < 180) return;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        using var fuente = new Font("Segoe UI", 9F);
        using var ventas = new Pen(ProductoEstilo.Texto, 2.5f);
        using var compras = new Pen(Color.SteelBlue, 2.5f) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash };
        if (permisos.Puede(Modulos.Ventas)) { g.DrawLine(ventas, 28, 62, 53, 62); g.DrawString("Ventas", fuente, Brushes.Black, 59, 53); }
        if (permisos.Puede(Modulos.Compras)) { g.DrawLine(compras, 143, 62, 168, 62); g.DrawString("Compras", fuente, Brushes.SteelBlue, 174, 53); }
        if (diasInicio.Length == 0)
        { g.DrawString("El gráfico se mostrará al cargar el resumen.", fuente, Brushes.Gray, 28, 95); return; }
        float izquierda = 100, derecha = chartPanel.Width - 30, arriba = 95, abajo = chartPanel.Height - 50;
        decimal maximo = Math.Max(1, diasInicio.Max(d => Math.Max(d.Ventas, d.Compras)));
        using var centrado = new StringFormat { Alignment = StringAlignment.Center };
        using var alineado = new StringFormat { Alignment = StringAlignment.Far };
        for (int i = 0; i <= 4; i++)
        {
            float y = abajo - (abajo - arriba) * i / 4;
            g.DrawLine(Pens.Gainsboro, izquierda, y, derecha, y);
            g.DrawString($"L {maximo * i / 4:N0}", fuente, Brushes.Gray, new RectangleF(0, y - 8, izquierda - 8, 20), alineado);
        }
        PointF[] Puntos(Func<DiaInicio, decimal> importe) => diasInicio.Select((dia, i) => new PointF(
            izquierda + (derecha - izquierda) * i / (diasInicio.Length - 1),
            abajo - (abajo - arriba) * (float)(importe(dia) / maximo))).ToArray();
        var puntosVentas = Puntos(d => d.Ventas); var puntosCompras = Puntos(d => d.Compras);
        if (permisos.Puede(Modulos.Compras)) g.DrawLines(compras, puntosCompras);
        if (permisos.Puede(Modulos.Ventas)) g.DrawLines(ventas, puntosVentas);
        for (int i = 0; i < diasInicio.Length; i++)
        {
            if (permisos.Puede(Modulos.Compras)) g.FillEllipse(Brushes.SteelBlue, puntosCompras[i].X - 3, puntosCompras[i].Y - 3, 6, 6);
            if (permisos.Puede(Modulos.Ventas)) g.FillEllipse(Brushes.Black, puntosVentas[i].X - 3, puntosVentas[i].Y - 3, 6, 6);
            g.DrawString(diasInicio[i].Fecha.ToString("dd/MM", CultureInfo.InvariantCulture), fuente, Brushes.Gray,
                new PointF(puntosVentas[i].X, abajo + 12), centrado);
        }
    }
}
