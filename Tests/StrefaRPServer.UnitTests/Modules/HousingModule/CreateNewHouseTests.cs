using System.Threading.Tasks;
using AltV.Net.Data;
using AltVStrefaRPServer.Models.Houses;
using AltVStrefaRPServer.Models.Interfaces.Managers;
using AltVStrefaRPServer.Modules.HousingModule;
using AltVStrefaRPServer.Services.Housing;
using AltVStrefaRPServer.Services.Housing.Factories;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using StrefaRPServer.UnitTests.Core;

namespace StrefaRPServer.UnitTests.Modules.HousingModule
{
    public class CreateNewHouseTests : ServerContextTestBase
    {
        private int _interiorId = 2;
        private Mock<IInteriorsManager> _interiorsManagerMock;
        private Mock<IInterior> _interiorMock;
        private Interior _interior;
        private HousesManager _housesManager;
        private HouseDatabaseService _houseDatabaseService;
        private HouseFactoryService _houseFactoryService;
        private readonly Position _testPosition = new Position(1, 2, 3);
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _interiorsManagerMock = new Mock<IInteriorsManager>();
            _houseFactoryService = new HouseFactoryService();
            _interiorMock = new Mock<IInterior>();
            
            _interiorsManagerMock.Setup(i => i.TryGetInterior(_interiorId, out _interior)).Returns(true);
        }

        [SetUp]
        public void SetUp()
        {
            _houseDatabaseService = new HouseDatabaseService(_mockFactory.Object);

            _housesManager = new HousesManager(_houseDatabaseService, _houseFactoryService,
                _interiorsManagerMock.Object, _mockFactory.Object, new Mock<ILogger<HousesManager>>().Object);
        }
        
        [Test]
        public async Task CantCreateHouseWithInteriorIdBelowOneAsync()
        {
            var interiorId = 0;
            var response = await _housesManager.AddNewHouseAsync(_testPosition, 10, interiorId);
            
            Assert.That(response, Is.EqualTo(AddNewHouseResponse.WrongInteriorId));
        }

        [Test]
        public async Task CantCreateHouseWithWrongInteriorIdAsync()
        {
            var response = await _housesManager.AddNewHouseAsync(_testPosition, 10, _interiorId - 1);
            
            Assert.That(response, Is.EqualTo(AddNewHouseResponse.InteriorNotFound));
        }
    }
}