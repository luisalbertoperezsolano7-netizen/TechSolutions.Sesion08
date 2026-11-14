using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TechSolutions.Sesion08
{
    public partial class FormVentas : Form
    {
        private Label lblCliente, lblProducto, lblCantidad, lblTotal;
        private TextBox txtIdCliente, txtIdProducto, txtCantidad, txtTotal;
        private Button btnProcesar;
        private DataGridView dgvVentas;

        public FormVentas()
        {
            InitializeComponent();
            CargarVentas();
        }

        private void InitializeComponent()
        {
            this.lblCliente = new Label { Text = "ID Cliente:", Location = new System.Drawing.Point(30, 30), AutoSize = true };
            this.txtIdCliente = new TextBox { Location = new System.Drawing.Point(130, 27), Size = new System.Drawing.Size(150, 22) };

            this.lblProducto = new Label { Text = "ID Producto:", Location = new System.Drawing.Point(30, 70), AutoSize = true };
            this.txtIdProducto = new TextBox { Location = new System.Drawing.Point(130, 67), Size = new System.Drawing.Size(150, 22) };

            this.lblCantidad = new Label { Text = "Cantidad:", Location = new System.Drawing.Point(30, 110), AutoSize = true };
            this.txtCantidad = new TextBox { Location = new System.Drawing.Point(130, 107), Size = new System.Drawing.Size(150, 22) };

            this.lblTotal = new Label { Text = "Total (S/):", Location = new System.Drawing.Point(30, 150), AutoSize = true };
            this.txtTotal = new TextBox { Location = new System.Drawing.Point(130, 147), Size = new System.Drawing.Size(150, 22) };

            this.btnProcesar = new Button { Text = "Registrar Venta", Location = new System.Drawing.Point(130, 190), Size = new System.Drawing.Size(150, 35) };
            this.btnProcesar.Click += new EventHandler(this.btnProcesar_Click);

            this.dgvVentas = new DataGridView { Location = new System.Drawing.Point(30, 240), Size = new System.Drawing.Size(550, 180), AllowUserToAddRows = false };
            this.dgvVentas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            this.ClientSize = new System.Drawing.Size(610, 450);
            this.Controls.Add(this.lblCliente);
            this.Controls.Add(this.txtIdCliente);
            this.Controls.Add(this.lblProducto);
            this.Controls.Add(this.txtIdProducto);
            this.Controls.Add(this.lblCantidad);
            this.Controls.Add(this.txtCantidad);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.txtTotal);
            this.Controls.Add(this.btnProcesar);
            this.Controls.Add(this.dgvVentas);
            this.Name = "FormVentas";
            this.Text = "Tech Solutions - Registro de Ventas y Transacciones";
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void CargarVentas()
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                string query = "SELECT IdVenta, IdCliente, Fecha, Total FROM dbo.Venta";
                SqlDataAdapter da = new SqlDataAdapter(query, cn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvVentas.DataSource = dt;
            }
        }

        private void btnProcesar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtIdCliente.Text) || string.IsNullOrEmpty(txtIdProducto.Text) || string.IsNullOrEmpty(txtCantidad.Text) || string.IsNullOrEmpty(txtTotal.Text))
            {
                MessageBox.Show("Complete todos los campos para registrar la venta.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                cn.Open();
                // 1. INICIAR LA TRANSACCIÓN SQL (COMMIT / ROLLBACK)
                SqlTransaction transaccion = cn.BeginTransaction();

                try
                {
                    // Paso A: Registrar la cabecera de la venta
                    string insertVenta = "INSERT INTO dbo.Venta (IdCliente, Total) VALUES (@idCliente, @total)";
                    using (SqlCommand cmdVenta = new SqlCommand(insertVenta, cn, transaccion))
                    {
                        cmdVenta.Parameters.AddWithValue("@idCliente", Convert.ToInt32(txtIdCliente.Text));
                        cmdVenta.Parameters.AddWithValue("@total", Convert.ToDecimal(txtTotal.Text));
                        cmdVenta.ExecuteNonQuery();
                    }

                    // Paso B: Descontar el stock del producto vendido
                    string updateStock = "UPDATE dbo.Producto SET Stock = Stock - @cantidad WHERE IdProducto = @idProducto";
                    using (SqlCommand cmdStock = new SqlCommand(updateStock, cn, transaccion))
                    {
                        cmdStock.Parameters.AddWithValue("@cantidad", Convert.ToInt32(txtCantidad.Text));
                        cmdStock.Parameters.AddWithValue("@idProducto", Convert.ToInt32(txtIdProducto.Text));
                        cmdStock.ExecuteNonQuery();
                    }

                    // 2. SI TODO ES CORRECTO, APLICAMOS COMMIT
                    transaccion.Commit();
                    MessageBox.Show("¡Venta registrada exitosamente con COMMIT y stock actualizado!", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    txtIdCliente.Clear();
                    txtIdProducto.Clear();
                    txtCantidad.Clear();
                    txtTotal.Clear();
                    CargarVentas();
                }
                catch (Exception ex)
                {
                    // 3. SI OCURRE UN ERROR, REVERTIMOS TODO CON ROLLBACK
                    transaccion.Rollback();
                    MessageBox.Show("Error en la operación. Se aplicó ROLLBACK automático. Detalle: " + ex.Message, "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}