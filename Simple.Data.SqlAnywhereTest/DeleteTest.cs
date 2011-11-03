﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Data.SqlAnywhereTest
{
    using NUnit.Framework;
    using Resources;

    [TestFixture]
    public class DeleteTest
    {
        [Test]
        public void TestDeleteByColumn()
        {
            var db = DatabaseHelper.Open();
            db.DeleteTest.Insert(Id: 1);
            var count = db.DeleteTest.DeleteById(1);
            Assert.AreEqual(1, count);
        }

        [Test]
        public void TestDeleteAll()
        {
            var db = DatabaseHelper.Open();
            db.DeleteTest.Insert(Id: 1);
            db.DeleteTest.Insert(Id: 2);
            var count = db.DeleteTest.DeleteAll();
            Assert.AreEqual(2, count.ReturnValue);
        }

        [Test]
        public void TestDeleteByColumnInTransaction()
        {
            var db = DatabaseHelper.Open();
            var tx = db.BeginTransaction();
            tx.DeleteTest.Insert(Id: 1);
            var count = tx.DeleteTest.DeleteById(1);
            tx.Commit();
            Assert.AreEqual(1, count);
        }

        [Test]
        public void TestDeleteAllInTransaction()
        {
            var db = DatabaseHelper.Open();
            var tx = db.BeginTransaction();
            tx.DeleteTest.Insert(Id: 1);
            tx.DeleteTest.Insert(Id: 2);
            var count = tx.DeleteTest.DeleteAll();
            tx.Commit();
            Assert.AreEqual(2, count.ReturnValue);
        }
    }
}
