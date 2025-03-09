using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GestorTareas
{
    public partial class frmLogin: Form
    {
        private string connectionString = "Server=(local)\\SQLEXPRESS;Database=TareasDB1;Integrated Security=True;";


        public frmLogin()
        {
            InitializeComponent();
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {

        }
        public void LimpiarCampos()
        {
            txtUsuario.Text = "";
            txtContrasena.Text = "";
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            string nombreUsuario = txtUsuario.Text;
            string contrasena = txtContrasena.Text;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM Usuarios WHERE nombreUsuario = @nombreUsuario " +
                               "AND contrasena = @contrasena";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@nombreUsuario", nombreUsuario);
                cmd.Parameters.AddWithValue("@contrasena", contrasena);

                int count = (int)cmd.ExecuteScalar();

                if (count > 0)
                {
                    // Mostrar pantalla de bienvenida
                    BienvenidaForm bienvenida = new BienvenidaForm();
                    bienvenida.ShowDialog();

                    // Después de cerrar la bienvenida, mostrar las tareas
                    TareasForm tareasForm = new TareasForm();
                    tareasForm.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Credenciales Incorrectas.");
                }

            }
        }

        private void btnSalirApp_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
