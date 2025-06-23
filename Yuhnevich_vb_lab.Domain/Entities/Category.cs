using System.Text.Json.Serialization;

namespace Yuhnevich_vb_lab.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
        [JsonIgnore]
        public List<Dish> Dishes { get; set; } = new List<Dish>();
    }
}