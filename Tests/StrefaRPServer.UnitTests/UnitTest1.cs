using System;
using AltV.Net.Elements.Entities;
using AltV.Net.Elements.Factories;
using NUnit.Framework;
using AltV.Net.Mock;

namespace StrefaRPServer.UnitTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            var mock = new MockBaseEntityPool(new MockPlayerPool(new PlayerFactory()), new MockVehiclePool(new VehicleFactory()));
            mock.GetOrCreate(IntPtr.Zero, BaseObjectType.Player, out IEntity player);
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}