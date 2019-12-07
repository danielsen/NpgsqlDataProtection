using System.Linq;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NpgsqlDataProtection.Data;
using NUnit.Framework;
using Should;

namespace NpgsqlDataProtection.Tests.Unit.Data
{
    [TestFixture]
    public class KeyStorageTests
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

        [Test]
        public void should_store_key()
        {
            var dataProtectionKey = new DataProtectionKey
            {
                FriendlyName = "Key1",
                Xml = "NotReallyXML"
            };
            
            _context = new KeyStorageContext(_builder.Options);
            _context.DataProtectionKeys.Add(dataProtectionKey);
            _context.SaveChanges();
            _context.DataProtectionKeys.Count().ShouldEqual(1);

            var updatedEntity = _context.DataProtectionKeys.Find(dataProtectionKey.Id);
            updatedEntity.FriendlyName.ShouldEqual(dataProtectionKey.FriendlyName);
            updatedEntity.Id.ShouldEqual(1);
            updatedEntity.Xml.ShouldEqual(dataProtectionKey.Xml);
        }
    }
}