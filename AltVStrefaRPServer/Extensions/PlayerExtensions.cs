using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Modules.CharacterModule;

namespace AltVStrefaRPServer.Extensions
{
    public static class PlayerExtensions
    {
        /// <summary>
        /// Gets character model from player. Returns null if character was not found
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static Character GetCharacter(this IPlayer player) 
            => CharacterManager.Instance.GetCharacter(player);

        /// <summary>
        /// Try to get character from all online players in the server. Returns false if character was not found
        /// </summary>
        /// <param name="player"></param>
        /// <param name="character"></param>
        /// <returns></returns>
        public static bool TryGetCharacter(this IPlayer player, out Character character) 
            => CharacterManager.Instance.TryGetCharacter(player, out character);
    }
}
