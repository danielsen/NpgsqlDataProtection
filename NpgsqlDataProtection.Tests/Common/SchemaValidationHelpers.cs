using System.Data;
using Npgsql;

namespace NpgsqlDataProtection.Tests.Common
{
    public static class SchemaValidationHelpers
    {
        private static void Execute(string connectionString, string sqlCommand)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (NpgsqlTransaction transaction = connection.BeginTransaction(IsolationLevel.Serializable))
                {
                    using (NpgsqlCommand command = new NpgsqlCommand(sqlCommand))
                    {
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }

                connection.Close();
            }
        }

        private static void ExecuteReader(string connectionString, string sqlCommand, out int rows)
        {
            rows = 0;
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(sqlCommand, connection))
                {
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                rows++;
                            }
                        }
                    }
                }

                connection.Close();
            }
        }

        public static bool TableExists(string connectionString, string tableName)
        {
            var sql = $@"SELECT 1 AS result FROM information_schema.tables WHERE table_name = '{tableName}'";
            ExecuteReader(connectionString, sql, out int rows);
            return rows > 0;
        }

        public static bool ColumnExists(string connectionString, string tableName, string columnName)
        {
            var sql = $@"SELECT 1 AS result FROM information_schema.columns 
                WHERE table_name = '{tableName}' AND column_name = '{columnName}'";
            ExecuteReader(connectionString, sql, out int rows);
            return rows > 0;
        }
    }
}