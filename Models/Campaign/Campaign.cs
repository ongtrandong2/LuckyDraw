using PGAdmin.Models.Reward;

namespace PGAdmin.Models.Campaign
{
    public enum SchemeType
    {
        MaxSpin = 1,  // (A) Nhận thưởng theo scheme có lượt quay cao nhất
        SumSpin = 2   // (B) Cộng tổng lượt thưởng của các scheme thoả mãn
    }
    public class Campaign
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ICollection<GiftRule> GiftRules { get; set; }
        public ICollection<RewardOrder> RewardOrders { get; set; }
        public ICollection<CampaignGift> CampaignGifts { get; set; }
        public bool PublicQrAccess { get; set; }
        public int NumberOfQrAccessScanPerUser { get; set; }
        public bool PGQrAccess { get; set; }
        public int NumberOfPGQrAccessScanPerUser { get; set; }

        public SchemeType Scheme { get; set; } = SchemeType.MaxSpin;
    }
}
