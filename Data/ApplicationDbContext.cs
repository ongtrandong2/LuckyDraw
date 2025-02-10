using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PGAdmin.Models;
using PGAdmin.Models.Location;
using PGAdmin.Models.Campaign;
using PGAdmin.Models.Reward;

namespace PGAdmin.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<PGAdmin.Models.PG> PG { get; set; } = default!;
        public DbSet<Product> Products { get; set; } = default!;
        public DbSet<ProductType> ProductTypes { get; set; } = default!;
        public DbSet<ProductCategory> ProductCategories { get; set; } = default!;

        public DbSet<AdministrativeRegion> AdministrativeRegions { get; set; } = default!;
        public DbSet<AdministrativeUnit> AdministrativeUnits { get; set; } = default!;
        public DbSet<Province> Provinces { get; set; } = default!;
        public DbSet<District> Districts { get; set; } = default!;
        public DbSet<Ward> Wards { get; set; } = default!;


        public DbSet<Campaign> Campaigns { get; set; } = default!;
        public DbSet<GiftRule> GiftRules { get; set; } = default!;
        public DbSet<GiftCondition> GiftConditions { get; set; } = default!;
        public DbSet<GiftProductRequirement> GiftProductRequirements { get; set; } = default!;
        public DbSet<CampaignGift> CampaignGifts { get; set; } = default!;
        public DbSet<PGAdmin.Models.Gift> Gift { get; set; } = default!;
        public DbSet<CheckIn> CheckIns { get; set; } = default!;

        public DbSet<RewardOrder> RewardOrders { get; set; } = default!;
        public DbSet<RewardOrderProduct> RewardOrderProducts { get; set; } = default!;
        public DbSet<RewardOrderDetail> RewardOrderDetails { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PG>()
                .HasOne(p => p.Province)
                .WithMany(t => t.PGs)
                .HasForeignKey(p => p.ProvinceCode);

            modelBuilder.Entity<PG>()
               .HasOne(p => p.District)
               .WithMany(t => t.PGs)
               .HasForeignKey(p => p.DistrictCode);

            modelBuilder.Entity<PG>()
               .HasOne(p => p.Ward)
               .WithMany(t => t.PGs)
               .HasForeignKey(p => p.WardCode);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Type)
                .WithMany(t => t.Products)
                .HasForeignKey(p => p.TypeId);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);

            modelBuilder.Entity<Province>()
                .HasOne(p => p.AdministrativeUnit)
                .WithMany(u => u.Provinces)
                .HasForeignKey(p => p.AdministrativeUnitId);

            modelBuilder.Entity<Province>()
                .HasOne(p => p.AdministrativeRegion)
                .WithMany(r => r.Provinces)
                .HasForeignKey(p => p.AdministrativeRegionId);

            modelBuilder.Entity<District>()
                .HasOne(d => d.Province)
                .WithMany(p => p.Districts)
                .HasForeignKey(d => d.ProvinceCode);

            modelBuilder.Entity<District>()
                .HasOne(d => d.AdministrativeUnit)
                .WithMany(u => u.Districts)
                .HasForeignKey(d => d.AdministrativeUnitId);

            modelBuilder.Entity<Ward>()
                .HasOne(w => w.District)
                .WithMany(d => d.Wards)
                .HasForeignKey(w => w.DistrictCode);

            modelBuilder.Entity<Ward>()
                .HasOne(w => w.AdministrativeUnit)
                .WithMany(u => u.Wards)
                .HasForeignKey(w => w.AdministrativeUnitId);

            modelBuilder.Entity<GiftRule>()
                .HasOne(g => g.Campaign)
                .WithMany(p => p.GiftRules)
                .HasForeignKey(g => g.CampaignId);

            modelBuilder.Entity<GiftCondition>()
                .HasOne(c => c.GiftRule)
                .WithMany(g => g.GiftConditions)
                .HasForeignKey(c => c.GiftRuleId);

            modelBuilder.Entity<GiftProductRequirement>()
                .HasOne(p => p.GiftCondition)
                .WithMany(c => c.GiftProducts)
                .HasForeignKey(p => p.GiftConditionId);

            modelBuilder.Entity<CheckIn>()
                .HasOne(c => c.Pg)
                .WithMany(p => p.CheckIns)
                .HasForeignKey(c => c.PgId);

            modelBuilder.Entity<RewardOrderProduct>()
            .HasOne(rp => rp.RewardOrder)
            .WithMany(ro => ro.Products)
            .HasForeignKey(rp => rp.RewardOrderId);

            modelBuilder.Entity<RewardOrderDetail>()
                .HasOne(rd => rd.RewardOrder)
                .WithMany(ro => ro.Details)
                .HasForeignKey(rd => rd.RewardOrderId);

            modelBuilder.Entity<RewardOrder>()
               .HasOne(ro => ro.Pg) 
               .WithMany()
               .HasForeignKey(ro => ro.PgId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RewardOrder>()
                .HasOne(ro => ro.Campaign)
                .WithMany(c => c.RewardOrders)
                .HasForeignKey(ro => ro.CampaignId) 
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CampaignGift>()
               .HasOne(cg => cg.Campaign)
               .WithMany(c => c.CampaignGifts)
               .HasForeignKey(cg => cg.CampaignId);
            modelBuilder.Entity<CampaignGift>()
                .Property(p => p.Version)
                .IsConcurrencyToken();
        }
    }
}
