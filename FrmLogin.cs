using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace Sistema_ERP

{
    public partial class FrmLogin : Form
    {
        public FrmLogin()
        {
            InitializeComponent();
        }

        private void FrmLogin_Load(object sender, EventArgs e)
        {
            cmbRol.SelectedIndex = 0;
        }

        private void pnlLogin_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            // Tomamos lo que escribió la persona (Trim quita espacios al inicio y al final)
            string usuario = txtUsuario.Text.Trim();
            string clave = txtClave.Text;

            // 1. Campos vacíos
            if (usuario == "")
            {
                MessageBox.Show("Ingrese el usuario.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsuario.Focus();
                return;
            }

            if (clave == "")
            {
                MessageBox.Show("Ingrese la contraseña.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtClave.Focus();
                return;
            }

            // 2. Longitud mínima
            if (usuario.Length < 3)
            {
                MessageBox.Show("El usuario debe tener al menos 3 caracteres.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsuario.Focus();
                return;
            }

            if (clave.Length < 4)
            {
                MessageBox.Show("La contraseña debe tener al menos 4 caracteres.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtClave.Focus();
                return;
            }

            // 3. El usuario no puede tener espacios
            if (usuario.Contains(" "))
            {
                MessageBox.Show("El usuario no puede contener espacios.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsuario.Focus();
                return;
            }

            // 4. Que haya un rol seleccionado
            if (cmbRol.SelectedIndex == -1)
            {
                MessageBox.Show("Seleccione un rol.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbRol.Focus();
                return;
            }

            // 5. Consulta a la base de datos, protegida con try/catch
            try
            {
                using (SqlConnection cn = ConexionDB.AbrirConexion())
                {
                    string sql = "SELECT id_empleado, nombre, rol FROM Empleado WHERE usuario = @usuario AND clave = @clave";

                    using (SqlCommand cmd = new SqlCommand(sql, cn))
                    {
                        cmd.Parameters.AddWithValue("@usuario", usuario);
                        cmd.Parameters.AddWithValue("@clave", clave);

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                MessageBox.Show("Bienvenido " + dr["nombre"], "Login", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("Usuario o contraseña incorrectos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                txtClave.Clear();
                                txtClave.Focus();
                            }
                        }
                    }
                }
            }
            catch (SqlException)
            {
                MessageBox.Show("No se pudo conectar con la base de datos. Revise la conexión.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
