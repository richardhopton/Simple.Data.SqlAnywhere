using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Simple.Data.Ado;
using Simple.Data.Ado.Schema;
using Simple.Data.TestHelper;
using iAnywhere.Data.SQLAnywhere;

namespace Simple.Data.SqlAnywhereTest.SchemaTests
{
    [TestFixture]
    public class DatabaseSchemaTests : DatabaseSchemaTestsBase
    {
        protected override Database GetDatabase()
        {
            return Database.OpenConnection(Properties.Settings.Default.ConnectionString);
        }

        [Test]
        public void TestTables()
        {
            Assert.AreEqual(1, Schema.Tables.Count(t => t.ActualName == "Users"));
        }

        [Test]
        public void TestColumns()
        {
            var table = Schema.FindTable("Users");
            Assert.AreEqual(1, table.Columns.Count(c => c.ActualName == "Id"));
        }

        [Test]
        public void TestPrimaryKey()
        {
            var table = Schema.FindTable("Customers");
            Assert.AreEqual(1, table.PrimaryKey.Length);
            Assert.AreEqual("CustomerId", table.PrimaryKey[0]);
        }

        [Test]
        public void TestForeignKey()
        {
            var table = Schema.FindTable("Orders");
            var fkey = table.ForeignKeys.Single();
            Assert.AreEqual("CustomerId", fkey.Columns[0]);
            Assert.AreEqual("Customers", fkey.MasterTable.Name);
            Assert.AreEqual("CustomerId", fkey.UniqueColumns[0]);
        }

        [Test]
        public void TestIdentityIsTrueWhenItShouldBe()
        {
            var column = Schema.FindTable("Customers").FindColumn("CustomerId");
            Assert.IsTrue(column.IsIdentity);
        }

        [Test]
        public void TestIdentityIsFalseWhenItShouldBe()
        {
            var column = Schema.FindTable("Customers").FindColumn("Name");
            Assert.IsFalse(column.IsIdentity);
        }

        [Test]
        public void TestNewTableIsAddedToSchemaAfterReset()
        {
            dynamic db = GetDatabase();
            db.Users.FindById(1); // Forces population of schema...

            using (var cn = new SAConnection(Properties.Settings.Default.ConnectionString))
            using (var cmd = cn.CreateCommand())
            {
                cn.Open();

                cmd.CommandText = @"IF EXISTS (SELECT * FROM sysobjects WHERE id=OBJECT_ID('dba.RuntimeTable') AND type='U') THEN
DROP TABLE dba.RuntimeTable; END IF;";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "CREATE TABLE dba.RuntimeTable (Id int)";
                cmd.ExecuteNonQuery();
            }

            db.GetAdapter().Reset();
            db.RuntimeTable.Insert(Id: 1);
            var row = db.RuntimeTable.FindById(1);
            Assert.AreEqual(1, row.Id);
        }
    }
}
