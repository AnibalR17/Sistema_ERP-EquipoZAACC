using System.ComponentModel;

namespace Sistema_ERP
{
    internal static class ModoDiseno
    {
        // DesignMode alone is false while a designer is constructing the control.
        internal static bool Activo(Control control)
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) return true;
            for (Control? actual = control; actual != null; actual = actual.Parent)
                if (actual.Site?.DesignMode == true) return true;
            return false;
        }
    }
}
