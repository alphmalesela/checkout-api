public class CheckoutResponse
{
    public int CheckoutId { get; set; }
    public bool IsCompleted { get; set; }
    public decimal TotalCost { get; set; }
    public List<CheckoutItemResponse>? Items { get; set; }
}