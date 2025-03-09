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
    public partial class CategoriasForm: Form
    {
        /// <summary>
        /// Cadena de conexión a la base de datos.
        /// </summary>
        private string connectionString = "Server=(local)\\SQLEXPRESS;Database=TareasDB1;Integrated Security=True;";

        /// <summary>
        /// Constructor del formulario de categorías.
        /// </summary>

        public CategoriasForm()
        {
            InitializeComponent();
            LoadCategorias();
            dtgCategorias.AllowUserToAddRows = false;
        }

      
        // <summary>
        /// Identificador de la categoría seleccionada para edición.
        /// </summary>
        private int categoriaIdSeleccionada = -1; // ID de la categoría en edición

        /// <summary>
        /// Evento que se ejecuta cuando el formulario se carga.
        /// </summary>
        private void CategoriasForm_Load(object sender, EventArgs e)
        {
            TestConexion();
            
        }

        /// <summary>
        /// Prueba la conexión a la base de datos.
        /// </summary>
        private void TestConexion()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
               //     MessageBox.Show("Conexión exitosa con la base de datos.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de conexión: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        /// <summary>
        /// Carga las categorías desde la base de datos y las muestra en la tabla.
        /// </summary>
        private void LoadCategorias()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT id, nombre, descripcion, fechaCreacion FROM Categorias";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dtgCategorias.Columns.Clear();
                    dtgCategorias.DataSource = dt;

                    // 🔹 Evitar filas vacías
                    foreach (DataGridViewRow row in dtgCategorias.Rows)
                    {
                        if (row.IsNewRow) continue; // Ignorar fila de nueva entrada
                        if (row.Cells["nombre"].Value == null || row.Cells["descripcion"].Value == null)
                        {
                            dtgCategorias.Rows.Remove(row); // Eliminar fila vacía
                        }
                    }

                    dtgCategorias.Refresh();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error SQL: " + ex.Message, "Error SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error general: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        

        private void label2_Click(object sender, EventArgs e)
        {

        }




        /// <summary>
        /// Agrega una nueva categoría a la base de datos.
        /// </summary>
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
                string nombre = txtNombre.Text.Trim();
                string descripcion = txtDescripcion.Text.Trim();

                // Validar que los campos no estén vacíos
                if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(descripcion))
                {
                    MessageBox.Show("Los campos Nombre y Descripción no pueden estar vacíos.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // Detener la ejecución si los campos están vacíos
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO Categorias (nombre, descripcion) VALUES (@nombre, @descripcion)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    cmd.Parameters.AddWithValue("@descripcion", descripcion);
                    cmd.ExecuteNonQuery();
                }

                LoadCategorias(); // Recargar la lista de categorías
                LimpiarControles();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error SQL al agregar: " + ex.Message, "Error SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Carga los datos de la categoría seleccionada en los campos de edición.
        /// </summary>
        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dtgCategorias.CurrentRow != null)
            {
                // Guardar el ID de la categoría seleccionada
                categoriaIdSeleccionada = Convert.ToInt32(dtgCategorias.CurrentRow.Cells["id"].Value);
                txtNombre.Text = dtgCategorias.CurrentRow.Cells["nombre"].Value.ToString();
                txtDescripcion.Text = dtgCategorias.CurrentRow.Cells["descripcion"].Value.ToString();

                // Habilitar el botón "Guardar Cambios"
                btnGuardarCambios.Visible = true;
                btnGuardarCambios.Enabled = true;
            }
            else
            {
                MessageBox.Show("Seleccione una categoría para editar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }
        /// <summary>
        /// Elimina la categoría seleccionada de la base de datos.
        /// </summary>
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtgCategorias.CurrentRow != null) // Verifica si hay una fila seleccionada
                {
                    int id = (int)dtgCategorias.CurrentRow.Cells["id"].Value;

                    if (MessageBox.Show("¿Estás seguro de eliminar esta categoría?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        using (SqlConnection conn = new SqlConnection(connectionString))
                        {
                            conn.Open();
                            string query = "DELETE FROM Categorias WHERE ID = @id";
                            SqlCommand cmd = new SqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("@id", id);
                            cmd.ExecuteNonQuery();
                        }
                        LoadCategorias();
                        LimpiarControles();
                    }
                }
                else
                {
                    MessageBox.Show("Seleccione una categoría para eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error SQL al eliminar: " + ex.Message, "Error SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// Limpia los campos de texto del formulario.
        /// </summary>

        private void LimpiarControles()
        {
            txtNombre.Text = "";
            txtDescripcion.Text = "";
        }

        /// <summary>
        /// Guarda los cambios realizados en una categoría seleccionada.
        /// </summary>
        /// <param name="sender">Objeto que genera el evento.</param>
        /// <param name="e">Argumentos del evento.</param>

        private void btnGuardarCambios_Click(object sender, EventArgs e)
        {
            if (categoriaIdSeleccionada == -1)
            {
                MessageBox.Show("No hay ninguna categoría seleccionada para actualizar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string nuevoNombre = txtNombre.Text.Trim();
            string nuevaDescripcion = txtDescripcion.Text.Trim();

            // Validar que los campos no estén vacíos
            if (string.IsNullOrEmpty(nuevoNombre) || string.IsNullOrEmpty(nuevaDescripcion))
            {
                MessageBox.Show("Los campos Nombre y Descripción no pueden estar vacíos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE Categorias SET nombre = @nombre, descripcion = @descripcion WHERE id = @id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@nombre", nuevoNombre);
                    cmd.Parameters.AddWithValue("@descripcion", nuevaDescripcion);
                    cmd.Parameters.AddWithValue("@id", categoriaIdSeleccionada);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Categoría actualizada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Recargar la tabla y actualizar ComboBox en TareasForm
                LoadCategorias();
                ActualizarCategoriasEnTareasForm();

                // Ocultar el botón "Guardar Cambios"
                btnGuardarCambios.Visible = false;
                btnGuardarCambios.Enabled = false;

                // Limpiar campos y resetear variable
                categoriaIdSeleccionada = -1;
                LimpiarControles();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar la categoría: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        /// <summary>
        /// Actualiza la lista de categorías en el formulario de tareas si está abierto.
        /// </summary>
        private void ActualizarCategoriasEnTareasForm()
        {
            foreach (Form frm in Application.OpenForms)
            {
                if (frm is TareasForm tareasForm)
                {
                    tareasForm.LoadCategorias(); // Llama al método de TareasForm
                    break;
                }
            }
        }


        /// <summary>
        /// Maneja el evento de cierre del formulario de categorías y vuelve al formulario de tareas.
        /// </summary>
        /// <param name="sender">Objeto que genera el evento.</param>
        /// <param name="e">Argumentos del evento.</param>

        private void btnSalirCateg_Click(object sender, EventArgs e)
        {
            this.Hide(); // Oculta CategoriasForm

            // Verifica si TareasForm ya está abierto
            foreach (Form frm in Application.OpenForms)
            {
                if (frm is TareasForm tareasForm)
                {
                    tareasForm.Show();
                    return;
                }
            }

            // Si no está abierto, crea una nueva instancia
            TareasForm nuevoTareasForm = new TareasForm();
            nuevoTareasForm.Show();
            this.Close();

        }

        /// <summary>
        /// Evento que se activa cuando el texto de la descripción cambia.
        /// </summary>
        /// <param name="sender">Objeto que genera el evento.</param>
        /// <param name="e">Argumentos del evento.</param>

        private void txtDescripcion_TextChanged(object sender, EventArgs e)
        {

        }
    }  
}

        
    