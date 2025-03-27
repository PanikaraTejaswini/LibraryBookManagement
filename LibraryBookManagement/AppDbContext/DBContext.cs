using LibraryBookManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryBookManagement.AppDbContext
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Borrowed> Borroweds { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>()
            .HasKey(a => a.Id); // Primary Key

            modelBuilder.Entity<Book>()
                .Property(a => a.Id)
                .ValueGeneratedOnAdd(); // Auto-Increment
            modelBuilder.Entity<Member>()
            .HasKey(a => a.Id); // Primary Key

            modelBuilder.Entity<Member>()
                .Property(a => a.Id)
                .ValueGeneratedOnAdd(); // Auto-Increment



            modelBuilder.Entity<Member>()
                .HasIndex(a => a.Email) // Unique Constraint
                .IsUnique();
        }
    }
}



     
