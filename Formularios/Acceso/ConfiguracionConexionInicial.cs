using Microsoft.Data.SqlClient;
using Sistema_ERP.Clases;

namespace Sistema_ERP;

internal sealed class ConfiguracionConexionInicial : FormularioTematico
{
    private readonly TextBox servidor = new();
    private readonly TextBox baseDatos = new();
    private readonly ComboBox autenticacion = new();
    private readonly TextBox usuarioSql = new();
    private readonly TextBox claveSql = new();
    private readonly CheckBox confiarCertificado = new();
    private readonly Label estado = new();
    private readonly Button probar = new();
    private readonly Button guardar = new();
    private readonly Button cancelar = new();
    private string? cadenaProbada;
    private bool probando;

    internal ConfiguracionConexionInicial(string? cadenaActual, string? errorAnterior)
    {
        Text = "Configuración de conexión · Sistema ERP";
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = true;
        ClientSize = new Size(1000, 700);
        MinimumSize = new Size(1000, 700);
        AutoScaleMode = AutoScaleMode.Dpi;
        Font = new Font("Segoe UI", 10F);
        BackColor = ProductoEstilo.Fondo;
        ForeColor = ProductoEstilo.Texto;

        ConstruirInterfaz();
        CargarCadena(cadenaActual);

        string mensaje = string.IsNullOrWhiteSpace(errorAnterior)
            ? "Configura el acceso a SQL Server y prueba la conexión para continuar."
            : $"No se pudo usar la conexión guardada.\r\n{errorAnterior}";
        MostrarEstado(mensaje, error: !string.IsNullOrWhiteSpace(errorAnterior));

        autenticacion.SelectedIndexChanged += (_, _) =>
        {
            ActualizarAutenticacion();
            ConfiguracionModificada();
        };
        servidor.TextChanged += (_, _) => ConfiguracionModificada();
        baseDatos.TextChanged += (_, _) => ConfiguracionModificada();
        usuarioSql.TextChanged += (_, _) => ConfiguracionModificada();
        claveSql.TextChanged += (_, _) => ConfiguracionModificada();
        confiarCertificado.CheckedChanged += (_, _) => ConfiguracionModificada();
        probar.Click += async (_, _) => await ProbarActualAsync();
        guardar.Click += async (_, _) => await GuardarYContinuarAsync();
        cancelar.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };
        FormClosing += (_, e) => { if (probando) e.Cancel = true; };

