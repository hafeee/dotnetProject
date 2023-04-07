using DbExploration.Data;
using FlowerSpot.Models;
using FlowerSpot.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace TestProjectNUnit.ControllerTest
{
    internal class LikeControllerTest
    {

        private ILikeService _likeService;

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
            /*_likeService.CreateLikeAsync(s1.Id, u1.Id);*/


            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("John:Password123"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

        }

        [Test]
        public async Task CreateNewLikeTest()
        {

            var response = await _client.PostAsync($"/likes/sightings/{s1.Id}", new StringContent(""));

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var like = JsonConvert.DeserializeObject<Like>(await response.Content.ReadAsStringAsync());
            Assert.AreEqual(1, like.Sighting.Id);
            Assert.AreEqual(1, like.User.Id);

        }

        [Test]
        public async Task DeleteLikeTest()
        {

            var response = await _client.PostAsync($"/likes/sightings/{s1.Id}", new StringContent(""));

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var response_del = await _client.DeleteAsync($"/likes/sightings/{s1.Id}");
            Assert.AreEqual(HttpStatusCode.OK, response_del.StatusCode);

        }

        [Test]
        public async Task GetLikesTest()
        {
            _userService.CreateUserAsync(new User { Username = "John2", Email = "john@example.com", Password = "Password123" });


            await _client.PostAsync($"/likes/sightings/{s1.Id}", new StringContent(""));

            // We switch the user to add additional like
            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("John2:Password123"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            await _client.PostAsync($"/likes/sightings/{s1.Id}", new StringContent(""));

            await _client.GetAsync($"/likes/sightings/{s1.Id}");

            var response_get = await _client.GetAsync($"/likes/sightings/{s1.Id}");
            var likes = JsonConvert.DeserializeObject<List<Like>>(await response_get.Content.ReadAsStringAsync());

            Assert.AreEqual(HttpStatusCode.OK, response_get.StatusCode);
            Assert.AreEqual(likes.Count, 2);
        }

    }
}
