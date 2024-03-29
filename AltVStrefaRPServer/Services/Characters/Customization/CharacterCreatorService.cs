﻿using AltVStrefaRPServer.Database;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Enums;
using AltVStrefaRPServer.Models.Inventory;
using AltVStrefaRPServer.Models.Server;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AltVStrefaRPServer.Models.Houses;

namespace AltVStrefaRPServer.Services.Characters.Customization
{
    public class CharacterCreatorService : ICharacterCreatorService
    {
        private readonly Func<ServerContext> _factory;

        public CharacterCreatorService(Func<ServerContext> factory)
        {
            _factory = factory;
        }

        public async Task<bool> CheckIfCharacterExistsAsync(string firstName, string lastName)
        {
            using (var context = _factory.Invoke())
            {
                return await context.Characters
                    .AsNoTracking()
                    .AnyAsync(c => c.FirstName.ToLower() == firstName
                                   && c.LastName.ToLower() == lastName);
            }
        }

        public async Task SaveNewCharacterAsync(Character character)
        {
            using (var context = _factory.Invoke())
            {
                context.Attach(character.Account);
                context.Update(character);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public Character CreateNewCharacter(int accountId, string firstName, string lastName, int age, int gender)
            => CreateNewDefaultCharacter(accountId, firstName, lastName, age, gender);

        private Character CreateNewDefaultCharacter(int accountId, string firstName, string lastName, int age, int gender)
        {
            var newCharacter = new Character
            {
                FirstName = firstName,
                LastName = lastName,
                AccountId = accountId,
                BankAccount = null,
                Age = age,
                Inventory = new PlayerInventoryContainer(18),
                Equipment = new PlayerEquipment(),
                Gender = (Gender)gender,
                Dimension = 0,
                Houses = new List<House>(),
                HotelRooms = new List<HotelRoom>(),
                X = AppSettings.Current.ServerConfig.SpawnPosition.X,
                Y = AppSettings.Current.ServerConfig.SpawnPosition.Y,
                Z = AppSettings.Current.ServerConfig.SpawnPosition.Z,
                ProfileImage = "default-profile-image.jpg",
                CreationDate = DateTime.Now,
                LastPlayed = DateTime.Now,
                TimePlayed = 0,
                BusinessRank = 0,
                FractionRank = 0,
                CanDriveVehicles = true,
                IsBanned = false,
                IsMuted = false,
            };
            newCharacter.AddMoney(AppSettings.Current.ServerConfig.StartingPlayerMoney);

            return newCharacter;
        }
    }
}
