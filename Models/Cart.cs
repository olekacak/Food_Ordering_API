public class Cart
{
    public int CartId { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedDate { get; set; }
     public TimeSpan  CreatedTime { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; }
    public ICollection<CartItem> CartItem { get; set; }
}
