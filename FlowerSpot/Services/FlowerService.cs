using DbExploration.Data;
using FlowerSpot.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Web.Http.ModelBinding;

namespace FlowerSpot.Services
{
    public class FlowerService : IFlowerService
    {
        private readonly FlowerDbContext _dbContext;

        public FlowerService(FlowerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Flower> CreateFlowerAsync(Flower flower)
        {
            try
            {
                _dbContext.Flowers.Add(flower);
                await _dbContext.SaveChangesAsync();
                return flower;
            }
            catch (Exception ex)
            {
                // We candle any exceptions here
                throw;
            }
        }

        public async Task<List<Flower>> GetFlowersAsync()
        {
            return await _dbContext.Flowers.ToListAsync();
        }

        public async Task<Flower> GetFlowerByIdAsync(int id)
        {
            return await _dbContext.Flowers.FindAsync(id);
        }

        public async Task<Flower> UpdateFlowerAsync(Flower newFlower)
        {
            var flower = await _dbContext.Flowers.FindAsync(newFlower.Id);

            if (flower == null)
            {
                return null;
            }

            _dbContext.Flowers.Update(newFlower);

            await _dbContext.SaveChangesAsync();

            return flower;
        }

        public async Task DeleteFlowerAsync(int id)
        {
            var flower = await _dbContext.Flowers.FindAsync(id);
            if (flower == null)
            {
                throw new Exception("Flower not found.");
            }
            _dbContext.Flowers.Remove(flower);
            await _dbContext.SaveChangesAsync();
        }
    }
}
