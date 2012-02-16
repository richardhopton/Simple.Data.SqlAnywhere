﻿using System.Collections.Generic;

using System.Linq;
using System.Reflection;
using NUnit.Framework;
using System.IO;
using Simple.Data.Ado;
using Simple.Data.SqlAnywhere;
using Simple.Data.TestHelper;
using System;

namespace Simple.Data.SqlAnywhereTest
{
    /// <summary>
    /// Summary description for FindTests
    /// </summary>
    [TestFixture]
    public class FindTests
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            DatabaseHelper.Reset();
        }

        [Test]
        public void TestFindById()
        {
            var db = DatabaseHelper.Open();
            var user = db.Users.FindById(1);
            Assert.AreEqual(1, user.Id);
        }

        [Test]
        public void TestFindByIdWithCast()
        {
            var db = DatabaseHelper.Open();
            var user = (User)db.Users.FindById(1);
            Assert.AreEqual(1, user.Id);
        }

        [Test]
        public void TestFindByReturnsOne()
        {
            var db = DatabaseHelper.Open();
            var user = (User)db.Users.FindByName("Bob");
            Assert.AreEqual(1, user.Id);
        }

        [Test]
        public void TestFindAllByName()
        {
            var db = DatabaseHelper.Open();
            IEnumerable<User> users = db.Users.FindAllByName("Bob").Cast<User>();
            Assert.AreEqual(1, users.Count());
        }

        [Test]
        public void TestFindAllByNameAsIEnumerableOfDynamic()
        {
            var db = DatabaseHelper.Open();
            IEnumerable<dynamic> users = db.Users.FindAllByName("Bob");
            Assert.AreEqual(1, users.Count());
        }

        [Test]
        public void TestFindAllByPartialName()
        {
            var db = DatabaseHelper.Open();
            IEnumerable<User> users = db.Users.FindAll(db.Users.Name.Like("Bob")).ToList<User>();
            Assert.AreEqual(1, users.Count());
        }

        [Test]
        public void TestFindAllByPartialNameOnChar()
        {
            var db = DatabaseHelper.Open();
            IEnumerable<User> users = db.UsersWithChar.FindAll(db.UsersWithChar.Name.Like("Bob%")).ToList<User>();
            Assert.AreEqual(1, users.Count());
        }

        [Test]
        public void TestFindAllByExcludedPartialName()
        {
            var db = DatabaseHelper.Open();
            IEnumerable<User> users = db.Users.FindAll(db.Users.Name.NotLike("Bob")).ToList<User>();
            Assert.AreEqual(2, users.Count());
        }
        
        [Test]
        public void TestAllCount()
        {
            var db = DatabaseHelper.Open();
            var count = db.Users.All().ToList().Count;
            Assert.AreEqual(3, count);
        }

        [Test]
        public void TestAllWithSkipCount()
        {
            var db = DatabaseHelper.Open();
            var count = db.Users.All().Skip(1).ToList().Count;
            Assert.AreEqual(2, count);
        }

        [Test]
        public void TestImplicitCast()
        {
            var db = DatabaseHelper.Open();
            User user = db.Users.FindById(1);
            Assert.AreEqual(1, user.Id);
        }

        [Test]
        public void TestImplicitEnumerableCast()
        {
            var db = DatabaseHelper.Open();
            foreach (User user in db.Users.All())
            {
                Assert.IsNotNull(user);
            }
        }

        [Test]
        public void TestFindWithSchemaQualification()
        {
            var db = DatabaseHelper.Open();
            
            var dboActual = db.dba.SchemaTable.FindById(1);
            var testActual = db.test.SchemaTable.FindById(1);

            Assert.IsNotNull(dboActual);
            Assert.AreEqual("Pass", dboActual.Description);
            Assert.IsNull(testActual);
        }

        [Test]
        public void TestFindWithCriteriaAndSchemaQualification()
        {
            var db = DatabaseHelper.Open();

            var dboActual = db.dba.SchemaTable.Find(db.dba.SchemaTable.Id == 1);

            Assert.IsNotNull(dboActual);
            Assert.AreEqual("Pass", dboActual.Description);
        }

        [Test]
        public void TestFindAllByIdWithSchemaQualification()
        {
            var db = DatabaseHelper.Open();
            var dbaCount = db.dba.SchemaTable.FindAllById(1).ToList().Count;
            var testCount = db.test.SchemaTable.FindAllById(1).ToList().Count;
            Assert.AreEqual(1, dbaCount);
            Assert.AreEqual(0, testCount);
        }
    
        [Test]
        public void TestFindOnAView()
        {
            var db = DatabaseHelper.Open();
            var u = db.VwCustomers.FindByCustomerId(1);
            Assert.IsNotNull(u);
        }

        [Test]
        public void TestCast()
        {
            var db = DatabaseHelper.Open();
            var userQuery = db.Users.All().Cast<User>() as IEnumerable<User>;
            Assert.IsNotNull(userQuery);
            var users = userQuery.ToList();
            Assert.AreNotEqual(0, users.Count);
        }

        [Test]
        public void FindByWithNamedParameter()
        {
            var db = DatabaseHelper.Open();
            var user = db.Users.FindBy(Name: "Bob");
            Assert.IsNotNull(user);

        }

        [Test]
        public void WithClauseShouldCastToStaticTypeWithCollection()
        {
            var db = DatabaseHelper.Open();
            Customer actual = db.Customers.WithOrders().FindByCustomerId(1);
            Assert.IsNotNull(actual);
            Assert.AreEqual(1, actual.Orders.Single().OrderId);
            Assert.AreEqual(new DateTime(2010, 10, 10), actual.Orders.Single().OrderDate);
        }

        [Test]
        public void NamedParameterAndWithClauseShouldCastToStaticTypeWithCollection()
        {
            var db = DatabaseHelper.Open();
            Customer actual = db.Customers.WithOrders().FindBy(CustomerId: 1);
            Assert.IsNotNull(actual);
            Assert.AreEqual(1, actual.Orders.Single().OrderId);
            Assert.AreEqual(new DateTime(2010, 10, 10), actual.Orders.Single().OrderDate);
        }

        [Test]
        public void ExpressionAndWithClauseShouldCastToStaticTypeWithCollection()
        {
            var db = DatabaseHelper.Open();
            Customer actual = db.Customers.WithOrders().Find(db.Customers.CustomerId == 1);
            Assert.IsNotNull(actual);
            Assert.AreEqual(1, actual.Orders.Single().OrderId);
            Assert.AreEqual(new DateTime(2010, 10, 10), actual.Orders.Single().OrderDate);
        }
    }
}
