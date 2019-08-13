using System;
using System.Linq;
using AltV.Net.Mock;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Inventory;
using AltVStrefaRPServer.Models.Inventory.Items;
using AltVStrefaRPServer.Models.Inventory.Responses;
using AltVStrefaRPServer.Services.Inventories;
using NUnit.Framework;

namespace StrefaRPServer.UnitTests.Modules.Inventory
{
    [TestFixture]
    public class PlayerEquipmentTests
    {
        private PlayerEquipment _playerEquipment;
        private ItemFactory _itemFactory;
        private Equipmentable _equipmentableItem;
        private InventoryItem _itemToEquip;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _itemFactory = new ItemFactory();
        }

        [SetUp]
        public void Setup()
        {
            var mockPlayer = new MockPlayer(IntPtr.Zero, 0);
            var character = new Character
            {
                Player = mockPlayer
            };

            _playerEquipment = new PlayerEquipment
            {
                Owner = character
            };

            _equipmentableItem = _itemFactory.CreateCombatPistol();
            _itemToEquip = new InventoryItem(_equipmentableItem, 1, 1);
        }

        [Test]
        public void CanEquipEquipmentableItem()
        {
            var response = _playerEquipment.EquipItem(_itemToEquip);

            Assert.That(response, Is.EqualTo(InventoryEquipItemResponse.ItemEquipped));
        }

        [Test]
        public void EquippedItemsAreAddedToListOfItems()
        {
            _playerEquipment.EquipItem(_itemToEquip);

            Assert.That(_playerEquipment.Items.Contains(_itemToEquip));
        }

        [Test]
        public void EquippedItemsAreAddedToDictionaryOfItems()
        {
            _playerEquipment.EquipItem(_itemToEquip);

            Assert.That(_playerEquipment.EquippedItems.ContainsValue(_itemToEquip));
            Assert.That(_playerEquipment.EquippedItems.ContainsKey((EquipmentSlot)_itemToEquip.SlotId));
        }

        [Test]
        public void EquippedItemsArePutOnCorrectSlot()
        {
            _playerEquipment.EquipItem(_itemToEquip);

            Assert.That(_itemToEquip.SlotId, Is.EqualTo((int)_equipmentableItem.EquipmentSlot));
        }

        [Test]
        public void CantEquipItemIfItemIsAlreadyEquippedAtTheSameSlot()
        {
            var secondItem = new InventoryItem(_itemFactory.CreateCombatPistol(), 1, 1);

            _playerEquipment.EquipItem(_itemToEquip);
            var response = _playerEquipment.EquipItem(secondItem);


            Assert.That(response, Is.EqualTo(InventoryEquipItemResponse.ItemAlreadyEquippedAtThatSlot));
            Assert.That(_playerEquipment.Items.Contains(secondItem), Is.False);
        }

        [Test]
        public void CanUnequipEquipedItem()
        {
            _playerEquipment.EquipItem(_itemToEquip);

            var response = _playerEquipment.UnequipItem(_itemToEquip);

            Assert.That(response, Is.EqualTo(InventoryUnequipItemResponse.ItemUnequipped));
        }

        [Test]
        public void UnequippingItemsRemovesItemFromInventory()
        {
            _playerEquipment.EquipItem(_itemToEquip);

            _playerEquipment.UnequipItem(_itemToEquip);

            Assert.That(_playerEquipment.Items.Contains(_itemToEquip), Is.False);
        }

        [Test]
        public void UnequippingItemsRemovesItemFromDictionary()
        {
            _playerEquipment.EquipItem(_itemToEquip);

            _playerEquipment.UnequipItem(_itemToEquip);

            Assert.That(_playerEquipment.EquippedItems.ContainsValue(_itemToEquip), Is.False);
            Assert.That(_playerEquipment.EquippedItems.ContainsKey((EquipmentSlot)_itemToEquip.SlotId), Is.False);
        }

        [Test]
        public void WontUnequipItemsThatAreNotEquiped()
        {
            var response = _playerEquipment.UnequipItem(_itemToEquip);

            Assert.That(response, Is.EqualTo(InventoryUnequipItemResponse.NoItemAtThatSlot));
        }

        [Test]
        public void UnequipsItemAtGivenSlot()
        {
            _playerEquipment.EquipItem(_itemToEquip);
            var itemSlot = _equipmentableItem.EquipmentSlot;

            var response = _playerEquipment.UnequipItem(itemSlot);

            Assert.That(response, Is.EqualTo(InventoryUnequipItemResponse.ItemUnequipped));
        }
    }
}
