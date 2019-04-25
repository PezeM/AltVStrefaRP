using System.Linq;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Helpers;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Dto;
using AltVStrefaRPServer.Services;
using AltVStrefaRPServer.Services.Characters;
using Newtonsoft.Json;

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
            Alt.OnClient("GetBusinessesEmployees", GetBusinessesEmployeesEvent);
            AltAsync.OnClient("UpdateEmployeeRank", UpdateEmployeeRankEvent);
        }

        private void GetBusinessesEmployeesEvent(IPlayer player, object[] args)
        {
            var startTime = Time.GetTimestampMs();
            if (!int.TryParse(args[0].ToString(), out int businessId))
            {
                _notificationService.ShowErrorNotfication(player, "Błędne ID biznesu.", 4000);
                return;
            }

            var business = _businessManager.GetBusiness(businessId);
            if (business == null)
            {
                _notificationService.ShowErrorNotfication(player, "Nie znaleziono takiego biznesu.", 4000);
                return;
            }

            var character = player.GetCharacter();
            if (character == null) return;

            var businessRank = _businessManager.GetBusinessRankForPlayer(business, character);
            if (businessRank == null)
            {
                _notificationService.ShowErrorNotfication(character.Player, "Nie masz ustalonych żadnych możliwości w biznesie.", 6000);
                return;
            }

            if (!businessRank.Permissions.CanManageEmployess)
            {
                _notificationService.ShowErrorNotfication(character.Player, "Nie masz odpowiednich uprawień.");
                return;
            }

            var businessEmployees = new BusinessEmployeesDto
            {
                BusinessRanks = business.BusinessRanks.Select(e => new BusinessRanksDto
                {
                    Id = e.Id,
                    RankName = e.RankName,
                    IsDefaultRank = e.IsDefaultRank
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

            var businessEmployeesObject = JsonConvert.SerializeObject(businessEmployees);
            player.Emit("populateBusinessEmployees", businessEmployeesObject);
            Alt.Log($"Business employess object: {businessEmployeesObject}: in {Time.GetTimestampMs() - startTime}ms.");

        }

        public void OpenBusinessMenu(Character character)
        {
            var startTime = Time.GetTimestampMs();
            var business = _businessManager.GetBusiness(character);
            if (business == null)
            {
                _notificationService.ShowErrorNotfication(character.Player, "Nie jesteś w żadnym biznesie.", 4000);
                return;
            }

            var businessRank = _businessManager.GetBusinessRankForPlayer(business, character);
            if (businessRank == null)
            {
                _notificationService.ShowErrorNotfication(character.Player, "Nie masz ustalonych żadnych uprawnień w biznesie.", 6000);
                return;
            }

            if (!businessRank.Permissions.CanOpenBusinessMenu)
            {
                _notificationService.ShowErrorNotfication(character.Player, "Nie możesz otwierać menu biznesu.",4000);
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
                Employees = business.Employees.Select(q => new BusinessEmployeeDto
                {
                    Id = q.Id,
                    LastName = q.LastName,
                    Name = q.FirstName,
                    RankId = q.BusinessRank,
                    RankName = business.BusinessRanks.FirstOrDefault(br => br.Id == q.BusinessRank).RankName,
                }).ToList(),
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
                _notificationService.ShowErrorNotfication(player, "Podano błędne ID pracownika,", 4000);
                return;
            }

            if (!int.TryParse(args[1].ToString(), out int newRankId))
            {
                _notificationService.ShowErrorNotfication(player, "Podano błędne ID nowego stanowiska.", 4000);
                return;
            }

            if (!int.TryParse(args[2].ToString(), out int businessId))
            {
                _notificationService.ShowErrorNotfication(player, "Podano błędne ID biznesu.", 4000);
                return;
            }

            var character = player.GetCharacter();
            if (character == null) return;

            var business = _businessManager.GetBusiness(businessId);
            if (business == null)
            {
                _notificationService.ShowErrorNotfication(player, "Nie znaleziono takiego biznesu.", 4000);
                return;
            }

            var businessRank = _businessManager.GetBusinessRankForPlayer(business, character);
            if (businessRank == null)
            {
                _notificationService.ShowErrorNotfication(player, "Nie masz ustalonych żadnych uprawnień w biznesie.", 6000);
                return;
            }

            if (!businessRank.Permissions.CanManageEmployess)
            {
                _notificationService.ShowErrorNotfication(player, "Nie posiadasz odpowiednich uprawień.", 4000);
                return;
            }

            if (business.BusinessRanks.FirstOrDefault(q => q.Id == newRankId) == null)
            {
                _notificationService.ShowErrorNotfication(player, "Nie znaleziono podanego stanowiska.", 5000);
                return;
            }

            var employee = await _characterDatabaseService.FindCharacterByIdAsync(employeeId).ConfigureAwait(false);
            if (employee == null)
            {
                _notificationService.ShowErrorNotfication(player, "Nie znaleziono takiego pracownika.", 5000);
                return;
            }

            if (!_businessManager.IsCharacterEmployee(business, employee))
            {
                _notificationService.ShowErrorNotfication(player, "Ten pracownik nie jest zatrudniony u ciebie w firmie.", 7000);
                return;
            }

            await _businessManager.UpdateEmployeeRank(business, employee, newRankId).ConfigureAwait(false);
        }
    }
}
