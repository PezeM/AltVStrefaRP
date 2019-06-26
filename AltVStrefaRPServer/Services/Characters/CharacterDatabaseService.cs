using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AltVStrefaRPServer.Database;
using AltVStrefaRPServer.Models;
using Microsoft.EntityFrameworkCore;

namespace AltVStrefaRPServer.Services.Characters
{
    public class CharacterDatabaseService : ICharacterDatabaseService
    {
        private readonly Func<ServerContext> _factory;

        public CharacterDatabaseService(Func<ServerContext> factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Searches for character in the database. Null is returned if no entity was found
        /// </summary>
        /// <param name="characterId">find</param>
        /// <returns></returns>
        public Task<Character> FindCharacterByIdAsync(int characterId)
        {
            using (var context = _factory.Invoke())
            {
                return context.Characters.FindAsync(characterId);
            }
        }

        /// <summary>
        /// Searches for character in database by first name and last name. Null is returned if no entity was found
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <returns></returns>
        public Task<Character> FindCharacterAsync(string firstName, string lastName)
        {
            using (var context = _factory.Invoke())
            {
                return context.Characters.AsNoTracking()
                    .Include(q => q.BankAccount)
                    .Include(q => q.Fraction)
                    .Include(q => q.Business)
                    .FirstOrDefaultAsync(c => c.FirstName == firstName && c.LastName == lastName);
            }
        }

        public Task UpdateCharacterAsync(Character character)
        {
            using (var context = _factory.Invoke())
            {
                context.Characters.Update(character);
                //_serverContext.Entry(character).State = EntityState.Detached;
                return context.SaveChangesAsync();
            }
        }

        public Task UpdateCharactersAsync(IEnumerable<Character> characters)
        {
            using (var context = _factory.Invoke())
            {
                context.Characters.UpdateRange(characters);
                return context.SaveChangesAsync();
            }
        }
    }
}
