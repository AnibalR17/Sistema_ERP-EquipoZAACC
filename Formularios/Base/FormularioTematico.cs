namespace Sistema_ERP;

public class FormularioTematico : Form
{
    protected override void OnLoad(EventArgs e)
    {
        if (!ModoDiseno.Activo(this)) TemaAplicacion.Aplicar(this);
        base.OnLoad(e);
    }
}
