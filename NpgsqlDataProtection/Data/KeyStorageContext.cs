using System;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NpgsqlDataProtection.Configuration;

namespace NpgsqlDataProtection.Data
{
    public class KeyStorageContext : DbContext, IDataProtectionKeyContext
    {
        private readonly SchemaConfiguration _configuration;

        public KeyStorageContext(DbContextOptions<KeyStorageContext> options)
            : base(options)
        {
            _configuration = new SchemaConfiguration();
        }

        public KeyStorageContext(DbContextOptions<KeyStorageContext> options,
            SchemaConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        public KeyStorageContext(DbContextOptions<KeyStorageContext> options, bool useDefaultSchema = true)
            : base(options)
        {
            _configuration = new SchemaConfiguration(useDefaultSchema);
        }

        public KeyStorageContext(DbContextOptions<KeyStorageContext> options,
            Action<ISchemaConfigurationEditable> config = null)
            : base(options)
        {
            _configuration = new SchemaConfiguration();
            config?.Invoke(_configuration);
        }
        
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DataProtectionKey>(entity =>
            {
                entity.ToTable(_configuration.Table);
                entity.HasKey(e => e.Id);

                entity.Property(e => e.FriendlyName)
                    .HasColumnName(_configuration.FriendlyNameColumn);

                entity.Property(e => e.Id)
                    .HasColumnName(_configuration.IdColumn);

                entity.Property(e => e.Xml)
                    .HasColumnName(_configuration.XmlColumn);
            });
        }
    }
}