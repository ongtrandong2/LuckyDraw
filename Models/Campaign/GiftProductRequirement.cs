namespace PGAdmin.Models.Campaign
{
    public class GiftProductRequirement
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        // Foreign key
        public int GiftConditionId { get; set; }
        public GiftCondition GiftCondition { get; set; }
    }
}
