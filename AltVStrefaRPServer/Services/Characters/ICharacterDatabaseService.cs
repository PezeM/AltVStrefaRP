using System.Collections.Generic;
using System.Threading.Tasks;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Dto;

namespace AltVStrefaRPServer.Services.Characters
{
    public interface ICharacterDatabaseService
    {
        Task<Character> FindCharacterByIdAsync(int characterId);
        Task<Character> FindCharacterAsync(string firstName, string lastName);
        Task<List<CharacterSelectDto>> GetCharacterList(int accountId);
        Task<Character> GetCharacterById(int characterId);
        Task UpdateCharacterAsync(Character character);
        Task UpdateCharactersAsync(IEnumerable<Character> characters);
    }
}
