using Microsoft.EntityFrameworkCore;
using WebApi.Data.Models;

namespace WebApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base()
        {
        }

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //Map Entity names to DB Table names
            modelBuilder.Entity<Thingslog>().ToTable("Thingslogtable");
        }

        public DbSet<Thingslog> Thingslog { get; set; }
    }
}
