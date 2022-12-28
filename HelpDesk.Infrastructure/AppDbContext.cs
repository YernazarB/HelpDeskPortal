using HelpDesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Organization> Organizations { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Organization>().HasIndex(x => x.Name).IsUnique(true);
            modelBuilder.Entity<User>().HasIndex(x => x.Username).IsUnique(true);

            base.OnModelCreating(modelBuilder);
        }

        public async Task SaveChangesAsync(int? userId)
        {
            if (userId == null)
            {
                await base.SaveChangesAsync();
                return;
            }

            var dateNow = DateTimeOffset.Now.ToUniversalTime();
            var entries = ChangeTracker.Entries<BaseEntity>().ToArray();
            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedDate = dateNow;
                    entry.Entity.CreatedById = userId.Value;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.ModifiedDate = dateNow;
                    entry.Entity.ModifiedById = userId.Value;
                }
            }

            await base.SaveChangesAsync();
        }
    }
}
