using System.Collections.Generic;
using System.Threading.Tasks;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Dto;

namespace AltVStrefaRPServer.Services.Characters
{
    public interface ILogin
    {
        Task<Account> GetAccountAsync(string username);
        Task CreateNewAccountAndSaveAsync(string username, string password);
        Task<bool> CheckIfAccountExistsAsync(string username);
        Task<List<CharacterSelectDto>> GetCharacterList(int accountId);
        Task<Character> GetCharacterById(int characterId);
        string GeneratePassword(string password);
        bool VerifyPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
        bool IsPasswordValid(string password);
    }
}
