using System.Drawing.Drawing2D;

namespace Sistema_ERP
{
    public sealed class DirectorioRoles : PantallaMaqueta
    {
        public DirectorioRoles() : base("Directorio y roles", "Clientes, proveedores y empleados", "El rol del empleado define que modulos puede ver. Cambia de rol arriba para probarlo.")
        {
            var filas = Filas(52, 190, -100);
            var tabs = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
            var tarjeta = Tarjeta(190); var contenido = Filas(54, -100, 24);
            var acciones = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
            var agregar = Boton("+ Agregar", true); agregar.Width = 125; acciones.Controls.Add(agregar); acciones.Controls.Add(Buscar());
            var tabla = Tabla("Nombre", "Telefono", "Correo");
            contenido.Controls.Add(acciones, 0, 0); contenido.Controls.Add(tabla, 0, 1); contenido.Controls.Add(Etiqueta("Mostrando 0 registros", 9), 0, 2);
            foreach (var nombre in new[] { "Clientes", "Proveedores", "Empleados" })
            {
                var tab = Boton(nombre, nombre == "Clientes"); tab.Width = 130;
                tab.Click += (_, _) => {
                    foreach (Button otro in tabs.Controls) { otro.BackColor = otro == tab ? ProductoEstilo.Texto : ProductoEstilo.Suave; otro.ForeColor = otro == tab ? Color.White : ProductoEstilo.Texto; }
                    tabla.Columns.Clear(); foreach (var campo in nombre == "Empleados" ? new[] { "Nombre", "Telefono", "Correo", "Rol" } : new[] { "Nombre", "Telefono", "Correo" }) tabla.Columns.Add(campo, campo);
                };
                tabs.Controls.Add(tab);
            }
            tarjeta.Controls.Add(contenido); filas.Controls.Add(tabs, 0, 0); filas.Controls.Add(tarjeta, 0, 1); Cuerpo.Controls.Add(filas);
            // La captura y el almacenamiento de datos quedan fuera del alcance de esta pantalla de diseno.
            agregar.Enabled = false;
            agregar.BackColor = ProductoEstilo.Suave;
        }
    }
}
