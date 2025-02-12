using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Checkout
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; } // User who initiated checkout

    [ForeignKey("UserId")]
    public required User User { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsCompleted { get; set; } = false; // False until checkout is finalized

    public ICollection<CheckoutItem> Items { get; set; } = new List<CheckoutItem>();
}
