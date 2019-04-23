using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Modules.CharacterModule;

namespace AltVStrefaRPServer
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
    }
}
