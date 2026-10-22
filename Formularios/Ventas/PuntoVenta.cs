using System.Drawing.Drawing2D;

namespace Sistema_ERP
{
    public sealed class PuntoVenta : PantallaMaqueta
    {
        public PuntoVenta() : base("Punto de Venta", "Facturación", "Busca un producto y haz clic para agregarlo al carrito. Luego elige cliente, método de pago y procesa.")
        {
            var columnas = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 300));
            columnas.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            var izquierda = Filas(108, 140, -100);
            var busqueda = Tarjeta(108); var buscar = Buscar("Buscar por nombre o código de barras..."); buscar.Dock = DockStyle.Top; busqueda.Controls.Add(buscar);
            var carrito = Tarjeta(140); var vacio = Filas(32, -100); vacio.Controls.Add(Etiqueta("Carrito"), 0, 0); var texto = Etiqueta("El carrito está vacío."); texto.ForeColor = Color.Gray; vacio.Controls.Add(texto, 0, 1); carrito.Controls.Add(vacio);
            izquierda.Controls.Add(busqueda, 0, 0); izquierda.Controls.Add(carrito, 0, 1);
            var resumen = Tarjeta(410); resumen.Padding = new Padding(24, 16, 24, 28);
            var filas = Filas(28, 40, 32, 30, 40, 26, 40, 26, 36, 54);
            ComboBox Opciones(params string[] valores) { var combo = new ComboBox { Dock = DockStyle.Top, DropDownStyle = ComboBoxStyle.DropDownList }; combo.Items.AddRange(valores); combo.SelectedIndex = 0; return combo; }
            filas.Controls.Add(Etiqueta("Cliente"), 0, 0); filas.Controls.Add(Opciones("Cliente general"), 0, 1);
            filas.Controls.Add(Etiqueta("Resumen"), 0, 2); filas.Controls.Add(Etiqueta("Subtotal                    L. 0.00", 11, true), 0, 3);
            filas.Controls.Add(Etiqueta("Total                 L. 0.00", 16, true), 0, 4);
            filas.Controls.Add(Etiqueta("Método de pago", 10), 0, 5); filas.Controls.Add(Opciones("Efectivo", "Tarjeta", "Transferencia"), 0, 6);
            filas.Controls.Add(Etiqueta("Monto recibido", 10), 0, 7);
            filas.Controls.Add(new NumericUpDown { Dock = DockStyle.Top, DecimalPlaces = 2, Maximum = 1000000, ThousandsSeparator = true }, 0, 8);
            var procesar = Boton("Procesar Venta", true); procesar.Name = "procesarVenta"; procesar.Dock = DockStyle.Fill;
            procesar.Click += (_, _) => DialogosMaqueta.Mostrar(FindForm()?.MdiParent ?? this, new ConfirmarVenta());
            filas.Controls.Add(procesar, 0, 9); resumen.Controls.Add(filas);
            columnas.Controls.Add(izquierda, 0, 0); columnas.Controls.Add(resumen, 1, 0); Cuerpo.Controls.Add(columnas);
        }
    }
}
