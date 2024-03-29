﻿using AltV.Net.Elements.Entities;
using System.Collections.Generic;

namespace AltVStrefaRPServer.Models.Interfaces.Managers
{
    public interface ICharacterManager
    {
        Character GetCharacter(int characterId);
        Character GetCharacter(IPlayer player);
        Character GetCharacter(string name, string lastName);
        Character GetCharacterByBankAccount(int bankAccountId);
        bool TryGetCharacter(IPlayer player, out Character character);
        IEnumerable<Character> GetAllCharacters();
        bool IntializeCharacter(IPlayer player, Character character);
        void RemoveCharacterDataFromServer(Character character);
    }
}