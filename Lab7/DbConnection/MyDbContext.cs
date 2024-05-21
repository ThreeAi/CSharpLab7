using Microsoft.EntityFrameworkCore;
using Lab7.Model;

namespace Lab7.DbConnection
{
    public class MyDbContext : DbContext
    {
        public MyDbContext()
        {
            //Database.EnsureDeleted();
            //Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Note>()
                .HasKey(n => new { n.Id });

            modelBuilder.Entity<Category>()
                .HasKey(c => new { c.Id });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost; Database=company; Username=postgres; Password=qwerty");
            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<Note> Notes { get; set; }
        public DbSet<Category> Categories { get; set; }

    }
}
