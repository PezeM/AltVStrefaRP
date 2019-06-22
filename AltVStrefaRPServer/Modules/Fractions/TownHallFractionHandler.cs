using System;
using System.Linq;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Extensions;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Dto.Fractions;
using AltVStrefaRPServer.Models.Enums;
using AltVStrefaRPServer.Models.Fractions;
using AltVStrefaRPServer.Models.Fractions.Permissions;
using AltVStrefaRPServer.Modules.CharacterModule;
using AltVStrefaRPServer.Modules.Vehicle;
using AltVStrefaRPServer.Services;
using AltVStrefaRPServer.Services.Characters;
using Newtonsoft.Json;

namespace AltVStrefaRPServer.Modules.Fractions
{
    public class TownHallFractionHandler
    {
        private FractionManager _fractionManager;
        private readonly INotificationService _notificationService;
        private readonly ICharacterDatabaseService _characterDatabaseService;
        private readonly VehicleManager _vehicleManager;

        public TownHallFractionHandler(FractionManager fractionManager, ICharacterDatabaseService characterDatabaseService,
            INotificationService notificationService, VehicleManager vehicleManager)
        {
            _fractionManager = fractionManager;
            _notificationService = notificationService;
            _characterDatabaseService = characterDatabaseService;
            _vehicleManager = vehicleManager;

            Alt.On<IPlayer, int, float>("TryToUpdateTaxValue", TryToUpdateTaxValue);  
            AltAsync.On<IPlayer, string, string>("TryToGetResidentData", async (player, firstName, lastName) 
                => await TryToGetResidentDataEvent(player, firstName, lastName));
            Alt.On<IPlayer>("TryToOpenFractionResidentsPage", TryToOpenFractionResidentsPage);
            Alt.On<IPlayer>("TryToOpenFractionTaxesPage", TryToOpenFractionTaxesPage);
        }

        private void TryToOpenFractionResidentsPage(IPlayer player)
        {
            var allOnlinePlayers = CharacterManager.Instance.GetAllCharacters().Select(q => q.GetFullName());
            player.Emit("openFractionsResidentsPage", JsonConvert.SerializeObject(allOnlinePlayers));
        }

        private async Task TryToGetResidentDataEvent(IPlayer player, string firstName, string lastName)
        {
            if (firstName.IsNullOrEmpty() || lastName.IsNullOrEmpty()) return;
            var character = CharacterManager.Instance.GetCharacter(firstName, lastName);
            if (character == null)
            {
                character = await _characterDatabaseService.FindCharacterAsync(firstName, lastName);
            }
            if (character == null)
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Nie znaleziono",
                    "Nie znaleziono żadnego mieszkańca z podanym imieniem i nazwiskiem.", 6500);
                return;
            }

            await player.EmitAsync("populateResidentData", CreateFractionResidentDto(character));
        }

        private void TryToOpenFractionTaxesPage(IPlayer player)
        {
            if (!player.TryGetCharacter(out Character character)) return;
            if (!_fractionManager.TryToGetTownHallFraction(out TownHallFraction townHallFraction)) return;
            if (!townHallFraction.HasPermission<OpenTaxesPagePermission>(character))
            {
                _notificationService.ShowErrorNotification(player, "Brak uprawnień",
                    "Nie posiadasz odpowiednich uprawnień do wykonania tej akcji.", 6500);
                return;
            }

            player.Emit("openFractionTaxesPage", JsonConvert.SerializeObject(townHallFraction.Taxes.Take(150)));
        }

        private void TryToUpdateTaxValue(IPlayer player, int taxId, float newTax)
        {
            if (!player.TryGetCharacter(out Character character)) return;
            if(!_fractionManager.TryToGetTownHallFraction(out TownHallFraction townHallFraction)) return;
            if (!(townHallFraction.GetEmployeeRank(character)?.RankType == RankType.Highest))
            {
                _notificationService.ShowErrorNotification(player, "Brak uprawnień",
                    "Nie posiadasz odpowiednich uprawnień do wykonania tej akcji.", 6500);
                return;
            }
            newTax = (float)Math.Round(newTax * 100f) / 100f;

            if (UpdateTax(taxId, newTax, townHallFraction))
            {
                player.Emit("updateTaxValue", taxId, newTax);
            }
            else
            {
                _notificationService.ShowErrorNotification(player, "Błąd", $"Nie udało się ustawić nowego podatku.");
            }

            Alt.Log($"[TAX UPDATE] ({character.Id}) updated tax ({taxId}) to {newTax*100}%.");
        }

        private static bool UpdateTax(int taxId, float newTax, TownHallFraction townHallFraction)
        {
            bool result = false;
            switch (taxId)
            {
                case 1:
                    result = townHallFraction.SetVehicleTax(newTax);
                    break;
                case 2:
                    result = townHallFraction.SetPropertyTax(newTax);
                    break;
                case 3:
                    result = townHallFraction.SetGunTax(newTax);
                    break;
                case 4:
                    result = townHallFraction.SetGlobalTax(newTax);
                    break;
            }

            return result;
        }

        private FractionResidentDto CreateFractionResidentDto(Character character)
        {
            return new FractionResidentDto
            {
                Id = character.Id,
                Age = character.Age,
                Name = character.FirstName,
                LastName = character.LastName,
                BankAccount = character.BankAccount != null ? character.BankAccount.AccountNumber : 0,
                BankMoney = character.BankAccount != null ? character.BankAccount.Money : 0,
                BusinessName = character.Business != null ? character.Business.BusinessName : "Brak",
                FractionName = character.Fraction != null ? character.Fraction.Name : "Brak",
                Vehicles = _vehicleManager.GetAllPlayerVehicles(character).Select(q => new VehicleDataDto(q.Model, q.PlateText)).ToList(),
            };
        }
    }
}
