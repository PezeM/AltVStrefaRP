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

namespace AltVStrefaRPServer.Modules.Businesses
{
    public class BusinessHandler
    {
        private BusinessManager _businessManager;
        private INotificationService _notificationService;

        public BusinessHandler(BusinessManager businessManager, INotificationService notificationService)
        {
            _businessManager = businessManager;
            _notificationService = notificationService;

            Alt.Log("Intialized business handler.");
            Alt.On<IPlayer, int>("GetBusinessEmployees", GetBusinessEmployeesEvent);
            AltAsync.On<IPlayer, int, int, int>("UpdateEmployeeRank", async (player, employeeId, newRankId, businessId) 
                => await UpdateEmployeeRankEvent(player, employeeId, newRankId, businessId));
            Alt.On<IPlayer, string, string, int>("AddNewEmployee", AddNewEmployeeEvent);
            AltAsync.On<IPlayer, int>("AcceptInviteToBusiness", async (player, businessId) 
                => await AcceptInviteToBusinessEvent(player, businessId));
            Alt.On<IPlayer, int>("GetBusinessRoles", GetBusinessRolesEvent);
            AltAsync.On<IPlayer, string, int>("UpdateBusinessRank", async (player, business, businessId) 
                => await UpdateBusinessRank(player, business, businessId));
            AltAsync.On<IPlayer, string, int>("AddNewRole", async (player, newRole, businessId) 
                => await AddNewRoleEvent(player, newRole, businessId));
            AltAsync.On<IPlayer, int, int>("DeleteEmployee", async (player, employeeId, businessId) 
                => await DeleteEmployeeEvent(player, employeeId, businessId));
            AltAsync.On<IPlayer, int, int>("DeleteRole", async (player, roleId, businessId) 
                => await DeleteRoleEvent(player, roleId, businessId));
            AltAsync.On<IPlayer, int>("DeleteBusiness", async (player, businessId) 
                => await DeleteBusinessEvent(player, businessId));
        }

        private bool GetBusiness(int businessId, out Business business)
        {
            business = _businessManager.GetBusiness(businessId);
            return business != null;
        }

        private void GetBusinessEmployeesEvent(IPlayer player, int businessId)
        {
            var startTime = Time.GetTimestampMs();
            var character = player.GetCharacter();
            if (character == null) return;

            if (!GetBusiness(businessId, out Business business))
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
            Alt.Log($"Character ID({character.Id}) requested business employees in {Time.GetTimestampMs() - startTime}ms.");
        }

        public void OpenBusinessMenu(Character character)
        {
            var startTime = Time.GetTimestampMs();
            if (!_businessManager.TryGetBusiness(character, out Business business))
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
            Alt.Log($"Character ID({character.Id}) opened business menu in {Time.GetTimestampMs() - startTime}ms.");
        }

        private async Task UpdateEmployeeRankEvent(IPlayer player, int employeeId, int newRankId, int businessId)
        {
            var startTime = Time.GetTimestampMs();
            var character = player.GetCharacter();
            if (character == null) return;

            if (!GetBusiness(businessId, out Business business))
            {
                _notificationService.ShowErrorNotification(player, "Błąd", "Nie znaleziono takiego biznesu.", 4000);
                return;
            }

            if (!business.GetBusinessRankForEmployee(character, out BusinessRank businessRank))
            {
                _notificationService.ShowErrorNotification(player, "Błąd", "Nie masz ustalonych żadnych możliwości w biznesie.", 6000);
                return;
            }

            if (!businessRank.Permissions.CanManageEmployess)
            {
                _notificationService.ShowErrorNotification(player, "Błąd", "Nie posiadasz odpowiednich uprawień.", 4000);
                return;
            }

            if (!business.IsCharacterEmployee(employeeId, out Character employee))
            {
                _notificationService.ShowErrorNotification(player, "Błąd", "Ten pracownik nie jest zatrudniony u ciebie w firmie.", 7000);
                return;
            }

            if (!business.CheckIfRankExists(newRankId))
            {
                _notificationService.ShowErrorNotification(player, "Błąd", "Nie znaleziono podanego stanowiska.", 5000);
                return;
            }

            await _businessManager.UpdateEmployeeRank(business, employee, newRankId).ConfigureAwait(false);
            await player.EmitAsync("successfullyUpdatedEmployeeRank", employeeId, newRankId).ConfigureAwait(false);
            AltAsync.Log($"Character ID({character.Id}) changed business rank of character ID({employee.Id}) to RankID({newRankId})" +
                    $" in {Time.GetTimestampMs() - startTime}ms.");
        }

