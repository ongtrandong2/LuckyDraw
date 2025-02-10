namespace PGAdmin.Models.Reward
{
    public class RewardOrderDetail
    {
        public int Id { get; set; }
        public string GiftName { get; set; }
        public int GiftId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ReceivingAt { get; set; }
        public int Status { get; set; }

        // Foreign key property
        public int RewardOrderId { get; set; }
        public RewardOrder RewardOrder { get; set; }
    }
}
