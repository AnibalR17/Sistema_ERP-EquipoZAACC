using Microsoft.Data.SqlClient;
using Sistema_ERP.Clases;

namespace Sistema_ERP;

internal sealed class GestionarPasskeys : FormularioTematico
{
    private sealed record ElementoPasskey
    {
        internal byte[] Id { get; init; }
        internal string Nombre { get; init; }

        internal ElementoPasskey(byte[] id, string nombre) => (Id, Nombre) = (id, nombre);

        public override string ToString() => Nombre;
    }

    private readonly AccesoUsuario acceso;
    private readonly TextBox clave = new() { UseSystemPasswordChar = true, MaxLength = 128, Width = 250 };
    private readonly TextBox nombrePasskey = new() { Text = "Passkey", MaxLength = 100, Width = 250 };
    private readonly Panel contenedorLista = new() { Location = new Point(26, 276), Size = new Size(548, 140) };
    private readonly ListBox lista = new() { IntegralHeight = false, Dock = DockStyle.Fill };
    private readonly Label estado = new() { AutoSize = false, Size = new Size(430, 32), TextAlign = ContentAlignment.MiddleLeft };

    internal GestionarPasskeys(AccesoUsuario acceso)
    {
        this.acceso = acceso;
        Text = "Mis passkeys";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = MinimizeBox = false;
        ClientSize = new Size(600, 510);

        var titulo = new Label { Text = "Passkeys de esta cuenta", AutoSize = true, Font = new Font("Segoe UI", 16, FontStyle.Bold), Location = new Point(24, 20) };
        var ayuda = new Label
        {
            Text = "Registra una passkey con Windows Hello en este equipo, con el teléfono o con una llave de seguridad FIDO2. Windows abre la pantalla segura para completar el registro.",
            AutoSize = false, Location = new Point(26, 64), Size = new Size(548, 48)
        };
        var lblClave = new Label { Text = "Confirma tu contraseña actual", AutoSize = true, Location = new Point(26, 126) };
        clave.Location = new Point(26, 151);
        var lblNombre = new Label { Text = "Nombre para identificarlo", AutoSize = true, Location = new Point(300, 126) };
        nombrePasskey.Location = new Point(300, 151);

        var registrar = new Button { Text = "Registrar passkey", Location = new Point(26, 195), Size = new Size(250, 38), UseVisualStyleBackColor = true };
        registrar.Click += Registrar_Click;
        var quitar = new Button { Text = "Revocar seleccionado", Location = new Point(300, 195), Size = new Size(250, 38), UseVisualStyleBackColor = true };
        quitar.Click += Revocar_Click;

        var lblLista = new Label { Text = "Passkeys activos", AutoSize = true, Location = new Point(26, 250) };
        contenedorLista.Controls.Add(lista);
        var nota = new Label
        {
            Text = "Windows Hello guarda la passkey en este equipo. El acceso por QR con teléfono requiere Windows 11 23H2 o posterior y Bluetooth en ambos dispositivos.",
            AutoSize = false, Location = new Point(26, 426), Size = new Size(548, 32)
        };
        var cerrar = new Button { Text = "Cerrar", DialogResult = DialogResult.Cancel, Location = new Point(474, 455), Size = new Size(100, 30), UseVisualStyleBackColor = true };
        estado.Location = new Point(26, 464);
        cerrar.Location = new Point(474, 465);

        Controls.AddRange([titulo, ayuda, lblClave, clave, lblNombre, nombrePasskey, registrar, quitar, lblLista, contenedorLista, nota, cerrar, estado]);
        CancelButton = cerrar;
        Load += (_, _) => Recargar();
    }

