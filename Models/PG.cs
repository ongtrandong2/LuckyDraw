using PGAdmin.Models.Location;
using PGAdmin.Models.Reward;

namespace PGAdmin.Models
{
    public class PG
    {
        public int Id { get; set; }             // Unique identifier (primary key)

        public string QrCode { get; set; }      // QR Code (string)

        public string FirstName { get; set; }        // Name of the PG (Tên PG)
        public string LastName { get; set; }        // Name of the PG (Tên PG)

        public string? Phone { get; set; }        // Name of the PG (Tên PG)

        public string? Email { get; set; }        // Name of the PG (Tên PG)

        public DateTime? DateOfBirth { get; set; } // Date of Birth (Ngày Sinh)

        public string Gender { get; set; }      // Gender (Giới tính)

        public double? Height { get; set; }      // Height (Chiều Cao)

        public double? Weight { get; set; }      // Height (Chiều Cao)

        public string? AddressText { get; set; }

        public string? ProvinceCode { get; set; }

        public Province? Province { get; set; }

        public string? DistrictCode { get; set; }

        public District? District { get; set; }

        public string? WardCode { get; set; }

        public Ward? Ward { get; set; }
        public int Status { get; set; }      // Status (Trạng thái)

        public string? ZaloId { get; set; }      // Zalo ID (ZaloID)

        public string? AvatarUrl { get; set; }

        public ICollection<CheckIn> CheckIns { get; set; }

        public ICollection<RewardOrder> RewardOrders { get; set; }
    }
}
