﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Simple.Data.SqlAnywhereTest
{
    using Ado;
    using Simple.Data.SqlAnywhere;

    [TestFixture]
    public class DatabaseOpenerTests
    {
        [Test]
        public void OpenNamedConnectionTest()
        {
            var db = Database.OpenNamedConnection("Test");
            Assert.IsNotNull(db);
            var user = db.Users.FindById(1);
            Assert.AreEqual(1, user.Id);
        }

        [Test]
        public void TestProviderIsSqlProvider()
        {
            var provider = new ProviderHelper().GetProviderByConnectionString(Properties.Settings.Default.ConnectionString);
            Assert.IsInstanceOf(typeof(SqlAnywhereConnectionProvider), provider);
        }

        [Test]
        public void TestProviderIsSqlProviderFromOpen()
        {
            Database db = DatabaseHelper.Open();
            Assert.IsInstanceOf(typeof(AdoAdapter), db.GetAdapter());
            Assert.IsInstanceOf(typeof(SqlAnywhereConnectionProvider), ((AdoAdapter)db.GetAdapter()).ConnectionProvider);
        }

        [Test]
        public void TestProviderIsSqlProviderFromOpenConnection()
        {
            Database db = Database.OpenConnection(Properties.Settings.Default.ConnectionString);
            Assert.IsInstanceOf(typeof(AdoAdapter), db.GetAdapter());
            Assert.IsInstanceOf(typeof(SqlAnywhereConnectionProvider), ((AdoAdapter)db.GetAdapter()).ConnectionProvider);
        }
    }
}
