using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TradeHubApp.Models;

namespace TradeHubApp.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public DbSet<Advertisement> Advertisements { get; set; }
    public DbSet<UserBalance> UserBalances { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<PrivateMessage> PrivateMessages { get; set; }
    public DbSet<Rating> Ratings { get; set; }
    public DbSet<Purchase> Purchases { get; set; }
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        //Database.EnsureDeleted();
        Database.EnsureCreated();
    }
}