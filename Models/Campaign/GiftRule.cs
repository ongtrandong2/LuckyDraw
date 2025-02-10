namespace PGAdmin.Models.Campaign
{
    public class GiftRule
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SpinCount { get; set; }
        public int Type { get; set; }
        public double? MinAmount { get; set; }
        public double? MaxAmount { get; set; }
        public int CampaignId { get; set; }
        public Campaign Campaign { get; set; }

        public ICollection<GiftCondition> GiftConditions { get; set; }
    }
}
