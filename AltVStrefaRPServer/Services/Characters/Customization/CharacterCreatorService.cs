using System;
using System.Threading.Tasks;
using AltVStrefaRPServer.Database;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Inventory;
using AltVStrefaRPServer.Models.Server;
using Microsoft.EntityFrameworkCore;

namespace AltVStrefaRPServer.Services.Characters.Customization
{
    public class CharacterCreatorService : ICharacterCreatorService
    {
        private readonly Func<ServerContext> _factory;

        public CharacterCreatorService(Func<ServerContext> factory)
        {
            _factory = factory;
        }

        public async Task<bool> CheckIfCharacterExistsAsync(string firstName, string lastName)
        {
            using (var context = _factory.Invoke())
            {
                return await context.Characters.AsNoTracking()
                    .AnyAsync(c => c.FirstName == firstName && c.LastName == lastName);
            }
        }

        public async Task SaveNewCharacter(Character character)
        {
            using (var context = _factory.Invoke())
            {
                await context.AddAsync(character).ConfigureAwait(false);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public Character CreateNewCharacter(int accountId, string firstName, string lastName, int age, int gender)
        {
            return new Character
            {
                FirstName = firstName,
                LastName = lastName,
                AccountId = accountId,
                BankAccount = null,
                Age = age,
                Money = AppSettings.Current.ServerConfig.StartingMoney,
                Inventory = new InventoryController(50),
                Gender = gender,
                Dimension = 0,
                X = AppSettings.Current.ServerConfig.SpawnPosition.X,
                Y = AppSettings.Current.ServerConfig.SpawnPosition.Y,
                Z = AppSettings.Current.ServerConfig.SpawnPosition.Z,
                ProfileImage = "default-profile-image.jpg",
                CreationDate = DateTime.Now,
                LastPlayed = DateTime.Now,
                TimePlayed = 0,
                BusinessRank = 0,
                FractionRank = 0,
                CanDriveVehicles = true,
                IsBanned = false,
                IsMuted = true,
            };
        }
    }
}
