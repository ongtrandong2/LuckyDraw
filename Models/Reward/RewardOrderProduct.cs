namespace PGAdmin.Models.Reward
{
    public class RewardOrderProduct
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public int RewardOrderId { get; set; }
        public RewardOrder RewardOrder { get; set; }
    }
}
