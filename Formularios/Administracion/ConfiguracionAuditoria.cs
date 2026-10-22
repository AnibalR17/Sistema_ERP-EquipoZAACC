using System.Drawing.Drawing2D;

namespace Sistema_ERP
{
    public sealed class ConfiguracionAuditoria : PantallaMaqueta
    {
        public ConfiguracionAuditoria() : base("Configuración y auditoría", "Bitácora del sistema", "Registro de acciones: quién, cuándo y qué módulo.")
        {
            var tarjeta = Tarjeta(165); var filas = Filas(44, -100, 24);
            var buscar = Buscar(); buscar.Dock = DockStyle.Left;
            filas.Controls.Add(buscar, 0, 0); filas.Controls.Add(Tabla("Fecha/Hora", "Usuario", "Módulo", "Descripción"), 0, 1); filas.Controls.Add(Etiqueta("Mostrando 0 registros", 9), 0, 2);
            tarjeta.Controls.Add(filas); Cuerpo.Controls.Add(tarjeta);
        }
    }
}
