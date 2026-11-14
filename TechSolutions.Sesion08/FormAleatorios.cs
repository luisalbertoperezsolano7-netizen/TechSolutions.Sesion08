using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TechSolutions.Sesion08
{
    public partial class FormAleatorios : Form
    {
        private TextBox txtAleatorios;
        private Button btnEjecutar;

        public FormAleatorios()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.txtAleatorios = new TextBox();
            this.btnEjecutar = new Button();
            this.SuspendLayout();

            // txtAleatorios (Diseño similar a Figura 1)
            this.txtAleatorios.Location = new System.Drawing.Point(30, 40);
            this.txtAleatorios.Multiline = true;
            this.txtAleatorios.Name = "txtAleatorios";
            this.txtAleatorios.ScrollBars = ScrollBars.Vertical;
            this.txtAleatorios.Size = new System.Drawing.Size(250, 160);

            // btnEjecutar
            this.btnEjecutar.Location = new System.Drawing.Point(300, 40);
            this.btnEjecutar.Name = "btnEjecutar";
            this.btnEjecutar.Size = new System.Drawing.Size(100, 40);
            this.btnEjecutar.Text = "EJECUTAR";
            this.btnEjecutar.UseVisualStyleBackColor = true;
            this.btnEjecutar.Click += new System.EventHandler(this.btnEjecutar_Click);

            // FormAleatorios
            this.ClientSize = new System.Drawing.Size(435, 230);
            this.Controls.Add(this.btnEjecutar);
            this.Controls.Add(this.txtAleatorios);
            this.Name = "FormAleatorios";
            this.Text = "Laboratorio 8 - Async/Await";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private string Aleatorio()
        {
            Thread.Sleep(500); // Espera simulada
            Random rnd = new Random();
            int n = rnd.Next(1, 500); // Rango de 1 a 500
            return n.ToString();
        }

        private async void btnEjecutar_Click(object sender, EventArgs e)
        {
            txtAleatorios.Text = "Iniciando el proceso" + Environment.NewLine;
            Task<string> hilo = new Task<string>(Aleatorio);
            hilo.Start();
            string rhilo = await hilo;
            txtAleatorios.Text += "Hilo: " + rhilo + Environment.NewLine;
            txtAleatorios.Text += "Finalizado el proceso";
        }
    }
}