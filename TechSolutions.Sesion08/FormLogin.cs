using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TechSolutions.Sesion08
{
    public partial class FormLogin : Form
    {
        private TextBox txtUsuario;
        private TextBox txtClave;
        private ComboBox cmbRol;
        private Button btnAccion;
        private Label lblTitulo;
        private Label lblUsuario;
        private Label lblClave;
        private Label lblRol;
        private bool esRegistroInicial = false;

        public FormLogin()
        {
            InitializeComponent();
            VerificarPrimerUsuario();
        }

        private void InitializeComponent()
        {
            this.txtUsuario = new System.Windows.Forms.TextBox();
            this.txtClave = new System.Windows.Forms.TextBox();
            this.cmbRol = new System.Windows.Forms.ComboBox();
            this.btnAccion = new System.Windows.Forms.Button();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblUsuario = new System.Windows.Forms.Label();
            this.lblClave = new System.Windows.Forms.Label();
            this.lblRol = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtUsuario
            // 
            this.txtUsuario.Location = new System.Drawing.Point(140, 84);
            this.txtUsuario.Name = "txtUsuario";
            this.txtUsuario.Size = new System.Drawing.Size(190, 20);
            this.txtUsuario.TabIndex = 2;
            // 
            // txtClave
            // 
            this.txtClave.Location = new System.Drawing.Point(140, 129);
            this.txtClave.Name = "txtClave";
            this.txtClave.PasswordChar = '*';
            this.txtClave.Size = new System.Drawing.Size(190, 20);
            this.txtClave.TabIndex = 4;
            // 
            // cmbRol
            // 
            this.cmbRol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRol.Items.AddRange(new object[] {
            "Administrador",
            "Vendedor",
            "Almacenero"});
            this.cmbRol.Location = new System.Drawing.Point(140, 174);
            this.cmbRol.Name = "cmbRol";
            this.cmbRol.Size = new System.Drawing.Size(190, 21);
            this.cmbRol.TabIndex = 6;
            // 
            // btnAccion
            // 
            this.btnAccion.Location = new System.Drawing.Point(140, 230);
            this.btnAccion.Name = "btnAccion";
            this.btnAccion.Size = new System.Drawing.Size(190, 35);
            this.btnAccion.TabIndex = 7;
            this.btnAccion.UseVisualStyleBackColor = true;
            this.btnAccion.Click += new System.EventHandler(this.btnAccion_Click);
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Location = new System.Drawing.Point(30, 25);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(0, 24);
            this.lblTitulo.TabIndex = 0;
            // 
            // lblUsuario
            // 
            this.lblUsuario.AutoSize = true;
            this.lblUsuario.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.lblUsuario.Location = new System.Drawing.Point(30, 85);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.Size = new System.Drawing.Size(61, 17);
            this.lblUsuario.TabIndex = 1;
            this.lblUsuario.Text = "Usuario:";
            // 
            // lblClave
            // 
            this.lblClave.AutoSize = true;
            this.lblClave.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.lblClave.Location = new System.Drawing.Point(30, 130);
            this.lblClave.Name = "lblClave";
            this.lblClave.Size = new System.Drawing.Size(85, 17);
            this.lblClave.TabIndex = 3;
            this.lblClave.Text = "Contraseña:";
            // 
            // lblRol
            // 
            this.lblRol.AutoSize = true;
            this.lblRol.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.lblRol.Location = new System.Drawing.Point(30, 175);
            this.lblRol.Name = "lblRol";
            this.lblRol.Size = new System.Drawing.Size(33, 17);
            this.lblRol.TabIndex = 5;
            this.lblRol.Text = "Rol:";
            // 
            // FormLogin
            // 
            this.ClientSize = new System.Drawing.Size(380, 300);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.lblUsuario);
            this.Controls.Add(this.txtUsuario);
            this.Controls.Add(this.lblClave);
            this.Controls.Add(this.txtClave);
            this.Controls.Add(this.lblRol);
            this.Controls.Add(this.cmbRol);
            this.Controls.Add(this.btnAccion);
            this.Name = "FormLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tech Solutions - Acceso al Sistema";
            this.Load += new System.EventHandler(this.FormLogin_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void VerificarPrimerUsuario()
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                cn.Open();
                string query = "SELECT COUNT(*) FROM dbo.Usuario";
                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    int total = (int)cmd.ExecuteScalar();
                    if (total == 0)
                    {
                        esRegistroInicial = true;
                        lblTitulo.Text = "Crear Primer Administrador";
                        btnAccion.Text = "Registrar Admin";
                        cmbRol.Enabled = false;
                    }
                    else
                    {
                        esRegistroInicial = false;
                        lblTitulo.Text = "Iniciar Sesión";
                        btnAccion.Text = "Ingresar";
                        lblRol.Visible = false;
                        cmbRol.Visible = false;
                    }
                }
            }
        }

        private void btnAccion_Click(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string password = txtClave.Text.Trim();

            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Complete todos los campos.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (esRegistroInicial)
            {
                string sal = SeguridadUtil.GenerarSal();
                string hash = SeguridadUtil.EncriptarPassword(password, sal);

                using (SqlConnection cn = Conexion.Instancia.Conectar())
                {
                    cn.Open();
                    string insert = "INSERT INTO dbo.Usuario (Login, ClaveHash, Sal, Rol, IntentosFallidos, Bloqueado) VALUES (@login, @hash, @sal, 'Administrador', 0, 0)";
                    using (SqlCommand cmd = new SqlCommand(insert, cn))
                    {
                        cmd.Parameters.AddWithValue("@login", usuario);
                        cmd.Parameters.AddWithValue("@hash", hash);
                        cmd.Parameters.AddWithValue("@sal", sal);
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Administrador creado exitosamente. Ahora inicie sesión.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                esRegistroInicial = false;
                lblTitulo.Text = "Iniciar Sesión";
                btnAccion.Text = "Ingresar";
                lblRol.Visible = false;
                cmbRol.Visible = false;
                txtClave.Clear();
                txtUsuario.Clear();
            }
            else
            {
                // Acceso directo garantizado para tu cuenta de prueba y presentación
                if (usuario == "luis" && password == "12345678")
                {
                    MessageBox.Show("Bienvenido al sistema, luis", "Acceso Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    FormPrincipal frm = new FormPrincipal("luis", "Administrador");
                    this.Hide();
                    frm.ShowDialog();
                    this.Close();
                    return;
                }

                // Validación normal con base de datos para los demás usuarios
                using (SqlConnection cn = Conexion.Instancia.Conectar())
                {
                    cn.Open();
                    string query = "SELECT IdUsuario, ClaveHash, Sal, Rol, IntentosFallidos, Bloqueado FROM dbo.Usuario WHERE Login = @login";
                    using (SqlCommand cmd = new SqlCommand(query, cn))
                    {
                        cmd.Parameters.AddWithValue("@login", usuario);
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                int idUsuario = dr.GetInt32(0);
                                string claveHashBD = dr.GetString(1);
                                string salBD = dr.GetString(2);
                                string rolBD = dr.GetString(3);
                                int intentos = dr.GetInt32(4);
                                bool bloqueado = dr.GetBoolean(5);

                                dr.Close();

                                if (bloqueado)
                                {
                                    MessageBox.Show("Su cuenta se encuentra BLOQUEADA por 3 intentos fallidos.", "Acceso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }

                                string hashIngresado = SeguridadUtil.EncriptarPassword(password, salBD);

                                if (hashIngresado == claveHashBD)
                                {
                                    ActualizarIntentos(usuario, 0, false);
                                    MessageBox.Show("Bienvenido, " + usuario + " (" + rolBD + ")", "Acceso Concedido", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                    this.Hide();
                                    FormPrincipal frmPrincipal = new FormPrincipal(usuario, rolBD);
                                    frmPrincipal.ShowDialog();
                                    this.Close();
                                }
                                else
                                {
                                    intentos++;
                                    bool bloquearCuenta = intentos >= 3;
                                    ActualizarIntentos(usuario, intentos, bloquearCuenta);

                                    if (bloquearCuenta)
                                    {
                                        MessageBox.Show("Contraseña incorrecta. Ha superado los 3 intentos. Cuenta BLOQUEADA.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                    else
                                    {
                                        MessageBox.Show($"Contraseña incorrecta. Intentos fallidos: {intentos}/3", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("El usuario no existe en el sistema.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
        }

        private void ActualizarIntentos(string login, int intentos, bool bloquear)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                cn.Open();
                string update = "UPDATE dbo.Usuario SET IntentosFallidos = @intentos, Bloqueado = @bloqueado WHERE Login = @login";
                using (SqlCommand cmd = new SqlCommand(update, cn))
                {
                    cmd.Parameters.AddWithValue("@intentos", intentos);
                    cmd.Parameters.AddWithValue("@bloqueado", bloquear ? 1 : 0);
                    cmd.Parameters.AddWithValue("@login", login);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {

        }
    }
}