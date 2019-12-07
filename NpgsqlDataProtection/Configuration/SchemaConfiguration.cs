using System;

namespace NpgsqlDataProtection.Configuration
{
    public interface ISchemaConfigurationEditable
    {
        string FriendlyNameColumn { get; set; }
        string IdColumn { get; set; }
        string Schema { get; set; }
        string Table { get; set; }
        string XmlColumn { get; set; }
    }
    
    public class SchemaConfiguration : ISchemaConfigurationEditable
    {
        public string FriendlyNameColumn { get; set; }
        public string IdColumn { get; set; }
        public string Schema { get; set; }
        public string Table { get; set; }
        public string XmlColumn { get; set; }

        public SchemaConfiguration(bool useDefaultSchema = true)
        {
            FriendlyNameColumn = useDefaultSchema
                ? RepositorySchemaConstants.DefaultFriendlyNameColumn
                : RepositorySchemaConstants.PgsqlFriendlyNameColumn;
            IdColumn = useDefaultSchema
                ? RepositorySchemaConstants.DefaultIdColumn
                : RepositorySchemaConstants.PgsqlIdColumn;
            Schema = RepositorySchemaConstants.Schema;
            Table = useDefaultSchema
                ? RepositorySchemaConstants.DefaultTable
                : RepositorySchemaConstants.PgsqlTable;
            XmlColumn = useDefaultSchema
                ? RepositorySchemaConstants.DefaultXmlColumn
                : RepositorySchemaConstants.PgsqlXmlColumn;
        }

        public SchemaConfiguration()
        {
            FriendlyNameColumn = RepositorySchemaConstants.DefaultFriendlyNameColumn;
            IdColumn = RepositorySchemaConstants.DefaultIdColumn;
            Schema = RepositorySchemaConstants.Schema;
            Table = RepositorySchemaConstants.DefaultTable;
            XmlColumn = RepositorySchemaConstants.DefaultXmlColumn;
        }

        public SchemaConfiguration(Action<ISchemaConfigurationEditable> config = null)
            : this()
        {
            config?.Invoke(this);
        }
    }
}