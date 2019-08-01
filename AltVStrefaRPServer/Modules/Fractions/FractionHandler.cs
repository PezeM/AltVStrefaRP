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
using AltVStrefaRPServer.Models.Interfaces.Managers;
using AltVStrefaRPServer.Modules.CharacterModule;
using AltVStrefaRPServer.Services;
using AltVStrefaRPServer.Services.Fractions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using FractionRankDto = AltVStrefaRPServer.Models.Dto.Fractions.FractionRankDto;

namespace AltVStrefaRPServer.Modules.Fractions
{
    public class FractionHandler
    {
        private readonly IFractionsManager _fractionsManager;
        private readonly INotificationService _notificationService;
        private readonly IFractionDatabaseService _fractionDatabaseService;
        private readonly ILogger<FractionHandler> _logger;

        public FractionHandler (IFractionsManager fractionsManager, INotificationService notificationService,
            IFractionDatabaseService fractionDatabaseService, ILogger<FractionHandler> logger)
        {
            _fractionsManager = fractionsManager;
            _notificationService = notificationService;
            _fractionDatabaseService = fractionDatabaseService;
            _logger = logger;

            Alt.On<IPlayer, int>("TryToOpenFractionEmployeesPage", TryToOpenFractionEmployeesPage);
            Alt.On<IPlayer, int, string, string> ("InviteEmployeeToFraction", InviteEmployeeToFractionEvent);
            AltAsync.On<IPlayer, int, Task>("AcceptFractionInvite", AcceptFractionInviteEventAsync);
            Alt.On<IPlayer, int>("CancelFractionInvite", CancelFractionInvite);
            AltAsync.On<IPlayer, int, int, Task>("TryToRemoveEmployeeFromFraction",  RemoveEmployeeFromFractionEventAsync);
            Alt.On<IPlayer, int>("TryToOpenFractionRanksPage", TryToOpenFractionRanksPage);
            AltAsync.On<IPlayer, int, int, Task>("TryToDeleteFractionRank", DeleteFractionRankEventAsync);
            AltAsync.On<IPlayer, int, int, int, Task>("UpdateFractionEmployeeRank", UpdateFractionEmployeeRankEventAsync);
            AltAsync.On<IPlayer, int, string, Task>("TryToAddNewFractionRank", AddNewFractionRankEventAsync);
            AltAsync.On<IPlayer, int, string, Task>("TryToUpdateFractionRank", UpdateFractionRankEventAsync);
        }

        public void OpenFractionMenu (Character character)
        {
            if (!_fractionsManager.TryToGetFraction (character, out Fraction fraction))
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
            if (!_fractionsManager.TryToGetFraction(fractionId, out Fraction fraction)) return;
            if (!fraction.HasPermission<ManageEmployeesPermission>(character))
            {
                _notificationService.ShowErrorNotification(player, "Brak uprawnień", "Nie posiadasz odpowiednich uprawnień.", 6000);
                return;
            }

            player.Emit("openFractionEmployeesPage", GetFractionEmloyeesDto(fraction));
        }

        public void InviteEmployeeToFractionEvent (IPlayer player, int fractionId, string firstName, string lastName)
        {
            if ((firstName == null || firstName.Length < 3) || (lastName == null || lastName.Length < 3)) return;
            if (!player.TryGetCharacter (out Character character)) return;
            if (!_fractionsManager.TryToGetFraction (fractionId, out Fraction fraction)) return;
            if (!fraction.HasPermission<ManageEmployeesPermission>(character))
            {
                _notificationService.ShowErrorNotification(player, "Brak uprawnień",
                    "Nie posiadasz uprawnień do zapraszania nowych pracowników.", 6000);
                return;
            }

            var newEmployee = CharacterManager.Instance.GetCharacter (firstName, lastName);
            if (newEmployee == null)
            {
                _notificationService.ShowErrorNotification (player, "Błąd!", $"Nie znaleziono nikogo z takim imieniem i nazwiskiem", 6000);
                return;
            }

            if (fraction.SendInviteToFraction(newEmployee))
            {
                _notificationService.ShowSuccessNotification(player, "Wysłano zaprosznie",
                    $"Pomyślnie wysłano zaproszenie do frakcji do {newEmployee.GetFullName()}", 6000);

                _logger.LogInformation("Character CID({characterId}) invited new employee {@character} CID({employeeId}) to fraction {@fraction}", 
                    character.Id, newEmployee, newEmployee.Id, fraction);
            }
            else
            {
                _notificationService.ShowErrorNotification (player, "Błąd!", $"Nie udało się zaprosić {newEmployee.GetFullName()} do frakcji.", 6000);
            }
        }

