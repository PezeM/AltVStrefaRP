﻿using System;
using System.Threading.Tasks;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Extensions;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Fractions;
using AltVStrefaRPServer.Modules.CharacterModule;
using AltVStrefaRPServer.Services;
using AltVStrefaRPServer.Services.Fractions;
using Newtonsoft.Json;

namespace AltVStrefaRPServer.Modules.Fractions
{
    public class FractionHandler
    {
        private readonly FractionManager _fractionManager;
        private readonly INotificationService _notificationService;
        private readonly IFractionDatabaseService _fractionDatabaseService;

        public FractionHandler (FractionManager fractionManager, INotificationService notificationService,
            IFractionDatabaseService fractionDatabaseService)
        {
            _fractionManager = fractionManager;
            _notificationService = notificationService;
            _fractionDatabaseService = fractionDatabaseService;

            AltAsync.On<IPlayer, int, string, string> ("InviteEmployeeToFraction", async (player, fractionId, firstName, lastName) 
                => await InviteEmployeeToFractionEvent (player, fractionId, firstName, lastName));
            AltAsync.On<IPlayer, int>("AcceptFractionInvite", async (player, fractionId) => await AcceptFractionInviteEvent(player, fractionId));
            AltAsync.On<IPlayer, int, int>("RemoveEmployeeFromFraction", async (player, employeeId, fractionId)
                => await RemoveEmployeeFromFractionEvent(player, employeeId, fractionId));
            AltAsync.On<IPlayer, int, int>("DeleteFractionRank", async (player, fractionId, rankId) 
                => await DeleteFractionRankEvent(player, fractionId, rankId));
            AltAsync.On<IPlayer, int, int, int>("UpdateFractionEmployeeRank", async (player, fractionId, employeeId, newRankId)
                => await UpdateFractionEmployeeRankEvent(player, fractionId, employeeId, newRankId));
        }

        public void OpenFractionMenu (Character character)
        {
            if (!_fractionManager.TryToGetFraction (character, out Fraction fraction))
            {
                _notificationService.ShowErrorNotfication (character.Player, "Brak frakcji", "Nie jesteś zatrudniony w żadnej frakcji.");
                return;
            }

            character.Player.Emit ("openFractionMenu", JsonConvert.SerializeObject (fraction));
        }

        public async Task InviteEmployeeToFractionEvent (IPlayer player, int fractionId, string firstName, string lastName)
        {
            if ((firstName == null || firstName.Length < 3) || (lastName == null || lastName.Length < 3)) return;
            if (!player.TryGetCharacter (out Character character)) return;
            if (!_fractionManager.TryToGetFraction (fractionId, out Fraction fraction)) return;

            if (!(fraction.GetEmployeePermissions(character)?.CanManageEmployees).Value)
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Brak uprawnień",
                    "Nie posiadasz uprawnień do zapraszania nowych pracowników.", 6000);
                return;
            }

            var newEmployee = CharacterManager.Instance.GetCharacter (firstName, lastName);
            if (newEmployee == null)
            {
                await _notificationService.ShowErrorNotificationAsync (player, "Błąd!", $"Nie znaleziono gracza z takim imieniem i nazwiskiem", 6000);
                return;
            }

            // Send inv to newEmployee
            await _notificationService.ShowSuccessNotificationAsync (player, "Wysłano zaprosznie",
                $"Pomyślnie wysłano zaproszenie do frakcji do {newEmployee.GetFullName()}", 6000);
            newEmployee.Player.EmitLocked ("showFractionInvite", fraction.Name, fractionId);
        }

        private async Task AcceptFractionInviteEvent(IPlayer player, int fractionId)
        {
            if(!player.TryGetCharacter(out Character character)) return;
            if (!_fractionManager.TryToGetFraction(fractionId, out Fraction fraction)) return;

            if (await fraction.AddNewEmployeeAsync(character, _fractionDatabaseService))
            {
                await _notificationService.ShowSuccessNotificationAsync(player, "Sukces", $"Pomyślnie dołączono do frakcji {fraction.Name}.");
            }
            else
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Wystąpił błąd w dołączaniu do frakcji.");
            }

            AltAsync.Log($"[JOIN FRACTION] ({character.Id}) {character.GetFullName()} joined fraction ID({fraction.Id}) {fraction.Name}");
        }

        private async Task RemoveEmployeeFromFractionEvent(IPlayer player, int employeeId, int fractionId)
        {
            if (!player.TryGetCharacter(out Character character)) return;
            if(!_fractionManager.TryToGetFraction(fractionId, out Fraction fraction)) return;

            if (!(fraction.GetEmployeePermissions(character)?.CanManageEmployees).Value)
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie posiadasz odpowiednich uprawnień.");
                return;
            }

            if (await fraction.RemoveEmployeeAsync(employeeId, _fractionDatabaseService))
            {
                await _notificationService.ShowSuccessNotificationAsync(player, "Sukces", "Pomyślnie usunięto pracownika z frakcji.");
            }
            else
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie udało się usunąć pracownika.");
            }

            AltAsync.Log($"[REMOVE EMPLOYEE FROM FRACTION] ({character.Id}) removed employee ID({employeeId}) " +
                         $"from fraction ID({fraction.Id}) {fraction.Name}");
        }

        private async Task DeleteFractionRankEvent(IPlayer player, int fractionId, int rankId)
        {
            if (!player.TryGetCharacter (out Character character)) return;
            if (!_fractionManager.TryToGetFraction (fractionId, out Fraction fraction)) return;
            if(!((fraction.GetEmployeePermissions(character)?.CanManageRanks).Value))
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie posiadasz odpowiednich uprawnień.");
                return;
            }

            if (await fraction.RemoveRankAsync(rankId, _fractionDatabaseService))
            {
                await _notificationService.ShowSuccessNotificationAsync(player, "Sukces", "Pomyślnie usunięto rolę.");
            }
            else
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie udało się usunąć roli.");
            }

            AltAsync.Log($"[REMOVE FRACTION] ({character.Id}) deleted rank ID({rankId}) from fraction ID({fractionId}) {fraction.Name}");
        }

        private async Task UpdateFractionEmployeeRankEvent(IPlayer player, int fractionId, int employeeId, int newRankId)
        {
            if (!player.TryGetCharacter (out Character character)) return;
            if (!_fractionManager.TryToGetFraction (fractionId, out Fraction fraction)) return;
            if (!((fraction.GetEmployeePermissions(character)?.CanManageEmployees).Value))
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie posiadasz odpowiednich uprawnień.");
            }

            if (await fraction.UpdateEmployeeRank(employeeId, newRankId, _fractionDatabaseService))
            {
                await _notificationService.ShowSuccessNotificationAsync(player, "Sukces", "Pomyślnie zmieniono rolę.");
            }
            else
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie udało się zmienić roli.");
            }

            AltAsync.Log($"[UPDATE EMPLOYEE RANK] ({character.Id}) changed employee rank to ID({newRankId}) in fraction ID({fractionId}) {fraction.Name}");
        }
    }
}