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
    /// <summary>
    /// Formulario de inicio de sesión para la aplicación Gestor de Tareas.
    /// </summary>
    public partial class frmLogin: Form
    {
        /// <summary>
        /// Cadena de conexión a la base de datos SQL Server.
        /// </summary>
        private string connectionString = "Server=(local)\\SQLEXPRESS;Database=TareasDB1;Integrated Security=True;";

        /// <summary>
        /// Constructor de la clase frmLogin.
        /// Inicializa los componentes y configura eventos clave.
        /// </summary>
        public frmLogin()
        {
            InitializeComponent();
            // Permitir que el formulario capture teclas antes de los controles
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(frmLogin_KeyDown);

            // Agregar eventos KeyDown a los TextBox
            txtUsuario.KeyDown += new KeyEventHandler(txtUsuario_KeyDown);
            txtContrasena.KeyDown += new KeyEventHandler(txtContrasena_KeyDown);
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// Limpia los campos de entrada del formulario.
        /// </summary>
        public void LimpiarCampos()
        {
            txtUsuario.Text = "";
            txtContrasena.Text = "";
        }

        /// <summary>
        /// Evento que maneja el clic en el botón de ingresar.
        /// Valida las credenciales del usuario en la base de datos.
        /// </summary>
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

        /// <summary>
        /// Evento que maneja el clic en el botón de salir.
        /// Cierra la aplicación.
        /// </summary>
        private void btnSalirApp_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        // Evento para capturar Enter en todo el formulario
        private void frmLogin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnIngresar.PerformClick();
            }
        }

        /// <summary>
        /// Evento que captura la tecla Enter en todo el formulario.
        /// Si se presiona Enter, se ejecuta el botón de ingresar.
        /// </summary>
        private void txtUsuario_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnIngresar.PerformClick();
            }
        }

        /// <summary>
        /// Evento que captura la tecla Enter en el campo de contraseña.
        /// Si se presiona Enter, se ejecuta el botón de ingresar.
        /// </summary>
        private void txtContrasena_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnIngresar.PerformClick();
            }
        }
    }
}
