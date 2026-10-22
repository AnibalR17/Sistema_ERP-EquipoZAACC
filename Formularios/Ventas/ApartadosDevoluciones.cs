using System.Drawing.Drawing2D;

namespace Sistema_ERP
{
    public sealed class ApartadosDevoluciones : PantallaMaqueta
    {
        public ApartadosDevoluciones() : base("Apartados y devoluciones", "Seguimiento de apartados y devoluciones", "Consulta los apartados y las devoluciones registradas.")
        {
            var tarjeta = Tarjeta(210); var filas = Filas(52, -100, 24);
            var tabs = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
            var tabla = Tabla("Cliente", "Producto", "Fecha", "Estado");
            foreach (var nombre in new[] { "Apartados", "Devoluciones" }) {
                var tab = Boton(nombre, nombre == "Apartados");
                tab.Click += (_, _) => { foreach (Button otro in tabs.Controls) { otro.BackColor = otro == tab ? ProductoEstilo.Texto : ProductoEstilo.Suave; otro.ForeColor = otro == tab ? Color.White : ProductoEstilo.Texto; } };
                tabs.Controls.Add(tab);
            }
            filas.Controls.Add(tabs, 0, 0); filas.Controls.Add(tabla, 0, 1); filas.Controls.Add(Etiqueta("Mostrando 0 registros", 9), 0, 2); tarjeta.Controls.Add(filas); Cuerpo.Controls.Add(tarjeta);
        }
    }
}
