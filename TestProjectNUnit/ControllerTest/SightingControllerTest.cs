using DbExploration.Data;
using FlowerSpot.Controllers;
using FlowerSpot.Models;
using FlowerSpot.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace TestProjectNUnit.ControllerTest
{
    [TestFixture]
    public class SightingControllerTests
    {
        private ISightingService _sightingService;
        private IUserService _userService;
        private FlowerDbContext _dbContext;
        private WebApplicationFactory<Program> _application;
        private HttpClient _client;
        private IFlowerService _flowerService;
        Sighting s1;
        User u1;

        [OneTimeSetUp]
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

            Flower f1 = new Flower { ImageRef = "link.to.image.com", Description = "Flower description", Name = "Cool flower" };
            u1 = new User { Username = "John", Email = "john@example.com", Password = "Password123" };
            s1 = new Sighting { Latitude = 12.2212, Flower = f1, Image = "link.to.image2.com", Longitude = 222.222, User = u1 };

            _flowerService.CreateFlowerAsync(f1);
            _userService.CreateUserAsync(u1);
            _sightingService.CreateSightingAsync(s1);



            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("John:Password123"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

        }



        [Test]
        public async Task GetAllSightingsTest()
        {
            var response = await _client.GetAsync("/sightings");
            var actualSightings = await response.Content.ReadAsAsync<List<Sighting>>();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.AreEqual(1, actualSightings.Count);
            Assert.AreEqual(s1.Image, actualSightings[0].Image);
        }

        [Test]
        public async Task GetOneSightingTest()
        {
            var response = await _client.GetAsync($"/sightings/{s1.Id}");
            var actualSightings = await response.Content.ReadAsAsync<Sighting>();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.AreEqual(s1.Image, actualSightings.Image);
        }


        [Test]
        public async Task DeleteSightingTest()
        {
            Flower f2 = new Flower { ImageRef = "link.to.image.com", Description = "Flower description 2", Name = "Cool flower" };
            Sighting s2 = new Sighting { Latitude = 12.2212, Flower = f2, Image = "link.to.image2.com", Longitude = 222.222, User = u1 };

            _flowerService.CreateFlowerAsync(f2);
            _sightingService.CreateSightingAsync(s2);
            
            var responses = await _client.GetAsync("/sightings");
            var actualSightings2 = await responses.Content.ReadAsAsync<List<Sighting>>();
            var response = await _client.DeleteAsync($"/sightings/{s2.Id}/{u1.Id}");
            var response_get = await _client.GetAsync($"/sightings/{s2.Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.AreEqual(HttpStatusCode.NotFound, response_get.StatusCode);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public async Task UpdateSightingTest()
        {

            var patchedSighting = new JObject();
            patchedSighting["longitude"] = 11.11;
            var content = new StringContent(patchedSighting.ToString(), Encoding.UTF8, "application/json");

            var response = await _client.PatchAsync($"/sightings/{s1.Id}/{s1.User.Id}", content);

            
            var response_get = await _client.GetAsync($"/sightings/{s1.Id}");
            var actualSighting = await response_get.Content.ReadAsAsync<Sighting>();

            // Assert
            Assert.AreEqual(actualSighting.Longitude, 11.11);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public async Task UpdateSightingInvalidTest()
        {
            
            _userService.CreateUserAsync(new User { Username = "John2", Email = "john2@example.com", Password = "Password123" });

            var patchedSighting = new JObject();
            patchedSighting["longitude"] = 11.11;

            var content = new StringContent(patchedSighting.ToString(), Encoding.UTF8, "application/json");

            var response = await _client.PatchAsync($"/sightings/{s1.Id}/{2}", content);


            var response_get = await _client.GetAsync($"/sightings/{s1.Id}");
            var actualSighting = await response_get.Content.ReadAsAsync<Sighting>();

            // Assert
            Assert.AreNotEqual(actualSighting.Longitude, 11.11);
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
        }
    }
}