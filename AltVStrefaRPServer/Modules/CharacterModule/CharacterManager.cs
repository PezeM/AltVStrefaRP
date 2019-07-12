using AltV.Net;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using AltVStrefaRPServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using AltVStrefaRPServer.Data;

namespace AltVStrefaRPServer.Modules.CharacterModule
{
    public class CharacterManager
    {
        private static readonly Lazy<CharacterManager> lazy = new Lazy<CharacterManager>(() => new CharacterManager());
        public static CharacterManager Instance { get { return lazy.Value; } }

        private Dictionary<int, Character> _characterList;

        private CharacterManager()
        {
            _characterList = new Dictionary<int, Character>();
        }


        public Character GetCharacter(IPlayer player)
            => _characterList.ContainsKey(player.Id) ? _characterList[player.Id] : null;

        public bool TryGetCharacter(IPlayer player, out Character character) 
            => _characterList.TryGetValue(player.Id, out character);

        public Character GetCharacter(int characterId)
            => _characterList.Values.FirstOrDefault(c => c.Id == characterId);

        public Character GetCharacter(string name, string lastName)
            => _characterList.Values.FirstOrDefault(c => c.FirstName == name && c.LastName == lastName);

        public IEnumerable<Character> GetAllCharacters() => _characterList.Values;

        /// <summary>
        /// Returns online character by bank account Id
        /// </summary>
        /// <param name="bankAccountId"></param>
        /// <returns></returns>
        public Character GetCharacterByBankAccount(int bankAccountId)
             => _characterList.Values.FirstOrDefault(c => c.BankAccount.Id == bankAccountId);

        /// <summary>
        /// Initializes character in the game world
        /// </summary>
        /// <param name="player"></param>
        /// <param name="character"></param>
        public bool IntializeCharacter(IPlayer player, Character character)
        {
            lock (_characterList)
            {
                if (_characterList.ContainsKey(player.Id)) return false;
                character.Player = player;
                player.SetSyncedMetaData(MetaData.PLAYER_NAME, character.GetFullName());
                player.SetSyncedMetaData(MetaData.REMOTE_ID, character.Id);

                // TODO: Setting skin and shared data
                player.SetPosition(character.X, character.Y, character.Z);
                //player.Spawn(character.GetPosition());
                player.Model = character.Gender == 0 ? (uint)PedModel.FreemodeMale01 : (uint)PedModel.FreemodeFemale01; 
                player.Dimension = character.Dimension;
                character.LastPlayed = DateTime.Now;

                _characterList.Add(player.Id, character);
                Alt.Log($"Initialized character {character.GetFullName()} with ID({player.Id}) CID({character.Id}) in the world.");
                return true;
            }
        }

        /// <summary>
        /// Removes character data from server memory
        /// </summary>
        /// <param name="characterData"></param>
        public void RemoveCharacterDataFromServer(Character characterData)
        {
            lock (_characterList)
            {
                _characterList.Remove(characterData.Player.Id);
            }
            Alt.Log($"Removed character data from server ID({characterData.Player.Id}) CID({characterData.Id})");
        }
    }
}
