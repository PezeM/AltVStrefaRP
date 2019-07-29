using AltVStrefaRPServer.Models;
using Moq;
using NUnit.Framework;

namespace StrefaRPServer.UnitTests.Modules.Money
{
    public class BankAccountTests
    {
        private BankAccount _bankAccount;
        private Mock<IMoney> _receiverMock;

        [SetUp]
        public void Setup()
        {
            _bankAccount = new BankAccount();
            _receiverMock = new Mock<IMoney>();
        }

        [TestCase(12.35f)]
        [TestCase(1135788)]
        public void DepositIncreasesMoney(float moneyToDeposit)
        {
            var result = _bankAccount.DepositMoney(moneyToDeposit);
            
            Assert.That(_bankAccount.Money, Is.EqualTo(moneyToDeposit));
            Assert.That(result, Is.True);
        }

        [TestCase(999.99f)]
        [TestCase(0)]
        public void WithdrawDecreasesMoney(float moneyToWithdraw)
        {
            _bankAccount.Money = 1000f;

            var expected = _bankAccount.Money - moneyToWithdraw;
            var result = _bankAccount.WithdrawMoney(moneyToWithdraw);

            Assert.That(_bankAccount.Money, Is.EqualTo(expected));
            Assert.That(result, Is.True);
        }

        [TestCase(1000.59f)]
        [TestCase(2000)]
        public void CantWithdrawMoreMoneyThanAccountHas(float moneyToWithdraw)
        {
            _bankAccount.Money = 1000.58f;

            var result = _bankAccount.WithdrawMoney(moneyToWithdraw);

            Assert.That(result, Is.False);
        }

        [Test]
        public void TransferingMoneyDescreasesBalance()
        {
            var depositedMoney = 1000f;
            var moneyToTransfer = 100.01f;
            var moneyLeft = depositedMoney - moneyToTransfer;
            _bankAccount.Money = 1000f;

            _bankAccount.TransferMoney(_receiverMock.Object, moneyToTransfer);

            Assert.That(_bankAccount.Money, Is.EqualTo(moneyLeft));
        }

        [Test]
        public void TransferingMoneyIncreasesBalanceOfReceiver()
        {
            var moneyToTransfer = 100.01f;
            var bankAccountInitialMoney = 1000f;
            var moneyLeft = bankAccountInitialMoney - moneyToTransfer;
            _bankAccount.Money = 1000f;

            _bankAccount.TransferMoney(_receiverMock.Object, moneyToTransfer);

            Assert.That(_bankAccount.Money, Is.EqualTo(moneyLeft));
            _receiverMock.VerifySet(m => m.Money = moneyToTransfer, Times.Once);
        }

        [TestCase(1000.01f)]
        [TestCase(2548f)]
        public void CantTransferMoreMoneyThanAccountHas(float moneyToTransfer)
        {
            var startMoney = 1000f;
            _bankAccount.Money = startMoney;

            var result = _bankAccount.TransferMoney(_receiverMock.Object, moneyToTransfer);

            Assert.That(result, Is.False);
            Assert.That(_bankAccount.Money, Is.EqualTo(startMoney));
            _receiverMock.VerifySet(m => m.Money = It.IsAny<float>(), Times.Never);
        }

        [TestCase(100f, 120.50f)]
        [TestCase(325.24f, 578.45f)]
        public void TransferingAmountAfterTaxIncreasesReceiverBalanceByMoneyBeforeTax(float amountBeforeTax, float amountAfterTax)
        {
            var startMoney = 1000f;
            var moneyLeftInBankAccount = startMoney - amountAfterTax;
            _bankAccount.Money = startMoney;

            var result = _bankAccount.TransferMoney(_receiverMock.Object, amountBeforeTax, amountAfterTax);

            Assert.That(result, Is.True);
            Assert.That(_bankAccount.Money, Is.EqualTo(moneyLeftInBankAccount));
            _receiverMock.VerifySet(m => m.Money = amountBeforeTax, Times.Once);
        }
    }
}
