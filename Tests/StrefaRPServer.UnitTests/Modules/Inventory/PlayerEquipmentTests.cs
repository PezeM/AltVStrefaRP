using System;
using System.Linq;
using AltV.Net.Mock;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Inventory;
using AltVStrefaRPServer.Services.Inventories;
using NUnit.Framework;

namespace StrefaRPServer.UnitTests.Modules.Inventory
{
    [TestFixture]
    public class PlayerEquipmentTests
    {
        private PlayerEquipment _playerEquipment;
        private ItemFactory _itemFactory;

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
        }

        [Test]
        public void CanEquipEquipmentableItem()
        {
            var combatPistol = _itemFactory.CreateCombatPistol();
            var equipmentableItem = new InventoryItem(combatPistol, 1, 1);

            _playerEquipment.EquipItem(equipmentableItem);

            Assert.That(_playerEquipment.Items.Contains(equipmentableItem));
        }
    }
}
