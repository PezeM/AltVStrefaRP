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
            // Check if user has empty space for key item
            if (newOwner.Inventory.HasEmptySlots()) return BuyHouseResponse.NotEnoughSpaceInInventoryForKey;

            // Add key item to character inventory
            // Generate new house lock pattern
            // Set house owner

            return BuyHouseResponse.HouseBought;
        }
    }
}