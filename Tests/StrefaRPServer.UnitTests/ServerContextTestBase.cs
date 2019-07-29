using System;
using AltVStrefaRPServer.Database;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace StrefaRPServer.UnitTests
{
    [TestFixture]
    public abstract class ServerContextTestBase
    {
        protected ServerContext _context;
        protected Mock<Func<ServerContext>> _mockFactory;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ServerContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ServerContext(options);
            _context.Database.EnsureCreated();

            _mockFactory = new Mock<Func<ServerContext>>();
            _mockFactory.Setup(f => f()).Returns(_context);
        }
    }
}
