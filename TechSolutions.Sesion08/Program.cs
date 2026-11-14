using System;
using System.Windows.Forms;

namespace TechSolutions.Sesion08
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            // Carga los ensamblados nativos requeridos por ReportViewer y SqlServerTypes
            SqlServerTypes.Utilities.LoadNativeAssemblies(AppDomain.CurrentDomain.BaseDirectory);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Puedes alternar entre FormLogin o FormAleatorios según requieras probar
            Application.Run(new FormLogin());
        }
    }
}