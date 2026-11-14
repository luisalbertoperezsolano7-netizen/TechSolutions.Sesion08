using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TechSolutions.Sesion08
{
    public partial class FormClientes : Form
    {
        private Label lblRucDni;
        private TextBox txtRucDni;
        private Label lblRazonSocial;
        private TextBox txtRazonSocial;
        private Label lblTelefono;
        private TextBox txtTelefono;
        private Label lblDireccion;
        private TextBox txtDireccion;
        private Button btnGuardar;
        private DataGridView dgvClientes;

        public FormClientes()
        {
            InitializeComponent();
            CargarClientes();
        }

        private void InitializeComponent()
        {
            this.lblRucDni = new Label();
            this.txtRucDni = new TextBox();
            this.lblRazonSocial = new Label();
            this.txtRazonSocial = new TextBox();
            this.lblTelefono = new Label();
            this.txtTelefono = new TextBox();
            this.lblDireccion = new Label();
            this.txtDireccion = new TextBox();
            this.btnGuardar = new Button();
            this.dgvClientes = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvClientes)).BeginInit();
            this.SuspendLayout();

            // RUC / DNI
            this.lblRucDni.AutoSize = true;
            this.lblRucDni.Location = new System.Drawing.Point(30, 30);
            this.lblRucDni.Text = "RUC / DNI:";

            this.txtRucDni.Location = new System.Drawing.Point(130, 27);
            this.txtRucDni.Size = new System.Drawing.Size(200, 22);

            // Razón Social / Nombres
            this.lblRazonSocial.AutoSize = true;
            this.lblRazonSocial.Location = new System.Drawing.Point(30, 70);
            this.lblRazonSocial.Text = "Razón Social:";

            this.txtRazonSocial.Location = new System.Drawing.Point(130, 67);
            this.txtRazonSocial.Size = new System.Drawing.Size(250, 22);

            // Teléfono
            this.lblTelefono.AutoSize = true;
            this.lblTelefono.Location = new System.Drawing.Point(30, 110);
            this.lblTelefono.Text = "Teléfono:";

            this.txtTelefono.Location = new System.Drawing.Point(130, 107);
            this.txtTelefono.Size = new System.Drawing.Size(200, 22);

            // Dirección
            this.lblDireccion.AutoSize = true;
            this.lblDireccion.Location = new System.Drawing.Point(30, 150);
            this.lblDireccion.Text = "Dirección:";

            this.txtDireccion.Location = new System.Drawing.Point(130, 147);
            this.txtDireccion.Size = new System.Drawing.Size(250, 22);

            // Botón Guardar
            this.btnGuardar.Location = new System.Drawing.Point(130, 190);
            this.btnGuardar.Size = new System.Drawing.Size(120, 35);
            this.btnGuardar.Text = "Registrar Cliente";
            this.btnGuardar.UseVisualStyleBackColor = true;
            this.btnGuardar.Click += new EventHandler(this.btnGuardar_Click);

            // DataGridView (Tabla para ver clientes)
            this.dgvClientes.Location = new System.Drawing.Point(30, 240);
            this.dgvClientes.Size = new System.Drawing.Size(550, 180);
            this.dgvClientes.AllowUserToAddRows = false;
            this.dgvClientes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // FormClientes
            this.ClientSize = new System.Drawing.Size(610, 450);
            this.Controls.Add(this.lblRucDni);
            this.Controls.Add(this.txtRucDni);
            this.Controls.Add(this.lblRazonSocial);
            this.Controls.Add(this.txtRazonSocial);
            this.Controls.Add(this.lblTelefono);
            this.Controls.Add(this.txtTelefono);
            this.Controls.Add(this.lblDireccion);
            this.Controls.Add(this.txtDireccion);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.dgvClientes);
            this.Name = "FormClientes";
            this.Text = "Tech Solutions - Gestión de Clientes";
            this.StartPosition = FormStartPosition.CenterScreen;
            ((System.ComponentModel.ISupportInitialize)(this.dgvClientes)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void CargarClientes()
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                string query = "SELECT IdCliente, RucDni, RazonSocial, Telefono, Direccion FROM dbo.Cliente";
                SqlDataAdapter da = new SqlDataAdapter(query, cn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvClientes.DataSource = dt;
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string rucDni = txtRucDni.Text.Trim();
            string razonSocial = txtRazonSocial.Text.Trim();
            string telefono = txtTelefono.Text.Trim();
            string direccion = txtDireccion.Text.Trim();

            if (string.IsNullOrEmpty(rucDni) || string.IsNullOrEmpty(razonSocial))
            {
                MessageBox.Show("RUC/DNI y Razón Social son obligatorios.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                cn.Open();
                string insert = "INSERT INTO dbo.Cliente (RucDni, RazonSocial, Telefono, Direccion) VALUES (@ruc, @razon, @tel, @dir)";
                using (SqlCommand cmd = new SqlCommand(insert, cn))
                {
                    cmd.Parameters.AddWithValue("@ruc", rucDni);
                    cmd.Parameters.AddWithValue("@razon", razonSocial);
                    cmd.Parameters.AddWithValue("@tel", telefono);
                    cmd.Parameters.AddWithValue("@dir", direccion);
                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Cliente registrado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            txtRucDni.Clear();
            txtRazonSocial.Clear();
            txtTelefono.Clear();
            txtDireccion.Clear();
            CargarClientes(); // Refrescar la tabla
        }
    }
}