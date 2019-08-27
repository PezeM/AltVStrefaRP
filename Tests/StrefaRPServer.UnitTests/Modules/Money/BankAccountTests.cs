using AltVStrefaRPServer.Models;
using NUnit.Framework;

namespace StrefaRPServer.UnitTests.Modules.Money
{
    public class BankAccountTests
    {
        private BankAccount _bankAccount;
        private BankAccount _receiverBankAccount;

        [SetUp]
        public void Setup()
        {
            _bankAccount = new BankAccount();
            _receiverBankAccount = new BankAccount();
        }

        [TestCase(12.35f)]
        [TestCase(1135788)]
        public void DepositIncreasesMoney(float moneyToDeposit)
        {
             _bankAccount.AddMoney(moneyToDeposit);
            
            Assert.That(_bankAccount.Money, Is.EqualTo(moneyToDeposit));
        }

        [TestCase(999.99f)]
        [TestCase(0)]
        public void WithdrawDecreasesMoney(float moneyToWithdraw)
        {
            _bankAccount.Money = 1000f;

            var expected = _bankAccount.Money - moneyToWithdraw;
            var result = _bankAccount.RemoveMoney(moneyToWithdraw);

            Assert.That(_bankAccount.Money, Is.EqualTo(expected));
            Assert.That(result, Is.True);
        }

        [TestCase(1000.59f)]
        [TestCase(2000)]
        public void CantWithdrawMoreMoneyThanAccountHas(float moneyToWithdraw)
        {
            _bankAccount.Money = 1000.58f;

            var result = _bankAccount.RemoveMoney(moneyToWithdraw);

            Assert.That(result, Is.False);
        }

        [Test]
        public void TransferingMoneyDecreasesBalance()
        {
            var depositedMoney = 1000f;
            var moneyToTransfer = 100.01f;
            var moneyLeft = depositedMoney - moneyToTransfer;
            _bankAccount.Money = 1000f;

            _bankAccount.TransferMoney(_receiverBankAccount, moneyToTransfer);

            Assert.That(_bankAccount.Money, Is.EqualTo(moneyLeft));
            Assert.That(_receiverBankAccount.Money, Is.EqualTo(moneyToTransfer));
        }

        [Test]
        public void TransferingMoneyIncreasesBalanceOfReceiver()
        {
            var moneyToTransfer = 100.01f;
            var bankAccountInitialMoney = 1000f;
            var moneyLeft = bankAccountInitialMoney - moneyToTransfer;
            var receiver = new BankAccount();
            _bankAccount.Money = 1000f;

            _bankAccount.TransferMoney(receiver, moneyToTransfer);

            Assert.That(_bankAccount.Money, Is.EqualTo(moneyLeft));
            Assert.That(receiver.Money, Is.EqualTo(moneyToTransfer));
        }

        [TestCase(1000.01f)]
        [TestCase(2548f)]
        public void CantTransferMoreMoneyThanAccountHas(float moneyToTransfer)
        {
            var startMoney = 1000f;
            _bankAccount.Money = startMoney;

            var result = _bankAccount.TransferMoney(_receiverBankAccount, moneyToTransfer);

            Assert.That(result, Is.False);
            Assert.That(_bankAccount.Money, Is.EqualTo(startMoney));
            Assert.That(_receiverBankAccount.Money, Is.EqualTo(0));
        }

        [TestCase(100f, 120.50f)]
        [TestCase(325.24f, 578.45f)]
        public void TransferingAmountAfterTaxIncreasesReceiverBalanceByMoneyBeforeTax(float amountBeforeTax, float amountAfterTax)
        {
            var startMoney = 1000f;
            var moneyLeftInBankAccount = startMoney - amountAfterTax;
            _bankAccount.Money = startMoney;

            var result = _bankAccount.TransferMoney(_receiverBankAccount, amountBeforeTax, amountAfterTax);

            Assert.That(result, Is.True);
            Assert.That(_bankAccount.Money, Is.EqualTo(moneyLeftInBankAccount));
            Assert.That(_receiverBankAccount.Money, Is.EqualTo(amountBeforeTax));
        }
    }
}
