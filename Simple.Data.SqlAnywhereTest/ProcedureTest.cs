﻿using System.Diagnostics;
using NUnit.Framework;

namespace Simple.Data.SqlAnywhereTest
{
    using System.Data;

    [TestFixture]
    public class ProcedureTest
    {
        [Test]
        public void GetCustomersTest()
        {
            var db = DatabaseHelper.Open();
            var results = db.GetCustomers();
            var actual = results.First();
            Assert.AreEqual(1, actual.CustomerId);
        }

        [Test]
        public void GetCustomerCountTest()
        {
            var db = DatabaseHelper.Open();
            var results = db.GetCustomerCount();
            Assert.AreEqual(1, results.ReturnValue);
        }

        [Test]
        public void FindGetCustomerCountAndInvokeTest()
        {
            var db = DatabaseHelper.Open();
            var getCustomerCount = db.GetCustomerCount;
            var results = getCustomerCount();
            Assert.AreEqual(1, results.ReturnValue);
        }

        [Test]
        public void FindGetCustomerCountUsingIndexerAndInvokeTest()
        {
            var db = DatabaseHelper.Open();
            var getCustomerCount = db["GetCustomerCount"];
            var results = getCustomerCount();
            Assert.AreEqual(1, results.ReturnValue);
        }

        [Test]
        public void GetCustomerCountSecondCallExecutesNonQueryTest()
        {
            var listener = new TestTraceListener();
            Trace.Listeners.Add(listener);
            var db = DatabaseHelper.Open();
            db.GetCustomerCount();
            Assert.IsFalse(listener.Output.Contains("ExecuteNonQuery"));
            db.GetCustomerCount();
            Assert.IsTrue(listener.Output.Contains("ExecuteNonQuery"));
            Trace.Listeners.Remove(listener);
        }

        [Test]
        public void GetCustomerAndOrdersTest()
        {
            var db = DatabaseHelper.Open();
            var results = db.GetCustomerAndOrders(1);
            var customer = results.FirstOrDefault();
            Assert.IsNotNull(customer);
            Assert.AreEqual(1, customer.CustomerId);
            Assert.IsTrue(results.NextResult());
            var order = results.FirstOrDefault();
            Assert.IsNotNull(order);
            Assert.AreEqual(1, order.OrderId);
        }

        [Test]
        public void GetCustomerAndOrdersStillWorksAfterZeroRecordCallTest()
        {
            var db = DatabaseHelper.Open();
            db.GetCustomerAndOrders(1000);
            var results = db.GetCustomerAndOrders(1);
            var customer = results.FirstOrDefault();
            Assert.IsNotNull(customer);
            Assert.AreEqual(1, customer.CustomerId);
            Assert.IsTrue(results.NextResult());
            var order = results.FirstOrDefault();
            Assert.IsNotNull(order);
            Assert.AreEqual(1, order.OrderId);
        }

        [Test]
        public void ScalarFunctionIsCalledCorrectly()
        {
            var db = DatabaseHelper.Open();
            var results = db.VarcharAndReturnInt("The answer to everything");
            Assert.AreEqual(42, results.ReturnValue);
        }
    }
}
