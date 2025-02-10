using System.ComponentModel.DataAnnotations;

namespace PGAdmin.Models.Campaign
{
    public class CampaignGift
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public int Quantity { get; set; }

        [ConcurrencyCheck]
        public long Version { get; set; }
        public int RemainingQuantity { get; set; }
        public int CampaignId { get; set; }
        public Campaign Campaign { get; set; }
    }
}
