using AltVStrefaRPServer.Models;
using NUnit.Framework;

namespace StrefaRPServer.UnitTests
{
    public class BankAccountTests
    {
        private BankAccount _bankAccount;

        [SetUp]
        public void Setup()
        {
            _bankAccount = new BankAccount();
        }

        [Test]
        public void BasicTest()
        {
            var moneyToDeposit = 20;
            _bankAccount.DepositMoney(moneyToDeposit);
            
            Assert.AreEqual(_bankAccount.Money, moneyToDeposit);
        }
    }
}
