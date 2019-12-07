using System.Linq;
using NpgsqlDataProtection.Configuration;
using NUnit.Framework;
using Should;

namespace NpgsqlDataProtection.Tests.Unit.Configuration
{
    [TestFixture]
    public class SchemaConfigurationTests
    {
        [Test]
        public void should_create_schema_config_with_default_values()
        {
            var config = new SchemaConfiguration();

            config.FriendlyNameColumn.ShouldEqual(RepositorySchemaConstants.DefaultFriendlyNameColumn);
            config.IdColumn.ShouldEqual(RepositorySchemaConstants.DefaultIdColumn);
            config.Schema.ShouldEqual(RepositorySchemaConstants.Schema);
            config.Table.ShouldEqual(RepositorySchemaConstants.DefaultTable);
            config.XmlColumn.ShouldEqual(RepositorySchemaConstants.DefaultXmlColumn);
            should_not_contain_string("_",
                new[] {config.FriendlyNameColumn, config.IdColumn, config.Schema, config.Table, config.XmlColumn});
        }

        [Test]
        public void should_create_schema_config_with_pgsql_formatted_values()
        {
            var config = new SchemaConfiguration(false);

            config.FriendlyNameColumn.ShouldEqual(RepositorySchemaConstants.PgsqlFriendlyNameColumn);
            config.IdColumn.ShouldEqual(RepositorySchemaConstants.PgsqlIdColumn);
            config.Schema.ShouldEqual(RepositorySchemaConstants.Schema);
            config.Table.ShouldEqual(RepositorySchemaConstants.PgsqlTable);
            config.XmlColumn.ShouldEqual(RepositorySchemaConstants.PgsqlXmlColumn);
            should_contain_string("_", new[] {config.Table, config.FriendlyNameColumn});
            should_be_lower_case(new[]
                {config.FriendlyNameColumn, config.IdColumn, config.Schema, config.Table, config.XmlColumn});
        }

        [Test]
        public void should_contain_custom_values()
        {
            var tableName = "Keys";
            var idColumn = "KeyId";

            var config = new SchemaConfiguration(c =>
            {
                c.IdColumn = idColumn;
                c.Table = tableName;
            });
            
            config.FriendlyNameColumn.ShouldEqual(RepositorySchemaConstants.DefaultFriendlyNameColumn);
            config.IdColumn.ShouldEqual(idColumn);
            config.Schema.ShouldEqual(RepositorySchemaConstants.Schema);
            config.Table.ShouldEqual(tableName);
            config.XmlColumn.ShouldEqual(RepositorySchemaConstants.DefaultXmlColumn);
            should_not_contain_string("_",
                new[] {config.FriendlyNameColumn, config.IdColumn, config.Schema, config.Table, config.XmlColumn});
        }

        private void should_not_contain_string(string value, string[] targets)
        {
            foreach (var target in targets)
            {
                target.ShouldNotContain(value);
            }
        }

        private void should_contain_string(string value, string[] targets)
        {
            foreach (var target in targets)
            {
                target.ShouldContain(value);
            }
        }

        private void should_be_lower_case(string[] targets)
        {
            foreach (var target in targets)
            {
                target.Any(char.IsUpper).ShouldBeFalse();
            }
        }
    }
}