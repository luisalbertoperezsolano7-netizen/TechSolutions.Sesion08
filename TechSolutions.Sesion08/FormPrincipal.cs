using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TechSolutions.Sesion08
{
    public partial class FormPrincipal : Form
    {
        private MenuStrip menuStrip1;
        private ToolStripMenuItem menuClientes;
        private ToolStripMenuItem menuProductos;
        private ToolStripMenuItem menuVentas;
        private ToolStripMenuItem menuReportes;
        private ToolStripMenuItem menuUsuarios;
        private ToolStripMenuItem menuCerrarSesion;

        private Label lblBienvenida;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel lblEstadoConexion;
        private ToolStripStatusLabel lblFechaHora;

        private Timer timerInactividad;
        private Timer timerRelog;
        private int tiempoInactivo = 0;
        private const int limiteInactividad = 300; // 5 minutos

        private string usuarioActual;
        private string rolActual;

        // Paneles de Métricas Ejecutivas (6 Tarjetas Profesionales)
        private Panel panelClientes, panelProductos, panelVentas, panelUsuarios, panelStockCritico, panelIngresos;
        private Label lblNumClientes, lblNumProductos, lblNumVentas, lblNumUsuarios, lblNumStockCritico, lblNumIngresos;

        public FormPrincipal(string usuario, string rol)
        {
            usuarioActual = usuario;
            rolActual = rol;
            InitializeComponent();
            ConfigurarPermisosPorRol();
            ConfigurarTimerInactividad();
            CargarMetricasEjecutivas();
        }

        private void InitializeComponent()
        {
            this.menuStrip1 = new MenuStrip();
            this.menuClientes = new ToolStripMenuItem();
            this.menuProductos = new ToolStripMenuItem();
            this.menuVentas = new ToolStripMenuItem();
            this.menuReportes = new ToolStripMenuItem();
            this.menuUsuarios = new ToolStripMenuItem();
            this.menuCerrarSesion = new ToolStripMenuItem();

            this.lblBienvenida = new Label();
            this.statusStrip1 = new StatusStrip();
            this.lblEstadoConexion = new ToolStripStatusLabel();
            this.lblFechaHora = new ToolStripStatusLabel();

            this.timerInactividad = new Timer();
            this.timerRelog = new Timer();

            this.panelClientes = new Panel();
            this.panelProductos = new Panel();
            this.panelVentas = new Panel();
            this.panelUsuarios = new Panel();
            this.panelStockCritico = new Panel();
            this.panelIngresos = new Panel();

            this.SuspendLayout();

            // Configuración de Menús
            this.menuClientes.Text = "Clientes";
            this.menuClientes.Click += MenuClientes_Click;

            this.menuProductos.Text = "Productos";
            this.menuProductos.Click += MenuProductos_Click;

            this.menuVentas.Text = "Ventas";
            this.menuVentas.Click += MenuVentas_Click;

            this.menuReportes.Text = "Reportes & LINQ";
            this.menuReportes.Click += MenuReportes_Click;

            this.menuUsuarios.Text = "Seguridad / Usuarios";
            this.menuUsuarios.Click += MenuUsuarios_Click;

            this.menuCerrarSesion.Text = "🚪 Cerrar Sesión";
            this.menuCerrarSesion.ForeColor = System.Drawing.Color.Red;
            this.menuCerrarSesion.Click += (s, e) => {
                this.Hide();
                new FormLogin().ShowDialog();
                this.Close();
            };

            this.menuStrip1.Items.Add(this.menuClientes);
            this.menuStrip1.Items.Add(this.menuProductos);
            this.menuStrip1.Items.Add(this.menuVentas);
            this.menuStrip1.Items.Add(this.menuReportes);
            this.menuStrip1.Items.Add(this.menuUsuarios);
            this.menuStrip1.Items.Add(this.menuCerrarSesion);

            // Título de bienvenida
            this.lblBienvenida.AutoSize = true;
            this.lblBienvenida.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold);
            this.lblBienvenida.Location = new System.Drawing.Point(25, 35);
            this.lblBienvenida.Text = $"Panel de Control Empresarial — Usuario: {usuarioActual} | Rol: [{rolActual}]";

            // Barra de Estado Inferior
            this.lblEstadoConexion.Text = "🟢 Servidor SQL Conectado (BDTechSolutions)";
            this.lblEstadoConexion.Spring = true;
            this.lblEstadoConexion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblFechaHora.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            this.statusStrip1.Items.Add(this.lblEstadoConexion);
            this.statusStrip1.Items.Add(this.lblFechaHora);

            this.timerRelog.Interval = 1000;
            this.timerRelog.Tick += (s, e) => { lblFechaHora.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"); };
            this.timerRelog.Start();

            // Creación de las 6 Tarjetas de Métricas Profesionales
            CrearTarjetaMetrica(panelClientes, "TOTAL CLIENTES", "0", 25, 80, System.Drawing.Color.FromArgb(41, 128, 185), out lblNumClientes);
            CrearTarjetaMetrica(panelProductos, "TOTAL PRODUCTOS", "0", 245, 80, System.Drawing.Color.FromArgb(39, 174, 96), out lblNumProductos);
            CrearTarjetaMetrica(panelVentas, "VENTAS REGISTRADAS", "0", 465, 80, System.Drawing.Color.FromArgb(142, 68, 173), out lblNumVentas);

            CrearTarjetaMetrica(panelUsuarios, "USUARIOS SISTEMA", "0", 25, 195, System.Drawing.Color.FromArgb(211, 84, 0), out lblNumUsuarios);
            CrearTarjetaMetrica(panelStockCritico, "STOCK CRÍTICO (<=10)", "0", 245, 195, System.Drawing.Color.FromArgb(192, 57, 43), out lblNumStockCritico);
            CrearTarjetaMetrica(panelIngresos, "INGRESOS TOTALES (S/)", "0.00", 465, 195, System.Drawing.Color.FromArgb(22, 160, 133), out lblNumIngresos);

            // Configuración de la ventana
            this.ClientSize = new System.Drawing.Size(695, 340);
            this.Controls.Add(this.panelClientes);
            this.Controls.Add(this.panelProductos);
            this.Controls.Add(this.panelVentas);
            this.Controls.Add(this.panelUsuarios);
            this.Controls.Add(this.panelStockCritico);
            this.Controls.Add(this.panelIngresos);
            this.Controls.Add(this.lblBienvenida);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormPrincipal";
            this.Text = "Tech Solutions - ERP Enterprise Dashboard";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void CrearTarjetaMetrica(Panel panel, string titulo, string valorInicial, int x, int y, System.Drawing.Color colorFondo, out Label lblValor)
        {
            panel.Location = new System.Drawing.Point(x, y);
            panel.Size = new System.Drawing.Size(205, 100);
            panel.BackColor = colorFondo;

            Label lblTitulo = new Label();
            lblTitulo.Text = titulo;
            lblTitulo.ForeColor = System.Drawing.Color.White;
            lblTitulo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F, System.Drawing.FontStyle.Bold);
            lblTitulo.Location = new System.Drawing.Point(12, 12);
            lblTitulo.AutoSize = true;

            lblValor = new Label();
            lblValor.Text = valorInicial;
            lblValor.ForeColor = System.Drawing.Color.White;
            lblValor.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold);
            lblValor.Location = new System.Drawing.Point(12, 42);
            lblValor.AutoSize = true;

            panel.Controls.Add(lblTitulo);
            panel.Controls.Add(lblValor);
        }

        private void CargarMetricasEjecutivas()
        {
            try
            {
                using (SqlConnection cn = Conexion.Instancia.Conectar())
                {
                    cn.Open();

                    lblNumClientes.Text = new SqlCommand("SELECT COUNT(*) FROM dbo.Cliente", cn).ExecuteScalar().ToString();
                    lblNumProductos.Text = new SqlCommand("SELECT COUNT(*) FROM dbo.Producto", cn).ExecuteScalar().ToString();
                    lblNumVentas.Text = new SqlCommand("SELECT COUNT(*) FROM dbo.Venta", cn).ExecuteScalar().ToString();
                    lblNumUsuarios.Text = new SqlCommand("SELECT COUNT(*) FROM dbo.Usuario", cn).ExecuteScalar().ToString();
                    lblNumStockCritico.Text = new SqlCommand("SELECT COUNT(*) FROM dbo.Producto WHERE Stock <= 10", cn).ExecuteScalar().ToString();

                    object ingresos = new SqlCommand("SELECT ISNULL(SUM(Total), 0) FROM dbo.Venta", cn).ExecuteScalar();
                    lblNumIngresos.Text = Convert.ToDecimal(ingresos).ToString("N2");
                }
            }
            catch
            {
                // Control silencioso
            }
        }

        private void ConfigurarPermisosPorRol()
        {
            if (rolActual == "Vendedor")
            {
                menuProductos.Enabled = false;
                menuUsuarios.Enabled = false;
            }
            else if (rolActual == "Almacenero")
            {
                menuVentas.Enabled = false;
                menuClientes.Enabled = false;
                menuUsuarios.Enabled = false;
            }
        }

        private void ConfigurarTimerInactividad()
        {
            timerInactividad.Interval = 1000;
            timerInactividad.Tick += TimerInactividad_Tick;
            timerInactividad.Start();

            this.MouseMove += (s, e) => ReiniciarContador();
            this.KeyPress += (s, e) => ReiniciarContador();
            this.menuStrip1.MouseMove += (s, e) => ReiniciarContador();
        }

        private void ReiniciarContador()
        {
            tiempoInactivo = 0;
        }

        private void TimerInactividad_Tick(object sender, EventArgs e)
        {
            tiempoInactivo++;
            if (tiempoInactivo >= limiteInactividad)
            {
                timerInactividad.Stop();
                MessageBox.Show("La sesión se ha cerrado automáticamente por inactividad.", "Seguridad", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.Hide();
                new FormLogin().ShowDialog();
                this.Close();
            }
        }

        private void MenuClientes_Click(object sender, EventArgs e)
        {
            new FormClientes().ShowDialog();
            CargarMetricasEjecutivas();
        }

        private void MenuProductos_Click(object sender, EventArgs e)
        {
            new FormProductos().ShowDialog();
            CargarMetricasEjecutivas();
        }

        private void MenuVentas_Click(object sender, EventArgs e)
        {
            new FormVentas().ShowDialog();
            CargarMetricasEjecutivas();
        }

        private void MenuReportes_Click(object sender, EventArgs e)
        {
            new FormReportes().ShowDialog();
        }

        private void MenuUsuarios_Click(object sender, EventArgs e)
        {
            if (rolActual != "Administrador")
            {
                MessageBox.Show("Módulo de seguridad exclusivo para Administradores.", "Acceso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                new FormUsuarios().ShowDialog();
                CargarMetricasEjecutivas();
            }
        }
    }
}