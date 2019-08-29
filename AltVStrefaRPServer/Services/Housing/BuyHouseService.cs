using System.Threading.Tasks;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Houses;
using AltVStrefaRPServer.Services.Money;

namespace AltVStrefaRPServer.Services.Housing
{
    public class BuyHouseService : IBuyHouseService
    {
        private readonly IMoneyService _moneyService;

        public BuyHouseService(IMoneyService moneyService)
        {
            _moneyService = moneyService;
        }
        
        public async Task<BuyHouseResponse> BuyHouseAsync(Character newOwner, House house)
        {
            // Check if house has owner
            if (house.HasOwner()) return BuyHouseResponse.HouseHasOwner;
            if (newOwner.Inventory.HasEmptySlots()) return BuyHouseResponse.NotEnoughSpaceInInventoryForKey;
            
            // Check if user has empty space for key item
            // Set house owner
            // Generate new house lock pattern
            // Add key item to character inventory

            return BuyHouseResponse.HouseBought;
        }
    }
}