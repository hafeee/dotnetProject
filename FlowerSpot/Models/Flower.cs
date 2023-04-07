using System.ComponentModel.DataAnnotations;

namespace FlowerSpot.Models
{
    public class Flower
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        public string ImageRef { get; set; }
        public string Description { get; set; }

    }
}