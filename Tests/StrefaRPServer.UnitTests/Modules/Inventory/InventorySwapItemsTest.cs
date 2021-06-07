using AltVStrefaRPServer.Models.Inventory;
using AltVStrefaRPServer.Services.Inventories;
using NUnit.Framework;
using StrefaRPServer.UnitTests.Core;
using System.Threading.Tasks;
using AltVStrefaRPServer.Models.Inventory.Responses;

namespace StrefaRPServer.UnitTests.Modules.Inventory
{
    public class InventorySwapItemsTest : ServerContextTestBase
    {
        private readonly int _inventoriesSlotsCount = 10;
        private InventoryTransferService _inventoryTransferService;
        private InventoryDatabaseService _inventoryDatabaseService;
        private InventoryContainer _swapToInventoryContainer;
        private InventoryContainer _swapFromInventoryContainer;
        private InventoryItem _itemToSwap;
        private InventoryItem _itemFromSwap;

        private ItemFactory _itemFactory;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _itemFactory = new ItemFactory();
        }

        [SetUp]
        public void Setups()
        {
            _inventoryDatabaseService = new InventoryDatabaseService(_mockFactory.Object);
            _inventoryTransferService = new InventoryTransferService(_inventoryDatabaseService, _mockFactory.Object);

            _swapToInventoryContainer = new InventoryContainer(_inventoriesSlotsCount);
            _swapFromInventoryContainer = new InventoryContainer(_inventoriesSlotsCount);

            _itemToSwap = new InventoryItem(_itemFactory.CreateBurger(), 1, 0);
            _itemFromSwap = new InventoryItem(_itemFactory.CreateBurger(), 1, 0);
        }

        [Test]
        public async Task SwapItemChangesItemsSlot()
        {
            var itemToSwapSlotId = 2;
            var itemFromSwapSlotId = 5;
            await _swapFromInventoryContainer.AddInventoryItemAsync(_itemToSwap, itemToSwapSlotId, _inventoryDatabaseService);
            await _swapFromInventoryContainer.AddInventoryItemAsync(_itemFromSwap, itemFromSwapSlotId, _inventoryDatabaseService);

            await _inventoryTransferService.SwapItemAsync(_swapFromInventoryContainer, _itemFromSwap, itemFromSwapSlotId, _itemToSwap, itemToSwapSlotId);
            
            Assert.That(_itemToSwap.SlotId, Is.EqualTo(itemFromSwapSlotId));
            Assert.That(_itemFromSwap.SlotId, Is.EqualTo(itemToSwapSlotId));
        }

        [Test]
        public async Task SwapItemBetweenInventoriesChangesItemsSlot()
        {
            var itemToSwapSlotId = 2;
            var itemFromSwapSlotId = 5;
            await _swapToInventoryContainer.AddInventoryItemAsync(_itemToSwap, itemToSwapSlotId, _inventoryDatabaseService);
            await _swapFromInventoryContainer.AddInventoryItemAsync(_itemFromSwap, itemFromSwapSlotId, _inventoryDatabaseService);

            await _inventoryTransferService.SwapItemAsync(_swapFromInventoryContainer, _itemFromSwap, itemFromSwapSlotId, _itemToSwap, itemToSwapSlotId,
                _swapToInventoryContainer);

            Assert.That(_itemToSwap.SlotId, Is.EqualTo(itemFromSwapSlotId));
            Assert.That(_itemFromSwap.SlotId, Is.EqualTo(itemToSwapSlotId));
        }

        [Test]
        public async Task SwapItemBetweenInventoriesChangesItemContainers()
        {
            var itemToSwapSlotId = 2;
            var itemFromSwapSlotId = 5;
            await _swapToInventoryContainer.AddInventoryItemAsync(_itemToSwap, itemToSwapSlotId, _inventoryDatabaseService);
            await _swapFromInventoryContainer.AddInventoryItemAsync(_itemFromSwap, itemFromSwapSlotId, _inventoryDatabaseService);

            await _inventoryTransferService.SwapItemAsync(_swapFromInventoryContainer, _itemFromSwap, itemFromSwapSlotId, _itemToSwap, itemToSwapSlotId,
                _swapToInventoryContainer);

            Assert.That(_swapToInventoryContainer.HasItem(_itemFromSwap), Is.True);
            Assert.That(_swapFromInventoryContainer.HasItem(_itemToSwap), Is.True);
        }

