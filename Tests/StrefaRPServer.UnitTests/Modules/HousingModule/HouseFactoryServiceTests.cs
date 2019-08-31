
using AltV.Net.Data;
using AltVStrefaRPServer.Services.Housing.Factories;
using NUnit.Framework;

namespace StrefaRPServer.UnitTests.Modules.HousingModule
{
    public class HouseFactoryServiceTests
    {
        private Position _housePosition = new Position(1f, 1f, 1f);
        private HouseFactoryService _houseFactoryService;
        
        [SetUp]
        public void Setup()
        {
            _houseFactoryService = new HouseFactoryService();
        }

        [Test]
        public void CreatesHouseAtCorrectPosition()
        {
            var newHouse = _houseFactoryService.CreateNewHouse(_housePosition, 10);

            var areEqual = _housePosition == newHouse.GetPosition();
            
            Assert.That(areEqual, Is.True);
        }

        [Test]
        public void CreatesHouseWithCorrectPrice()
        {
            var housePrice = 1000;
            var newHouse = _houseFactoryService.CreateNewHouse(_housePosition, housePrice);
            
            Assert.That(newHouse.Price, Is.EqualTo(housePrice));            
        }

        [Test]
        public void CreatesHouseWithFlat()
        {
            var newHouse = _houseFactoryService.CreateNewHouse(_housePosition, 10);
            
            Assert.That(newHouse.Flat, Is.Not.Null);
        }
        
        [Test]
        public void NewFlatIsCreatedWithLockedStatus()
        {
            var newFlat = _houseFactoryService.CreateNewFlat();
            
            Assert.That(newFlat.IsLocked, Is.True);
        }

        [Test]
        public void NewFlatIsCreatedWithRandomLockPattern()
        {
            var newFlat = _houseFactoryService.CreateNewFlat();

            Assert.That(newFlat.LockPattern, Is.Not.Null);
        }

        [Test]
        public void CreatesNewHotelRoomLocked()
        {
            var newHotel = _houseFactoryService.CreateNewHotelRoom(1);
            
            Assert.That(newHotel.IsLocked, Is.True);
        }

        [Test]
        public void CreatesNewHotelRoomWithCorrectRoomNumber()
        {
            var roomNumber = 321;
            var newHotel = _houseFactoryService.CreateNewHotelRoom(roomNumber);

            Assert.That(newHotel.HotelRoomNumber, Is.EqualTo(roomNumber));
        }
    }
}