        private void AddNewEmployeeEvent(IPlayer player, string name, string lastName, int businessId)
        {
            var startTime = Time.GetTimestampMs();
            var character = player.GetCharacter();
            if (character == null) return;

            if (!GetBusiness(businessId, out Business business))
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

            Alt.Log($"Character ID({character.Id}) invited {character.GetFullName()} ID({character.Id}) " +
                    $"to business({business.BusinessName}) ID({business.Id}) in {Time.GetTimestampMs() - startTime}ms.");
        }

        private async Task AcceptInviteToBusinessEvent(IPlayer player, int businessId)
        {
            var character = player.GetCharacter();
            if (character == null) return;

            if (!GetBusiness(businessId, out Business business))
            {
                _notificationService.ShowErrorNotification(player, "Błąd", "Nie znaleziono takiego biznesu.", 4000);
                return;
            }

            if (await _businessManager.AddNewEmployee(business, character).ConfigureAwait(false))
            {
                _notificationService.ShowSuccessNotification(player, "Nowa praca!", $"Pomyślnie dołączono do biznesu {business.BusinessName}.", 5000);
            }
            else
            {
                _notificationService.ShowErrorNotification(player, "Błąd", "Wystąpił błąd podczas dołączania do biznesu.", 5000);
                return;
            }

            AltAsync.Log($"Character ID({character.Id}) joined business {business.BusinessName} ID({business.Id}).");
        }

        private void GetBusinessRolesEvent(IPlayer player, int businessId)
        {
            var startTime = Time.GetTimestampMs();
            var character = player.GetCharacter();
            if (character == null) return;

            if (!GetBusiness(businessId, out Business business))
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
            Alt.Log($"Character ID({character.Id}) requested list of business roles in {Time.GetTimestampMs() - startTime}ms.");
        }

        private async Task UpdateBusinessRank(IPlayer player, string newPermissionsString, int businessId)
        {
            var startTime = Time.GetTimestampMs();
            var character = player.GetCharacter();
            if (character == null) return;

            if (!GetBusiness(businessId, out Business business))
            {
                _notificationService.ShowErrorNotification(player, "Błąd", "Nie znaleziono takiego biznesu.", 4000);
                return;
            }
            
            if (!business.GetBusinessRankForEmployee(character, out BusinessRank businessRank))
            {
                _notificationService.ShowErrorNotification(character.Player, "Błąd", "Nie masz ustalonych żadnych możliwości w biznesie.", 6000);
                return;
            }

            if (!businessRank.Permissions.CanManageRanks)
            {
                _notificationService.ShowErrorNotification(player, "Błąd", "Nie posiadasz odpowiednich uprawień.", 4000);
                return;
            }

            BusinessPermissionsDto newPermissions;
            try
            {
                newPermissions = JsonConvert.DeserializeObject<BusinessPermissionsDto>(newPermissionsString);
            }
            catch (Exception e)
            {
                Alt.Log($"Error in deserializing new business permissions: {e}");
                throw;
            }

            if (!business.GetBusinessRank(newPermissions.RankId, out BusinessRank businessRankToUpdate))
            {
                _notificationService.ShowErrorNotification(player, "Błąd", "Nie znaleziono takiego stanowiska w biznesie.", 4000);
                return;
            }

            await _businessManager.UpdateBusinessRank(businessRankToUpdate, newPermissions).ConfigureAwait(false);
            _notificationService.ShowSuccessNotification(player, "Zaktualizowano stanowisko", "Pomyślnie zaktualizowano stanowisko.");
            AltAsync.Log($"Character ID({character.Id}) changed permissions in rank ID({businessRank.Id})" +
                    $" in {Time.GetTimestampMs() - startTime}ms.");
        }

        private async Task AddNewRoleEvent(IPlayer player, string newRankString, int businessId)
        {
            var startTime = Time.GetTimestampMs();
            var character = player.GetCharacter();
            if (character == null) return;

            if (!GetBusiness(businessId, out Business business))
            {
                _notificationService.ShowErrorNotification(player, "Błąd", "Nie znaleziono takiego biznesu.", 4000);
                return;
            }

            if (!business.GetBusinessRankForEmployee(character, out BusinessRank businessRank))
            {
                _notificationService.ShowErrorNotification(character.Player, "Błąd", "Nie masz ustalonych żadnych możliwości w biznesie.", 6000);
                return;
            }

            if (!businessRank.Permissions.CanManageRanks)
            {
                _notificationService.ShowErrorNotification(player, "Błąd", "Nie posiadasz odpowiednich uprawień.", 4000);
                return;
            }

            BusinessNewRankDto newRank;
            try
            {
                newRank = JsonConvert.DeserializeObject<BusinessNewRankDto>(newRankString);
            }
            catch (Exception e)
            {
                Alt.Log($"Error in deserializing new business permissions: {e}");
                throw;
            }

            if(newRank == null) return;
            if (await _businessManager.AddNewBusinessRank(business, newRank).ConfigureAwait(false))
            {
                _notificationService.ShowSuccessNotification(player, "Zaktualizowano stanowisko", "Pomyślnie zaktualizowano stanowisko");
            }
            else
            {
                _notificationService.ShowErrorNotification(player, "Błąd", "Nie udało się dodać nowego stanowiska.", 5000);
                return;
            }

            AltAsync.Log($"Character ID({character.Id}) added new role {newRank.RankName} to business ID({business.Id})" +
                    $" in {Time.GetTimestampMs() - startTime}ms.");
        }