    private void Registrar_Click(object? sender, EventArgs e)
    {
        string nombre = nombrePasskey.Text.Trim();
        if (string.IsNullOrWhiteSpace(nombre))
        {
            MostrarEstado("Escribe un nombre para identificar este passkey.", true);
            nombrePasskey.Focus();
            return;
        }
        if (!ValidarClave()) return;

        try
        {
            var identidad = PasskeyDatos.IdentidadActual(acceso);
            var existentes = PasskeyDatos.CredencialesActuales(acceso);
            MostrarEstado("Sigue las indicaciones de Windows para elegir dónde registrar la passkey.", false);
            var registro = Passkeys.Registrar(Handle, identidad, existentes);
            PasskeyDatos.Guardar(acceso, registro, nombre);
            nombrePasskey.Text = "Passkey";
            MostrarEstado("El passkey quedó registrado en esta cuenta.", false);
            Recargar();
        }
        catch (OperationCanceledException) { MostrarEstado("Se canceló el registro; no se guardaron cambios.", false); }
        catch (Exception ex)
        {
            MostrarEstado("No se pudo completar el registro del passkey.", true);
            MessageBox.Show(this,
                $"No se pudo completar el registro del passkey.\n\n{ex.Message}\n\nSi el error menciona PasskeyUsuario, aplica Datos/Migraciones/006_passkeys.sql.",
                "Error al registrar passkey", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        finally { clave.Clear(); }
    }

    private void Revocar_Click(object? sender, EventArgs e)
    {
        if (lista.SelectedItem is not ElementoPasskey seleccionado)
        {
            MostrarEstado("Selecciona un passkey de la lista.", true);
            return;
        }
        if (!ValidarClave()) return;
        if (MessageBox.Show(this, $"¿Revocar «{seleccionado.Nombre}»? Ya no podrá usarse para iniciar sesión.",
            "Revocar passkey", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
        {
            clave.Clear();
            return;
        }
        try
        {
            PasskeyDatos.Revocar(acceso, seleccionado.Id);
            MostrarEstado("El passkey fue revocado.", false);
            Recargar();
        }
        catch (Exception) { MostrarEstado("No se pudo revocar el passkey. Recarga la lista e inténtalo nuevamente.", true); }
        finally { clave.Clear(); }
    }

    private bool ValidarClave()
    {
        if (string.IsNullOrWhiteSpace(clave.Text))
        {
            MostrarEstado("Confirma tu contraseña actual antes de continuar.", true);
            clave.Focus();
            return false;
        }
        try
        {
            if (PasskeyDatos.ValidarClaveActual(acceso, clave.Text)) return true;
            clave.Clear();
            MostrarEstado("La contraseña no coincide o la cuenta está bloqueada.", true);
            clave.Focus();
        }
        catch (Exception) { clave.Clear(); MostrarEstado("No se pudo validar la contraseña actual.", true); }
        return false;
    }

    private void Recargar()
    {
        try
        {
            lista.Items.Clear();
            foreach (var fila in PasskeyDatos.Listar(acceso))
            {
                string fecha = HoraLocal(fila.CreadaEn).ToString("g");
                string ultimoUso = fila.UltimoUsoEn is DateTime ultimo ? $" · usado {HoraLocal(ultimo):g}" : " · sin usos";
                lista.Items.Add(new ElementoPasskey(fila.Id, $"{fila.Nombre} · agregado {fecha}{ultimoUso}"));
            }
            if (lista.Items.Count == 0) MostrarEstado("Todavía no hay passkeys activos en esta cuenta.", false);
        }
        catch (SqlException ex) when (ex.Number == 208)
        {
            MostrarEstado("No se encontró la tabla de passkeys. Cierra la sesión y vuelve a abrir el programa para aplicar la migración 006_passkeys.sql.", true);
        }
        catch (Exception ex)
        {
            MostrarEstado($"No se pudo cargar la lista. {DiagnosticoConexion.Describir(ex)}", true);
        }
    }

    private static DateTime HoraLocal(DateTime fechaUtc) => fechaUtc.Kind switch
    {
        DateTimeKind.Utc => fechaUtc.ToLocalTime(),
        DateTimeKind.Local => fechaUtc,
        _ => DateTime.SpecifyKind(fechaUtc, DateTimeKind.Utc).ToLocalTime()
    };

    private void MostrarEstado(string mensaje, bool error)
    {
        estado.Text = mensaje;
        estado.ForeColor = error ? ProductoEstilo.Error : SystemColors.GrayText;
    }
}
