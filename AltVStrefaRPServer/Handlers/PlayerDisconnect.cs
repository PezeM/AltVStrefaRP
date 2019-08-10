using System;
using System.Threading.Tasks;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Extensions;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Modules.CharacterModule;
using AltVStrefaRPServer.Services.Characters;
using Microsoft.Extensions.Logging;

namespace AltVStrefaRPServer.Handlers
{
    public class PlayerDisconnect
    {
        private readonly ICharacterDatabaseService _characterDatabaseService;
        private readonly ILogger<PlayerDisconnect> _logger;

        public PlayerDisconnect(ICharacterDatabaseService characterDatabaseService, ILogger<PlayerDisconnect> logger)
        {
            _characterDatabaseService = characterDatabaseService;
            _logger = logger;

            _logger.LogDebug("Player disconnect handler initialized");
            AltAsync.OnPlayerDisconnect += OnPlayerDisconnectAsync;
        }

        private async Task OnPlayerDisconnectAsync(ReadOnlyPlayer player, IPlayer origin, string reason)
        {
            if (!player.TryGetCharacter(out Character character)) return;

            await AltAsync.Do(() =>
            {
                character.Dimension = player.Dimension;
                character.UpdatePosition(player.Position);

                character.TimePlayed += (DateTime.Now - character.LastPlayed).Minutes;
                character.LastPlayed = DateTime.Now;
            });

            _logger.LogInformation("Character {characterName} CID({characterId}) left the server. Reason {reason}. ID({playerId})",
                character.GetFullName(), character.Id, reason, player.Id);
            CharacterManager.Instance.RemoveCharacterDataFromServer(character);
            await _characterDatabaseService.UpdateCharacterAsync(character);
        }
    }
}
