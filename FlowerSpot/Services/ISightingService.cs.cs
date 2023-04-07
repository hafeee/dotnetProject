using FlowerSpot.Models;

namespace FlowerSpot.Services
{
    public interface ISightingService
    {
        Task<Sighting> GetSightingAsync(int sightingId);
        Task<IEnumerable<Sighting>> GetAllSightingsAsync();
        Task<Sighting> CreateSightingAsync(Sighting sighting);
        Task DeleteSightingAsync(int sightingId, int userId);
        Task<Sighting> UpdateSightingAsync( Sighting sighting);
    }
}
