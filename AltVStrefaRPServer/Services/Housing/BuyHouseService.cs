using System.Threading.Tasks;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Enums;
using AltVStrefaRPServer.Models.Houses;
using AltVStrefaRPServer.Services.Inventories;
using AltVStrefaRPServer.Services.Money;

namespace AltVStrefaRPServer.Services.Housing
{
    public class BuyHouseService : IBuyHouseService
    {
        private readonly IMoneyService _moneyService;
        private readonly IItemFactory _itemFactory;
        private readonly IInventoryDatabaseService _inventoryDatabaseService;
        private readonly IHouseDatabaseService _houseDatabaseService;

        public BuyHouseService(IMoneyService moneyService, IItemFactory itemFactory, IInventoryDatabaseService inventoryDatabaseService, IHouseDatabaseService houseDatabaseService)
        {
            _moneyService = moneyService;
            _itemFactory = itemFactory;
            _inventoryDatabaseService = inventoryDatabaseService;
            _houseDatabaseService = houseDatabaseService;
        }
        
        public async Task<BuyHouseResponse> BuyHouseAsync(Character newOwner, House house)
        {
            // Check if house has owner
            if (house.HasOwner() && house.Owner?.Id != newOwner.Id) return BuyHouseResponse.HouseHasOwner;
            // Check if user has empty space for key item
            if (newOwner.Inventory.HasEmptySlots()) return BuyHouseResponse.NotEnoughSpaceInInventoryForKey;

            // Remove money from player
            if (!await _moneyService.RemoveMoneyFromBankAccountAsync(newOwner, house.Price, TransactionType.PropertiesBuy))
                return BuyHouseResponse.NotEnoughMoney;
            
            // Generate new house lock pattern
            house.CreateLockPattern();
            // Add key item to character inventory
            await newOwner.Inventory.AddItemAsync(_itemFactory.CreateHouseKeyItem(house.LockPattern), 1,
                _inventoryDatabaseService);
            house.ChangeOwner(newOwner);
            await _houseDatabaseService.UpdateHouseAsync(house);
            
            return BuyHouseResponse.HouseBought;
        }
    }
}