﻿using AltVStrefaRPServer.Models;
using System.Threading.Tasks;

namespace AltVStrefaRPServer.Services.Characters.Customization
{
    public interface ICharacterCreatorService
    {
        Task<bool> CheckIfCharacterExistsAsync(string firstName, string lastName);
        Character CreateNewCharacter(int accountId, string firstName, string lastName, int age, int gender);
        Task SaveNewCharacterAsync(Character character);
    }
}
