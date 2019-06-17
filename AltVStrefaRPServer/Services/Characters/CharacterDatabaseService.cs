using System.Collections.Generic;
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
        public Task<Character> FindCharacterByIdAsync(int characterId)
            => _serverContext.Characters.FindAsync(characterId);

        /// <summary>
        /// Searches for character in database by first name and last name. Null is returned if no entity was found
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <returns></returns>
        public Task<Character> FindCharacterAsync(string firstName, string lastName) 
            => _serverContext.Characters.AsNoTracking().FirstOrDefaultAsync(c => c.FirstName == firstName && c.LastName == lastName);

        public Task UpdateCharacterAsync(Character character)
        {
            _serverContext.Characters.Update(character);
            //_serverContext.Entry(character).State = EntityState.Detached;
            return _serverContext.SaveChangesAsync();
        }

        public Task UpdateCharactersAsync(IEnumerable<Character> characters)
        {
            _serverContext.Characters.UpdateRange(characters);
            return _serverContext.SaveChangesAsync();
        }
    }
}
