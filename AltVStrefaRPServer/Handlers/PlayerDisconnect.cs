using System;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltVStrefaRPServer.Extensions;
using AltVStrefaRPServer.Modules.CharacterModule;
using AltVStrefaRPServer.Services.Characters;

namespace AltVStrefaRPServer.Handlers
{
    public class PlayerDisconnect
    {
        private ICharacterDatabaseService _characterDatabaseService;

        public PlayerDisconnect(ICharacterDatabaseService characterDatabaseService)
        {
            _characterDatabaseService = characterDatabaseService;

            Alt.Log("Player disconnect handler intialized");
            AltAsync.OnPlayerDisconnect += OnPlayerDisconnectAsync;
        }

        private async Task OnPlayerDisconnectAsync(ReadOnlyPlayer player, string reason)
        {
            var character = player.GetCharacter();
            if (character == null) return;

            character.Dimension = player.Dimension;
            character.UpdatePosition(player.Position);

            character.TimePlayed += (DateTime.Now - character.LastPlayed).Minutes;
            character.LastPlayed = DateTime.Now;

            CharacterManager.Instance.RemoveCharacterDataFromServer(character);
            Alt.Log($"CID({character.Id}) ID({player.Id}) {player.Name} left the server. Reason {reason}");
            await _characterDatabaseService.SaveCharacterAsync(character).ConfigureAwait(false);
        }
    }
}
