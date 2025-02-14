public class UpdateCheckoutItemRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; } // New quantity (0 means remove item)
}