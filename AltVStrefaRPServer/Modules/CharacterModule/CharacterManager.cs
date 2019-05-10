using System;
using System.Collections.Generic;
using System.Linq;
using AltV.Net;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using AltVStrefaRPServer.Models;

namespace AltVStrefaRPServer.Modules.CharacterModule
{
    public class CharacterManager
    {
        private static readonly Lazy<CharacterManager> lazy = new Lazy<CharacterManager>(() => new CharacterManager());

        public static CharacterManager Instance { get { return lazy.Value; } }

        private CharacterManager()
        {
        }

        public static Dictionary<int, Character> CharactersList { get; private set; } = new Dictionary<int, Character>();

        public Character GetCharacter(IPlayer player)
            => CharactersList.ContainsKey(player.Id) ? CharactersList[player.Id] : null;

        public Character GetCharacter(int characterId)
            => CharactersList.Values.FirstOrDefault(c => c.Id == characterId);

        public Character GetCharacter(string name, string lastName)
            => CharactersList.Values.FirstOrDefault(c => c.FirstName == name && c.LastName == lastName);

        /// <summary>
        /// Returns online character by bank account Id
        /// </summary>
        /// <param name="bankAccountId"></param>
        /// <returns></returns>
        public Character GetCharacterByBankAccount(int bankAccountId)
             => CharactersList.Values.FirstOrDefault(c => c.BankAccount.Id == bankAccountId);

        /// <summary>
        /// Initializes character in the game world
        /// </summary>
        /// <param name="player"></param>
        /// <param name="character"></param>
        public void IntializeCharacter(IPlayer player, Character character)
        {
            character.Player = player;
            player.Name = character.GetFullName();

            // TODO: Setting skin and shared data
            player.SetPosition(character.X, character.Y, character.Z);
            //player.Spawn(character.GetPosition());
            player.SetSyncedMetaData("remoteId", character.Id);
            player.Model = character.Gender == 0 ? (uint)PedModel.FreemodeMale01 : (uint)PedModel.FreemodeFemale01; 
            player.Dimension = character.Dimension;
            character.LastPlayed = DateTime.Now;

            CharactersList.Add(player.Id, character);
            Alt.Log($"Initialized character {character.GetFullName()} with ID({player.Id}) CID({character.Id}) in the world.");
        }

        /// <summary>
        /// Removes character data from server memory
        /// </summary>
        /// <param name="characterData"></param>
        public void RemoveCharacterDataFromServer(Character characterData)
        {
            CharactersList.Remove(characterData.Player.Id);
            Alt.Log($"Removed character data from server ID({characterData.Player.Id}) CID({characterData.Id})");
        }
    }
}
