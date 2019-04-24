using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Dto;
using AltVStrefaRPServer.Services;
using Newtonsoft.Json;

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
            Alt.OnClient("GetBusinessesEmployess", GetBusinessesEmployessEvent);
        }

        private void GetBusinessesEmployessEvent(IPlayer player, object[] args)
        {
            if (!int.TryParse(args[0].ToString(), out int businessId))
            {
                _notificationService.ShowErrorNotfication(player, "Błędne ID biznesu,", 4000);
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

            var businessEmployess = new BusinessEmployessDto
            {
                BusinessRanks = business.BusinessRanks.Select(e => new BusinessRanksDto
                {
                    Id = e.Id,
                    RankName = e.RankName,
                    IsDefaultRank = e.IsDefaultRank
                }).ToList(),
                BusinessEmployess = business.Employees.Select(e => new BusinessEmployeeExtendedDto
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

            var businessEmployessObject = JsonConvert.SerializeObject(businessEmployess);
            Alt.Log($"Business employess object: {businessEmployessObject}");
            player.Emit("populateBusinessEmployess", businessEmployessObject);
        }

        public void OpenBusinessMenu(Character character)
        {
            var business = _businessManager.GetBusiness(character);
            if (business == null)
            {
                _notificationService.ShowErrorNotfication(character.Player, "Nie jesteś w żadnym biznesie.", 4000);
                return;
            }

            var businessRank = _businessManager.GetBusinessRankForPlayer(business, character);
            if (businessRank == null)
            {
                _notificationService.ShowErrorNotfication(character.Player, "Nie masz ustalonych żadnych możliwości w biznesie.", 6000);
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
            Alt.Log($"Business object: {businessObject}");
            character.Player.Emit("openBusinessMenu", businessObject);
        }
    }
}
