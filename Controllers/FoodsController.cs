using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodOrderingAbata.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FoodsController : ControllerBase
    {
        private readonly FoodOrderingContext _context;

        public FoodsController(FoodOrderingContext context)
        {
            _context = context;
        }

        // GET: foods
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Food>>> GetFood()
        {
            var foods = await _context.Food
                .Select(f => new Food
                {
                    FoodId = f.FoodId,
                    Name = f.Name,
                    Price = f.Price,
                    Description = f.Description
                })
                .ToListAsync();

            return Ok(foods);
        }

        // GET: foods/5
        [HttpGet("{foodId}")]
        public async Task<ActionResult<Food>> GetFoodById(int foodId)
        {
            var food = await _context.Food
                .Where(f => f.FoodId == foodId)
                .Select(f => new Food
                {
                    FoodId = f.FoodId,
                    Name = f.Name,
                    Price = f.Price,
                    Description = f.Description
                })
                .FirstOrDefaultAsync();

            if (food == null)
            {
                // Return a custom response if the food does not exist
                return NotFound(new { Message = $"No food item found with FoodId: {foodId}" });
            }

            return Ok(food);
        }

    }
}
