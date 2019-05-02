using System.Collections.Generic;
using System.Threading.Tasks;
using AltVStrefaRPServer.Models;

namespace AltVStrefaRPServer.Services.Characters
{
    public interface ICharacterDatabaseService
    {
        Task<Character> FindCharacterByIdAsync(int characterId);
        Task<Character> FindCharacterAsync(string firstName, string lastName);
        Task UpdateCharacterAsync(Character character);
        Task UpdateCharactersAsync(IEnumerable<Character> characters);
    }
}
