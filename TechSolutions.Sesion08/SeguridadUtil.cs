using System;
using System.Security.Cryptography;
using System.Text;

namespace TechSolutions.Sesion08
{
    public class SeguridadUtil
    {
        public static string CrearSal()
        {
            byte[] bytesSal = new byte[16];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytesSal);
            }
            return Convert.ToBase64String(bytesSal);
        }

        public static string CrearHash(string password, string sal)
        {
            byte[] bytesSal;
            try
            {
                bytesSal = Convert.FromBase64String(sal);
            }
            catch
            {
                bytesSal = Encoding.UTF8.GetBytes(sal ?? "");
            }

            byte[] bytesPassword = Encoding.UTF8.GetBytes(password);
            byte[] combinados = new byte[bytesSal.Length + bytesPassword.Length];

            Buffer.BlockCopy(bytesSal, 0, combinados, 0, bytesSal.Length);
            Buffer.BlockCopy(bytesPassword, 0, combinados, bytesSal.Length, bytesPassword.Length);

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(combinados);
                return Convert.ToBase64String(hashBytes);
            }
        }

        public static string GenerarSal()
        {
            return CrearSal();
        }

        public static string EncriptarPassword(string password, string sal)
        {
            return CrearHash(password, sal);
        }
    }
}