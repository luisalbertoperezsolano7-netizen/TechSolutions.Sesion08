using System.Data.SqlClient;

namespace TechSolutions.Sesion08
{
    public class Conexion
    {
        private static Conexion instancia = null;
        // Forzamos el nombre exacto de la base de datos
        private string cadenaConexion = "Server=.;Database=BDTechSolutions;Integrated Security=True;";

        private Conexion() { }

        public static Conexion Instancia
        {
            get
            {
                if (instancia == null)
                {
                    instancia = new Conexion();
                }
                return instancia;
            }
        }

        public SqlConnection Conectar()
        {
            return new SqlConnection(cadenaConexion);
        }
    }
}