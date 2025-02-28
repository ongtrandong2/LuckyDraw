﻿namespace PGAdmin.Models.Location
{
    public class AdministrativeRegion
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameEn { get; set; }
        public string CodeName { get; set; }
        public string CodeNameEn { get; set; }

        public ICollection<Province> Provinces { get; set; }
    }
}
