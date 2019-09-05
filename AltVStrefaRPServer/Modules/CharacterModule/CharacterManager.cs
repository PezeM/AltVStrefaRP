using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using AltVStrefaRPServer.Data;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Interfaces.Managers;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using AltVStrefaRPServer.Models.Inventory;
using AltVStrefaRPServer.Modules.Core;

namespace AltVStrefaRPServer.Modules.CharacterModule
{
    public sealed class CharacterManager : ICharacterManager
    {
        private static readonly Lazy<CharacterManager> lazy = new Lazy<CharacterManager>(() => new CharacterManager());
        public static CharacterManager Instance { get { return lazy.Value; } }

        private Dictionary<int, Character> _characters;

        private CharacterManager()
        {
            _characters = new Dictionary<int, Character>();
        }

        public Character GetCharacter(IPlayer player)
            => _characters.ContainsKey(player.Id) ? _characters[player.Id] : null;

        public bool TryGetCharacter(IPlayer player, out Character character)
            => _characters.TryGetValue(player.Id, out character);

        public Character GetCharacter(int characterId)
            => _characters.Values.FirstOrDefault(c => c.Id == characterId);

        public Character GetCharacter(string name, string lastName)
            => _characters.Values.FirstOrDefault(c => c.FirstName == name && c.LastName == lastName);

        public IEnumerable<Character> GetAllCharacters() => _characters.Values;

        /// <summary>
        /// Returns online character by bank account Id
        /// </summary>
        /// <param name="bankAccountId"></param>
        /// <returns></returns>
        public Character GetCharacterByBankAccount(int bankAccountId)
             => _characters.Values.FirstOrDefault(c => c.BankAccount.Id == bankAccountId);

        /// <summary>
        /// Initializes character in the game world
        /// </summary>
        /// <param name="player"></param>
        /// <param name="character"></param>
        public bool IntializeCharacter(IPlayer player, Character character)
        {
            lock (_characters)
            {
                if (_characters.ContainsKey(player.Id)) return false;
                character.Player = player;
                player.SetSyncedMetaData(MetaData.PLAYER_NAME, character.GetFullName());
                player.SetSyncedMetaData(MetaData.REMOTE_ID, character.Id);
                character.Equipment.InitializeEquipment();

                // TODO: Setting skin and shared data
                player.SetPosition(character.X, character.Y, character.Z);
                player.Model = character.Gender == 0 ? (uint)PedModel.FreemodeMale01 : (uint)PedModel.FreemodeFemale01;
                player.Dimension = character.Dimension;
                EquipItems(character);

                character.LastPlayed = DateTime.Now;
                character.Player.Emit("blipManagerLoadAllBlips", BlipManager.Instance.GetBlips());

                _characters.Add(player.Id, character);
                Log.ForContext<CharacterManager>().Information("Initialized character {characterName} CID({characterId}) ID({playerId}) in the world",
                    character.GetFullName(), character.Id, player.Id);
                return true;
            }
        }

        /// <summary>
        /// Removes character data from server memory
        /// </summary>
        /// <param name="character"></param>
        public void RemoveCharacterDataFromServer(Character character)
        {
            lock (character)
            {
                _characters.Remove(character.Player.Id);
                Log.ForContext<CharacterManager>().Information("Removed character {characterName} CID({characterId}) from server", character.GetFullName(), character.Id);
            }
        }

        private void EquipItems(Character character)
        {
            foreach (var item in character.Equipment.Items)
            {
                item.Item.UseItem(character);
            }
        }
    }
}
