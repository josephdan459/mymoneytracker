using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using mymoneytracker;

namespace mymoneytracker_test
{
    [TestClass]
    public class Test_Transactions
    {
        [TestMethod]
        public void TestNewTransactionDefaultValues()
        {
            var t = new TransactionModel();

            Assert.IsNotNull(t);
            Assert.AreEqual(t.Payee, "Payee");
        }
    }
}
