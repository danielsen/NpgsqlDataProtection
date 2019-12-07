# NpgsqlDataProtection

`NpgsqlDataProtection` provides a PostgreSQL backend for DataProtection key storage. It
can be configured to use the default table schema or a custom schema.

### Packages

- Current Version: 1.0.0
- Target Framework: .NET Standard 2.0

### Dependencies

- [Npgsql](https://www.nuget.org/packages/Npgsql)
- [Microsoft.AspNetCore.DataProtection.EntityFrameworkCore](https://www.nuget.org/packages/Microsoft.AspNetCore.DataProtection.EntityFrameworkCore/)
- [https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Design/](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Design/)

### Usage

Two extension methods are provided for IDataProtectionBuilder:

- `PersistKeysToPostgres(string connectionString, bool useDefaultSchema = true)`
- `PersistKeysToPostgres(string connectionString, Action<ISchemaConfigurationEditable> config = null)`

In the first form, setting `useDefaultSchema` to false configures the storage schema to use
postgresql friendly object names. In short, table and column names will be converted to 
snake case. For example, the default table "DataProtectionKeys" becomes "data\_protection\_keys". For more coustom configurations use the second form and pass in a custom `ISchemaconfigurationEditable` function.

In most cases that's all you will need to do. However, if you plan to access the key
storage outside of the normal workflow in your code, you will probably want to 
configure a separate `KeyStorageContext` using one of the `KeyStorageContext` constructors.

- `KeyStorageContext(DbContextOptions<KeyStorageContext> options, bool useDefaultSchema = true)`
- `KeyStorageContext(DbContextOptions<KeyStroageContext> options, Action<ISchemaConfigurationEditable> config = null)`

### Example

The following example shows how to use the extensions with the default schema:

        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services
                    .AddDataProtection()
                    .PersistKeysToPostgres("YourConnectionString");
            }
        }

### Caveats

The package does not configure key encryption.
