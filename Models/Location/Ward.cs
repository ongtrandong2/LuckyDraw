﻿using System.ComponentModel.DataAnnotations;

namespace PGAdmin.Models.Location
{
    public class Ward
    {
        [Key]
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameEn { get; set; }
        public string FullName { get; set; }
        public string FullNameEn { get; set; }
        public string CodeName { get; set; }

        public string DistrictCode { get; set; }
        public District District { get; set; }

        public int? AdministrativeUnitId { get; set; }
        public AdministrativeUnit AdministrativeUnit { get; set; }

        public ICollection<PG> PGs { get; set; }
    }
}
