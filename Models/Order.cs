public class Order
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public TimeSpan  OrderTime { get; set; }
    public string OrderStatus { get; set; } 

    public Cart Cart { get; set; }  
}
