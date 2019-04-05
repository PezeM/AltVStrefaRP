using System.Threading.Tasks;
using AltVStrefaRPServer.Database;
using AltVStrefaRPServer.Models;

namespace AltVStrefaRPServer.Services.Money
{
    public class MoneyService
    {
        private ServerContext _serverContext;

        public MoneyService(ServerContext serverContext)
        {
            _serverContext = serverContext;
        }

        /// <summary>
        /// Gives character a certain amount of money
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool GiveMoney(Character receiver, float amount)
        {
            receiver.Money += amount;
            receiver.Player.SetSyncedMetaData("money", receiver.Money);
            return true;
        }

        /// <summary>
        /// Removes certain amount of money from character
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="amount"></param>
        /// <returns>True if character had enough money</returns>
        public bool RemoveMoney(Character receiver, float amount)
        {
            if (receiver.Money < amount) return false;
            receiver.Money -= amount;
            return true;
        }

        /// <summary>
        /// Transfers money from one character to another
        /// </summary>
        /// <param name="source"></param>
        /// <param name="receiver"></param>
        /// <param name="amount"></param>
        /// <returns>True if source character had enough money to transfer</returns>
        public bool TransferMoneyToAnotherPlayer(Character source, Character receiver, float amount)
        {
            if (source.Money < amount) return false;
            source.Money -= amount;
            receiver.Money += amount;
            receiver.Player.SetSyncedMetaData("money", receiver.Money);
            source.Player.SetSyncedMetaData("money", source.Money);
            LogMoneyTransaction(source.Id, receiver.Id, TransactionType.PlayerToPlayer, amount);
            return true;
        }

        /// <summary>
        /// Transfers money from one character to another
        /// </summary>
        /// <param name="source"></param>
        /// <param name="receiver"></param>
        /// <param name="amount"></param>
        /// <returns>True if source character had enough money to transfer</returns>
        public async Task<bool> TransferMoneyToAnotherPlayerAsync(Character source, Character receiver, float amount)
        {
            if (source.Money < amount) return false;
            source.Money -= amount;
            receiver.Money += amount;
            receiver.Player.SetSyncedMetaData("money", receiver.Money);
            source.Player.SetSyncedMetaData("money", source.Money);
            await LogMoneyTransactionAsync(source.Id, receiver.Id, TransactionType.PlayerToPlayer, amount).ConfigureAwait(false);
            return true;
        }


        /// <summary>
        /// Saves transaction information to database
        /// </summary>
        /// <param name="source">Source of the transaction</param>
        /// <param name="receiver">Receiver of the transactions</param>
        /// <param name="type">Type of the transaction <see cref="TransactionType"/></param>
        /// <param name="amount">Amount of transaction</param>
        private void LogMoneyTransaction(int source, int receiver, TransactionType type, float amount)
        {
            _serverContext.MoneyTransactions.Add(new MoneyTransaction(source, receiver, type, amount));
            _serverContext.SaveChanges();
        }

        /// <summary>
        /// Saves transaction information to database
        /// </summary>
        /// <param name="source">Source of the transaction</param>
        /// <param name="receiver">Receiver of the transactions</param>
        /// <param name="type">Type of the transaction <see cref="TransactionType"/></param>
        /// <param name="amount">Amount of transaction</param>
        private async Task LogMoneyTransactionAsync(int source, int receiver, TransactionType type, float amount)
        {
            await _serverContext.MoneyTransactions.AddAsync(new MoneyTransaction(source, receiver, type, amount)).ConfigureAwait(false);
            await _serverContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