        private async Task DeleteEmployeeEvent(IPlayer player, int employeeId, int businessId)
        {
            var startTime = Time.GetTimestampMs();
            var character = player.GetCharacter();
            if (character == null) return;

            if (!GetBusiness(businessId, out Business business))
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
                _notificationService.ShowErrorNotification(player, "Błąd", "Nie posiadasz odpowiednich uprawień.", 4000);
                return;
            }

            if (!business.IsCharacterEmployee(employeeId, out Character employee))
            {
                _notificationService.ShowErrorNotification(player, "Błąd", "Nie jest pracownikiem w firmie.", 4000);
                return;
            }

            if (await _businessManager.RemoveEmployee(business, employee))
            {
                _notificationService.ShowSuccessNotification(player, "Usunięto pracownika", $"Pomyślnie usunięto {employee.GetFullName()} z firmy.", 5000);
                AltAsync.Log($"Character ID({character.Id}) deleted employee ID({employee.Id}) from business {business.BusinessName} " +
                             $"ID({business.Id}) in {Time.GetTimestampMs() - startTime}ms.");
            }
            else
            {
                _notificationService.ShowErrorNotification(player, "Błąd", $"Wystąpił błąd podczas usuwania {employee.GetFullName()} z firmy.", 6500);
            }
        }

        private async Task DeleteRoleEvent(IPlayer player, int rankId, int businessId)
        {
            var startTime = Time.GetTimestampMs();
            var character = player.GetCharacter();
            if (character == null) return;

            if (!GetBusiness(businessId, out Business business))
            {
                _notificationService.ShowErrorNotification(player, "Błąd", "Nie znaleziono takiego biznesu.", 4000);
                return;
            }

            if (!business.GetBusinessRankForEmployee(character, out BusinessRank businessRank))
            {
                _notificationService.ShowErrorNotification(character.Player, "Błąd", "Nie masz ustalonych żadnych możliwości w biznesie.", 6000);
                return;
            }

            if (!businessRank.Permissions.CanManageRanks)
            {
                _notificationService.ShowErrorNotification(player, "Błąd", "Nie posiadasz odpowiednich uprawień.", 4000);
                return;
            }

            if (!business.CheckIfRankExists(rankId)) return;

            if (await _businessManager.RemoveRank(business, rankId))
            {
                _notificationService.ShowSuccessNotification(player, "Usunięto stanowisko", $"Pomyślnie usunięto stanowisko.", 5000);
                AltAsync.Log($"Character ID({character.Id}) deleted rank ID({rankId}) " +
                             $"from business {business.BusinessName} ID({business.Id}) in {Time.GetTimestampMs() - startTime}ms.");
            }
            else
            {
                _notificationService.ShowErrorNotification(player, "Błąd", $"Nie udało się usunąć stanowiska: {businessRank.RankName}.");
            }
        }

        private async Task DeleteBusinessEvent(IPlayer player, int businessId)
        {
            var startTime = Time.GetTimestampMs();
            var character = player.GetCharacter();
            if (character == null) return;

            if (!GetBusiness(businessId, out Business business))
            {
                _notificationService.ShowErrorNotification(player, "Błąd", "Nie znaleziono takiego biznesu.", 4000);
                return;
            }

            if (!business.GetBusinessRankForEmployee(character, out BusinessRank businessRank))
            {
                _notificationService.ShowErrorNotification(character.Player, "Błąd", "Nie masz ustalonych żadnych możliwości w biznesie.", 6000);
                return;
            }

            if (!businessRank.IsOwnerRank)
            {
                _notificationService.ShowErrorNotification(player, "Błąd", "Nie posiadasz odpowiednich uprawień.", 4000);
                return;
            }

            if (await _businessManager.DeleteBusiness(business))
            {
                _notificationService.ShowSuccessNotification(player, "Usunięto biznes", $"Pomyślnie usunięto biznes {business.BusinessName}.", 5000);
                AltAsync.Log($"Character ID({character.Id}) deleted business ID({business.BusinessName}) " +
                             $"ID({business.Id}) in {Time.GetTimestampMs() - startTime}ms.");
            }
            else
            {
                _notificationService.ShowErrorNotification(player, "Błąd", "Nie udało się usunąć biznesu.", 5000);
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

        private static List<BusinessPermissionsDto> GetBusinessRanksInfo(Models.Businesses.Business business)
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
