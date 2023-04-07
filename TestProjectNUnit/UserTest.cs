using DbExploration.Data;
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
    public class UserServiceTests
    {
        private UserService _userService;
        private FlowerDbContext _dbContext;

        [SetUp]
        public void Setup()
        {
            // create mock DbContext and UserService
            var options = new DbContextOptionsBuilder<FlowerDbContext>()
        .UseInMemoryDatabase(databaseName: "test")
            .Options;

            _dbContext = new FlowerDbContext(options);
            _userService = new UserService(_dbContext);
        }

        [Test]
        public async Task GetUserAsync_ReturnsUser()
        {
            // Arrange
            var user = new User { Username = "John", Password="Pass", Email="email@email.com" };
            _userService.CreateUserAsync(user);
         

            // Act
            var result = await _userService.GetUserByIdAsync(1);
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(user.Id, result.Id);
            Assert.AreNotEqual("Pass", result.Password);
            Assert.AreEqual(user.Username, result.Username);
        }
    }
}
