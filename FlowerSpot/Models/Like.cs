using System.ComponentModel.DataAnnotations;

namespace FlowerSpot.Models
{
    public class Like
    {
        [Key]
        public int Id { get; set; }

        public User User { get; set; }

        public Sighting Sighting { get; set; }
    }
}
