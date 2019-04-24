using System.Linq;
using AltV.Net;
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
                Employees = business.Employees.Select(q => new BusinessEmployee
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