        ActualizarAutenticacion();
        AcceptButton = guardar;
        CancelButton = cancelar;
    }

    private void ConstruirInterfaz()
    {
        var distribucion = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = ProductoEstilo.Fondo,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        distribucion.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 350));
        distribucion.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        distribucion.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        Controls.Add(distribucion);

        var panelMarca = new Panel
        {
            Name = "panelConexionInfo",
            Dock = DockStyle.Fill,
            BackColor = ProductoEstilo.Suave
        };
        distribucion.Controls.Add(panelMarca, 0, 0);
        AgregarTexto(panelMarca, "Sistema ERP", new Point(34, 32), new Size(270, 34), 16F, FontStyle.Bold, ProductoEstilo.Texto);
        AgregarTexto(panelMarca, "Conecta tu\nbase de datos", new Point(34, 151), new Size(280, 116), 26F, FontStyle.Bold, ProductoEstilo.Texto);
        AgregarTexto(panelMarca,
            "Configura el servidor que utilizará el sistema para preparar la base y validar los usuarios del ERP.",
            new Point(36, 276), new Size(270, 92), 10.5F, FontStyle.Regular, ProductoEstilo.TextoSecundario);

        AgregarTarjeta(panelMarca, "01  Servidor", "Indica el nombre del servidor y su instancia.", 392);
        AgregarTarjeta(panelMarca, "02  Acceso", "Selecciona autenticación de Windows o de SQL Server.", 468);
        AgregarTarjeta(panelMarca, "03  Verificación", "La conexión se prueba antes de guardar los datos.", 544);
        AgregarTexto(panelMarca, "La configuración se protege para esta cuenta de Windows.",
            new Point(35, 634), new Size(275, 40), 8.5F, FontStyle.Regular, ProductoEstilo.TextoSecundario);

        var formulario = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 9,
            BackColor = ProductoEstilo.Superficie,
            Padding = new Padding(42, 28, 38, 24)
        };
        formulario.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        formulario.RowStyles.Add(new RowStyle(SizeType.Absolute, 78));
        for (int i = 0; i < 5; i++) formulario.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
        formulario.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        formulario.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        formulario.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
        distribucion.Controls.Add(formulario, 1, 0);

        var encabezado = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent, Margin = Padding.Empty };
        AgregarTexto(encabezado, "Configuración inicial", new Point(0, 0), new Size(500, 38), 23F, FontStyle.Bold, ProductoEstilo.Texto);
        AgregarTexto(encabezado, "Conexión técnica; el usuario del ERP se solicita después.", new Point(1, 42), new Size(520, 24), 9.5F, FontStyle.Regular, ProductoEstilo.TextoSecundario);
        formulario.Controls.Add(encabezado, 0, 0);

        servidor.Name = "txtServidor";
        servidor.PlaceholderText = @"Ejemplo: localhost\SQLEXPRESS";
        servidor.Text = @"localhost\SQLEXPRESS";
        servidor.AccessibleName = "Servidor o instancia de SQL Server";
        formulario.Controls.Add(CrearCampo("Servidor o instancia", servidor), 0, 1);

        baseDatos.Name = "txtBaseDatos";
        baseDatos.PlaceholderText = "Ejemplo: EquipoZAACC";
        baseDatos.Text = "EquipoZAACC";
        baseDatos.AccessibleName = "Nombre de la base de datos";
        formulario.Controls.Add(CrearCampo("Base de datos del ERP", baseDatos), 0, 2);

        autenticacion.Name = "cmbAutenticacion";
        autenticacion.DropDownStyle = ComboBoxStyle.DropDownList;
        autenticacion.Items.AddRange(["Autenticación de Windows", "Usuario de SQL Server"]);
        autenticacion.SelectedIndex = 0;
        autenticacion.AccessibleName = "Tipo de autenticación de SQL Server";
        formulario.Controls.Add(CrearCampo("Método de autenticación", autenticacion), 0, 3);

        usuarioSql.Name = "txtUsuarioSql";
        usuarioSql.PlaceholderText = "Usuario de SQL Server";
        usuarioSql.AccessibleName = "Usuario de SQL Server";
        formulario.Controls.Add(CrearCampo("Usuario de SQL Server", usuarioSql), 0, 4);

        claveSql.Name = "txtClaveSql";
        claveSql.PlaceholderText = "Contraseña de SQL Server";
        claveSql.UseSystemPasswordChar = true;
        claveSql.AccessibleName = "Contraseña de SQL Server";
        formulario.Controls.Add(CrearCampo("Contraseña de SQL Server", claveSql), 0, 5);

        confiarCertificado.Name = "chkConfiarCertificado";
        confiarCertificado.AutoSize = true;
        confiarCertificado.Text = "Confiar en un certificado local (solo pruebas)";
        confiarCertificado.ForeColor = ProductoEstilo.TextoSecundario;
        confiarCertificado.Margin = new Padding(0, 6, 0, 0);
        formulario.Controls.Add(confiarCertificado, 0, 6);

        estado.Name = "lblEstadoConexion";
        estado.AutoSize = false;
        estado.Dock = DockStyle.Fill;
        estado.TextAlign = ContentAlignment.MiddleLeft;
        estado.Padding = new Padding(12, 8, 10, 8);
        estado.BackColor = ProductoEstilo.Suave;
        estado.Font = new Font("Segoe UI", 9.2F);
        estado.AutoEllipsis = true;
        formulario.Controls.Add(estado, 0, 7);

        var acciones = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            AutoSize = false,
            BackColor = Color.Transparent,
            Margin = Padding.Empty,
            Padding = new Padding(0, 8, 0, 0)
        };
        PrepararBoton(guardar, "Guardar y continuar", principal: true);
        PrepararBoton(probar, "Probar conexión");
        PrepararBoton(cancelar, "Cancelar");
        acciones.Controls.Add(guardar);
        acciones.Controls.Add(probar);
        acciones.Controls.Add(cancelar);
        formulario.Controls.Add(acciones, 0, 8);
    }

    private static void AgregarTexto(Control padre, string texto, Point ubicacion, Size tamano,
        float tamanoFuente, FontStyle estilo, Color color)
    {
        var etiqueta = new Label
        {
            AutoSize = false,
            BackColor = Color.Transparent,
            Font = new Font("Segoe UI", tamanoFuente, estilo),
            ForeColor = color,
            Location = ubicacion,
            Size = tamano,
            Text = texto,
            TextAlign = ContentAlignment.MiddleLeft
        };
        padre.Controls.Add(etiqueta);
    }

    private static void AgregarTarjeta(Control padre, string titulo, string descripcion, int y)
    {
        var tarjeta = new Panel
        {
            BackColor = ProductoEstilo.Superficie,
            Location = new Point(30, y),
            Size = new Size(282, 62)
        };
        AgregarTexto(tarjeta, titulo, new Point(12, 6), new Size(244, 22), 9.5F, FontStyle.Bold, ProductoEstilo.Texto);
        AgregarTexto(tarjeta, descripcion, new Point(12, 29), new Size(248, 28), 8F, FontStyle.Regular, ProductoEstilo.TextoSecundario);
        padre.Controls.Add(tarjeta);
    }

    private static Control CrearCampo(string etiqueta, Control entrada)
    {
        var campo = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Margin = Padding.Empty,
            Padding = Padding.Empty,
            BackColor = Color.Transparent
        };
        campo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        campo.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
        campo.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        campo.Controls.Add(new Label
        {
            AutoSize = false,
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent,
            ForeColor = ProductoEstilo.TextoSecundario,
            Font = new Font("Segoe UI", 9F),
            Text = etiqueta,
            TextAlign = ContentAlignment.MiddleLeft,
            Margin = Padding.Empty
        }, 0, 0);
        entrada.Dock = DockStyle.Top;
        entrada.Margin = Padding.Empty;
        entrada.Font = new Font("Segoe UI", 10F);
        entrada.Height = 34;
        if (entrada is TextBox texto)
        {
            texto.BorderStyle = BorderStyle.FixedSingle;
            texto.BackColor = ProductoEstilo.Campo;
            texto.ForeColor = ProductoEstilo.Texto;
        }
        campo.Controls.Add(entrada, 0, 1);
        return campo;
    }

    private static void PrepararBoton(Button boton, string texto, bool principal = false)
    {
        boton.Text = texto;
        boton.Margin = new Padding(8, 0, 0, 0);
        boton.Font = new Font("Segoe UI", 9.2F);
        ProductoEstilo.Boton(boton, principal);
        boton.MinimumSize = new Size(texto == "Guardar y continuar" ? 168 : 142, 42);
    }

    private void CargarCadena(string? cadena)
    {
        if (string.IsNullOrWhiteSpace(cadena)) return;
        try
        {
            var builder = new SqlConnectionStringBuilder(cadena);
            if (!string.IsNullOrWhiteSpace(builder.DataSource)) servidor.Text = builder.DataSource;
            if (!string.IsNullOrWhiteSpace(builder.InitialCatalog)
                && !new[] { "master", "model", "msdb", "tempdb" }.Contains(builder.InitialCatalog, StringComparer.OrdinalIgnoreCase))
                baseDatos.Text = builder.InitialCatalog;
            autenticacion.SelectedIndex = builder.IntegratedSecurity ? 0 : 1;
            usuarioSql.Text = builder.UserID;
            claveSql.Text = builder.Password;
            confiarCertificado.Checked = builder.TrustServerCertificate;
        }
        catch (ArgumentException)
        {
            // La pantalla conserva valores de ejemplo cuando la configuración anterior está mal formada.
        }
    }

    private void ActualizarAutenticacion()
    {
        bool usaSql = autenticacion.SelectedIndex == 1;
        usuarioSql.Enabled = claveSql.Enabled = usaSql;
    }

    private void ConfiguracionModificada()
    {
        cadenaProbada = null;
        if (IsHandleCreated && !IsDisposed)
            MostrarEstado("Hay cambios pendientes. Prueba la conexión para continuar.");
    }

    private string CrearCadenaConexion()
    {
        if (string.IsNullOrWhiteSpace(servidor.Text))
            throw new InvalidOperationException("Escribe el servidor o la instancia de SQL Server.");
        if (string.IsNullOrWhiteSpace(baseDatos.Text))
            throw new InvalidOperationException("Escribe el nombre de la base de datos del ERP.");

        bool integrada = autenticacion.SelectedIndex == 0;
        if (!integrada && string.IsNullOrWhiteSpace(usuarioSql.Text))
            throw new InvalidOperationException("Escribe el usuario de SQL Server.");
        if (!integrada && string.IsNullOrEmpty(claveSql.Text))
            throw new InvalidOperationException("Escribe la contraseña de SQL Server.");

        var builder = new SqlConnectionStringBuilder
        {
            DataSource = servidor.Text.Trim(),
            InitialCatalog = baseDatos.Text.Trim(),
            IntegratedSecurity = integrada,
            TrustServerCertificate = confiarCertificado.Checked,
            ConnectTimeout = 8,
            PersistSecurityInfo = false,
            ApplicationName = "Sistema ERP"
        };
        builder["Encrypt"] = "True";
        if (!integrada)
        {
            builder.UserID = usuarioSql.Text.Trim();
            builder.Password = claveSql.Text;
        }
        return builder.ConnectionString;
    }

    private async Task<bool> ProbarActualAsync()
    {
        if (probando) return false;
        string cadena;
        try { cadena = CrearCadenaConexion(); }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            cadenaProbada = null;
            MostrarEstado(ex.Message, error: true);
            return false;
        }

        probando = true;
        probar.Enabled = guardar.Enabled = cancelar.Enabled = false;
        UseWaitCursor = true;
        MostrarEstado("Probando acceso al servidor y a la base de datos…");
        try
        {
            string resultado = await Task.Run(() => ValidadorConexion.Verificar(cadena));
            bool sigueVigente = string.Equals(cadena, CrearCadenaConexion(), StringComparison.Ordinal);
            cadenaProbada = sigueVigente ? cadena : null;
            MostrarEstado(sigueVigente ? resultado : "Los datos cambiaron durante la prueba. Vuelve a probar.", exito: sigueVigente);
            return sigueVigente;
        }
        catch (Exception ex)
        {
            cadenaProbada = null;
            MostrarEstado(DiagnosticoConexion.Describir(ex), error: true);
            return false;
        }
        finally
        {
            probando = false;
            if (!IsDisposed)
            {
                UseWaitCursor = false;
                probar.Enabled = guardar.Enabled = cancelar.Enabled = true;
            }
        }
    }

    private async Task GuardarYContinuarAsync()
    {
        string cadena;
        try { cadena = CrearCadenaConexion(); }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            MostrarEstado(ex.Message, error: true);
            return;
        }

        if (!string.Equals(cadenaProbada, cadena, StringComparison.Ordinal)
            && !await ProbarActualAsync()) return;

        try
        {
            Conexion.GuardarCadenaConexion(cadena);
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            MostrarEstado($"No se pudo guardar la configuración: {ex.Message}", error: true);
        }
    }

    private void MostrarEstado(string mensaje, bool error = false, bool exito = false)
    {
        estado.Text = mensaje;
        estado.ForeColor = error ? ProductoEstilo.Error : exito ? ProductoEstilo.Exito : ProductoEstilo.TextoSecundario;
    }
}
