using System.Text.Json.Serialization;

namespace PGAdmin.Models
{
    public class ProductCategory
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string? Note { get; set; }

        [JsonIgnore]
        public ICollection<Product> Products { get; set; }
    }
}
