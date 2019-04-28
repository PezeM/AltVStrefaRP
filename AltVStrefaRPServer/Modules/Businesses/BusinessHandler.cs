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
        }

        private void GetBusinessEmployeesEvent(IPlayer player, object[] args)
        {
            var startTime = Time.GetTimestampMs();
            if (!int.TryParse(args[0].ToString(), out int businessId))
            {
                _notificationService.ShowErrorNotfication(player, "Błąd", "Błędne ID biznesu.", 4000);
                return;
            }

            var business = _businessManager.GetBusiness(businessId);
            if (business == null)
            {
                _notificationService.ShowErrorNotfication(player, "Błąd", "Nie znaleziono takiego biznesu.", 4000);
                return;
            }

            var character = player.GetCharacter();
            if (character == null) return;

            var businessRank = _businessManager.GetBusinessRankForPlayer(business, character);
            if (businessRank == null)
            {
                _notificationService.ShowErrorNotfication(character.Player, "Błąd", "Nie masz ustalonych żadnych możliwości w biznesie.", 6000);
                return;
            }

            if (!businessRank.Permissions.CanManageEmployess)
            {
                _notificationService.ShowErrorNotfication(character.Player, "Błąd", "Nie masz odpowiednich uprawień.");
                return;
            }

            var employeeRanksInfo = new BusinessEmployeesDto
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

            var businessRanksObject = JsonConvert.SerializeObject(employeeRanksInfo);
            player.Emit("populateEmployeeRanks", businessRanksObject);
            Alt.Log($"Business employess object: {businessRanksObject}: in {Time.GetTimestampMs() - startTime}ms.");

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

            var businessRank = _businessManager.GetBusinessRankForPlayer(business, character);
            if (businessRank == null)
            {
                _notificationService.ShowErrorNotfication(character.Player, "Błąd", "Nie masz ustalonych żadnych uprawnień w biznesie.", 6000);
                return;
            }

            if (!businessRank.Permissions.CanOpenBusinessMenu)
            {
                _notificationService.ShowErrorNotfication(character.Player, "Błąd", "Nie możesz otwierać menu biznesu.", 4000);
                return;
            }

            var businessInfo = new BusinessInfoDto
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

            var businessObject = JsonConvert.SerializeObject(businessInfo);
            character.Player.Emit("openBusinessMenu", businessObject);
            Alt.Log($"Business object: {businessObject} in {Time.GetTimestampMs() - startTime}ms.");
        }

        private async Task UpdateEmployeeRankEvent(IPlayer player, object[] args)
        {
            var startTime = Time.GetTimestampMs();
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

            if (!int.TryParse(args[2].ToString(), out int businessId))
            {
                _notificationService.ShowErrorNotfication(player, "Błąd", "Podano błędne ID biznesu.", 4000);
                return;
            }

            var character = player.GetCharacter();
            if (character == null) return;

            var business = _businessManager.GetBusiness(businessId);
            if (business == null)
            {
                _notificationService.ShowErrorNotfication(player, "Błąd", "Nie znaleziono takiego biznesu.", 4000);
                return;
            }

            var businessRank = _businessManager.GetBusinessRankForPlayer(business, character);
            if (businessRank == null)
            {
                _notificationService.ShowErrorNotfication(player, "Błąd", "Nie masz ustalonych żadnych uprawnień w biznesie.", 6000);
                return;
            }

            if (!businessRank.Permissions.CanManageEmployess)
            {
                _notificationService.ShowErrorNotfication(player, "Błąd", "Nie posiadasz odpowiednich uprawień.", 4000);
                return;
            }

            if (business.BusinessRanks.FirstOrDefault(q => q.Id == newRankId) == null)
            {
                _notificationService.ShowErrorNotfication(player, "Błąd", "Nie znaleziono podanego stanowiska.", 5000);
                return;
            }

            var employee = await _characterDatabaseService.FindCharacterByIdAsync(employeeId).ConfigureAwait(false);
            if (employee == null)
            {
                _notificationService.ShowErrorNotfication(player, "Błąd", "Nie znaleziono takiego pracownika.", 5000);
                return;
            }

            if (!_businessManager.IsCharacterEmployee(business, employee))
            {
                _notificationService.ShowErrorNotfication(player, "Błąd", "Ten pracownik nie jest zatrudniony u ciebie w firmie.", 7000);
                return;
            }

            await _businessManager.UpdateEmployeeRank(business, employee, newRankId).ConfigureAwait(false);
            player.Emit("successfullyUpdatedEmployeeRank", employeeId, newRankId);
            Alt.Log($"ID({character.Id}) changed business rank of player ID({employee.Id}) to RankID({newRankId})" +
                    $" in {Time.GetTimestampMs() - startTime}ms.");
        }

        private void AddNewEmployeeEvent(IPlayer player, object[] args)
        {
            var startTime = Time.GetTimestampMs();
            if (args.Length < 3)
            {
                _notificationService.ShowErrorNotfication(player, "Błąd", "Nie podano imienia lub nazwiska pracownika.", 4000);
                return;
            }

            var name = args[0].ToString();
            var lastName = args[1].ToString();
            if (!int.TryParse(args[2].ToString(), out int businessId))
            {
                _notificationService.ShowErrorNotfication(player, "Błąd", "Błędne ID biznesu.", 4000);
                return;
            }

            var character = player.GetCharacter();
            if (character == null) return;

            var business = _businessManager.GetBusiness(businessId);
            if (business == null)
            {
                _notificationService.ShowErrorNotfication(player, "Błąd", "Nie znaleziono takiego biznesu.", 4000);
                return;
            }

            var businessRank = _businessManager.GetBusinessRankForPlayer(business, character);
            if (businessRank == null)
            {
                _notificationService.ShowErrorNotfication(player, "Błąd", "Nie masz ustalonych żadnych uprawnień w biznesie.", 6000);
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

            // TODO: Send event to newEmployee with question if he wants to join this business
            newEmployee.Player.Emit("showConfirmModal", "Oferta pracy", $"Otrzymałeś zaproszenie do firmy {business.BusinessName}. " +
                                                                        $"Czy chcesz je przyjąć?", (int)ConfirmModalType.BusinessInvite, businessId);

            // To test
            player.Emit("showConfirmModal", "Oferta pracy", $"Otrzymałeś zaproszenie do firmy {business.BusinessName}. " +
                                                            $"Czy chcesz je przyjąć?", (int)ConfirmModalType.BusinessInvite, businessId);

            player.Emit("successfullyInvitedNewEmployee");
            Alt.Log($"Character ID({character.Id}) invited {character.GetFullName()} ID({character.Id}) " +
                    $"to business({business.BusinessName}) ID({business.Id})");
        }

        private async Task AcceptInviteToBusinessEvent(IPlayer player, object[] args)
        {
            if (args.Length < 1) return;
            if (!int.TryParse(args[0].ToString(), out int businessId))
            {
                _notificationService.ShowErrorNotfication(player, "Błąd", "Błędne ID biznesu.", 4000);
                return;
            }

            var character = player.GetCharacter();
            if (character == null) return;

            var business = _businessManager.GetBusiness(businessId);
            if (business == null)
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
                _notificationService.ShowErrorNotfication(player, "Błąd", "Wystąpił błąd podczas dołączania do biznesu.", 4000);
                return;
            }

            AltAsync.Log($"Character ID({character.Id}) joined business {business.BusinessName} ID({business.Id}).");
        }

        private void GetBusinessRolesEvent(IPlayer player, object[] args)
        {
            var startTime = Time.GetTimestampMs();
            if (!int.TryParse(args[0].ToString(), out int businessId))
            {
                _notificationService.ShowErrorNotfication(player, "Błąd", "Błędne ID biznesu.", 4000);
                return;
            }

            var business = _businessManager.GetBusiness(businessId);
            if (business == null)
            {
                _notificationService.ShowErrorNotfication(player, "Błąd", "Nie znaleziono takiego biznesu.", 4000);
                return;
            }

            var character = player.GetCharacter();
            if (character == null) return;

            List<BusinessPermissionsDto> businessRanksInfo = GetBusinessRanksInfo(business);
            var businessPermissionsObject = JsonConvert.SerializeObject(businessRanksInfo);
            player.Emit("populateBusinessRanksInfo", businessPermissionsObject);
            Alt.Log($"Business employess object: {businessPermissionsObject}: in {Time.GetTimestampMs() - startTime}ms.");
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
