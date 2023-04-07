using DbExploration.Data;
using FlowerSpot.Models;
using FlowerSpot.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text.Json;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Newtonsoft.Json.Linq;

namespace TestProjectNUnit.ControllerTest
{
    internal class FlowerControllerTest
    {
        private ISightingService _sightingService;
        private IUserService _userService;
        private FlowerDbContext _dbContext;
        private WebApplicationFactory<Program> _application;
        private HttpClient _client;
        private IFlowerService _flowerService;

        [SetUp]
        public void Setup()
        {
            _application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddDbContext<FlowerDbContext>(options =>
                        options.UseInMemoryDatabase("TestDB"));
                });
                builder.UseEnvironment("Testing");
            });
            _client = _application.CreateClient();
            _dbContext = _application.Services.GetService<FlowerDbContext>();
            _sightingService = _application.Services.GetService<ISightingService>(); // new SightingService(_dbContext);
            _userService = _application.Services.GetService<IUserService>();
            _flowerService = _application.Services.GetService<IFlowerService>();
        }

        [Test]
        public async Task GetAllFlowersTest()
        {
            await _userService.CreateUserAsync(new User { Username = "John2", Email = "john@example.com", Password = "Password123" });
            await _flowerService.CreateFlowerAsync(new Flower { ImageRef = "link.to.image.com", Description = "Flower description", Name = "Cool flower" });

            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("John2:Password123")); // We used credentials of an already used User
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            var response = await _client.GetAsync("/flowers");
            var flowers = JsonConvert.DeserializeObject<List<Flower>>(await response.Content.ReadAsStringAsync());
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);


        }

        [Test]
        public async Task CreateNewFlowerTest()
        {
            await _userService.CreateUserAsync(new User { Username = "John2", Email = "john@example.com", Password = "Password123" });
            var f1 = new Flower { ImageRef = "link.to.image.com", Description = "Flower description", Name = "Cool flower" };
            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("John2:Password123")); // We used credentials of an already used User
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            var json = JsonSerializer.Serialize(f1);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/flowers", content);

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var response_get = await _client.GetAsync("/flowers");
            var flowers = JsonConvert.DeserializeObject<List<Flower>>(await response_get.Content.ReadAsStringAsync());
            Assert.AreEqual(HttpStatusCode.OK, response_get.StatusCode);
            Assert.AreEqual(1, flowers.Count);
            Assert.AreEqual(flowers[0].Name, f1.Name);

        }

        [Test]
        public async Task PatchFlowerTest()
        {
            await _userService.CreateUserAsync(new User { Username = "John2", Email = "john@example.com", Password = "Password123" });
            var f1 = new Flower { ImageRef = "link.to.image.com", Description = "Flower description", Name = "Cool flower" };
            await _flowerService.CreateFlowerAsync(f1);

            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("John2:Password123")); // We used credentials of an already used User
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            var patchedFlower = new JObject();
            patchedFlower["description"] = "New flower description";
            var content = new StringContent(patchedFlower.ToString(), Encoding.UTF8, "application/json");


            var response = await _client.PatchAsync($"/flowers/{f1.Id}", content);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var response_get = await _client.GetAsync($"/flowers/{f1.Id}");
            var flower_get = JsonConvert.DeserializeObject<Flower>(await response_get.Content.ReadAsStringAsync());
            Assert.AreEqual(HttpStatusCode.OK, response_get.StatusCode);
            Assert.AreEqual(patchedFlower["description"].ToString(), flower_get.Description.ToString());

        }


        [Test]
        public async Task DeleteFlowerTest()
        {
            await _userService.CreateUserAsync(new User { Username = "John2", Email = "john@example.com", Password = "Password123" });
            var f1 = new Flower { ImageRef = "link.to.image.com", Description = "Flower description", Name = "Cool flower" };
            await _flowerService.CreateFlowerAsync(f1);

            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("John2:Password123")); // We used credentials of an already used User
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
            var response = await _client.DeleteAsync($"/flowers/{f1.Id}");

            var response_get = await _client.GetAsync($"/flowers/{f1.Id}");
            Assert.AreEqual(HttpStatusCode.NoContent, response_get.StatusCode);
        }


        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
        }

    }
}
