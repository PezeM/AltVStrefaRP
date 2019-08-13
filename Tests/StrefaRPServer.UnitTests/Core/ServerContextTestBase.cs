using System;
using AltVStrefaRPServer.Database;
using Moq;
using NUnit.Framework;

namespace StrefaRPServer.UnitTests.Core
{
    [TestFixture]
    public abstract class ServerContextTestBase
    {
        protected MockServerContext _context;
        protected Mock<Func<MockServerContext>> _mockFactory;

        [SetUp]
        public void Setup()
        {
            _context = new MockServerContext();
            _context.Database.EnsureCreated();

            _mockFactory = new Mock<Func<MockServerContext>>();
            _mockFactory.Setup(f => f()).Returns(_context);
        }
    }
}
