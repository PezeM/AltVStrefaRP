using AltVStrefaRPServer.Models;

namespace AltVStrefaRPServer.Services.Characters.Accounts
{
    public class AccountFactoryService : IAccountFactoryService
    {
        public Account CreateNewAccount(string login, string password)
        {
            return new Account
            {
                Username = login,
                Password = password,
                AdminLevel = AdminLevel.None
            };
        }
    }
}
