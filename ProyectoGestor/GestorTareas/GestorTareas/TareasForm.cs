using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GestorTareas
{
    public partial class TareasForm: Form
    {

        private string connectionString = "Server=(local)\\SQLEXPRESS;Database=TareasDB1;Integrated Security=True;";

        public TareasForm()
        {
            InitializeComponent();
            CargarDatosIniciales();
            ConfigurarDataGridView();
            dtgTareas.AllowUserToAddRows = false; // Evita que se agregue una fila en blanco automáticamente
            dtgTareas.DataError += (s, e) => { e.Cancel = true; }; // Previene errores de validación
            dtgTareas.CellValidating += dtgTareas_CellValidating;


        }
        private void TareasForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            dtgTareas.EndEdit();  // Finaliza la edición antes de cerrar
            this.Validate();       // Evita validaciones pendientes
        }
        private void ConfigurarDataGridView()
        {
            dtgTareas.Columns.Clear();

            dtgTareas.Columns.Add("colID", "ID");
            dtgTareas.Columns.Add("colTitulo", "Título");
            dtgTareas.Columns.Add("colDescripcion", "Descripción");
            dtgTareas.Columns.Add("colCategoria", "Categoría");
            dtgTareas.Columns.Add("colEstado", "Estado");
            dtgTareas.Columns.Add("colUsuario", "Usuario"); // 🔹 Nueva columna
            dtgTareas.Columns.Add("colFechaCreacion", "Fecha de Creación"); // 🔹 Nueva columna
            dtgTareas.Columns.Add("colFechaVencimiento", "Fecha de Vencimiento");

            dtgTareas.Columns["colID"].DataPropertyName = "ID";
            dtgTareas.Columns["colTitulo"].DataPropertyName = "titulo";
            dtgTareas.Columns["colDescripcion"].DataPropertyName = "descripcion";
            dtgTareas.Columns["colCategoria"].DataPropertyName = "Categoria";
            dtgTareas.Columns["colEstado"].DataPropertyName = "Estado";
            dtgTareas.Columns["colUsuario"].DataPropertyName = "Usuario"; // 🔹 Asociamos con la consulta
            dtgTareas.Columns["colFechaCreacion"].DataPropertyName = "fechaCreacion"; // 🔹 Asociamos con la consulta
            dtgTareas.Columns["colFechaVencimiento"].DataPropertyName = "fechaVencimiento";

            dtgTareas.Columns["colFechaCreacion"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dtgTareas.Columns["colFechaVencimiento"].DefaultCellStyle.Format = "dd/MM/yyyy";
        }


        private void CargarDatosIniciales()
        {
            try
            {
                LoadTareas();
                LoadCategorias();
                LoadEstados();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos iniciales: " + 
                                ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void LoadTareas()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
            SELECT 
                T.ID, 
                T.titulo, 
                T.descripcion, 
                C.nombre AS Categoria, 
                E.nombreEstado AS Estado, 
                U.nombreUsuario AS Usuario, 
                T.fechaCreacion, 
                T.fechaVencimiento 
            FROM Tareas T 
            INNER JOIN Categorias C ON T.categoriaId = C.ID 
            INNER JOIN Estados E ON T.estadoId = E.ID
            INNER JOIN Usuarios U ON T.usuarioId = U.ID"; // 🔹 Unimos con Usuarios

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    System.Data.DataTable dt = new System.Data.DataTable();
                    da.Fill(dt);
                    dtgTareas.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar las tareas: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        public void LoadCategorias()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT id, nombre FROM Categorias";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cmbCategoria.DataSource = dt;
                cmbCategoria.DisplayMember = "nombre";
                cmbCategoria.ValueMember = "id";
            }
        }

        private void LoadEstados()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT ID, nombreEstado FROM Estados";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                System.Data.DataTable dt = new System.Data.DataTable();
                da.Fill(dt);
                cmbEstado.DataSource = dt;
                cmbEstado.DisplayMember = "nombreEstado";
                cmbEstado.ValueMember = "ID";
            }
        }

        private void TareasForm_Load(object sender, EventArgs e)
        {
            
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            string titulo = txtTitulo.Text.Trim();
            string descripcion = txtDescripcion.Text.Trim();

            if (string.IsNullOrWhiteSpace(titulo) || string.IsNullOrWhiteSpace(descripcion) ||
                cmbCategoria.SelectedIndex == -1 || cmbEstado.SelectedIndex == -1)
            {
                MessageBox.Show("Todos los campos son obligatorios.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int categoriaId = (int)cmbCategoria.SelectedValue;
            int usuarioId = 1; // ⚠️ Si usas login, cambia esto por el ID del usuario autenticado
            int estadoId = (int)cmbEstado.SelectedValue;
            DateTime fechaVencimiento = dateTimePicker1.Value;
            DateTime fechaCreacion = DateTime.Now; // 🔹 Guardamos la fecha actual

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO Tareas (titulo, descripcion, categoriaId, usuarioId, estadoId, fechaCreacion, fechaVencimiento) " +
                                   "VALUES (@titulo, @descripcion, @categoriaId, @usuarioId, @estadoId, @fechaCreacion, @fechaVencimiento)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@titulo", titulo);
                    cmd.Parameters.AddWithValue("@descripcion", descripcion);
                    cmd.Parameters.AddWithValue("@categoriaId", categoriaId);
                    cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                    cmd.Parameters.AddWithValue("@estadoId", estadoId);
                    cmd.Parameters.AddWithValue("@fechaCreacion", fechaCreacion);
                    cmd.Parameters.AddWithValue("@fechaVencimiento", fechaVencimiento);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Tarea guardada correctamente.");
                }
                LoadTareas();
                txtTitulo.Clear();
                txtDescripcion.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar la tarea: " + ex.Message);
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {

            if (dtgTareas.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecciona una tarea para editar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow row = dtgTareas.SelectedRows[0];

            txtTitulo.Text = row.Cells["colTitulo"].Value?.ToString() ?? "";
            txtDescripcion.Text = row.Cells["colDescripcion"].Value?.ToString() ?? "";
            cmbCategoria.Text = row.Cells["colCategoria"].Value?.ToString();
            cmbEstado.Text = row.Cells["colEstado"].Value?.ToString();
            dateTimePicker1.Value = row.Cells["colFechaVencimiento"].Value != DBNull.Value
                ? Convert.ToDateTime(row.Cells["colFechaVencimiento"].Value)
                : DateTime.Now;

            txtTitulo.Tag = row.Cells["colID"].Value; // 🔹 Guardamos el ID correctamente

            btnAgregar.Enabled = false;
            btnGuardar.Visible = true;
        }




        private void btnEliminar_Click(object sender, EventArgs e)
        {
            int id = (int)dtgTareas.CurrentRow.Cells[0].Value;

            if (MessageBox.Show("¿Estás seguro de eliminar esta tarea?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "DELETE FROM Tareas WHERE ID = @id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }

                LoadTareas();
            }

        }

        private void cmbCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dtgTareas_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnCategorias_Click(object sender, EventArgs e)
        {
            this.Hide(); // Oculta el formulario actual

            using (CategoriasForm categoriasForm = new CategoriasForm())
            {
                categoriasForm.ShowDialog(); // Muestra el formulario de categorías
            }

            this.Show(); // Vuelve a mostrar el formulario actual
            LoadCategorias(); // Recarga las categorías en el ComboBox
        }

        private void dtgTareas_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            // Evita validación si la fila es nueva o el formulario se está cerrando
            if (dtgTareas.Rows[e.RowIndex].IsNewRow || this.Disposing) return;

            // Verifica si hay celdas vacías en la fila
            foreach (DataGridViewCell cell in dtgTareas.Rows[e.RowIndex].Cells)
            {
                if (cell.Value == null || string.IsNullOrWhiteSpace(cell.Value.ToString()))
                {
                    MessageBox.Show("No se pueden guardar filas con campos vacíos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true; // Cancela la edición de la celda
                    return;
                }
            }
        }
        private void dtgTareas_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (dtgTareas.Columns[e.ColumnIndex].Name != "tuColumnaOpcional") // Evita validar columnas que puedan ser opcionales
            {
                if (string.IsNullOrWhiteSpace(e.FormattedValue.ToString()))
                {
                    MessageBox.Show("No se pueden guardar filas con campos vacíos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true; // Cancela la edición de la celda
                }
            }
        }
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

        private void HabilitarEdicion()
        {
            if (dtgTareas.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dtgTareas.SelectedRows[0];

                txtTitulo.Text = row.Cells["colTitulo"].Value?.ToString() ?? "";
                txtDescripcion.Text = row.Cells["colDescripcion"].Value?.ToString() ?? "";
                cmbCategoria.Text = row.Cells["colCategoria"].Value?.ToString();
                cmbEstado.Text = row.Cells["colEstado"].Value?.ToString();
                dateTimePicker1.Value = row.Cells["colFechaVencimiento"].Value != DBNull.Value
                    ? Convert.ToDateTime(row.Cells["colFechaVencimiento"].Value)
                    : DateTime.Now;

                txtTitulo.Tag = row.Cells["colID"].Value; // 🔹 Guardamos el ID correctamente

                // Deshabilitar el botón "Agregar" y mostrar el botón "Guardar"
                btnAgregar.Enabled = false;
                btnGuardar.Visible = true;
            }
            else
            {
                MessageBox.Show("Selecciona una tarea para editar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void GuardarEdicion()
        {
            if (txtTitulo.Tag != null) // Asegura que hay una tarea en edición
            {
                int tareaId = Convert.ToInt32(txtTitulo.Tag);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE Tareas SET titulo = @titulo, descripcion = @descripcion, categoriaId = @categoriaId, estadoId = @estadoId, fechaVencimiento = @fechaVencimiento WHERE id = @id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@titulo", txtTitulo.Text);
                    cmd.Parameters.AddWithValue("@descripcion", txtDescripcion.Text);
                    cmd.Parameters.AddWithValue("@categoriaId", cmbCategoria.SelectedValue);
                    cmd.Parameters.AddWithValue("@estadoId", cmbEstado.SelectedValue);
                    cmd.Parameters.AddWithValue("@fechaVencimiento", dateTimePicker1.Value);
                    cmd.Parameters.AddWithValue("@id", tareaId);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Tarea actualizada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Recargar la lista de tareas y limpiar los campos
                LoadTareas();
                LimpiarCampos();
            }
        }
        private void LimpiarCampos()
        {
            txtTitulo.Clear();
            txtDescripcion.Clear();
            cmbCategoria.SelectedIndex = -1;
            cmbEstado.SelectedIndex = -1;
            dateTimePicker1.Value = DateTime.Now;
            txtTitulo.Tag = null; // 🔹 Limpiamos el ID guardado

            btnAgregar.Enabled = true;
            btnGuardar.Visible = false; // 🔹 Ocultamos el botón "Guardar"
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (txtTitulo.Tag == null)
            {
                MessageBox.Show("No hay una tarea seleccionada para actualizar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int tareaId = Convert.ToInt32(txtTitulo.Tag);

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"UPDATE Tareas 
                             SET titulo = @titulo, 
                                 descripcion = @descripcion, 
                                 categoriaId = @categoriaId, 
                                 estadoId = @estadoId, 
                                 fechaVencimiento = @fechaVencimiento 
                             WHERE id = @id";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@titulo", txtTitulo.Text.Trim());
                    cmd.Parameters.AddWithValue("@descripcion", txtDescripcion.Text.Trim());
                    cmd.Parameters.AddWithValue("@categoriaId", cmbCategoria.SelectedValue);
                    cmd.Parameters.AddWithValue("@estadoId", cmbEstado.SelectedValue);
                    cmd.Parameters.AddWithValue("@fechaVencimiento", dateTimePicker1.Value);
                    cmd.Parameters.AddWithValue("@id", tareaId);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Tarea actualizada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadTareas(); // 🔄 Recargar la lista después de actualizar
                LimpiarCampos(); // 🧹 Limpiar formulario
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar la tarea: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            // Buscar si el login ya está abierto
            frmLogin loginForm = Application.OpenForms.OfType<frmLogin>().FirstOrDefault();

            if (loginForm == null)
            {
                // Si no está abierto, crearlo y mostrarlo
                loginForm = new frmLogin();
                loginForm.Show();
            }
            else
            {
                // Si está abierto, limpiar los campos y mostrarlo
                loginForm.LimpiarCampos();
                loginForm.Show();
            }

            // Cerrar solo TareasForm
            this.Close();
        }

        private void txtTitulo_TextChanged(object sender, EventArgs e)
        {

        }

   
    }




}
