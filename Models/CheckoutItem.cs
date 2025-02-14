using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class CheckoutItem
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int CheckoutId { get; set; }

    [ForeignKey("CheckoutId")]
    public Checkout? Checkout { get; set; }

    [Required]
    public int ProductId { get; set; }

    [ForeignKey("ProductId")]
    public Product? Product { get; set; }

    [Required]
    public int Quantity { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalPrice { get; set; }
}
