using static System.Net.Mime.MediaTypeNames;

namespace FlowerSpot.Models
{
    public class Sighting
    {
        public int Id { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public User User { get; set; }
        public Flower Flower { get; set; }
        public String Image { get; set; }
        public String MotivationalQuote { get; set; }
    }
}
