using FlowerSpot.Models;

namespace FlowerSpot.Services
{
    public interface ILikeService
    {
        Task<Like> GetLikeAsync(int likeId);
        Task<IEnumerable<Like>> GetLikesBySightingIdAsync(int sightingId);
        Task<Like> CreateLikeAsync(int sightingId, int userId);
        Task DeleteLikeAsync(int likeId, int userId);
        Task<Like> UpdateLikeAsync(Like like);
        Task<Like> GetLikeAsync(int sightingId, int userId);
    }
}
