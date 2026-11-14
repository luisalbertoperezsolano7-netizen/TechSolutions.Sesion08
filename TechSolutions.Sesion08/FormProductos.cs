using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TechSolutions.Sesion08
{
    public partial class FormProductos : Form
    {
        private Label lblCodigo, lblNombre, lblPrecio, lblStock;
        private TextBox txtCodigo, txtNombre, txtPrecio, txtStock;
        private Button btnGuardar;
        private DataGridView dgvProductos;

        public FormProductos()
        {
            InitializeComponent();
            CargarProductos();
        }

        private void InitializeComponent()
        {
            this.lblCodigo = new Label { Text = "Código:", Location = new System.Drawing.Point(30, 30), AutoSize = true };
            this.txtCodigo = new TextBox { Location = new System.Drawing.Point(130, 27), Size = new System.Drawing.Size(200, 22) };

            this.lblNombre = new Label { Text = "Nombre Prod:", Location = new System.Drawing.Point(30, 70), AutoSize = true };
            this.txtNombre = new TextBox { Location = new System.Drawing.Point(130, 67), Size = new System.Drawing.Size(250, 22) };

            this.lblPrecio = new Label { Text = "Precio (S/):", Location = new System.Drawing.Point(30, 110), AutoSize = true };
            this.txtPrecio = new TextBox { Location = new System.Drawing.Point(130, 107), Size = new System.Drawing.Size(200, 22) };

            this.lblStock = new Label { Text = "Stock:", Location = new System.Drawing.Point(30, 150), AutoSize = true };
            this.txtStock = new TextBox { Location = new System.Drawing.Point(130, 147), Size = new System.Drawing.Size(200, 22) };

            this.btnGuardar = new Button { Text = "Guardar Producto", Location = new System.Drawing.Point(130, 190), Size = new System.Drawing.Size(140, 35) };
            this.btnGuardar.Click += new EventHandler(this.btnGuardar_Click);

            this.dgvProductos = new DataGridView { Location = new System.Drawing.Point(30, 240), Size = new System.Drawing.Size(550, 180), AllowUserToAddRows = false };
            this.dgvProductos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            this.ClientSize = new System.Drawing.Size(610, 450);
            this.Controls.Add(this.lblCodigo);
            this.Controls.Add(this.txtCodigo);
            this.Controls.Add(this.lblNombre);
            this.Controls.Add(this.txtNombre);
            this.Controls.Add(this.lblPrecio);
            this.Controls.Add(this.txtPrecio);
            this.Controls.Add(this.lblStock);
            this.Controls.Add(this.txtStock);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.dgvProductos);
            this.Name = "FormProductos";
            this.Text = "Tech Solutions - Gestión de Productos Tecnológicos";
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void CargarProductos()
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                string query = "SELECT IdProducto, Codigo, Nombre, Precio, Stock FROM dbo.Producto";
                SqlDataAdapter da = new SqlDataAdapter(query, cn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvProductos.DataSource = dt;
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string codigo = txtCodigo.Text.Trim();
            string nombre = txtNombre.Text.Trim();

            if (string.IsNullOrEmpty(codigo) || string.IsNullOrEmpty(nombre))
            {
                MessageBox.Show("Código y Nombre son obligatorios.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                cn.Open();
                string insert = "INSERT INTO dbo.Producto (Codigo, Nombre, Precio, Stock) VALUES (@cod, @nom, @pre, @sto)";
                using (SqlCommand cmd = new SqlCommand(insert, cn))
                {
                    cmd.Parameters.AddWithValue("@cod", codigo);
                    cmd.Parameters.AddWithValue("@nom", nombre);
                    cmd.Parameters.AddWithValue("@pre", Convert.ToDecimal(txtPrecio.Text));
                    cmd.Parameters.AddWithValue("@sto", Convert.ToInt32(txtStock.Text));
                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Producto registrado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            txtCodigo.Clear(); txtNombre.Clear(); txtPrecio.Clear(); txtStock.Clear();
            CargarProductos();
        }
    }
}