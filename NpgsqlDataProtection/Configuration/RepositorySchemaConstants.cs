namespace NpgsqlDataProtection.Configuration
{
    public static class RepositorySchemaConstants
    {
        public const string DefaultFriendlyNameColumn = "FriendlyName";
        public const string DefaultIdColumn = "Id";
        public const string DefaultTable = "DataProtectionKeys";
        public const string DefaultXmlColumn = "Xml";
        
        public const string PgsqlFriendlyNameColumn = "friendly_name";
        public const string PgsqlIdColumn = "id";
        public const string PgsqlTable = "data_protection_keys";
        public const string PgsqlXmlColumn = "xml";

        public const string Schema = "public";
    }
}