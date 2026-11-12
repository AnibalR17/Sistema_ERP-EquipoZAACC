namespace Sistema_ERP;

public class FormularioTematico : Form
{
    protected override void OnLoad(EventArgs e)
    {
        if (!ModoDiseno.Activo(this))
        {
            DisposicionControles.Preparar(this);
            TemaAplicacion.Aplicar(this);
        }
        base.OnLoad(e);
    }

    protected override void OnDpiChanged(DpiChangedEventArgs e)
    {
        base.OnDpiChanged(e);
        if (!ModoDiseno.Activo(this)) DisposicionControles.Preparar(this);
    }
}
