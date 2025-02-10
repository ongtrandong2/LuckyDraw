namespace PGAdmin.Models.Location
{
    public class AdministrativeUnit
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string FullNameEn { get; set; }
        public string ShortName { get; set; }
        public string ShortNameEn { get; set; }
        public string CodeName { get; set; }
        public string CodeNameEn { get; set; }

        public ICollection<Province> Provinces { get; set; }
        public ICollection<District> Districts { get; set; }
        public ICollection<Ward> Wards { get; set; }
    }
}