        private void CancelFractionInvite(IPlayer player, int fractionId)
        {
            if(!player.TryGetCharacter(out Character character)) return;
            if (!_fractionsManager.TryToGetFraction(fractionId, out Fraction fraction)) return;

            if (fraction.CancelFractionInvite(character))
            {
                _notificationService.ShowSuccessNotification(player, "Sukces", "Pomyślnie odrzucono zaproszenie do frakcji");
            }
        }

        public async Task AcceptFractionInviteEventAsync(IPlayer player, int fractionId)
        {
            if(!player.TryGetCharacter(out Character character)) return;
            if (!_fractionsManager.TryToGetFraction(fractionId, out Fraction fraction)) return;

            if (await fraction.AddNewEmployeeAsync(character, _fractionDatabaseService))
            {
                await _notificationService.ShowSuccessNotificationAsync(player, "Sukces", $"Pomyślnie dołączono do frakcji {fraction.Name}.");
                _logger.LogInformation("Character CID({characterId}) {@character} joined fraction {@fraction} ID({fractionID})", 
                    character.Id, character, fraction, fraction.Id);
            }
            else
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Wystąpił błąd w dołączaniu do frakcji.");
            }
        }

        private async Task RemoveEmployeeFromFractionEventAsync(IPlayer player, int fractionId, int employeeId)
        {
            if (!player.TryGetCharacter(out Character character)) return;
            if(!_fractionsManager.TryToGetFraction(fractionId, out Fraction fraction)) return;
            if (!fraction.HasPermission<ManageEmployeesPermission>(character))
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Brak uprawnień",
                    "Nie posiadasz uprawnień do wyrzucania pracowników.", 6000);
                return;
            }

            if (await fraction.RemoveEmployeeAsync(character, employeeId, _fractionDatabaseService))
            {
                // Maybe send some notification to user that he has been removed
                await player.EmitAsync("succesfullyRemovedEmployeeFromFraction", employeeId);
                _logger.LogInformation("Character CID({characterId}) {@character} removed employee ID({employeeId}) from fraction {@fraction}", 
                    character.Id, character, employeeId, fraction);
            }
            else
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie udało się usunąć pracownika.");
            }
        }

        private void TryToOpenFractionRanksPage(IPlayer player, int fractionId)
        {
            if (!player.TryGetCharacter(out Character character)) return;
            if (!_fractionsManager.TryToGetFraction(fractionId, out Fraction fraction)) return;
            if (!fraction.HasPermission<ManageRanksPermission>(character))
            {
                _notificationService.ShowErrorNotification(player, "Błąd", "Nie posiadasz odpowiednich uprawnień.");
                return;
            }

            player.Emit("openFractionRanksPage", JsonConvert.SerializeObject(GetAllFractionRanks(fraction)));
        }

        private async Task DeleteFractionRankEventAsync(IPlayer player, int fractionId, int rankId)
        {
            if (!player.TryGetCharacter (out Character character)) return;
            if (!_fractionsManager.TryToGetFraction (fractionId, out Fraction fraction)) return;
            if (!fraction.HasPermission<ManageRanksPermission>(character))
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie posiadasz odpowiednich uprawnień.");
                return;
            }

            if (await fraction.RemoveRankAsync(character, rankId, _fractionDatabaseService))
            {
                await player.EmitAsync("succesfullyDeletedFractionRank", rankId);
                _logger.LogInformation("Character CID({characterId}) removed rank ID({rankId}) from fraction {fractionName}", character.Id, rankId, fraction.Name);
            }
            else
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie udało się usunąć roli.");
            }
        }

        public async Task UpdateFractionEmployeeRankEventAsync(IPlayer player, int fractionId, int employeeId, int newRankId)
        {
            if (!player.TryGetCharacter (out Character character)) return;
            if (!_fractionsManager.TryToGetFraction (fractionId, out Fraction fraction)) return;
            if (!fraction.HasPermission<ManageEmployeesPermission>(character))
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Brak uprawnień",
                    "Nie posiadasz odpowiednich uprawnień", 6000);
                return;
            }

            if (await fraction.UpdateEmployeeRankAsync(character, employeeId, newRankId, _fractionDatabaseService))
            {
                await player.EmitAsync("succesfullyUpdatedEmployeeRank", employeeId, newRankId);
                _logger.LogInformation("Character CID({characterId}) changed employee ID({employeeId}) rank to rank ID({rankId}) in fraction {fractionName}",
                    character.Id, employeeId, newRankId, fraction.Name);
            }
            else
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie udało się zmienić roli.");
            }
        }

        private async Task AddNewFractionRankEventAsync(IPlayer player, int fractionId, string newRankString)
        {
            if (!player.TryGetCharacter (out Character character)) return;
            if (!_fractionsManager.TryToGetFraction (fractionId, out Fraction fraction)) return;
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
                _logger.LogError(e, "Error in deserializing fraction rank. New rank string = {newRankString}", newRankString);
                return;
            }
            if(newRank == null) return;

            if (await fraction.AddNewRankAsync(newRank, _fractionDatabaseService))
            {
                player.EmitLocked("succesfullyAddedNewFractionRank", newRank.RankName, JsonConvert.SerializeObject(GetAllFractionRanks(fraction)));
                _logger.LogInformation("Character CID({characterId}) added new rank {@newRank} to fraction {fractionName}", 
                    character.Id, newRank, fraction.Name);

            }
            else
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie udało się dodać nowej roli.");
            }
        }

        
        private async Task UpdateFractionRankEventAsync(IPlayer player, int fractionId, string updatedRankString)
        {
            if (!player.TryGetCharacter (out Character character)) return;
            if (!_fractionsManager.TryToGetFraction (fractionId, out Fraction fraction)) return;
            if (!fraction.HasPermission<ManageRanksPermission>(character))
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie posiadasz odpowiednich uprawnień.");
                return;
            }

            UpdatedFractionRankDto updatedRank;
            try
            {
                updatedRank = JsonConvert.DeserializeObject<UpdatedFractionRankDto>(updatedRankString);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error in deserializing fraction rank. updated rank string = {updatedRank}", updatedRankString);
                return;
            }
            if(updatedRank == null) return;

            if (await fraction.UpdateRankAsync(character, updatedRank, _fractionDatabaseService))
            {
                await player.EmitAsync("succesfullyUpdatedFractionRank", JsonConvert.SerializeObject(updatedRank));
                _logger.LogInformation("Character CID({characterId}) updated rank {rankName} in fraction {fractionName}", 
                    character.Id, updatedRank.RankName, fraction.Name);
            }
            else
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie udało się zaktualizować roli.");
            }
        }

        public async Task<bool> SetFractionOwnerAsync(int fractionId, Character newOwner)
        {
            if (!_fractionsManager.TryToGetFraction(fractionId, out Fraction fraction)) return false;
            if (newOwner == null) return false;

            return await fraction.SetFractionOwnerAsync(newOwner, _fractionDatabaseService);
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

        private static List<FullFractionRankDto> GetAllFractionRanks(Fraction fraction)
        {
            return fraction.FractionRanks.Select(q => new FullFractionRankDto
            {
                Id = q.Id,
                Priority = q.Priority,
                RankName = q.RankName,
                RankType = (int)q.RankType,
                Permissions = q.Permissions.ToList()
            }).ToList();
        }
    }
}