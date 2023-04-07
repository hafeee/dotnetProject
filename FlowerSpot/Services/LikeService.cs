using DbExploration.Data;
using FlowerSpot.Models;
using Microsoft.EntityFrameworkCore;

namespace FlowerSpot.Services
{
    public class LikeService : ILikeService
    {
        private readonly FlowerDbContext _dbContext;

        public LikeService(FlowerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Like> CreateLikeAsync(int sightingId, int userId)
        {
            var sighting = await _dbContext.Sightings.FindAsync(sightingId);
            if (sighting == null)
            {
                throw new ArgumentException("Sighting does not exist", nameof(sightingId));
            }

            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                throw new ArgumentException("User does not exist", nameof(userId));
            }

            var existingLike = await _dbContext.Likes.FirstOrDefaultAsync(l => l.Sighting.Id == sightingId && l.User.Id == userId);
            if (existingLike != null)
            {
                throw new InvalidOperationException("Like already exists for this sighting and user");
            }

            var like = new Like
            {
                Sighting = sighting,
                User = user
            };

            _dbContext.Likes.Add(like);
            await _dbContext.SaveChangesAsync();

            return like;
        
        }

        public async Task<Like> GetLikeAsync(int sightingId, int userId)
        {
            var like = await _dbContext.Likes
                .SingleOrDefaultAsync(l => l.Sighting.Id == sightingId && l.User.Id == userId);

            return like;
        }

        public async Task DeleteLikeAsync(int likeId, int userId)
        {
            var like = await _dbContext.Likes.SingleOrDefaultAsync(l => l.Id == likeId && l.User.Id == userId);
            if (like == null)
            {
                throw new KeyNotFoundException($"Like with id {likeId} and user id {userId} not found.");
            }

            _dbContext.Likes.Remove(like);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Like> GetLikeAsync(int likeId)
        {
            return await _dbContext.Likes.FindAsync(likeId);
        }

        public async Task<IEnumerable<Like>> GetLikesBySightingIdAsync(int sightingId)
        {
            var likes = await _dbContext.Likes.Where(l => l.Sighting.Id == sightingId).ToListAsync();
            return likes;
        }
        public async Task<Like> UpdateLikeAsync(Like like)
        {
            var existingLike = await _dbContext.Likes.FindAsync(like.Id);

            if (existingLike == null)
            {
                throw new ArgumentException($"Like with id {like.Id} not found");
            }

            existingLike.User = like.User;
            existingLike.Sighting = like.Sighting;

            await _dbContext.SaveChangesAsync();

            return existingLike;
        }
    }
}
