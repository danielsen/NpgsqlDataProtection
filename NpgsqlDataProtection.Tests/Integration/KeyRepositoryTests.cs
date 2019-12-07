using System.Linq;
using System.Xml.Linq;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using NpgsqlDataProtection.Data;
using NpgsqlDataProtection.Tests.Common;
using NUnit.Framework;
using Should;

namespace NpgsqlDataProtection.Tests.Integration
{
    [TestFixture]
    public class KeyRepositoryTests
    {
        private DatabaseFixture? _databaseFixture;

        [TearDown]
        public void TearDown()
        {
            _databaseFixture?.Dispose();
        }

        [Test]
        public void should_create_table_from_new_key_repository()
        {
            _databaseFixture = new DatabaseFixture(false);
            var keyRepository = new KeyRepository(_databaseFixture.TestDatabase.ConnectionString);
            should_have_correct_schema(_databaseFixture.TestDatabase.ConnectionString, keyRepository);
        }

        [Test]
        public void should_create_table_from_context()
        {
            _databaseFixture = new DatabaseFixture();
            var keyRepository = new KeyRepository(_databaseFixture.TestDatabase.ConnectionString);
            should_have_correct_schema(_databaseFixture.TestDatabase.ConnectionString, keyRepository);
        }

        [Test]
        public void should_store_key()
        {
            _databaseFixture = new DatabaseFixture(false);
            var name = "SampleKey";

            var keyRepository = new KeyRepository(_databaseFixture.TestDatabase.ConnectionString);
            keyRepository.StoreElement(new XElement("Key", 1), name);

            _databaseFixture.Context.DataProtectionKeys.Count().ShouldEqual(1);

            var elements = keyRepository.GetAllElements();
            elements.Count().ShouldEqual(1);
            elements.First().Value.ShouldEqual("1");
        }

        [Test]
        public void should_store_key_in_context_configured_store()
        {
            _databaseFixture = new DatabaseFixture(true);
            var name = "SampleKey";

            var keyRepository = new KeyRepository(_databaseFixture.TestDatabase.ConnectionString);
            keyRepository.StoreElement(new XElement("Key", 1), name);

            _databaseFixture.Context.DataProtectionKeys.Count().ShouldEqual(1);

            var elements = keyRepository.GetAllElements();
            elements.Count().ShouldEqual(1);
            elements.First().Value.ShouldEqual("1");
        }

        [Test]
        public void should_get_all_keys_from_repository()
        {
            _databaseFixture = new DatabaseFixture();
            var name = "SampleKey";
            var xmlElement = new XElement("Key", 1);

            _databaseFixture.Context.DataProtectionKeys.Add(new DataProtectionKey
            {
                FriendlyName = name,
                Xml = xmlElement.ToString()
            });
            _databaseFixture.Context.SaveChanges();

            var keyRepository = new KeyRepository(_databaseFixture.TestDatabase.ConnectionString);
            var elements = keyRepository.GetAllElements();
            elements.Count.ShouldEqual(1);
            elements.First().Value.ShouldEqual("1");
        }

        private void should_have_correct_schema(string connectionString, KeyRepository keyRepository)
        {
            SchemaValidationHelpers.TableExists(_databaseFixture.TestDatabase.ConnectionString,
                keyRepository.Table).ShouldBeTrue();
            SchemaValidationHelpers.ColumnExists(_databaseFixture.TestDatabase.ConnectionString,
                keyRepository.Table, keyRepository.FriendlyNameColumn);
            SchemaValidationHelpers.ColumnExists(_databaseFixture.TestDatabase.ConnectionString,
                keyRepository.Table, keyRepository.IdColumn);
            SchemaValidationHelpers.ColumnExists(_databaseFixture.TestDatabase.ConnectionString,
                keyRepository.Table, keyRepository.XmlColumn);
        }
    }
}