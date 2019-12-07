using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NpgsqlDataProtection.Data;
using TransientContext.Common;
using TransientContext.Postgresql;

namespace NpgsqlDataProtection.Tests.Common
{
    public class DatabaseFixture : IDisposable
    {
        public ITestDatabase TestDatabase { get; }
        public KeyStorageContext Context { get; }

        public DatabaseFixture(bool createSchema = true)
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets("57a04ea5-19b4-483f-af4d-a96930e166e5")
                .Build();

            TestDatabase = new TestDatabaseBuilder()
                .WithConfiguration(configuration)
                .Build();

            TestDatabase.Create();

            var builder = new DbContextOptionsBuilder<KeyStorageContext>()
                .UseNpgsql(TestDatabase.ConnectionString);

            Context = new KeyStorageContext(builder.Options);

            if (createSchema)
                Context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            TestDatabase.Drop();
        }
    }
}