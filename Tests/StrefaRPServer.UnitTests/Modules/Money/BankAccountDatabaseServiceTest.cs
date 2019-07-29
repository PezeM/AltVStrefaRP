using System.Linq;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Services.Money.Bank;
using NUnit.Framework;
using StrefaRPServer.UnitTests.Core;

namespace StrefaRPServer.UnitTests.Modules.Money
{
    [TestFixture]
    public class BankAccountDatabaseServiceTest : ServerContextTestBase
    {
        [Test]
        public void ReturnsAllBankAccounts()
        {
            var bankAccountDatabaseService = new BankAccountDatabaseService(_mockFactory.Object);
            var newData = new[]
            {
                new BankAccount{ AccountNumber = 1, Character = new Character()}, 
                new BankAccount{ AccountNumber = 2, Character = new Character()}, 
                new BankAccount{ AccountNumber = 3, Character = new Character()}, 
            };
            _context.BankAccounts.AddRange(newData);
            _context.SaveChanges();

            var result = bankAccountDatabaseService.GetAllBankAccounts();

            Assert.AreEqual(newData.Length, result.Count());
            Assert.AreEqual(newData, result.ToArray());
        }
        
        [Test]
        public void BasicTest()
        {
            var bankAccountDatabase = new BankAccountDatabaseService(_mockFactory.Object);
            var result = bankAccountDatabase.GetAllBankAccounts();

            Assert.AreEqual(0, result.Count());
        }
    }
}
