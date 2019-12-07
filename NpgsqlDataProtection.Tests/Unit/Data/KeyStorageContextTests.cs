using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using NpgsqlDataProtection.Configuration;
using NpgsqlDataProtection.Data;
using NUnit.Framework;
using Should;

namespace NpgsqlDataProtection.Tests.Unit.Data
{
    [TestFixture]
    public class KeyStorageContextTests
    {
        private DbContextOptionsBuilder<KeyStorageContext> _builder;
        private KeyStorageContext _context;
        
        [SetUp]
        public void Setup()
        {
            _builder = new DbContextOptionsBuilder<KeyStorageContext>()
                .UseInMemoryDatabase("DataProtection");

            _context?.Database.EnsureDeleted();
        }
        
        [TearDown]
        public void Teardown()
        {
            _context = null;
        }

        [Test, NonParallelizable]
        public void should_create_context_with_preconfigured_schema()
        {
            var schema = new SchemaConfiguration(c =>
            {
                c.FriendlyNameColumn = "Name";
                c.IdColumn = "KeyId";
                c.Table = "DataKeys";
                c.XmlColumn = "KeyXml";
            });
            _context = new KeyStorageContext(_builder.Options, schema);

            var dataProtectionKeyEntity = _context.Model.FindEntityType(typeof(DataProtectionKey));

            dataProtectionKeyEntity.ShouldNotBeNull();
            dataProtectionKeyEntity.GetTableName().ShouldEqual("DataKeys");

            var expectedProperties = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("FriendlyName", "Name"),
                new KeyValuePair<string, string>("Id", "KeyId"),
                new KeyValuePair<string, string>("Xml", "KeyXml")
            };
            should_have_correct_properties(dataProtectionKeyEntity, expectedProperties);
        }

        [Test, NonParallelizable]
        public void should_create_context_with_custom_schema_with_action()
        {
            _context = new KeyStorageContext(_builder.Options, c =>
            {
                c.FriendlyNameColumn = "Name";
                c.IdColumn = "KeyId";
                c.Table = "DataKeys";
                c.XmlColumn = "KeyXml";
            });

            var dataProtectionKeyEntity = _context.Model.FindEntityType(typeof(DataProtectionKey));

            dataProtectionKeyEntity.ShouldNotBeNull();
            dataProtectionKeyEntity.GetTableName().ShouldEqual("DataKeys");

            var expectedProperties = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("FriendlyName", "Name"),
                new KeyValuePair<string, string>("Id", "KeyId"),
                new KeyValuePair<string, string>("Xml", "KeyXml")
            };
            should_have_correct_properties(dataProtectionKeyEntity, expectedProperties);
        }

        private void should_have_correct_properties(IEntityType entityType,
            List<KeyValuePair<string, string>> expectedProperties)
        {
            var properties = entityType.GetProperties();
            foreach (var property in properties)
            {
                expectedProperties.First(p => p.Key == property.Name).Value
                    .ShouldEqual(property.GetColumnName());
            }
        }
    }
}