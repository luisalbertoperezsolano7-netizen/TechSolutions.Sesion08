using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace TechSolutions.Sesion08
{
    public partial class FormUsuarios : Form
    {
        private Label lblTituloReg, lblUsuario, lblContraseña, lblRol, lblBuscar;
        private TextBox txtUsuario, txtContraseña, txtBuscar;
        private ComboBox cmbRol;
        private Button btnRegistrar, btnBuscar;
        private CheckBox chkInsegura;
        private DataGridView dgvUsuarios;

        public FormUsuarios()
        {
            InitializeComponent();
            CargarUsuarios();
        }

        private void InitializeComponent()
        {
            this.lblTituloReg = new Label { Text = "Registrar usuario", Location = new System.Drawing.Point(30, 20), Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold), AutoSize = true };

            this.lblUsuario = new Label { Text = "Usuario:", Location = new System.Drawing.Point(30, 60), AutoSize = true };
            this.txtUsuario = new TextBox { Location = new System.Drawing.Point(130, 57), Size = new System.Drawing.Size(220, 22) };

            this.lblContraseña = new Label { Text = "Contraseña:", Location = new System.Drawing.Point(30, 100), AutoSize = true };
            this.txtContraseña = new TextBox { Location = new System.Drawing.Point(130, 97), Size = new System.Drawing.Size(220, 22), PasswordChar = '*' };

            this.lblRol = new Label { Text = "Rol:", Location = new System.Drawing.Point(30, 140), AutoSize = true };
            this.cmbRol = new ComboBox { Location = new System.Drawing.Point(130, 137), Size = new System.Drawing.Size(220, 22), DropDownStyle = ComboBoxStyle.DropDownList };
            this.cmbRol.Items.AddRange(new object[] { "Administrador", "Vendedor", "Almacenero" });
            this.cmbRol.SelectedIndex = 0;

            this.btnRegistrar = new Button { Text = "Registrar", Location = new System.Drawing.Point(380, 75), Size = new System.Drawing.Size(120, 45) };
            this.btnRegistrar.Click += new EventHandler(this.btnRegistrar_Click);

            this.lblBuscar = new Label { Text = "Buscar usuario (comparar consultas)", Location = new System.Drawing.Point(30, 190), Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold), AutoSize = true };
            this.txtBuscar = new TextBox { Location = new System.Drawing.Point(30, 225), Size = new System.Drawing.Size(320, 22) };

            this.btnBuscar = new Button { Text = "Buscar", Location = new System.Drawing.Point(370, 222), Size = new System.Drawing.Size(100, 28) };
            this.btnBuscar.Click += new EventHandler(this.btnBuscar_Click);

            this.chkInsegura = new CheckBox { Text = "Usar consulta concatenada (INSEGURA - solo demostración)", Location = new System.Drawing.Point(30, 260), AutoSize = true };

            this.dgvUsuarios = new DataGridView { Location = new System.Drawing.Point(30, 295), Size = new System.Drawing.Size(650, 200), AllowUserToAddRows = false };
            this.dgvUsuarios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            this.ClientSize = new System.Drawing.Size(710, 520);
            this.Controls.Add(this.lblTituloReg);
            this.Controls.Add(this.lblUsuario);
            this.Controls.Add(this.txtUsuario);
            this.Controls.Add(this.lblContraseña);
            this.Controls.Add(this.txtContraseña);
            this.Controls.Add(this.lblRol);
            this.Controls.Add(this.cmbRol);
            this.Controls.Add(this.btnRegistrar);
            this.Controls.Add(this.lblBuscar);
            this.Controls.Add(this.txtBuscar);
            this.Controls.Add(this.btnBuscar);
            this.Controls.Add(this.chkInsegura);
            this.Controls.Add(this.dgvUsuarios);
            this.Name = "FormUsuarios";
            this.Text = "Usuarios - Tech Solutions";
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void CargarUsuarios()
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                string query = "SELECT IdUsuario AS Id, Login, ClaveHash, Sal, Rol, IntentosFallidos, Bloqueado FROM dbo.Usuario";
                SqlDataAdapter da = new SqlDataAdapter(query, cn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvUsuarios.DataSource = dt;
            }
        }

        // Métodos auxiliares para generar Salt y Hash seguros reales
        private string GenerarSalt()
        {
            byte[] saltBytes = new byte[16];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        private string GenerarHash(string password, string salt)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                string combinado = password + salt;
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combinado));
                return Convert.ToBase64String(bytes);
            }
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUsuario.Text) || string.IsNullOrEmpty(txtContraseña.Text))
            {
                MessageBox.Show("Ingrese usuario y contraseña.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Generar una Sal real y aleatoria (NUNCA MÁS "DEFAULT")
            string saltGenerada = GenerarSalt();
            string hashGenerado = GenerarHash(txtContraseña.Text, saltGenerada);

            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                cn.Open();
                string query = "INSERT INTO dbo.Usuario (Login, ClaveHash, Sal, Rol, IntentosFallidos, Bloqueado) VALUES (@login, @hash, @sal, @rol, 0, 0)";
                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.AddWithValue("@login", txtUsuario.Text.Trim());
                    cmd.Parameters.AddWithValue("@hash", hashGenerado);
                    cmd.Parameters.AddWithValue("@sal", saltGenerada);
                    cmd.Parameters.AddWithValue("@rol", cmbRol.SelectedItem.ToString());
                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("¡Usuario registrado con éxito con Salt y Hash seguros!", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            txtUsuario.Clear();
            txtContraseña.Clear();
            CargarUsuarios();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                DataTable dt = new DataTable();
                if (chkInsegura.Checked)
                {
                    // Consulta concatenada (Demostración Insegura)
                    string query = "SELECT IdUsuario AS Id, Login, ClaveHash, Sal, Rol, IntentosFallidos, Bloqueado FROM dbo.Usuario WHERE Login = '" + txtBuscar.Text.Trim() + "'";
                    SqlDataAdapter da = new SqlDataAdapter(query, cn);
                    da.Fill(dt);
                }
                else
                {
                    // Consulta Segura con Parámetros
                    string query = "SELECT IdUsuario AS Id, Login, ClaveHash, Sal, Rol, IntentosFallidos, Bloqueado FROM dbo.Usuario WHERE Login = @login";
                    using (SqlCommand cmd = new SqlCommand(query, cn))
                    {
                        cmd.Parameters.AddWithValue("@login", txtBuscar.Text.Trim());
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);
                    }
                }
                dgvUsuarios.DataSource = dt;
            }
        }
    }
}