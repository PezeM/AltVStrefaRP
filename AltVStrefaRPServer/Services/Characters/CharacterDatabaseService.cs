using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AltVStrefaRPServer.Database;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Dto;
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
        public async Task<Character> FindCharacterByIdAsync(int characterId)
        {
            using (var context = _factory.Invoke())
            {
                return await context.Characters.FindAsync(characterId);
            }
        }

        /// <summary>
        /// Searches for character in database by first name and last name. Null is returned if no entity was found
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <returns></returns>
        public async Task<Character> FindCharacterAsync(string firstName, string lastName)
        {
            using (var context = _factory.Invoke())
            {
                return await context.Characters.AsNoTracking()
                    .Include(q => q.BankAccount)
                    .Include(q => q.Fraction)
                    .Include(q => q.Business)
                    .FirstOrDefaultAsync(c => c.FirstName == firstName && c.LastName == lastName);
            }
        }

        public async Task<List<CharacterSelectDto>> GetCharacterList(int accountId)
        {
            using (var context = _factory.Invoke())
            {
                return await context.Characters.AsNoTracking().Where(c => c.AccountId == accountId).Select(c => new CharacterSelectDto
                {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Money = c.Money,
                    ProfileImage = c.ProfileImage,
                    TimePlayed = c.TimePlayed
                }).ToListAsync();
            }
        }

        public async Task<Character> GetCharacterById(int characterId)
        {
            using (var context = _factory.Invoke())
            {
                return await context.Characters
                    .Include(c => c.BankAccount)
                    .Include(c => c.Account)
                    .FirstOrDefaultAsync(c => c.Id == characterId);
            }
        }

        public async Task UpdateCharacterAsync(Character character)
        {
            using (var context = _factory.Invoke())
            {
                context.Characters.Update(character);
                //_serverContext.Entry(character).State = EntityState.Detached;
                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateCharactersAsync(IEnumerable<Character> characters)
        {
            using (var context = _factory.Invoke())
            {
                context.Characters.UpdateRange(characters);
                await context.SaveChangesAsync();
            }
        }
    }
}
