namespace PGAdmin.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int TypeId { get; set; }
        public ProductType Type { get; set; }
        public int CategoryId { get; set; }
        public ProductCategory Category { get; set; }
        public double UnitPrice { get; set; }
    }
}
