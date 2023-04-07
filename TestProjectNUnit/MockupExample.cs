/*
 I left this here for demonstration purposes how we can go step further and use Mockups in testing
 */


/*using DbExploration.Data;
using FlowerSpot.Models;
using FlowerSpot.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProjectNUnit
{
    [TestFixture]
    public class SightingServiceTests
    {
        private Mock<DbSet<Sighting>> _sightingsDbSetMock;
        private Mock<FlowerDbContext> _dbContextMock;
        private ISightingService _sightingService;

        [SetUp]
        public void Setup()
        {
            _sightingsDbSetMock = new Mock<DbSet<Sighting>>();
            _dbContextMock = new Mock<FlowerDbContext>();
            _dbContextMock.Setup(x => x.Sightings).Returns(_sightingsDbSetMock.Object);
            _sightingService = new SightingService(_dbContextMock.Object);
        }

        [Test]
        public async Task GetSightingsAsync_ShouldReturnAllSightings()
        {
            User user1 = new User { Username = "test_username", Email = "test@username.com", Password = "Password" };
            Flower flower1 = new Flower { Name = "Flowe name", ImageRef = "link.to.image.com", Description = "DESC" };
            // Arrange
            var expectedSightings = new List<Sighting>
            {
                new Sighting { Id = 1, Longitude = 1.0, Latitude = 2.0, User = user1, Flower = flower1, Image = "image1.jpg" },
                new Sighting { Id = 2, Longitude = 3.0, Latitude = 4.0, User = user1, Flower = flower1, Image = "image2.jpg" }
            };
            _sightingsDbSetMock.As<IAsyncEnumerable<Sighting>>().Setup(x => x.GetAsyncEnumerator(default)).Returns(new TestAsyncEnumerator<Sighting>(expectedSightings.GetEnumerator()));
            
            // Act
            var result = await _sightingService.GetAllSightingsAsync();

            // Assert
            Assert.AreEqual(expectedSightings.Count, result.Count());
            Assert.IsTrue(result.All(x => expectedSightings.Contains(x)));
        }
    }

}
*/