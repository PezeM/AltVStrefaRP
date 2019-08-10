using System;
using System.Collections.Generic;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Extensions;
using AltVStrefaRPServer.Helpers;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Dto;
using AltVStrefaRPServer.Models.Enums;
using AltVStrefaRPServer.Modules.CharacterModule;
using AltVStrefaRPServer.Services;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using AltVStrefaRPServer.Models.Businesses;
using AltVStrefaRPServer.Models.Interfaces.Managers;
using Microsoft.Extensions.Logging;

namespace AltVStrefaRPServer.Modules.Businesses
{
    public class BusinessHandler
    {
        private readonly IBusinessesManager _businessesManager;
        private readonly INotificationService _notificationService;
        private readonly ILogger<BusinessHandler> _logger;

        public BusinessHandler(IBusinessesManager businessesManager, INotificationService notificationService, ILogger<BusinessHandler> logger)
        {
            _businessesManager = businessesManager;
            _notificationService = notificationService;
            _logger = logger;

            Alt.On<IPlayer, int>("GetBusinessEmployees", GetBusinessEmployeesEvent);
            AltAsync.On<IPlayer, int, int, int, Task>("UpdateEmployeeRank", (player, employeeId, newRankId, businessId) 
                => UpdateEmployeeRankEventAsync(player, employeeId, newRankId, businessId));
            Alt.On<IPlayer, string, string, int>("AddNewEmployee", AddNewEmployeeEvent);
            AltAsync.On<IPlayer, int, Task>("AcceptInviteToBusiness", AcceptInviteToBusinessEventAsync);
            Alt.On<IPlayer, int>("GetBusinessRoles", GetBusinessRolesEvent);
            AltAsync.On<IPlayer, string, int, Task>("UpdateBusinessRank", UpdateBusinessRankAsync);
            AltAsync.On<IPlayer, string, int, Task>("AddNewRole", AddNewRankEventAsync);
            AltAsync.On<IPlayer, int, int, Task>("DeleteEmployee", DeleteEmployeeEventAsync);
            AltAsync.On<IPlayer, int, int, Task>("DeleteRole", DeleteRankEventAsync);
            AltAsync.On<IPlayer, int, Task>("DeleteBusiness", DeleteBusinessEventAsync);
        }

        private void GetBusinessEmployeesEvent(IPlayer player, int businessId)
        {
            var character = player.GetCharacter();
            if (character == null) return;

            if (!_businessesManager.TryGetBusiness(businessId, out Business business))
            {
                _notificationService.ShowErrorNotification(player, "Błąd", "Nie znaleziono takiego biznesu.", 4000);
                return;
            }

            if (!business.GetBusinessRankForEmployee(character, out BusinessRank businessRank))
            {
                _notificationService.ShowErrorNotification(character.Player, "Błąd", "Nie masz ustalonych żadnych możliwości w biznesie.", 6000);
                return;
            }

            if (!businessRank.Permissions.CanManageEmployess)
            {
                _notificationService.ShowErrorNotification(character.Player, "Błąd", "Nie masz odpowiednich uprawień.");
                return;
            }

            player.Emit("populateEmployeeRanks", JsonConvert.SerializeObject(GetBusinessEmployeesDto(business)));
        }

        public void OpenBusinessMenu(Character character)
        {
            var startTime = Time.GetTimestampMs();
            if (!_businessesManager.TryGetBusiness(character, out Business business))
            {
                _notificationService.ShowErrorNotification(character.Player, "Błąd", "Nie jesteś w żadnym biznesie.", 4000);
                return;
            }

            if (!business.GetBusinessRankForEmployee(character, out BusinessRank businessRank))
            {
                _notificationService.ShowErrorNotification(character.Player, "Błąd", "Nie masz ustalonych żadnych możliwości w biznesie.", 6000);
                return;
            }

            if (!businessRank.Permissions.CanOpenBusinessMenu)
            {
                _notificationService.ShowErrorNotification(character.Player, "Błąd", "Nie możesz otwierać menu biznesu.", 4000);
                return;
            }

            character.Player.Emit("openBusinessMenu", JsonConvert.SerializeObject(GetBusinessInfoDto(business)));
            _logger.LogDebug("Character {characterName} CID({characterId}) opened business menu in {elapsedTime}ms", 
                character.Id, character.GetFullName(), Time.GetElapsedTime(startTime));
        }

