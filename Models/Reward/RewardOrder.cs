
namespace PGAdmin.Models.Reward
{
    public class RewardOrder
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CustomerZaloId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public string? Signature { get; set; }
        public int? PgId { get; set; } // Foreign key to PG entity (assuming PG is another entity)
        public PG Pg { get; set; } // Navigation property for PG
        public int SpintCount { get; set; }

        public int CampaignId { get; set; }  // Foreign key to the Campaign entity
        public Campaign.Campaign Campaign { get; set; }

        // Collection navigation properties
        public ICollection<RewardOrderProduct> Products { get; set; }
        public ICollection<RewardOrderDetail> Details { get; set; }
    }
}
