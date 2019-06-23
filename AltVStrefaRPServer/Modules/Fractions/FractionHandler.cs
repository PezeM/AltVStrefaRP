using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Extensions;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Dto;
using AltVStrefaRPServer.Models.Dto.Fractions;
using AltVStrefaRPServer.Models.Enums;
using AltVStrefaRPServer.Models.Fractions;
using AltVStrefaRPServer.Models.Fractions.Permissions;
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

            Alt.On<IPlayer, int>("TryToOpenFractionEmployeesPage", TryToOpenFractionEmployeesPage);
            AltAsync.On<IPlayer, int, string, string> ("InviteEmployeeToFraction", async (player, fractionId, firstName, lastName) 
                => await InviteEmployeeToFractionEvent (player, fractionId, firstName, lastName));
            AltAsync.On<IPlayer, int>("AcceptFractionInvite", async (player, fractionId) => await AcceptFractionInviteEvent(player, fractionId));
            AltAsync.On<IPlayer, int, int>("RemoveEmployeeFromFraction", async (player, employeeId, fractionId)
                => await RemoveEmployeeFromFractionEvent(player, employeeId, fractionId));
            AltAsync.On<IPlayer, int, int>("DeleteFractionRank", async (player, fractionId, rankId) 
                => await DeleteFractionRankEvent(player, fractionId, rankId));
            AltAsync.On<IPlayer, int, int, int>("UpdateFractionEmployeeRank", async (player, fractionId, employeeId, newRankId)
                => await UpdateFractionEmployeeRankEvent(player, fractionId, employeeId, newRankId));
            AltAsync.On<IPlayer, int, string>("AddNewFractionRank", async (player, fractionId, newRank) 
                => await AddNewFractionRankEvent(player, fractionId, newRank));
            AltAsync.On<IPlayer, int, int, string>("UpdateFractionRank", async(player, fractionId, rankId, updatedRank) 
                => await UpdateFractionRankEvent(player, fractionId, rankId, updatedRank));
        }

        public void OpenFractionMenu (Character character)
        {
            if (!_fractionManager.TryToGetFraction (character, out Fraction fraction))
            {
                _notificationService.ShowErrorNotification (character.Player, "Brak frakcji", "Nie jesteś zatrudniony w żadnej frakcji.");
                return;
            }

            if (!fraction.HasPermission<OpenMenuPermission>(character))
            {
                _notificationService.ShowErrorNotification(character.Player, "Brak uprawnień", "Nie posiadasz odpowiednich uprawnień");
                return;
            }

            if (fraction is PoliceFraction)
            {
                character.Player.Emit ("openFractionMenu", (int)FractionsEnum.Police, null);
            }
            else if (fraction is SamsFraction)
            {
                character.Player.Emit ("openFractionMenu", (int)FractionsEnum.Sams, null);
            }
            else if (fraction is TownHallFraction townHallFraction)
            {
                character.Player.Emit("openFractionMenu", (int)FractionsEnum.Townhall, GetTownHallFractionDto(townHallFraction));
            }
        }

        private void TryToOpenFractionEmployeesPage(IPlayer player, int fractionId)
        {
            if (!player.TryGetCharacter(out Character character)) return;
            if (!_fractionManager.TryToGetFraction(fractionId, out Fraction fraction)) return;
            if (!fraction.HasPermission<ManageEmployeesPermission>(character))
            {
                _notificationService.ShowErrorNotification(player, "Brak uprawnień", "Nie posiadasz odpowiednich uprawnień.", 6000);
                return;
            }

            player.Emit("openFractionEmployeesPage", GetFractionEmloyeesDto(fraction));
        }

        public async Task InviteEmployeeToFractionEvent (IPlayer player, int fractionId, string firstName, string lastName)
        {
            if ((firstName == null || firstName.Length < 3) || (lastName == null || lastName.Length < 3)) return;
            if (!player.TryGetCharacter (out Character character)) return;
            if (!_fractionManager.TryToGetFraction (fractionId, out Fraction fraction)) return;
            if (!fraction.HasPermission<ManageEmployeesPermission>(character))
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

        public async Task AcceptFractionInviteEvent(IPlayer player, int fractionId)
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
            if (!fraction.HasPermission<ManageEmployeesPermission>(character))
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Brak uprawnień",
                    "Nie posiadasz uprawnień do wrzucania pracowników.", 6000);
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
            if (!fraction.HasPermission<ManageRanksPermission>(character))
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

            AltAsync.Log($"[REMOVE FRACTION RANK] ({character.Id}) deleted rank ID({rankId}) from fraction ID({fractionId}) {fraction.Name}");
        }

        public async Task UpdateFractionEmployeeRankEvent(IPlayer player, int fractionId, int employeeId, int newRankId)
        {
            if (!player.TryGetCharacter (out Character character)) return;
            if (!_fractionManager.TryToGetFraction (fractionId, out Fraction fraction)) return;
            if (!fraction.HasPermission<ManageEmployeesPermission>(character))
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Brak uprawnień",
                    "Nie posiadasz odpowiednich uprawnień", 6000);
                return;
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

        private async Task AddNewFractionRankEvent(IPlayer player, int fractionId, string newRankString)
        {
            if (!player.TryGetCharacter (out Character character)) return;
            if (!_fractionManager.TryToGetFraction (fractionId, out Fraction fraction)) return;
            if (!fraction.HasPermission<ManageRanksPermission>(character))
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie posiadasz odpowiednich uprawnień.");
                return;
            }

            NewFractionRankDto newRank;
            try
            {
                newRank = JsonConvert.DeserializeObject<NewFractionRankDto>(newRankString);
            }
            catch (Exception e)
            {
                AltAsync.Log($"Error in deserializing new fraction permissions: {e}");
                return;
            }
            if(newRank == null) return;

            if (await fraction.AddNewRank(newRank, _fractionDatabaseService))
            {
                await _notificationService.ShowSuccessNotificationAsync(player, "Sukces", $"Pomyślnie dodano nową rolę o nazwie {newRank.RankName}.");
            }
            else
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie udało się dodać nowej roli.");
            }

            AltAsync.Log($"[ADD NEW FRACTION RANK] ({character.Id}) added new role ({newRank.RankName}) to fraction ID({fraction.Id}) {fraction.Name}");
        }

        
        private async Task UpdateFractionRankEvent(IPlayer player, int fractionId, int rankId, string updatedRank)
        {
            if (!player.TryGetCharacter (out Character character)) return;
            if (!_fractionManager.TryToGetFraction (fractionId, out Fraction fraction)) return;
            if (!fraction.HasPermission<ManageRanksPermission>(character))
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie posiadasz odpowiednich uprawnień.");
                return;
            }

            NewFractionRankDto updatedPermissions;
            try
            {
                updatedPermissions = JsonConvert.DeserializeObject<NewFractionRankDto>(updatedRank);
            }
            catch (Exception e)
            {
                AltAsync.Log($"Error in deserializing new fraction permissions: {e}");
                return;
            }
            if(updatedPermissions == null) return;

            if (await fraction.UpdateRank(rankId, updatedPermissions, _fractionDatabaseService))
            {
                await _notificationService.ShowSuccessNotificationAsync(player, "Sukces", 
                    $"Pomyślnie zaktualizowano rolę o nazwie {updatedPermissions.RankName}");
            }
            else
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie udało się zaktualizować roli.");
            }

            AltAsync.Log($"[UPADTE FRACTION RANK] ({character.Id}) updated fraction rank {updatedPermissions.RankName} in fraction ID({fraction.Id}) {fraction.Name}");
        }

        public Task<bool> SetFractionOwner(int fractionId, Character newOwner)
        {
            if (!_fractionManager.TryToGetFraction(fractionId, out Fraction fraction)) return Task.FromResult(false);
            if (newOwner == null) return Task.FromResult(false);

            return fraction.SetFractionOwner(newOwner, _fractionDatabaseService);
        }

        private FractionEmployeesDto GetFractionEmloyeesDto(Fraction fraction)
        {
            return new FractionEmployeesDto
            {
                Employees = fraction.Employees.Select(e => new FractionEmployeeDto
                {
                    Id = e.Id,
                    Name = e.FirstName,
                    LastName = e.LastName,
                    Age = e.Age,
                    RankId = e.FractionRank,
                    RankName = fraction.GetEmployeeRank(e).RankName
                }).ToList(),
                Ranks = fraction.FractionRanks.Select(r => new FractionRankDto
                {
                    Id = r.Id,
                    RankName = r.RankName,
                    Priority = r.Priority,
                    RankType = (int)r.RankType
                }).ToList()
            };
        }

        private static TownHallFractionDto GetTownHallFractionDto(TownHallFraction townHallFraction)
        {
            return new TownHallFractionDto
            {
                Id = townHallFraction.Id,
                Money = townHallFraction.Money,
                EmployeesCount = townHallFraction.EmployeesCount,
                RolesCount = townHallFraction.FractionRanks.Count,
                CreationDate = townHallFraction.CreationDate.ToShortDateString(),
                Taxes = new List<TaxDto>
                {
                    new TaxDto(1, "Podatek od pojazdów", townHallFraction.VehicleTax),
                    new TaxDto(2, "Podatek od nieruchomości", townHallFraction.PropertyTax),
                    new TaxDto(3, "Podatek od broni", townHallFraction.GunTax),
                    new TaxDto(4, "Podatek globalny", townHallFraction.GlobalTax)
                }
            };
        }
    }
}