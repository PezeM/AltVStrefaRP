using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AltV.Net;
using AltVStrefaRPServer.Database;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace AltVStrefaRPServer.Services.Character
{
    public class Login : ILogin
    {
        private ServerContext _serverContext;
        private HashingService _hashingService;
        private Regex _regex;

        public Login(ServerContext serverContext, HashingService hashingService)
        {
            _serverContext = serverContext;
            _hashingService = hashingService;
            _regex = new Regex("^(?=.*[0-9])(?=.*[!@#$%^&*])[a-zA-Z0-9!@#$%^&*]{6,18}$");
        }

        /// <summary>
        /// Gets account from database by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns>Account if found,null if account is not found</returns>
        public async Task<Account> GetAccountAsync(string username)
            => await _serverContext.Accounts.AsNoTracking().FirstOrDefaultAsync(a => a.Username == username);

        /// <summary>
        /// Checks if account with given username is already in database
        /// </summary>
        /// <param name="username"></param>
        /// <returns>Returns true if there is already account with given username</returns>
        public async Task<bool> CheckIfAccountExistsAsync(string username)
            => await _serverContext.Accounts.AsNoTracking().AnyAsync(a => a.Username == username);

        /// <summary>
        /// Generates new hashed password and creates new account in the database
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task CreateNewAccountAndSaveAsync(string username, string password)
        {
            await _serverContext.Accounts.AddAsync(new Account
            {
                Username = username,
                Password = GeneratePassword(password),
            }).ConfigureAwait(false);
            await _serverContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<List<CharacterSelectDto>> GetCharacterList(int accountId)
            => await _serverContext.Characters.AsNoTracking().Where(c => c.AccountId == accountId).Select(c => new CharacterSelectDto
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                BackgroundImage = c.BackgroundImage,
                ProfileImage = c.ProfileImage,
                TimePlayed = c.TimePlayed
            }).ToListAsync();

        public async Task<Models.Character> GetCharacterById(int characterId)
            => await _serverContext.Characters.AsNoTracking().FirstOrDefaultAsync(c => c.Id == characterId);

        public string GeneratePassword(string password) => _hashingService.Hash(password, 1000);

        public bool VerifyPassword(string password) => _hashingService.Verify(password, _hashingService.Hash(password, 1000));

        public bool VerifyPassword(string password, string hashedPassword)
            => _hashingService.Verify(password, hashedPassword);


        /// <summary>
        /// Checks if password passes requirements
        /// </summary>
        /// <param name="password"></param>
        /// <returns>True is password is valid</returns>
        public bool IsPasswordValid(string password) => _regex.IsMatch(password);
    }
}
