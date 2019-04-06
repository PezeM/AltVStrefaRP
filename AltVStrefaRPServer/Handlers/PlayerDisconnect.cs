using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltVStrefaRPServer.Database;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Modules.Character;

namespace AltVStrefaRPServer.Handlers
{
    public class PlayerDisconnect
    {
        private ServerContext _serverContext;

        public PlayerDisconnect(ServerContext serverContext)
        {
            _serverContext = serverContext;

            Alt.Log("Player disconnect handler intialized");
            AltAsync.OnPlayerDisconnect += OnPlayerDisconnectAsync;
        }

        private async Task OnPlayerDisconnectAsync(ReadOnlyPlayer player, string reason)
        {
            var character = CharacterManager.Instance.GetCharacter(player);
            if (character == null) return;

            character.Dimension = player.Dimension;
            character.X = player.Position.X;
            character.Y = player.Position.Y;
            character.Z = player.Position.Z;

            CharacterManager.Instance.RemoveCharacterDataFromServer(character);
            Alt.Log($"CID({character.Id}) ID({player.Id}) {player.Name} left the server. Reason {reason}");
            await SaveCharacterAsync(character);
        }

        private async Task SaveCharacterAsync(Character character)
        {
            _serverContext.Characters.Update(character);
            await _serverContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
