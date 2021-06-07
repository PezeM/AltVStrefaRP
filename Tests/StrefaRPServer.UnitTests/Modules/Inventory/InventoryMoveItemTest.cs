using AltVStrefaRPServer.Models.Inventory;
using AltVStrefaRPServer.Models.Inventory.Items;
using AltVStrefaRPServer.Models.Inventory.Responses;
using AltVStrefaRPServer.Services.Inventories;
using NUnit.Framework;

namespace StrefaRPServer.UnitTests.Modules.Inventory
{
    public class InventoryMoveItemTest
    {
        private const int InventorySlotsNumber = 10;
        private InventoryContainer _inventory;
        private ItemFactory _itemFactory;
        private InventoryItem _item;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _itemFactory = new ItemFactory();
        }

        [SetUp]
        public void Setup()
        {
            _inventory = new InventoryContainer(InventorySlotsNumber);
            _item = new InventoryItem(_itemFactory.CreateBurger(), 5, 0);    
        }

        [Test]
        [TestCase(5)]
        [TestCase(8)]
        [TestCase(3)]
        public void MovesItemToNewSlot(int newSlotId)
        {
            _inventory.AddInventoryItem(_item);

            var response = _inventory.MoveItemToSlot(_item, newSlotId);
            
            Assert.That(response, Is.EqualTo(InventoryMoveItemResponse.ItemMoved));
            Assert.That(_item.SlotId, Is.EqualTo(newSlotId));
        }

        [Test]
        public void MovesItemToNewSlotBySlotId()
        {
            var newSlotId = 2;
            _inventory.AddInventoryItem(_item);

            _inventory.MoveItemToSlot(_item.Id, newSlotId);
            
            Assert.That(_item.SlotId, Is.EqualTo(newSlotId));
        }

        [Test]
        public void WontMoveItemThatIsNotInInventory()
        {
            int newSlotId = 2;

            var response = _inventory.MoveItemToSlot(_item.Id, newSlotId);

            Assert.That(response, Is.EqualTo(InventoryMoveItemResponse.ItemNotFound));
        }

        [Test]
        [TestCase(0, 1)]
        [TestCase(5, 6)]
        public void WontMoveItemIfSlotIsOccupied(int firstItemSlot, int secondItemSlot)
        {
            var secondItem = new InventoryItem(_itemFactory.CreateCombatPistol(), 1, secondItemSlot);
            _inventory.AddInventoryItem(_item, firstItemSlot);
            _inventory.AddInventoryItem(secondItem, secondItemSlot);

            var response = _inventory.MoveItemToSlot(_item, secondItemSlot);

            Assert.That(response, Is.EqualTo(InventoryMoveItemResponse.SlotOccupied));
            Assert.That(_item.SlotId, Is.Not.EqualTo(secondItemSlot));
        }

        [Test]
        public void CantMoveItemToSlotIdHigherThanMaxSlotsNumber()
        {
            _inventory.AddInventoryItem(_item);
            var outOfRangeSlotNumber = InventorySlotsNumber + 1;

            _inventory.MoveItemToSlot(_item, outOfRangeSlotNumber);

            Assert.That(_item.SlotId, Is.Not.EqualTo(outOfRangeSlotNumber));
        }
    }
}
