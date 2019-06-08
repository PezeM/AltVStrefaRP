using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Extensions;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Fractions;
using AltVStrefaRPServer.Modules.CharacterModule;
using AltVStrefaRPServer.Services;
using Newtonsoft.Json;

namespace AltVStrefaRPServer.Modules.Fractions
{
    public class FractionHandler
    {
        private readonly FractionManager _fractionManager;
        private readonly INotificationService _notificationService;

        public FractionHandler (FractionManager fractionManager, INotificationService notificationService)
        {
            _fractionManager = fractionManager;
            _notificationService = notificationService;

            AltAsync.On<IPlayer, int, string, string> ("InviteEmployeeToFraction", async (player, fractionId, firstName, lastName) 
                => await InviteEmployeeToFractionEvent (player, fractionId, firstName, lastName));
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
            if ((firstName == null || firstName.Length < 3) || (lastName == null || lastName.Length < 3))
            {
                await _notificationService.ShowErrorNotificationAsync (player, "Błąd!", $"Nie znaleziono gracza z takim imieniem i nazwiskiem", 6000);
                return;
            }

            if (!player.TryGetCharacter (out Character character)) return;
            if (!_fractionManager.TryToGetFraction (fractionId, out Fraction fraction)) return;

            var newEmployee = CharacterManager.Instance.GetCharacter (firstName, lastName);
            if (newEmployee == null)
            {
                await _notificationService.ShowErrorNotificationAsync (player, "Błąd!", $"Nie znaleziono gracza z takim imieniem i nazwiskiem", 6000);
                return;
            }

            // Send invitation to newEmployee

            await _notificationService.ShowSuccessNotificationAsync (player, "Wysłano zaprosznie",
                $"Pomyślnie wysłano zaproszenie do biznesu do {newEmployee.GetFullName()}", 6000);
            newEmployee.Player.EmitLocked ("showFractionInvite", fraction.Name, fractionId);
        }
    }
}