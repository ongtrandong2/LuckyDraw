namespace PGAdmin.Models.Campaign
{
    public class GiftCondition
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        // Foreign key
        public int GiftRuleId { get; set; }
        public GiftRule GiftRule { get; set; }

        public ICollection<GiftProductRequirement> GiftProducts { get; set; }
    }
}
