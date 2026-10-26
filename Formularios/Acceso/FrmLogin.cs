using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoginERP
{
    public partial class FrmLogin : Form
    {
        public FrmLogin()
        {
            InitializeComponent();
            if (Sistema_ERP.ModoDiseno.Activo(this)) return;
        }

        private void FrmLogin_Load(object sender, EventArgs e)
        {
            if (Sistema_ERP.ModoDiseno.Activo(this)) return;
            cmbRol.SelectedIndex = 0;
        }

        private void pnlLogin_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            if (txtUsuario.Text == "" || txtClave.Text == "")
            {
                MessageBox.Show("Complete el usuario y la contrasena");
                return;
            }

            MessageBox.Show("Datos listos para validar");
        }

        private void cmbRol_SelectedIndexChanged(object sender, System.EventArgs e)
        {

        }
    }
}
