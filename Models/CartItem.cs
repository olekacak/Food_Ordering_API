public class CartItem
{
    public int CartItemId { get; set; }
    public int CartId { get; set; }
    public int FoodId { get; set; }
    public int Quantity { get; set; }
    public Cart? Cart { get; set; } 
    public Food? Food { get; set; } 
}
