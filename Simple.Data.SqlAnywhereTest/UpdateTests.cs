﻿using System.Dynamic;
using System.Linq;
using NUnit.Framework;
using Simple.Data.SqlAnywhereTest.Resources;

namespace Simple.Data.SqlAnywhereTest
{
    using System.Collections.Generic;

    [TestFixture]
    public class UpdateTests
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            DatabaseHelper.Reset();
        }

        [Test]
        public void TestUpdateWithNamedArguments()
        {
            var db = DatabaseHelper.Open();

            db.Users.UpdateById(Id: 1, Name: "Ford", Password: "hoopy", Age: 29);
            var user = db.Users.FindById(1);
            Assert.IsNotNull(user);
            Assert.AreEqual("Ford", user.Name);
            Assert.AreEqual("hoopy", user.Password);
            Assert.AreEqual(29, user.Age);
        }

        [Test]
        public void TestUpdateWithStaticTypeObject()
        {
            var db = DatabaseHelper.Open();

            var user = new User { Id = 2, Name = "Zaphod", Password = "zarquon", Age = 42 };

            db.Users.Update(user);

            User actual = db.Users.FindById(2);

            Assert.IsNotNull(user);
            Assert.AreEqual("Zaphod", actual.Name);
            Assert.AreEqual("zarquon", actual.Password);
            Assert.AreEqual(42, actual.Age);
        }

        [Test]
        public void TestUpdateWithDynamicTypeObject()
        {
            var db = DatabaseHelper.Open();

            dynamic user = new ExpandoObject();
            user.Id = 3;
            user.Name = "Marvin";
            user.Password = "diodes";
            user.Age = 42000000;

            db.Users.Update(user);

            var actual = db.Users.FindById(3);

            Assert.IsNotNull(user);
            Assert.AreEqual("Marvin", actual.Name);
            Assert.AreEqual("diodes", actual.Password);
            Assert.AreEqual(42000000, actual.Age);
        }

        [Test]
        public void TestUpdateWithVarBinaryMaxColumn()
        {
            var db = DatabaseHelper.Open();
            var blob = new Blob
                           {
                               Id = 1,
                               Data = new byte[] {9, 8, 7, 6, 5, 4, 3, 2, 1, 0}
                           };
            db.Blobs.Insert(blob);

            var newData = blob.Data = new byte[] {0,1,2,3,4,5,6,7,8,9};

            db.Blobs.Update(blob);

            blob = db.Blobs.FindById(1);
            
            Assert.IsTrue(newData.SequenceEqual(blob.Data));
        }

        [Test]
        public void ToListShouldExecuteQuery()
        {
            var db = DatabaseHelper.Open();
            List<Customer> customers = db.Customers.All().ToList<Customer>();
            foreach (var customer in customers)
            {
                customer.Address = "Updated";
            }

            db.Customers.Update(customers);
        }
    }
}