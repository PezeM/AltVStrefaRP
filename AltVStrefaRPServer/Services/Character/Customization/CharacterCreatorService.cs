using System.Threading.Tasks;
using AltVStrefaRPServer.Database;
using Microsoft.EntityFrameworkCore;

namespace AltVStrefaRPServer.Services.Character.Customization
{
    public class CharacterCreatorService : ICharacterCreatorService
    {
        private ServerContext _serverContext;

        public CharacterCreatorService(ServerContext serverContext)
        {
            _serverContext = serverContext;
        }

        public async Task<bool> CheckIfCharacterExistsAsync(string firstName, string lastName)
            => await _serverContext.Characters.AsNoTracking().AnyAsync(c => c.FirstName == firstName && c.LastName == lastName);

        public async Task SaveNewCharacter(Models.Character character)
        {
            await _serverContext.AddAsync(character).ConfigureAwait(false);
            await _serverContext.SaveChangesAsync().ConfigureAwait(false);
        }
        public Models.Character CreateNewCharacter(int accountId, string firstName, string lastName, int age, int gender)
        {
            return new Models.Character
            {
                FirstName = firstName,
                LastName = lastName,
                AccountId = accountId,
                BankAccountId = 0,
                Age = age,
                Money = 250f,
                Gender = gender,
                Dimension = 0,
                X = AppSettings.Current.ServerConfig.SpawnPosition.X,
                Y = AppSettings.Current.ServerConfig.SpawnPosition.Y,
                Z = AppSettings.Current.ServerConfig.SpawnPosition.Z,
                BackgroundImage = "../images/profile-card-background.jpg",
                ProfileImage = "../images/default-profile-image.jpg",
                TimePlayed = 0
            };
        }
    }
}
