using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

[Route("[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly FoodOrderingContext _context;

    public OrdersController(FoodOrderingContext context)
    {
        _context = context;
    }

    // POST: orders/place?userId=1
    [HttpPost("place")]
    public async Task<IActionResult> CompleteOrder([FromQuery] int userId)
    {
        // Find the user's cart with a pending order
        var existingCart = await _context.Cart
            .Include(c => c.Order)
            .FirstOrDefaultAsync(c => c.UserId == userId && 
                                       c.Order.OrderStatus.ToLower() == "pending");

        if (existingCart == null)
        {
            return NotFound("No pending orders found for the specified user.");
        }

        // Update the order status to 'Complete'
        existingCart.Order.OrderStatus = "Complete";

        // Update the OrderTime to the current time
        existingCart.Order.OrderTime = DateTime.Now.TimeOfDay; // Or use a timezone-aware method if needed

        _context.Cart.Update(existingCart);
        await _context.SaveChangesAsync();

        return Ok("Order status updated to complete.");
    }
}
