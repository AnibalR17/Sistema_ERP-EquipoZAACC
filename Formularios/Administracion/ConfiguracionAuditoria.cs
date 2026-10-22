using System.Drawing.Drawing2D;

namespace Sistema_ERP
{
    public sealed class ConfiguracionAuditoria : PantallaMaqueta
    {
        public ConfiguracionAuditoria() : base("Configuracion y auditoria", "Bitacora del sistema", "Registro de acciones: quien, cuando y que modulo.")
        {
            var tarjeta = Tarjeta(165); var filas = Filas(44, -100, 24);
            var buscar = Buscar(); buscar.Dock = DockStyle.Left;
            filas.Controls.Add(buscar, 0, 0); filas.Controls.Add(Tabla("Fecha/Hora", "Usuario", "Modulo", "Descripcion"), 0, 1); filas.Controls.Add(Etiqueta("Mostrando 0 registros", 9), 0, 2);
            tarjeta.Controls.Add(filas); Cuerpo.Controls.Add(tarjeta);
        }
    }
}
