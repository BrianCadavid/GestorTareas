using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GestorTareas
{

    /// <summary>
    /// Formulario de bienvenida para la aplicación Gestor de Tareas.
    /// </summary>
    public partial class BienvenidaForm : Form
    {
        /// <summary>
        /// Constructor del formulario de bienvenida.
        /// </summary>
        public BienvenidaForm()
        {
            InitializeComponent();
            ConfigurarFormulario();
        }


        /// <summary>
        /// Configura el diseño y los elementos del formulario de bienvenida.
        /// </summary>

        private void ConfigurarFormulario()
        {
            // Configuración de la ventana
            this.Text = "Bienvenido";
            this.Size = new Size(420, 280);
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(42, 126, 187);
            this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, this.Width, this.Height, 25, 25));
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(42, 126, 187);
            this.Padding = new Padding(5); // Margen interno

            // Evento Paint para dibujar el borde y fondo con degradado
            this.Paint += (s, e) =>
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle, Color.FromArgb(30, 100, 180), Color.FromArgb(60, 140, 220), 45F))
                {
                    e.Graphics.FillRectangle(brush, this.ClientRectangle);
                }
                using (Pen pen = new Pen(Color.Black, 2)) // Borde blanco
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
                }
            };


            // Label de bienvenida
            Label lblTitulo = new Label();
            lblTitulo.Text = "🔥¡Bienvenido al Gestor de Tareas!🔥";
            lblTitulo.Font = new Font("Arial", 14, FontStyle.Bold);
            lblTitulo.ForeColor = Color.White;
            lblTitulo.AutoSize = true;
            lblTitulo.Location = new Point(20, 30);
            this.Controls.Add(lblTitulo);

            // Crear iconos amarillos y texto blanco
            this.Controls.Add(CrearIcono(50, 100));
            this.Controls.Add(CrearTexto("Administra tus tareas", 70, 100));
            this.Controls.Add(CrearIcono(50, 130));
            this.Controls.Add(CrearTexto("Organiza tu día", 70, 130));
            this.Controls.Add(CrearIcono(50, 160));
            this.Controls.Add(CrearTexto("¡Sé más productivo!", 70, 160));

            // Botón de continuar
            Button btnContinuar = new Button();
            btnContinuar.Text = "Continuar";
            btnContinuar.Font = new Font("Arial", 12, FontStyle.Bold);
            btnContinuar.Size = new Size(140, 40);
            btnContinuar.Location = new Point(135, 200);
            btnContinuar.BackColor = Color.White;
            btnContinuar.ForeColor = Color.Black;
            btnContinuar.FlatAppearance.BorderColor = Color.Black;
            btnContinuar.FlatAppearance.BorderSize = 2; // Para que se vea mejor
            btnContinuar.Click += (s, e) => this.Close();
            this.Controls.Add(btnContinuar);

        }

        /// <summary>
        /// Crea un Label que representa un icono.
        /// </summary>
        /// <param name="x">Posición X del icono.</param>
        /// <param name="y">Posición Y del icono.</param>
        /// <returns>Un Label con un icono de emoji.</returns>


        private Label CrearIcono(int x, int y)
        {
            Label icono = new Label();
            icono.Text = "🚀\r\n";
            icono.Font = new Font("Arial", 12, FontStyle.Bold);
            icono.ForeColor = Color.Gold; // Icono amarillo
            icono.AutoSize = true;
            icono.Location = new Point(x, y);
            return icono;
        }

        /// <summary>
        /// Crea un Label con un texto descriptivo.
        /// </summary>
        /// <param name="texto">Texto a mostrar.</param>
        /// <param name="x">Posición X del texto.</param>
        /// <param name="y">Posición Y del texto.</param>
        /// <returns>Un Label con el texto especificado.</returns>

        private Label CrearTexto(string texto, int x, int y)
        {
            Label label = new Label();
            label.Text = texto;
            label.Font = new Font("Arial", 12, FontStyle.Bold);
            label.ForeColor = Color.WhiteSmoke; // Texto blanco
            label.AutoSize = true;
            label.Location = new Point(x + 25, y); // Mueve el texto un poco más a la derecha
            return label;
        }
        /// <summary>
        /// Importa la función de la API de Windows para crear bordes redondeados.
        /// </summary>
        /// <param name="left">Coordenada izquierda.</param>
        /// <param name="top">Coordenada superior.</param>
        /// <param name="right">Coordenada derecha.</param>
        /// <param name="bottom">Coordenada inferior.</param>
        /// <param name="width">Ancho del redondeo.</param>
        /// <param name="height">Altura del redondeo.</param>
        /// <returns>Un identificador de región redondeada.</returns>

        // Código para hacer los bordes redondeados
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int left, int top, int right, int bottom, int width, int height);
    }
}