        [Test]
        public async Task SwapItemsBetweenInventoriesReturnsItemsSwappedResponseIfSucceeded()
        {
            var itemToSwapSlotId = 2;
            var itemFromSwapSlotId = 5;
            await _swapToInventoryContainer.AddInventoryItemAsync(_itemToSwap, itemToSwapSlotId, _inventoryDatabaseService);
            await _swapFromInventoryContainer.AddInventoryItemAsync(_itemFromSwap, itemFromSwapSlotId, _inventoryDatabaseService);

            var response = await _inventoryTransferService.SwapItemAsync(_swapFromInventoryContainer, _itemFromSwap, itemFromSwapSlotId, _itemToSwap, itemToSwapSlotId,
                _swapToInventoryContainer);

            Assert.That(response.Type, Is.EqualTo(InventorySwapItemResponseType.ItemsSwapped));
        }

        [Test]
        public async Task SwapItemsReturnsItemNewSlotsIds()
        {
            var itemToSwapSlotId = 2;
            var itemFromSwapSlotId = 5;
            await _swapToInventoryContainer.AddInventoryItemAsync(_itemToSwap, itemToSwapSlotId, _inventoryDatabaseService);
            await _swapFromInventoryContainer.AddInventoryItemAsync(_itemFromSwap, itemFromSwapSlotId, _inventoryDatabaseService);

            var response = await _inventoryTransferService.SwapItemAsync(_swapFromInventoryContainer, _itemFromSwap, itemFromSwapSlotId, _itemToSwap, itemToSwapSlotId,
                _swapToInventoryContainer);

            Assert.That(response.SelectedItemNewSlotId, Is.EqualTo(itemToSwapSlotId));
            Assert.That(response.SwappedItemNewSlotId, Is.EqualTo(itemFromSwapSlotId));
        }

        [Test]
        public async Task SwapItemsReturnsItemsNotFoundIfItemWasNotInInventory()
        {
            var itemToSwapSlotId = 2;
            var itemFromSwapSlotId = 5;
            await _swapFromInventoryContainer.AddInventoryItemAsync(_itemToSwap, itemToSwapSlotId, _inventoryDatabaseService);

            var response = await _inventoryTransferService.SwapItemAsync(_swapFromInventoryContainer, _itemFromSwap, itemFromSwapSlotId, _itemToSwap, itemToSwapSlotId);

            Assert.That(response.Type, Is.EqualTo(InventorySwapItemResponseType.ItemsNotFound));
        }

        [Test]
        public async Task SwapItemsBetweenInventoriesReturnsItemNotFoundIfItemWasNotInInventory()
        {
            var itemToSwapSlotId = 2;
            var itemFromSwapSlotId = 5;
            await _swapToInventoryContainer.AddInventoryItemAsync(_itemToSwap, itemToSwapSlotId, _inventoryDatabaseService);

            var resposne = await _inventoryTransferService.SwapItemAsync(_swapFromInventoryContainer, _itemFromSwap, itemFromSwapSlotId, _itemToSwap, itemToSwapSlotId,
                _swapToInventoryContainer);

            Assert.That(resposne.Type, Is.EqualTo(InventorySwapItemResponseType.ItemsNotFound));
        }

        [Test]
        public async Task SwapItemBetweenInventoriesSavesChangesToDatabase()
        {
            var itemToSwapSlotId = 2;
            var itemFromSwapSlotId = 5;
            await _swapToInventoryContainer.AddInventoryItemAsync(_itemToSwap, itemToSwapSlotId, _inventoryDatabaseService);
            await _swapFromInventoryContainer.AddInventoryItemAsync(_itemFromSwap, itemFromSwapSlotId, _inventoryDatabaseService);

            await _inventoryTransferService.SwapItemAsync(_swapFromInventoryContainer, _itemFromSwap, itemFromSwapSlotId, _itemToSwap, itemToSwapSlotId,
                _swapToInventoryContainer);

            var isSwapToInventoryUpdated = (await _mockFactory.Object.Invoke().Inventories.FindAsync(_swapToInventoryContainer.Id)).HasItem(_itemFromSwap);
            var isSwapFromInventoryUpdated = (await _mockFactory.Object.Invoke().Inventories.FindAsync(_swapFromInventoryContainer.Id)).HasItem(_itemToSwap);

            Assert.That(isSwapFromInventoryUpdated, Is.True);
            Assert.That(isSwapToInventoryUpdated, Is.True);
        }
    }
}
