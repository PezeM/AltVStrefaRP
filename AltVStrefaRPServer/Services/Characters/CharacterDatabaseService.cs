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
        /// Searches for character in the database. If no entity was found then null is returned
        /// </summary>
        /// <param name="characterId">find</param>
        /// <returns></returns>
        public async Task<Character> FindCharacterByIdAsync(int characterId) 
            => await _serverContext.Characters.AsNoTracking().FirstOrDefaultAsync(c => c.Id == characterId);

        public async Task SaveCharacterAsync(Character character)
        {
            _serverContext.Characters.Update(character);
            await _serverContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
