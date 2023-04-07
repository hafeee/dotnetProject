using DbExploration.Data;
using FlowerSpot.Models;
using Microsoft.Azure.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;

namespace FlowerSpot.Services
{

    public class SightingService : ISightingService
    {
        private readonly FlowerDbContext _dbContext;

        public SightingService(FlowerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Sighting> GetSightingAsync(int sightingId)
        {
            return await _dbContext.Sightings.Include(s => s.User).Include(s => s.Flower).FirstOrDefaultAsync(s => s.Id == sightingId);
        }

        public async Task<IEnumerable<Sighting>> GetAllSightingsAsync()
        {
            return await _dbContext.Sightings
                .Include(s => s.User)
                .Include(s => s.Flower)
                .ToListAsync();
        }

        public async Task<Sighting> CreateSightingAsync(Sighting sighting)
        {
            _dbContext.Sightings.Add(sighting);

            // Make HTTP request to get quote of the day
            using (var httpClient = new HttpClient())
            {
                try
                {
                    /*var response = await httpClient.GetAsync("https://quotes.rest/qod");*/
                    Uri siteUri = new Uri("https://zenquotes.io/api/today");
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, siteUri);
                    HttpResponseMessage response = httpClient.Send(request);
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var jArray = JArray.Parse(json);
                        var quoteResponse = jArray[0]["q"].ToString();
                        sighting.MotivationalQuote = quoteResponse;
                    }
                    else
                    {
                        sighting.MotivationalQuote = "You can do it Fam!";
                        // We should have some kind of error handling here, either not return sighting at all, or just add a our random quote and append that to sighting
                    }
                }
                catch(Exception er)
                {
                    // We will have this here in case of wronf URL, or the service is down or 
                    sighting.MotivationalQuote = "You can do it Fam!";
                }
            }

            await _dbContext.SaveChangesAsync();
            return sighting;
        }

        public async Task DeleteSightingAsync(int sightingId, int userId)
        {
            var sighting = await _dbContext.Sightings.FindAsync(sightingId);
            if (sighting == null)
            {
                throw new KeyNotFoundException("Sighting not found.");
            }
            if (sighting.User.Id != userId)
            {
                throw new Exception("User is not authorized to delete this sighting.");
            }
            _dbContext.Sightings.Remove(sighting);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Sighting> UpdateSightingAsync( Sighting sighting)
        {
            // Get the existing sighting from the database
            var existingSighting = await _dbContext.Sightings.FindAsync(sighting.Id);
            if (existingSighting == null)
            {
                throw new NotFoundException("Sighting not found");
            }

            // Check if the user who created the sighting is updating it
            if (existingSighting.User.Id != sighting.User.Id)
            {
                throw new UnauthorizedException("Only the user who created the sighting can update it");
            }

            // Update the existing sighting with the new values
            existingSighting.Latitude = sighting.Latitude;
            existingSighting.Longitude = sighting.Longitude;
            existingSighting.Image = sighting.Image;
            existingSighting.Flower = sighting.Flower;


            // Save changes to the database
            await _dbContext.SaveChangesAsync();

            return existingSighting;
        }



    }

}
