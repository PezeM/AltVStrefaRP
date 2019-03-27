using System;
using System.Collections.Generic;
using System.Linq;
using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;

namespace AltVStrefaRPServer.Modules.Character
{
    public class CharacterManager
    {
        private static readonly Lazy<CharacterManager> lazy = new Lazy<CharacterManager>(() => new CharacterManager());

        public static CharacterManager Instance { get { return lazy.Value; } }

        private CharacterManager()
        {
        }

        public static Dictionary<int, Models.Character> CharactersList { get; private set; } = new Dictionary<int, Models.Character>();

        public Models.Character GetCharacter(IPlayer player)
            => CharactersList.ContainsKey(player.Id) ? CharactersList[player.Id] : null;

        public Models.Character GetCharacter(int characterId)
            => CharactersList.Values.FirstOrDefault(c => c.Id == characterId);

        /// <summary>
        /// Initializes character in the game world
        /// </summary>
        /// <param name="player"></param>
        /// <param name="character"></param>
        public void IntializeCharacter(IPlayer player, Models.Character character)
        {
            character.Player = player;
            player.Name = character.GetFullName();

            // TODO: Setting skin and shared data
            player.Dimension = character.Dimension;
            player.Position = new Position(character.X, character.Y, character.Z);

            CharactersList.Add(player.Id, character);
            Alt.Log($"Initialized character {character.GetFullName()} with ID({player.Id}) CID({character.Id}) in the world.");
        }
    }
}
