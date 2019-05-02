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
using AltVStrefaRPServer.Services.Characters;
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
        private ICharacterDatabaseService _characterDatabaseService;

        public BusinessHandler(BusinessManager businessManager, INotificationService notificationService,
            ICharacterDatabaseService characterDatabaseService)
        {
            _businessManager = businessManager;
            _notificationService = notificationService;
            _characterDatabaseService = characterDatabaseService;

            Alt.Log("Intialized business handler.");
            Alt.OnClient("GetBusinessEmployees", GetBusinessEmployeesEvent);
            AltAsync.OnClient("UpdateEmployeeRank", UpdateEmployeeRankEvent);
            Alt.OnClient("AddNewEmployee", AddNewEmployeeEvent);
            AltAsync.OnClient("AcceptInviteToBusiness", AcceptInviteToBusinessEvent);
            Alt.OnClient("GetBusinessRoles", GetBusinessRolesEvent);
            AltAsync.OnClient("UpdateBusinessRank", UpdateBusinessRank);
            AltAsync.OnClient("AddNewRole", AddNewRoleEvent);
            AltAsync.OnClient("DeleteEmployee", DeleteEmployeeEvent);
            AltAsync.OnClient("DeleteRole", DeleteRoleEvent);
        }

        private bool GetBusiness(string businessIdString, out Business business)
        {
            business = null;
            if (!int.TryParse(businessIdString, out int businessId)) return false;

            business = _businessManager.GetBusiness(businessId);
            if (business == null) return false; 

            return true;
        }

        private void GetBusinessEmployeesEvent(IPlayer player, object[] args)
        {
            var startTime = Time.GetTimestampMs();
            var character = player.GetCharacter();
            if (character == null) return;

            if (!GetBusiness(args[0].ToString(), out Business business))
            {
                _notificationService.ShowErrorNotfication(player, "Błąd", "Nie znaleziono takiego biznesu.", 4000);
                return;
            }

            if (!business.GetBusinessRankForEmployee(character, out BusinessRank businessRank))
            {
                _notificationService.ShowErrorNotfication(character.Player, "Błąd", "Nie masz ustalonych żadnych możliwości w biznesie.", 6000);
                return;
            }

            if (!businessRank.Permissions.CanManageEmployess)
            {
                _notificationService.ShowErrorNotfication(character.Player, "Błąd", "Nie masz odpowiednich uprawień.");
                return;
            }

            player.Emit("populateEmployeeRanks", JsonConvert.SerializeObject(GetBusinessEmployeesDto(business)));
            Alt.Log($"Character ID({character.Id}) requested business employees in {Time.GetTimestampMs() - startTime}ms.");
        }

        public void OpenBusinessMenu(Character character)
        {
            var startTime = Time.GetTimestampMs();
            var business = _businessManager.GetBusiness(character);
            if (business == null)
            {
                _notificationService.ShowErrorNotfication(character.Player, "Błąd", "Nie jesteś w żadnym biznesie.", 4000);
                return;
            }

            if (!business.GetBusinessRankForEmployee(character, out BusinessRank businessRank))
            {
                _notificationService.ShowErrorNotfication(character.Player, "Błąd", "Nie masz ustalonych żadnych możliwości w biznesie.", 6000);
                return;
            }

            if (!businessRank.Permissions.CanOpenBusinessMenu)
            {
                _notificationService.ShowErrorNotfication(character.Player, "Błąd", "Nie możesz otwierać menu biznesu.", 4000);
                return;
            }

            character.Player.Emit("openBusinessMenu", JsonConvert.SerializeObject(GetBusinessInfoDto(business)));
            Alt.Log($"Character ID({character.Id}) opened business menu in {Time.GetTimestampMs() - startTime}ms.");
        }

        private async Task UpdateEmployeeRankEvent(IPlayer player, object[] args)
        {
            var startTime = Time.GetTimestampMs();
            var character = player.GetCharacter();
            if (character == null) return;
            if (args.Length < 3) return;

            if (!int.TryParse(args[0].ToString(), out int employeeId))
            {
                _notificationService.ShowErrorNotfication(player, "Błąd", "Podano błędne ID pracownika,", 4000);
                return;
            }

            if (!int.TryParse(args[1].ToString(), out int newRankId))
            {
                _notificationService.ShowErrorNotfication(player, "Błąd", "Podano błędne ID nowego stanowiska.", 4000);
                return;
            }

            if (!GetBusiness(args[2].ToString(), out Business business))
            {
                _notificationService.ShowErrorNotfication(player, "Błąd", "Nie znaleziono takiego biznesu.", 4000);
                return;
            }

            if (!business.GetBusinessRankForEmployee(character, out BusinessRank businessRank))
            {
                _notificationService.ShowErrorNotfication(player, "Błąd", "Nie masz ustalonych żadnych możliwości w biznesie.", 6000);
                return;
            }

            if (!businessRank.Permissions.CanManageEmployess)
            {
                _notificationService.ShowErrorNotfication(player, "Błąd", "Nie posiadasz odpowiednich uprawień.", 4000);
                return;
            }

            if (!business.IsCharacterEmployee(employeeId, out Character employee))
            {
                _notificationService.ShowErrorNotfication(player, "Błąd", "Ten pracownik nie jest zatrudniony u ciebie w firmie.", 7000);
                return;
            }

            if (!business.CheckIfRankExists(newRankId))
            {
                _notificationService.ShowErrorNotfication(player, "Błąd", "Nie znaleziono podanego stanowiska.", 5000);
                return;
            }

            await _businessManager.UpdateEmployeeRank(business, employee, newRankId).ConfigureAwait(false);
            await player.EmitAsync("successfullyUpdatedEmployeeRank", employeeId, newRankId).ConfigureAwait(false);
            AltAsync.Log($"Character ID({character.Id}) changed business rank of character ID({employee.Id}) to RankID({newRankId})" +
                    $" in {Time.GetTimestampMs() - startTime}ms.");
        }

        private void AddNewEmployeeEvent(IPlayer player, object[] args)
        {
            var startTime = Time.GetTimestampMs();
            var character = player.GetCharacter();
            if (character == null) return;
            if (args.Length < 3) return;

            var name = args[0].ToString();
            var lastName = args[1].ToString();

            if (!GetBusiness(args[2].ToString(), out Business business))
            {
                _notificationService.ShowErrorNotfication(player, "Błąd", "Nie znaleziono takiego biznesu.", 4000);
                return;
            }

            if (!business.GetBusinessRankForEmployee(character, out BusinessRank businessRank))
            {
                _notificationService.ShowErrorNotfication(player, "Błąd", "Nie masz ustalonych żadnych możliwości w biznesie.", 6000);
                return;
            }

            if (!businessRank.Permissions.CanInviteNewMembers)
            {
                _notificationService.ShowErrorNotfication(player, "Błąd", "Nie posiadasz odpowiednich uprawień.", 4000);
                return;
            }

            var newEmployee = CharacterManager.Instance.GetCharacter(name, lastName);
            if (newEmployee == null)
            {
                _notificationService.ShowErrorNotfication(player, "Błąd", "Nie znaleziono osoby z takim imieniem i nazwiskiem.", 7000);
                return;
            }

            if (!business.CanAddNewMember(newEmployee))
            {
                _notificationService.ShowErrorNotfication(player, "Błąd", $"Nie można zatrudnić {newEmployee.GetFullName()}" +
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

        private async Task AcceptInviteToBusinessEvent(IPlayer player, object[] args)
        {
            if (args.Length < 1) return;
            var character = player.GetCharacter();
            if (character == null) return;

            if (!GetBusiness(args[0].ToString(), out Business business))
            {
                _notificationService.ShowErrorNotfication(player, "Błąd", "Nie znaleziono takiego biznesu.", 4000);
                return;
            }

            if (await _businessManager.AddNewEmployee(business, character).ConfigureAwait(false))
            {
                _notificationService.ShowSuccessNotification(player, "Nowa praca!", $"Pomyślnie dołączono do biznesu {business.BusinessName}.", 5000);
            }
            else
            {
                _notificationService.ShowErrorNotfication(player, "Błąd", "Wystąpił błąd podczas dołączania do biznesu.", 5000);
                return;
            }

            AltAsync.Log($"Character ID({character.Id}) joined business {business.BusinessName} ID({business.Id}).");
        }

        private void GetBusinessRolesEvent(IPlayer player, object[] args)
        {
            var startTime = Time.GetTimestampMs();
            var character = player.GetCharacter();
            if (character == null) return;

            if (!GetBusiness(args[0].ToString(), out Business business))
            {
                _notificationService.ShowErrorNotfication(player, "Błąd", "Nie znaleziono takiego biznesu.", 4000);
                return;
            }

            player.Emit("populateBusinessRanksInfo", JsonConvert.SerializeObject(GetBusinessRanksInfo(business)));
            Alt.Log($"Character ID({character.Id}) requested list of business roles in {Time.GetTimestampMs() - startTime}ms.");
        }

        private async Task UpdateBusinessRank(IPlayer player, object[] args)
        {
            var startTime = Time.GetTimestampMs();
            if (args.Length < 2) return;
            var character = player.GetCharacter();
            if (character == null) return;

            if (!GetBusiness(args[1].ToString(), out Business business))
            {
                _notificationService.ShowErrorNotfication(player, "Błąd", "Nie znaleziono takiego biznesu.", 4000);
                return;
            }
            
            if (!business.GetBusinessRankForEmployee(character, out BusinessRank businessRank))
            {
                _notificationService.ShowErrorNotfication(character.Player, "Błąd", "Nie masz ustalonych żadnych możliwości w biznesie.", 6000);
                return;
            }

            if (!businessRank.Permissions.CanManageRanks)
            {
                _notificationService.ShowErrorNotfication(player, "Błąd", "Nie posiadasz odpowiednich uprawień.", 4000);
                return;
            }

            BusinessPermissionsDto newPermissions;
            try
            {
                newPermissions = JsonConvert.DeserializeObject<BusinessPermissionsDto>(args[0].ToString());
            }
            catch (Exception e)
            {
                Alt.Log($"Error in deserializing new business permissions: {e}");
                throw;
            }

            if (!business.GetBusinessRank(newPermissions.RankId, out BusinessRank businessRankToUpdate))
            {
                _notificationService.ShowErrorNotfication(player, "Błąd", "Nie znaleziono takiego stanowiska w biznesie.", 4000);
                return;
            }

            await _businessManager.UpdateBusinessRank(businessRankToUpdate, newPermissions).ConfigureAwait(false);
            _notificationService.ShowSuccessNotification(player, "Zaktualizowano stanowisko", "Pomyślnie zaktualizowano stanowisko.");
            AltAsync.Log($"Character ID({character.Id}) changed permissions in rank ID({businessRank.Id})" +
                    $" in {Time.GetTimestampMs() - startTime}ms.");
        }

        private async Task AddNewRoleEvent(IPlayer player, object[] args)
        {
            var startTime = Time.GetTimestampMs();
            if (args.Length < 2) return;
            var character = player.GetCharacter();
            if (character == null) return;

            if (!GetBusiness(args[1].ToString(), out Business business))
            {
                _notificationService.ShowErrorNotfication(player, "Błąd", "Nie znaleziono takiego biznesu.", 4000);
                return;
            }

            if (!business.GetBusinessRankForEmployee(character, out BusinessRank businessRank))
            {
                _notificationService.ShowErrorNotfication(character.Player, "Błąd", "Nie masz ustalonych żadnych możliwości w biznesie.", 6000);
                return;
            }

            if (!businessRank.Permissions.CanManageRanks)
            {
                _notificationService.ShowErrorNotfication(player, "Błąd", "Nie posiadasz odpowiednich uprawień.", 4000);
                return;
            }

            BusinessNewRankDto newRank;
            try
            {
                newRank = JsonConvert.DeserializeObject<BusinessNewRankDto>(args[0].ToString());
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
                _notificationService.ShowErrorNotfication(player, "Błąd", "Nie udało się dodać nowego stanowiska.", 5000);
                return;
            }

            AltAsync.Log($"Character ID({character.Id}) added new role {newRank.RankName} to business ID({business.Id})" +
                    $" in {Time.GetTimestampMs() - startTime}ms.");
        }

        private async Task DeleteEmployeeEvent(IPlayer player, object[] args)
        {
            var startTime = Time.GetTimestampMs();
            if (args.Length < 2) return;
            var character = player.GetCharacter();
            if (character == null) return;

            if (!int.TryParse(args[0].ToString(), out int employeeId)) return;
            if (!GetBusiness(args[1].ToString(), out Business business))
            {
                _notificationService.ShowErrorNotfication(player, "Błąd", "Nie znaleziono takiego biznesu.", 4000);
                return;
            }

            if (!business.GetBusinessRankForEmployee(character, out BusinessRank businessRank))
            {
                _notificationService.ShowErrorNotfication(character.Player, "Błąd", "Nie masz ustalonych żadnych możliwości w biznesie.", 6000);
                return;
            }

            if (!businessRank.Permissions.CanManageEmployess)
            {
                _notificationService.ShowErrorNotfication(player, "Błąd", "Nie posiadasz odpowiednich uprawień.", 4000);
                return;
            }

            if (!business.IsCharacterEmployee(employeeId, out Character employee))
            {
                _notificationService.ShowErrorNotfication(player, "Błąd", "Nie jest pracownikiem w firmie.", 4000);
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
                _notificationService.ShowErrorNotfication(player, "Błąd", $"Wystąpił błąd podczas usuwania {employee.GetFullName()} z firmy.", 6500);
            }
        }

        private async Task DeleteRoleEvent(IPlayer player, object[] args)
        {
            var startTime = Time.GetTimestampMs();
            if (args.Length < 2) return;
            var character = player.GetCharacter();
            if (character == null) return;

            if (!int.TryParse(args[0].ToString(), out int rankId)) return;
            if (!GetBusiness(args[1].ToString(), out Business business))
            {
                _notificationService.ShowErrorNotfication(player, "Błąd", "Nie znaleziono takiego biznesu.", 4000);
                return;
            }

            if (!business.GetBusinessRankForEmployee(character, out BusinessRank businessRank))
            {
                _notificationService.ShowErrorNotfication(character.Player, "Błąd", "Nie masz ustalonych żadnych możliwości w biznesie.", 6000);
                return;
            }

            if (!businessRank.Permissions.CanManageRanks)
            {
                _notificationService.ShowErrorNotfication(player, "Błąd", "Nie posiadasz odpowiednich uprawień.", 4000);
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
                _notificationService.ShowErrorNotfication(player, "Błąd", $"Nie udało się usunąć stanowiska: {businessRank.RankName}.");
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
