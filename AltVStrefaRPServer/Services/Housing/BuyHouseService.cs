using System;
using System.Linq;
using System.Threading.Tasks;
using AltVStrefaRPServer.Database;
using AltVStrefaRPServer.Extensions;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Enums;
using AltVStrefaRPServer.Models.Houses;
using AltVStrefaRPServer.Services.Characters;
using AltVStrefaRPServer.Services.Inventories;
using AltVStrefaRPServer.Services.Money;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.ApplicationServices;

namespace AltVStrefaRPServer.Services.Housing
{
    public class BuyHouseService : IBuyHouseService
    {
        private readonly IMoneyService _moneyService;
        private readonly IItemFactory _itemFactory;
        private readonly IInventoryDatabaseService _inventoryDatabaseService;
        private readonly IHouseDatabaseService _houseDatabaseService;
        private readonly Func<ServerContext> _factory;
        private readonly ICharacterDatabaseService _characterDatabaseService;

        public BuyHouseService(IMoneyService moneyService, IItemFactory itemFactory, IInventoryDatabaseService inventoryDatabaseService,
            IHouseDatabaseService houseDatabaseService, Func<ServerContext> factory, ICharacterDatabaseService characterDatabaseService)
        {
            _moneyService = moneyService;
            _itemFactory = itemFactory;
            _inventoryDatabaseService = inventoryDatabaseService;    
            _houseDatabaseService = houseDatabaseService;
            _factory = factory;
            _characterDatabaseService = characterDatabaseService;
        }

        public async Task<BuyHouseResponse> BuyHouseAsync(Character newOwner, House house)
        {
            // Check if house has owner
            if (house.Flat.HasOwner()) return BuyHouseResponse.HouseHasOwner;
            // Check if user has empty space for key item
            if (!newOwner.Inventory.HasEmptySlots()) return BuyHouseResponse.NotEnoughSpaceInInventoryForKey;

            // Remove money from player
            if (!await _moneyService.RemoveMoneyFromBankAccountAsync(newOwner, house.Price, TransactionType.PropertiesBuy))
                return BuyHouseResponse.NotEnoughMoney;


            // Generate new house lock pattern
            house.Flat.CreateLockPattern();
            // house.Flat.ChangeOwner(newOwner); // This is causing error, updating owner of the flat

            // This or update flat or detach interior
            using (var context = _factory.Invoke())
            {
                //context.DetachLocal(house.Flat, house.Flat.Id);
                //var local = context.Set<Flat>().Local.FirstOrDefault(f => f.Id.Equals(house.Flat.Id));

                //if (local != null)
                //{
                //    context.Entry(local).State = EntityState.Detached;
                //}

                //context.Entry(house.Flat).State = EntityState.Modified;

                context.Flats.Update(house.Flat);
                //context.Houses.Update(house);
                //context.Characters.Update(newOwner);
                await _characterDatabaseService.UpdateCharacterAsync(newOwner);
                await context.SaveChangesAsync();

                // Add key item to character inventory
                var response = await newOwner.Inventory.AddItemAsync(_itemFactory.CreateHouseKeyItem(house.Flat.LockPattern), 1, _inventoryDatabaseService);
                if (!response.AnyChangesMade)
                {
                    // Add money
                    return BuyHouseResponse.NotEnoughSpaceInInventoryForKey;
                }
            }

            //await _houseDatabaseService.UpdateHouseAsync(house);

            return BuyHouseResponse.HouseBought;
        }
    }
}