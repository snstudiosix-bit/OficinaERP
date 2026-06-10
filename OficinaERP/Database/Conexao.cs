using Microsoft.Data.Sqlite;

namespace OficinaERP.Database
{
    public class Conexao
    {
        private static string caminho = "oficina.db";

        public static SqliteConnection Abrir()
        {
            var conn = new SqliteConnection($"Data Source={caminho}");
            conn.Open();
            return conn;
        }
    }
}