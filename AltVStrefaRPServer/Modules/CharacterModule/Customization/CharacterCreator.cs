using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Services.Characters.Accounts;
using AltVStrefaRPServer.Services.Characters.Customization;
using Microsoft.Extensions.Logging;

namespace AltVStrefaRPServer.Modules.CharacterModule.Customization
{
    public class CharacterCreator
    {
        private readonly IAccountDatabaseService _accountDatabaseService;
        private readonly ICharacterCreatorService _characterCreatorService;
        private readonly ILogger<CharacterCreator> _logger;

        public CharacterCreator(ICharacterCreatorService characterCreatorService, IAccountDatabaseService accountDatabaseService,
            ILogger<CharacterCreator> logger)
        {
            _characterCreatorService = characterCreatorService;
            _accountDatabaseService = accountDatabaseService;
            _logger = logger;

            AltAsync.On<IStrefaPlayer, Task>("tryToCreateNewCharacter", TryToCreateNewCharacterAsync);
        }

        private async Task TryToCreateNewCharacterAsync(IStrefaPlayer player)
        {
            // Get name and username

            // Create new character 
            if (player.AccountId == 0)
            {
                Alt.Log("Can't create new character. Account ID was 0.");
                await player.KickAsync("Nie byłeś poprawnie zalogowany.");
                return;
            }

            // Check if users exists
            if (await _characterCreatorService.CheckIfCharacterExistsAsync(player.AccountId.ToString().ToLower(), player.AccountId.ToString().ToLower()))
            {
                // Error to user 
                _logger.LogInformation("Character already exists");
                return;
            }

            // Create character, temporary name/last name
            var playerAccount = await _accountDatabaseService.GetAccountAsync(player.AccountId);
            var character = _characterCreatorService.CreateNewCharacter(player.AccountId, player.AccountId.ToString(), player.AccountId.ToString(), 10, 0);
            if (playerAccount != null)
                character.Account = playerAccount;

            await _characterCreatorService.SaveNewCharacter(character).ConfigureAwait(false);
            if (CharacterManager.Instance.IntializeCharacter(player, character))
            {
                await player.EmitAsync("CharacterCreatedSuccessfully");
            }
            else
            {
                // Emit error that player with given ID is already on the server
            }
        }
    }
}