        private async Task UpdateEmployeeRankEventAsync(IPlayer player, int employeeId, int newRankId, int businessId)
        {
            var startTime = Time.GetTimestampMs();
            if (!player.TryGetCharacter(out var character)) return;

            if (!_businessesManager.TryGetBusiness(businessId, out Business business))
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie znaleziono takiego biznesu.", 4000);
                return;
            }

            if (!business.GetBusinessRankForEmployee(character, out BusinessRank businessRank))
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie masz ustalonych żadnych możliwości w biznesie.", 6000);
                return;
            }

            if (!businessRank.Permissions.CanManageEmployess)
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie posiadasz odpowiednich uprawień.", 4000);
                return;
            }

            if (!business.IsCharacterEmployee(employeeId, out Character employee))
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Ten pracownik nie jest zatrudniony u ciebie w firmie.", 7000);
                return;
            }

            if (!business.CheckIfRankExists(newRankId))
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie znaleziono podanego stanowiska.", 5000);
                return;
            }

            await _businessesManager.UpdateEmployeeRankAsync(business, employee, newRankId).ConfigureAwait(false);
            player.EmitLocked("successfullyUpdatedEmployeeRank", employeeId, newRankId);
            _logger.LogInformation("Character {characterName} CID({characterId}) changed business rank of character CID({employeeId}) to RankID({rankId}) in {elapsedTime}",
                character.GetFullName(), character.Id, employee.Id, newRankId, Time.GetElapsedTime(startTime));
        }

        private void AddNewEmployeeEvent(IPlayer player, string name, string lastName, int businessId)
        {
            var startTime = Time.GetTimestampMs();
            if (!player.TryGetCharacter(out var character)) return;

            if (!_businessesManager.TryGetBusiness(businessId, out Business business))
            {
                _notificationService.ShowErrorNotification(player, "Błąd", "Nie znaleziono takiego biznesu.", 4000);
                return;
            }

            if (!business.GetBusinessRankForEmployee(character, out BusinessRank businessRank))
            {
                _notificationService.ShowErrorNotification(player, "Błąd", "Nie masz ustalonych żadnych możliwości w biznesie.", 6000);
                return;
            }

            if (!businessRank.Permissions.CanInviteNewMembers)
            {
                _notificationService.ShowErrorNotification(player, "Błąd", "Nie posiadasz odpowiednich uprawień.", 4000);
                return;
            }

            var newEmployee = CharacterManager.Instance.GetCharacter(name, lastName);
            if (newEmployee == null)
            {
                _notificationService.ShowErrorNotification(player, "Błąd", "Nie znaleziono osoby z takim imieniem i nazwiskiem.", 7000);
                return;
            }

            if (!business.CanAddNewMember(newEmployee))
            {
                _notificationService.ShowErrorNotification(player, "Błąd", $"Nie można zatrudnić {newEmployee.GetFullName()}" +
                                                                  $", ponieważ pracuje juz w jakimś biznesie.", 8000);
                return;
            }

            newEmployee.Player.Emit("showConfirmModal", "Oferta pracy", $"Otrzymałeś zaproszenie do firmy {business.BusinessName}. " +
                                                                        $"Czy chcesz je przyjąć?", (int)ConfirmModalType.BusinessInvite, business.Id);

            // To test
            //player.Emit("showConfirmModal", "Oferta pracy", $"Otrzymałeś zaproszenie do firmy {business.BusinessName}. " +
            //                                                $"Czy chcesz je przyjąć?", (int)ConfirmModalType.BusinessInvite, business.Id);

            _notificationService.ShowSuccessNotification(player, "Wysłano zaproszenie", 
                                                        $"Zaproszenie do firmy zostało wysłane do {newEmployee.GetFullName()}.", 6500);

            _logger.LogInformation("Character {characterName} CID({characterId}) invited {newEmployeeName} CID({newEmployeeId}) " +
                                   "to business {businessName} ID(businessID) in {elapsedTime}ms",
                character.GetFullName(), character.Id, newEmployee.GetFullName(), newEmployee.Id, business.BusinessName, business.Id, Time.GetElapsedTime(startTime));
        }

        private async Task AcceptInviteToBusinessEventAsync(IPlayer player, int businessId)
        {
            if (!player.TryGetCharacter(out var character)) return;

            if (!_businessesManager.TryGetBusiness(businessId, out Business business))
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie znaleziono takiego biznesu.", 4000);
                return;
            }

            if (await _businessesManager.AddNewEmployeeAsync(business, character).ConfigureAwait(false))
            {
                await _notificationService.ShowSuccessNotificationAsync(player, "Nowa praca!", $"Pomyślnie dołączono do biznesu {business.BusinessName}.", 5000);
            }
            else
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Wystąpił błąd podczas dołączania do biznesu.", 5000);
                return;
            }

            _logger.LogInformation("Character {characterName} CID({characterId}) joined business {businessName} ID({businessId})", 
                character.GetFullName(), character.Id, business.BusinessName, business.Id);
        }

        private void GetBusinessRolesEvent(IPlayer player, int businessId)
        {
            if (!player.TryGetCharacter(out var character)) return;

            if (!_businessesManager.TryGetBusiness(businessId, out Business business))
            {
                _notificationService.ShowErrorNotification(player, "Błąd", "Nie znaleziono takiego biznesu.", 4000);
                return;
            }

            if (!business.GetBusinessRankForEmployee(character, out BusinessRank businessRank))
            {
                _notificationService.ShowErrorNotification(character.Player, "Błąd", "Nie masz ustalonych żadnych możliwości w biznesie.", 6000);
                return;
            }

            if (!businessRank.Permissions.CanManageEmployess)
            {
                _notificationService.ShowErrorNotification(character.Player, "Błąd", "Nie masz odpowiednich uprawień.");
                return;
            }
            player.Emit("populateBusinessRanksInfo", JsonConvert.SerializeObject(GetBusinessRanksInfo(business)));
        }

        private async Task UpdateBusinessRankAsync(IPlayer player, string newPermissionsString, int businessId)
        {
            var startTime = Time.GetTimestampMs();
            if (!player.TryGetCharacter(out var character)) return;

            if (!_businessesManager.TryGetBusiness(businessId, out Business business))
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie znaleziono takiego biznesu.", 4000);
                return;
            }
            
            if (!business.GetBusinessRankForEmployee(character, out BusinessRank businessRank))
            {
                await _notificationService.ShowErrorNotificationAsync(character.Player, "Błąd", "Nie masz ustalonych żadnych możliwości w biznesie.", 6000);
                return;
            }

            if (!businessRank.Permissions.CanManageRanks)
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie posiadasz odpowiednich uprawień.", 4000);
                return;
            }

            BusinessPermissionsDto newPermissions;
            try
            {
                newPermissions = JsonConvert.DeserializeObject<BusinessPermissionsDto>(newPermissionsString);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error in deserializing business permissions. New permissions string = {newPermissionsString}", newPermissionsString);
                throw;
            }

            if (!business.GetBusinessRank(newPermissions.RankId, out BusinessRank businessRankToUpdate))
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie znaleziono takiego stanowiska w biznesie.", 4000);
                return;
            }

            await _businessesManager.UpdateBusinessRankAsync(businessRankToUpdate, newPermissions).ConfigureAwait(false);
            await _notificationService.ShowSuccessNotificationAsync(player, "Zaktualizowano stanowisko", "Pomyślnie zaktualizowano stanowisko.");
            _logger.LogDebug("Character {characterName} CID({characterId}) changed permissions of rank RankId({rankId}) in {elapsedTime}ms", 
                character.GetFullName(), character.Id, businessRank.Id, Time.GetElapsedTime(startTime));
        }

        private async Task AddNewRankEventAsync(IPlayer player, string newRankString, int businessId)
        {
            var startTime = Time.GetTimestampMs();
            if (!player.TryGetCharacter(out var character)) return;

            if (!_businessesManager.TryGetBusiness(businessId, out Business business))
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie znaleziono takiego biznesu.", 4000);
                return;
            }

            if (!business.GetBusinessRankForEmployee(character, out BusinessRank businessRank))
            {
                await _notificationService.ShowErrorNotificationAsync(character.Player, "Błąd", "Nie masz ustalonych żadnych możliwości w biznesie.", 6000);
                return;
            }

            if (!businessRank.Permissions.CanManageRanks)
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie posiadasz odpowiednich uprawień.", 4000);
                return;
            }

            BusinessNewRankDto newRank;
            try
            {
                newRank = JsonConvert.DeserializeObject<BusinessNewRankDto>(newRankString);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error in deserializing new business rank. New rank string = {newRankString}", newRankString);
                throw;
            }

            if(newRank == null) return;
            if (await _businessesManager.AddNewBusinessRankAsync(business, newRank).ConfigureAwait(false))
            {
                await _notificationService.ShowSuccessNotificationAsync(player, "Zaktualizowano stanowisko", "Pomyślnie zaktualizowano stanowisko");
                _logger.LogInformation("Character {characterName} CID({characterId}) added new rank {@newRank} to business {businessName} ID({businessId}) in {elapsedTime}ms", 
                    character.GetFullName(), character.Id, newRank, business.BusinessName, business.Id, Time.GetElapsedTime(startTime));
            }
            else
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie udało się dodać nowego stanowiska.", 5000);
                return;
            }
        }

        private async Task DeleteEmployeeEventAsync(IPlayer player, int employeeId, int businessId)
        {
            var startTime = Time.GetTimestampMs();
            if (!player.TryGetCharacter(out var character)) return;

            if (!_businessesManager.TryGetBusiness(businessId, out Business business))
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie znaleziono takiego biznesu.", 4000);
                return;
            }

            if (!business.GetBusinessRankForEmployee(character, out BusinessRank businessRank))
            {
                await _notificationService.ShowErrorNotificationAsync(character.Player, "Błąd", "Nie masz ustalonych żadnych możliwości w biznesie.", 6000);
                return;
            }

            if (!businessRank.Permissions.CanManageEmployess)
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie posiadasz odpowiednich uprawień.", 4000);
                return;
            }

            if (!business.IsCharacterEmployee(employeeId, out Character employee))
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie jest pracownikiem w firmie.", 4000);
                return;
            }

            if (await _businessesManager.RemoveEmployeeAsync(business, employee))
            {
                await _notificationService.ShowSuccessNotificationAsync(player, "Usunięto pracownika", $"Pomyślnie usunięto {employee.GetFullName()} z firmy.", 5000);
                _logger.LogInformation("Character {characterName} CID({characterId}) removed employee {employeeName} ID({employeeId}) " +
                                       "from business {businessName} ID({businessId}) in {elapsedTime}",
                    character.GetFullName(), character.Id, employee.GetFullName(), employee.Id, business.BusinessName, business.Id, Time.GetElapsedTime(startTime));
            }
            else
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", $"Wystąpił błąd podczas usuwania {employee.GetFullName()} z firmy.", 6500);
            }
        }

        private async Task DeleteRankEventAsync(IPlayer player, int rankId, int businessId)
        {
            var startTime = Time.GetTimestampMs();
            if (!player.TryGetCharacter(out var character)) return;

            if (!_businessesManager.TryGetBusiness(businessId, out Business business))
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie znaleziono takiego biznesu.", 4000);
                return;
            }

            if (!business.GetBusinessRankForEmployee(character, out BusinessRank businessRank))
            {
                await _notificationService.ShowErrorNotificationAsync(character.Player, "Błąd", "Nie masz ustalonych żadnych możliwości w biznesie.", 6000);
                return;
            }

            if (!businessRank.Permissions.CanManageRanks)
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie posiadasz odpowiednich uprawień.", 4000);
                return;
            }

            if (!business.CheckIfRankExists(rankId)) return;

            if (await _businessesManager.RemoveRankAsync(business, rankId))
            {
                await _notificationService.ShowSuccessNotificationAsync(player, "Usunięto stanowisko", $"Pomyślnie usunięto stanowisko.", 5000);
                _logger.LogInformation("Character {characterName} CID({characterId}) deleted rank {rankName} ID({rankId}) from business {businessName} ID({businessId}) in {elapsedTime}ms", 
                    character.GetFullName(), character.Id, businessRank.RankName, businessRank.Id, business.Id, business.BusinessName, Time.GetElapsedTime(startTime));
            }
            else
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", $"Nie udało się usunąć stanowiska: {businessRank.RankName}.");
            }
        }

        private async Task DeleteBusinessEventAsync(IPlayer player, int businessId)
        {
            var startTime = Time.GetTimestampMs();
            if (!player.TryGetCharacter(out var character)) return;

            if (!_businessesManager.TryGetBusiness(businessId, out Business business))
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie znaleziono takiego biznesu.", 4000);
                return;
            }

            if (!business.GetBusinessRankForEmployee(character, out BusinessRank businessRank))
            {
                await _notificationService.ShowErrorNotificationAsync(character.Player, "Błąd", "Nie masz ustalonych żadnych możliwości w biznesie.", 6000);
                return;
            }

            if (!businessRank.IsOwnerRank)
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie posiadasz odpowiednich uprawień.", 4000);
                return;
            }

            if (await _businessesManager.DeleteBusinessAsync(business))
            {
                await _notificationService.ShowSuccessNotificationAsync(player, "Usunięto biznes", $"Pomyślnie usunięto biznes {business.BusinessName}.", 5000);
                _logger.LogInformation("Character {characterName} CID({characterId}) deleted business {businessName} ID({businessId}) in {elapsedTime}ms", 
                    character.GetFullName(), character.Id, business.BusinessName, business.Id, Time.GetTimestamp() - startTime);
            }
            else
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie udało się usunąć biznesu.", 5000);
            }
        }

        private static BusinessInfoDto GetBusinessInfoDto(Business business)
        {
            return new BusinessInfoDto
            {
                BusinessId = business.Id,
                BusinessName = business.BusinessName,
                CreatedAt = business.CreatedAt,
                EmployeesCount = business.EmployeesCount,
                Money = business.Money,
                Type = business.Type,
                OwnerId = business.OwnerId,
                MaxMembersCount = business.MaxMembersCount,
                Transactions = business.Transactions,
                Ranks = business.BusinessRanks.Count,
                MaxRanksCount = business.MaxRanksCount
            };
        }

        private static BusinessEmployeesDto GetBusinessEmployeesDto(Business business)
        {
            return new BusinessEmployeesDto
            {
                BusinessRanks = business.BusinessRanks.Select(e => new BusinessRanksDto
                {
                    Id = e.Id,
                    RankName = e.RankName,
                    IsDefaultRank = e.IsDefaultRank,
                    IsOwnerRank = e.IsOwnerRank
                }).ToList(),
                BusinessEmployees = business.Employees.Select(e => new BusinessEmployeeExtendedDto
                {
                    Id = e.Id,
                    LastName = e.LastName,
                    Name = e.FirstName,
                    Age = e.Age,
                    Gender = e.Gender,
                    RankId = e.BusinessRank,
                    RankName = business.BusinessRanks.FirstOrDefault(br => br.Id == e.BusinessRank).RankName,
                }).ToList()
            };
        }

        private static List<BusinessPermissionsDto> GetBusinessRanksInfo(Business business)
        {
            return business.BusinessRanks.Select(q => new BusinessPermissionsDto
            {
                Id = q.Permissions.Id,
                RankId = q.Id,
                RankName = q.RankName,
                CanOpenBusinessMenu = q.Permissions.CanOpenBusinessMenu,
                HaveBusinessKeys = q.Permissions.HaveBusinessKeys,
                CanOpenBusinessInventory = q.Permissions.CanOpenBusinessInventory,
                HaveVehicleKeys = q.Permissions.HaveVehicleKeys,
                CanInviteNewMembers = q.Permissions.CanInviteNewMembers,
                CanManageEmployees = q.Permissions.CanManageEmployess,
                CanManageRanks = q.Permissions.CanManageRanks,
            }).ToList();
        }
    }
}
