using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Services.Characters.Customization;
using Microsoft.Extensions.Logging;

namespace AltVStrefaRPServer.Modules.CharacterModule.Customization
{
    public class CharacterCreator
    {
        private ICharacterCreatorService _characterCreatorService;
        private ILogger<CharacterCreator> _logger;

        public CharacterCreator(ICharacterCreatorService characterCreatorService, ILogger<CharacterCreator> logger)
        {
            _characterCreatorService = characterCreatorService;
            _logger = logger;

            AltAsync.On<IPlayer, Task>("tryToCreateNewCharacter", TryToCreateNewCharacterAsync);
        }

        private async Task TryToCreateNewCharacterAsync(IPlayer player)
        {
            // Get name and username

            // Create new character 
            if (!player.GetData("accountId", out int accountId))
            {
                // Return to user
                Alt.Log($"User {player.Name} doesn't have accountId {accountId}");
                return;
            }

            // Check if users exists
            if (await _characterCreatorService.CheckIfCharacterExistsAsync(accountId.ToString().ToLower(), accountId.ToString().ToLower()))
            {
                // Error to user 
                _logger.LogInformation("Character already exists");
                return;
            }

            // Create character, temporary name/last name
            var character = _characterCreatorService.CreateNewCharacter(accountId, accountId.ToString(), accountId.ToString(), 10, 0);
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
