using Microsoft.EntityFrameworkCore;
using ItBookShop.Models;

namespace ItBookShop.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<LikedBook> LikedBooks => Set<LikedBook>();
    public DbSet<Book> Books => Set<Book>();

}
