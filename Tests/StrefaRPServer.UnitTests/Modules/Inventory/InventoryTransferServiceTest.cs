using System.Linq;
using AltVStrefaRPServer.Models.Inventory;
using AltVStrefaRPServer.Services.Inventories;
using NUnit.Framework;
using StrefaRPServer.UnitTests.Core;
using System.Threading.Tasks;
using AltVStrefaRPServer.Models.Inventory.Responses;

namespace StrefaRPServer.UnitTests.Modules.Inventory
{
    public class InventoryTransferServiceTest : ServerContextTestBase
    {
        private readonly int _inventoriesSlotsCount = 10;
        private ItemFactory _itemFactory;
        private InventoryDatabaseService _inventoryDatabaseService;
        private InventoryTransferService _inventoryTransferService;
        private InventoryContainer _inventoryContainer;
        private InventoryItem _item;

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

            _inventoryContainer = new InventoryContainer(_inventoriesSlotsCount);

            _item = new InventoryItem(_itemFactory.CreateBurger(), 1, 0);
        }

        [Test]
        public async Task ItemTransferReturnsItemTransferedResponse()
        {
            var receiverInventory = new InventoryContainer(_inventoriesSlotsCount);
            await _inventoryContainer.AddInventoryItemAsync(_item, _inventoryDatabaseService);

            var response = await _inventoryTransferService.TransferItemAsync(_inventoryContainer, receiverInventory, _item, 1);

            Assert.That(response, Is.EqualTo(InventoryTransferItemResponse.ItemTransfered));
        }

        [Test]
        public async Task ItemTransferRemovesItemFromSourceInventory()
        {
            var receiverInventory = new InventoryContainer(_inventoriesSlotsCount);
            await _inventoryContainer.AddInventoryItemAsync(_item, _inventoryDatabaseService);

            await _inventoryTransferService.TransferItemAsync(_inventoryContainer, receiverInventory, _item, 1);

            Assert.That(_inventoryContainer.HasItem(_item), Is.False);
        }

        [Test]
        public async Task ItemTransferAddsItemToReceiverInventory()
        {
            var receiverInventory = new InventoryContainer(_inventoriesSlotsCount);
            await _inventoryContainer.AddInventoryItemAsync(_item, _inventoryDatabaseService);

            await _inventoryTransferService.TransferItemAsync(_inventoryContainer, receiverInventory, _item, 1);

            Assert.That(receiverInventory.HasItem(_item), Is.True);
        }

        [Test]
        public async Task ItemTransferMovesItemToCorrectSlot()
        {
            var newSlot = 5;
            var receiverInventory = new InventoryContainer(_inventoriesSlotsCount);
            await _inventoryContainer.AddInventoryItemAsync(_item, _inventoryDatabaseService);

            await _inventoryTransferService.TransferItemAsync(_inventoryContainer, receiverInventory, _item, newSlot);

            Assert.That(_item.SlotId, Is.EqualTo(newSlot));
        }

        [Test]
        public async Task ItemTransferChangesItemInventoryId()
        {
            var receiverInventory = new InventoryContainer(_inventoriesSlotsCount);
            await _inventoryContainer.AddInventoryItemAsync(_item, _inventoryDatabaseService);

            await _inventoryTransferService.TransferItemAsync(_inventoryContainer, receiverInventory, _item, 1);

            Assert.That(_item.InventoryId, Is.EqualTo(receiverInventory.Id));
        }

        [Test]
        public async Task ItemTransferSavesChangesToDatabase()
        {
            var receiverInventory = new InventoryContainer(_inventoriesSlotsCount);
            await _inventoryContainer.AddInventoryItemAsync(_item, _inventoryDatabaseService);

            await _inventoryTransferService.TransferItemAsync(_inventoryContainer, receiverInventory, _item, 1);

            var isItemInDatabase = (await _mockFactory.Object.Invoke().Inventories.FindAsync(receiverInventory.Id)).Items.Contains(_item);
            Assert.That(isItemInDatabase, Is.True);
        }
    }
}
