using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

[Route("[controller]")]
[ApiController]
public class CartsController : ControllerBase
{
    private readonly FoodOrderingContext _context;

    public CartsController(FoodOrderingContext context)
    {
        _context = context;
    }

    // POST: carts/add?userId=1
    [HttpPost("add")]
    public async Task<IActionResult> AddToCart([FromQuery] int userId, [FromBody] CartItem cartItem)
    {
        if (cartItem == null || cartItem.FoodId <= 0 || cartItem.Quantity <= 0)
        {
            return BadRequest("Invalid CartItem data.");
        }

        // Check if the user exists
        var userExists = await _context.User.AnyAsync(u => u.UserId == userId);
        if (!userExists)
        {
            return NotFound($"User with ID {userId} does not exist."); // Return 404 with userId
        }

        // Check if the user has any cart with a "Pending" order
        var existingCart = await _context.Cart
            .Include(c => c.Order)
            .FirstOrDefaultAsync(c => c.UserId == userId && c.Order.OrderStatus.ToLower() == "pending");

        if (existingCart != null)
        {
            // Use the existing cart with "Pending" status
            var result = await AddCartItem(existingCart.CartId, cartItem.FoodId, cartItem.Quantity);
            if (result is OkResult)
            {
                return Ok("Item added to the existing cart."); // Custom success message
            }
            return result; // Forward the result if it's an error
        }
        else
        {
            // Create a new cart and new order if there are no pending carts
            var newCart = await CreateNewCart(userId);
            var result = await AddCartItem(newCart.CartId, cartItem.FoodId, cartItem.Quantity);
            if (result is OkResult)
            {
                return Ok("New cart created and item added."); // Custom success message
            }
            return result; // Forward the result if it's an error
        }
    }


    private async Task<IActionResult> AddCartItem(int cartId, int foodId, int quantity)
    {
        var foodExists = await _context.Food.AnyAsync(f => f.FoodId == foodId);
        if (!foodExists)
        {
            return NotFound("The specified food item does not exist."); // Return 404
        }

        var cartExists = await _context.Cart.AnyAsync(c => c.CartId == cartId);
        if (!cartExists)
        {
            return NotFound("The specified cart does not exist."); // Return 404
        }

        // Check if the CartItem already exists
        var existingCartItem = await _context.CartItem
            .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.FoodId == foodId);

        if (existingCartItem != null)
        {
            // Update the existing cart item quantity
            existingCartItem.Quantity += quantity;
            _context.CartItem.Update(existingCartItem);
        }
        else
        {
            // Create a new cart item if it doesn't exist
            var cartItem = new CartItem
            {
                CartId = cartId,
                FoodId = foodId,
                Quantity = quantity
            };

            _context.CartItem.Add(cartItem);
        }

        await _context.SaveChangesAsync();
        return Ok(); // Return a success response
    }

    private async Task<Cart> CreateNewCart(int userId)
    {
        var malaysiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Kuala_Lumpur");
        DateTime utcNow = DateTime.UtcNow;
        DateTime malaysiaTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, malaysiaTimeZone);

        // Create a new order
        var newOrder = new Order
        {
            OrderDate = malaysiaTime,
            OrderTime = malaysiaTime.TimeOfDay,
            OrderStatus = "Pending"
        };

        _context.Order.Add(newOrder);
        await _context.SaveChangesAsync();

        // Create a new cart
        var newCart = new Cart
        {
            UserId = userId,
            CreatedDate = malaysiaTime,
            CreatedTime = malaysiaTime.TimeOfDay,
            OrderId = newOrder.OrderId,
            Order = newOrder
        };

        _context.Cart.Add(newCart);
        await _context.SaveChangesAsync();

        return newCart;
    }

}
