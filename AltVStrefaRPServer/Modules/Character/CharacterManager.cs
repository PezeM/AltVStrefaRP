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
        /// Returns online character by bank account Id
        /// </summary>
        /// <param name="bankAccountId"></param>
        /// <returns></returns>
        public Models.Character GetCharacterByBankAccount(int bankAccountId)
             => CharactersList.Values.FirstOrDefault(c => c.BankAccount.Id == bankAccountId);

        /// <summary>
        /// Initializes character in the game world
        /// </summary>
        /// <param name="player"></param>
        /// <param name="character"></param>
        public void IntializeCharacter(IPlayer player, Models.Character character)
        {
            character.Player = player;
            player.Name = character.GetFullName();

            Alt.Log($"Putting character at position ${character.GetPosition()}");
            // TODO: Setting skin and shared data
            //player.Position = new Position(character.X, character.Y, character.Z);
            player.Spawn(character.GetPosition());
            player.Dimension = character.Dimension;
            character.LastPlayed = DateTime.Now;

            CharactersList.Add(player.Id, character);
            Alt.Log($"Initialized character {character.GetFullName()} with ID({player.Id}) CID({character.Id}) in the world.");
        }

        /// <summary>
        /// Removes character data from server memory
        /// </summary>
        /// <param name="characterData"></param>
        public void RemoveCharacterDataFromServer(Models.Character characterData)
        {
            CharactersList.Remove(characterData.Player.Id);
            Alt.Log($"Removed character data from server for ID({characterData.Player.Id}) CID({characterData.Id})");
        }
    }
}
