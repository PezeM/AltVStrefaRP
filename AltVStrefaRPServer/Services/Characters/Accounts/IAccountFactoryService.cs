using AltVStrefaRPServer.Models;

namespace AltVStrefaRPServer.Services.Characters.Accounts
{
    public interface IAccountFactoryService
    {
        Account CreateNewAccount(string login, string password);
    }
}
