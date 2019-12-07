using System;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.DependencyInjection;
using NpgsqlDataProtection.Configuration;
using NpgsqlDataProtection.Data;

namespace NpgsqlDataProtection
{
    public static class DataProtectionBuilderExtensions
    {
        public static IDataProtectionBuilder PersistKeysToPostgres(this IDataProtectionBuilder builder,
            string connectionString, bool useDefaultSchema = true)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            builder.Services.Configure<KeyManagementOptions>(options =>
                options.XmlRepository = new KeyRepository(connectionString, useDefaultSchema: useDefaultSchema));

            return builder;
        }

        public static IDataProtectionBuilder PersistKeysToPostgres(this IDataProtectionBuilder builder,
            string connectionString, Action<ISchemaConfigurationEditable> config = null)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
                
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            builder.Services.Configure<KeyManagementOptions>(options =>
                options.XmlRepository = new KeyRepository(connectionString, config: config));

            return builder;
        }
    }
}