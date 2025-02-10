using System.ComponentModel.DataAnnotations.Schema;

namespace PGAdmin.Models
{
    public class CheckIn
    {
        public int Id { get; set; }
        public DateTime? CheckinTime { get; set; }
        public DateTime? CheckoutTime { get; set; }
        public int PgId { get; set; }
        public PG Pg { get; set; }
        public string? Note { get; set; }
        public string? CheckinLocation { get; set; }
        public string? CheckoutLocation { get; set; }
    }
}
