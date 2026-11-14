using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;

namespace TechSolutions.Sesion08
{
    public partial class FormReportes : Form
    {
        private Button btnReporteClientes, btnReporteProductos, btnReporteVentas, btnFiltrarLinq;
        private Microsoft.Reporting.WinForms.ReportViewer reportViewer1;

        public FormReportes()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.btnReporteClientes = new Button { Text = "Reporte Clientes", Location = new System.Drawing.Point(20, 20), Size = new System.Drawing.Size(120, 35) };
            this.btnReporteClientes.Click += (s, e) => CargarReporteClientes();

            this.btnReporteProductos = new Button { Text = "Reporte Productos", Location = new System.Drawing.Point(150, 20), Size = new System.Drawing.Size(120, 35) };
            this.btnReporteProductos.Click += (s, e) => CargarDatosReporte("SELECT Codigo, Nombre, Precio, Stock FROM dbo.Producto", "ReporteGeneral.rdlc");

            // Botón Ventas actualizado para apuntar directamente a ReporteVentas.rdlc con sus columnas reales
            this.btnReporteVentas = new Button { Text = "Reporte Ventas", Location = new System.Drawing.Point(280, 20), Size = new System.Drawing.Size(120, 35) };
            this.btnReporteVentas.Click += (s, e) => CargarReporteVentas();

            this.btnFiltrarLinq = new Button { Text = "Filtrar Stock (LINQ)", Location = new System.Drawing.Point(410, 20), Size = new System.Drawing.Size(140, 35) };
            this.btnFiltrarLinq.Click += new EventHandler(this.btnFiltrarLinq_Click);

            this.reportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();
            this.reportViewer1.Location = new System.Drawing.Point(20, 75);
            this.reportViewer1.Size = new System.Drawing.Size(745, 455);
            this.reportViewer1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            this.ClientSize = new System.Drawing.Size(785, 550);
            this.Controls.Add(this.btnReporteClientes);
            this.Controls.Add(this.btnReporteProductos);
            this.Controls.Add(this.btnReporteVentas);
            this.Controls.Add(this.btnFiltrarLinq);
            this.Controls.Add(this.reportViewer1);
            this.Name = "FormReportes";
            this.Text = "Tech Solutions - Módulo de Reportes con ReportViewer";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.FormReportes_Load);
        }

        private void FormReportes_Load(object sender, EventArgs e)
        {
            CargarDatosReporte("SELECT Codigo, Nombre, Precio, Stock FROM dbo.Producto", "ReporteGeneral.rdlc");
        }

        private void CargarReporteClientes()
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                string query = "SELECT idCliente, RucDni AS DNI, RazonSocial AS Nombre, Direccion, Telefono FROM dbo.Cliente";
                SqlDataAdapter da = new SqlDataAdapter(query, cn);
                DataTable dtClientes = new DataTable();
                da.Fill(dtClientes);

                reportViewer1.ProcessingMode = ProcessingMode.Local;
                reportViewer1.LocalReport.ReportPath = "ReporteClientes.rdlc";

                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", dtClientes));
                reportViewer1.RefreshReport();
            }
        }

        private void CargarReporteVentas()
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                // Consulta limpia que extrae exactamente IdVenta, IdCliente, Fecha y Total
                string query = "SELECT IdVenta, IdCliente, Fecha, Total FROM dbo.Venta";
                SqlDataAdapter da = new SqlDataAdapter(query, cn);
                DataTable dtVentas = new DataTable();
                da.Fill(dtVentas);

                reportViewer1.ProcessingMode = ProcessingMode.Local;
                reportViewer1.LocalReport.ReportPath = "ReporteVentas.rdlc";

                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", dtVentas));
                reportViewer1.RefreshReport();
            }
        }

        private void CargarDatosReporte(string query, string reporteRdlc)
        {
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                SqlDataAdapter da = new SqlDataAdapter(query, cn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                reportViewer1.ProcessingMode = ProcessingMode.Local;
                reportViewer1.LocalReport.ReportPath = reporteRdlc;

                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", dt));
                reportViewer1.RefreshReport();
            }
        }

        private void btnFiltrarLinq_Click(object sender, EventArgs e)
        {
            DataTable dtProductos = new DataTable();
            using (SqlConnection cn = Conexion.Instancia.Conectar())
            {
                string query = "SELECT Codigo, Nombre, Precio, Stock FROM dbo.Producto";
                SqlDataAdapter da = new SqlDataAdapter(query, cn);
                da.Fill(dtProductos);
            }

            var resultadoLinq = from row in dtProductos.AsEnumerable()
                                where row.Field<int>("Stock") <= 10
                                orderby row.Field<int>("Stock") ascending
                                select new
                                {
                                    Codigo = row.Field<string>("Codigo"),
                                    Nombre = row.Field<string>("Nombre"),
                                    Precio = row.Field<decimal>("Precio"),
                                    Stock = row.Field<int>("Stock")
                                };

            DataTable dtFiltrado = ToDataTable(resultadoLinq.ToList());

            reportViewer1.ProcessingMode = ProcessingMode.Local;
            reportViewer1.LocalReport.ReportPath = "ReporteGeneral.rdlc";

            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", dtFiltrado));
            reportViewer1.RefreshReport();

            MessageBox.Show("Consulta con LINQ ejecutada y cargada en el ReportViewer exitosamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            System.ComponentModel.PropertyDescriptorCollection props =
                System.ComponentModel.TypeDescriptor.GetProperties(typeof(T));
            foreach (System.ComponentModel.PropertyDescriptor prop in props)
            {
                dataTable.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            foreach (T item in items)
            {
                var values = new object[props.Count];
                for (int i = 0; i < props.Count; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }
    }
}