using Microsoft.EntityFrameworkCore;
using ItBookShop.Models;

namespace ItBookShop.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<LikedBook> LikedBooks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // PK ของ Book
        modelBuilder.Entity<Book>()
            .HasKey(b => b.Isbn13);

        // Composite PK ของ LikedBook
        modelBuilder.Entity<LikedBook>()
            .HasKey(lb => new { lb.UserId, lb.BookId });

        modelBuilder.Entity<LikedBook>()
            .HasOne(lb => lb.User)
            .WithMany(u => u.LikedBooks)
            .HasForeignKey(lb => lb.UserId);

        modelBuilder.Entity<LikedBook>()
            .HasOne(lb => lb.Book)
            .WithMany(b => b.LikedBooks)
            .HasForeignKey(lb => lb.BookId);
    }
}
