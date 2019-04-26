using System.Threading.Tasks;
using AltVStrefaRPServer.Database;
using AltVStrefaRPServer.Models;
using Microsoft.EntityFrameworkCore;

namespace AltVStrefaRPServer.Services.Characters
{
    public class CharacterDatabaseService : ICharacterDatabaseService
    {
        private ServerContext _serverContext;

        public CharacterDatabaseService(ServerContext serverContext)
        {
            _serverContext = serverContext;
        }

        /// <summary>
        /// Searches for character in the database. Null is returned if no entity was found
        /// </summary>
        /// <param name="characterId">find</param>
        /// <returns></returns>
        public async Task<Character> FindCharacterByIdAsync(int characterId) 
            => await _serverContext.Characters.FirstOrDefaultAsync(c => c.Id == characterId);

        /// <summary>
        /// Searches for character in database by first name and last name. Null is returned if no entity was found
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <returns></returns>
        public async Task<Character> FindCharacterAsync(string firstName, string lastName) 
            => await _serverContext.Characters.FirstOrDefaultAsync(c => c.FirstName == firstName && c.LastName == lastName);

        public async Task SaveCharacterAsync(Character character)
        {
            _serverContext.Characters.Update(character);
            await _serverContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
