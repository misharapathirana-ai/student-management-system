using System.Data.SQLite;

namespace StudentManagementApp 
{
    public class DBConnection
    {
        private static string connectionString = "Data Source=student.db;Version=3;";

        public static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(connectionString);
        }
    }
}