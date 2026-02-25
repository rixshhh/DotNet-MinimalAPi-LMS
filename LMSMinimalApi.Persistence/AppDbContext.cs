using Microsoft.EntityFrameworkCore;

namespace LMSMinimalApi.Persistence
{
    public sealed class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Books> Books { get; init; }
        public DbSet<Categories> Categories { get; init; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Type t = typeof(AppDbContext);
            modelBuilder.ApplyConfigurationsFromAssembly(t.Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
