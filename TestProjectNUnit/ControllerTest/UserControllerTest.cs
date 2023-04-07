using DbExploration.Data;
using FlowerSpot.Models;
using FlowerSpot.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace TestProjectNUnit.ControllerTest
{
    [TestFixture]
    public class UserControllerTests
    {
        private IUserService _userService;
        private FlowerDbContext _dbContext;
        private WebApplicationFactory<Program> _application;
        private HttpClient _client;

        [SetUp]
        public void Setup()
        {
            _application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
            });
            _client = _application.CreateClient();
            _dbContext = _application.Services.GetService<FlowerDbContext>();
            _userService = _application.Services.GetService<IUserService>();

        }
        [Test]
        public async Task GetAllCustomersTest()
        {
            await _userService.CreateUserAsync(new User { Username = "John2", Email = "john@example.com", Password = "Password123" });
            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("John2:Password123")); // We used credentials of an already used User
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            var response = await _client.GetAsync("/users");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public async Task RegisterUserTest()
        {
            // Arrange
            var user = new User { Username = "newuser3456", Password = "password123", Email = "newuser@example.com" };
            var json = JsonSerializer.Serialize(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/users/register", content);
            var data = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        }

        [Test]
        public async Task UpdateUserTest()
        {
            var user = new User { Username = "John2", Email = "john@example.com", Password = "Password123" };
            await _userService.CreateUserAsync(user);


            var patchedUser = new JObject();
            patchedUser["email"] = "newemail@example.com";

            var content = new StringContent(patchedUser.ToString(), Encoding.UTF8, "application/json");

            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("John2:Password123"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            var response = await _client.PatchAsync($"/users/{user.Id}", content);

            // We will getch it again for double checking
            var response2 = await _client.GetAsync($"/users/{user.Id}");

            response.EnsureSuccessStatusCode();
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            User updatedUser = await response.Content.ReadFromJsonAsync<User>();
            User updatedUserGet = await response2.Content.ReadFromJsonAsync<User>();

            Assert.AreEqual(patchedUser["email"].ToString(), updatedUser.Email.ToString());
            Assert.AreEqual(patchedUser["email"].ToString(), updatedUserGet.Email.ToString());
        }



        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
        }
    }

    }
