using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AltVStrefaRPServer.Services.Characters
{
    public interface ICharacterDatabaseService
    {
        Task<Character> FindCharacterByIdAsync(int characterId);
        Task<Character> FindCharacterAsync(string firstName, string lastName);
        Task<List<CharacterSelectDto>> GetCharacterListAsync(int accountId);
        Task<Character> GetCharacterByIdAsync(int characterId);
        Task UpdateCharacterAsync(Character character);
        Task UpdateCharactersAsync(IEnumerable<Character> characters);
    }
}
