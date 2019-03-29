﻿using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Services.Character.Customization;

namespace AltVStrefaRPServer.Modules.Character.Customization
{
    public class CharacterCreator
    {
        private ICharacterCreatorService _characterCreatorService;

        public CharacterCreator(ICharacterCreatorService characterCreatorService)
        {
            _characterCreatorService = characterCreatorService;

            Alt.Log("Character creator initialized.");
            AltAsync.OnClient("tryToCreateNewCharacter", TryToCreateNewCharacterAsync);
        }

        private async Task TryToCreateNewCharacterAsync(IPlayer player, object[] args)
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
            if (await _characterCreatorService.CheckIfCharacterExistsAsync(accountId.ToString(), accountId.ToString()))
            {
                // Error to user 
                Alt.Log($"User already exists");
                return;
            }

            // Create character, temporary name/last name
            var character = _characterCreatorService.CreateNewCharacter(accountId, accountId.ToString(), accountId.ToString(), 10, 0);
            await _characterCreatorService.SaveNewCharacter(character).ConfigureAwait(false);
            CharacterManager.Instance.IntializeCharacter(player, character);
            player.Emit("CharacterCreatedSuccessfully");
        }
    }
}
