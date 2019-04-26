using System.Threading.Tasks;
using AltV.Net.Async;
using AltVStrefaRPServer.Models;

namespace AltVStrefaRPServer.Services.Characters
{
    public interface ICharacterDatabaseService
    {
        Task<Character> FindCharacterByIdAsync(int characterId);
        Task<Character> FindCharacterAsync(string firstName, string lastName);
        Task SaveCharacterAsync(Character character);
    }
}
