using FlowerSpot.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace FlowerSpot.Services
{
    public interface IFlowerService
    {
        Task<List<Flower>> GetFlowersAsync();
        Task<Flower> CreateFlowerAsync(Flower flower);
        Task<Flower> UpdateFlowerAsync(Flower Flower);
        Task DeleteFlowerAsync(int id);
        Task<Flower> GetFlowerByIdAsync(int id);

    }
}
