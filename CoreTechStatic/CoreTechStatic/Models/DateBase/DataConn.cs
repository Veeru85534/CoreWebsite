using CoreTechStatic.Models.Banner;
using CoreTechStatic.Models.Static;
using Microsoft.EntityFrameworkCore;

namespace CoreTechStatic.Models.DateBase
{
    public class DataConn : DbContext
    {
        public DataConn(DbContextOptions<DataConn> options) : base(options)
        {

        }
        public DbSet<Products> Product { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Specification> Specification { get; set; }
        public DbSet<Active> Active { get; set; }
        public DbSet<Avalbility> Avalbility { get; set; }
        public DbSet<Cart> Cart { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<StateMaster> StateMaster { get; set; }
        public DbSet<HsnMaster> HsnMaster { get; set; }
        public DbSet<CityMaster> CityMaster { get; set; }
        public DbSet<PriceMaster> PriceMaster { get; set; }
        public DbSet<WishList> WishList { get; set; }
        public DbSet<BannerPage> BannerPage { get; set; }
        public DbSet<SocialMedia> SocialMedia { get; set; }
        public DbSet<AboutUs> AboutUs { get; set; }
    }
}
