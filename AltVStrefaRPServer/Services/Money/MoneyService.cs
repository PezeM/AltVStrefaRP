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
            return true;
        }
    }
}
