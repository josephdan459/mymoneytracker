using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace mymoneytracker_test
{
    [TestClass]
    public class Test_Transactions
    {
        [TestMethod]
        public void TestNewTransactionDefaultValues()
        {
            var t = new mymoneytracker.TransactionModel();

            Assert.IsNotNull(t);
            Assert.AreEqual(t.Payee, "Payee");
        }
    }
}
