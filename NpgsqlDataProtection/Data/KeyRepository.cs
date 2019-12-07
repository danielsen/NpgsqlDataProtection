using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Npgsql;
using NpgsqlDataProtection.Configuration;

namespace NpgsqlDataProtection.Data
{
    public class KeyRepository : IXmlRepository
    {
        private readonly string _connectionString;
        private readonly string _schema;
        private readonly SchemaConfiguration _configuration;

        private static readonly Regex IdentifierRegex = new Regex(@"^[\p{L}_][\p{L}\p{N}@$#_]{0,127}$");

        private static bool ValidateSqlIdentifier(string identifier)
        {
            return IdentifierRegex.IsMatch(identifier);
        }

        public string Table => _configuration.Table;
        public string FriendlyNameColumn => _configuration.FriendlyNameColumn;
        public string IdColumn => _configuration.IdColumn;
        public string XmlColumn => _configuration.XmlColumn;

        public KeyRepository(string connectionString, string schema = "public",
            Action<ISchemaConfigurationEditable> config = null)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            if (string.IsNullOrWhiteSpace(schema))
                throw new ArgumentNullException(nameof(schema));

            if (!ValidateSqlIdentifier(schema))
                throw new ArgumentException($"Provided schema name: '{schema}' is invalid.", nameof(schema));

            _connectionString = connectionString;
            _schema = schema;
            _configuration = new SchemaConfiguration();
            config?.Invoke(_configuration);

            if (!ValidateSqlIdentifier(_configuration.Table))
                throw new ArgumentException($"Provided table name: '{_configuration.Table}' is invalid.",
                    nameof(_configuration.Table));

            CreateTable();
            MaybeCreateFriendlyNameIndex();
        }

        public KeyRepository(string connectionString, string schema = "public", bool useDefaultSchema = true)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            if (string.IsNullOrWhiteSpace(schema))
                throw new ArgumentNullException(nameof(schema));

            if (!ValidateSqlIdentifier(schema))
                throw new ArgumentException($"Provided schema name: '{schema}' is invalid.", nameof(schema));

            _connectionString = connectionString;
            _schema = schema;
            _configuration = new SchemaConfiguration(useDefaultSchema);

            if (!ValidateSqlIdentifier(_configuration.Table))
                throw new ArgumentException($"Provided table name: '{_configuration.Table}' is invalid.",
                    nameof(_configuration.Table));

            CreateTable();
            MaybeCreateFriendlyNameIndex();
        }
        
        public KeyRepository(string connectionString) 
            : this(connectionString, "public", true){}

        private void CreateTable()
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
                {
                    var sql = $@"CREATE TABLE IF NOT EXISTS ""{_schema}"".""{_configuration.Table}"" (
                    ""{_configuration.IdColumn}"" SERIAL PRIMARY KEY,
                    ""{_configuration.FriendlyNameColumn}"" TEXT COLLATE pg_catalog.default NOT NULL UNIQUE,
                    ""{_configuration.XmlColumn}"" TEXT COLLATE pg_catalog.default NOT NULL)";

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }
        }

        private bool FriendlyNameConstraintExists()
        {
            var rows = 0;
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var sql = $@"SELECT 1 AS Result FROM information_schema.constraint_column_usage
                    WHERE table_name = '{_configuration.Table}' AND
                    constraint_name = '{_configuration.Table}_{_configuration.FriendlyNameColumn}_key'";
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
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

            return rows > 0;
        }

        private void MaybeCreateFriendlyNameIndex()
        {
            if (FriendlyNameConstraintExists())
                return;

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
                {
                    var sql = $@"ALTER TABLE ONLY ""{_schema}"".""{_configuration.Table}""
                            ADD CONSTRAINT ""{_configuration.Table}_{_configuration.FriendlyNameColumn}_key""
                            UNIQUE (""{_configuration.FriendlyNameColumn}"")";
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }

                connection.Close();
            }
        }

        public IReadOnlyCollection<XElement> GetAllElements()
        {
            var elementList = new List<XElement>();
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var sql =
                    $@"SELECT ""{_configuration.XmlColumn}"" FROM ""{_schema}"".""{_configuration.Table}""";

                using (var command = new NpgsqlCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            elementList.Add(XElement.Parse(reader.GetString(0)));
                        }

                        reader.Close();
                    }
                }

                connection.Close();
            }

            return new ReadOnlyCollection<XElement>(elementList);
        }

        public void StoreElement(XElement element, string friendlyName)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
                {
                    var sql = $@"INSERT INTO ""{_schema}"".""{_configuration.Table}""
                        (""{_configuration.FriendlyNameColumn}"", ""{_configuration.XmlColumn}"")
                        VALUES (@friendlyName, @xmlData)
                        ON CONFLICT (""{_configuration.FriendlyNameColumn}"")
                        DO UPDATE SET ""{_configuration.XmlColumn}"" = @xmlData";

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.Add(new NpgsqlParameter("@friendlyName", friendlyName));
                        command.Parameters.Add(new NpgsqlParameter("@xmlData", element.ToString()));

                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }

                connection.Close();
            }
        }
    }
}