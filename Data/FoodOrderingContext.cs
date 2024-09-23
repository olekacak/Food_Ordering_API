using Microsoft.EntityFrameworkCore;

public class FoodOrderingContext : DbContext
{
    public DbSet<Food> Food { get; set; }
    public DbSet<Cart> Cart { get; set; }
    public DbSet<CartItem> CartItem { get; set; }
    public DbSet<Order> Order { get; set; }
    public DbSet<User> User { get; set; } 


    public FoodOrderingContext(DbContextOptions<FoodOrderingContext> options)
        : base(options)
    {
    }
}