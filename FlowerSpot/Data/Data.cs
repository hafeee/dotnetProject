using FlowerSpot.Models;
using Microsoft.EntityFrameworkCore;

namespace DbExploration.Data;


public class FlowerDbContext : DbContext
{
    public FlowerDbContext(DbContextOptions<FlowerDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Flower> Flowers { get; set; }
    public DbSet<Sighting> Sightings { get; set; }
    public DbSet<Like> Likes { get; set; }


}