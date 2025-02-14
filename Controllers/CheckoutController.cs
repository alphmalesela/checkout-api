using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/checkout")]
public class CheckoutController : ControllerBase
{
    private readonly CheckoutContext _context;

    public CheckoutController(CheckoutContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> AddCheckout([FromBody] CheckoutRequest request)
    {
        var user = HttpContext.Items["User"] as User;
        if (user == null)
        {
            return Unauthorized(new { error = "Unauthorized request" });
        }

        var checkout = await _context.Checkouts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == user.Id && !c.IsCompleted);

        if (checkout == null)
        {
            checkout = new Checkout
            {
                UserId = user.Id
            };
            _context.Checkouts.Add(checkout);
            await _context.SaveChangesAsync();
        }

        foreach (var item in request.Items!)
        {
            var product = await _context.Products.FindAsync(item.ProductId);
            if (product == null || product.Quantity < item.Quantity)
            {
                return BadRequest(new { error = $"Product {item.ProductId} is unavailable or insufficient stock." });
            }

            var existingItem = checkout.Items.FirstOrDefault(ci => ci.ProductId == item.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
                existingItem.TotalPrice = existingItem.Quantity * product.Price;
            }
            else
            {
                checkout.Items.Add(new CheckoutItem
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    TotalPrice = item.Quantity * product.Price
                });
            }
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = "Checkout updated successfully", checkoutId = checkout.Id });
    }

    [HttpPut("{checkoutId}")]
    public async Task<IActionResult> UpdateCheckout(int checkoutId, [FromBody] UpdateCheckoutRequest request)
    {
        var user = HttpContext.Items["User"] as User;
        if (user == null)
        {
            return Unauthorized(new { error = "Unauthorized request" });
        }

        var checkout = await _context.Checkouts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == checkoutId && c.UserId == user.Id);

        if (checkout == null)
        {
            return NotFound(new { error = "Checkout not found" });
        }

        if (checkout.IsCompleted)
        {
            return BadRequest(new { error = "Cannot update a completed checkout" });
        }

        foreach (var item in request.Items!)
        {
            var checkoutItem = checkout.Items.FirstOrDefault(ci => ci.ProductId == item.ProductId);

            if (checkoutItem == null)
            {
                return BadRequest(new { error = $"Product {item.ProductId} is not in the checkout." });
            }

            if (item.Quantity == 0)
            {
                _context.CheckoutItems.Remove(checkoutItem);
            }
            else
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null || product.Quantity < item.Quantity)
                {
                    return BadRequest(new { error = $"Not enough stock for product {item.ProductId}." });
                }

                checkoutItem.Quantity = item.Quantity;
                checkoutItem.TotalPrice = item.Quantity * product.Price;
            }
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = "Checkout updated successfully", checkoutId = checkout.Id });
    }

    [HttpDelete("{checkoutId}")]
    public async Task<IActionResult> CancelCheckout(int checkoutId)
    {
        var user = HttpContext.Items["User"] as User;
        if (user == null)
        {
            return Unauthorized(new { error = "Unauthorized request" });
        }

        var checkout = await _context.Checkouts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == checkoutId && c.UserId == user.Id);

        if (checkout == null)
        {
            return NotFound(new { error = "Checkout not found" });
        }

        if (checkout.IsCompleted)
        {
            return BadRequest(new { error = "Cannot cancel a completed checkout." });
        }

        // Remove checkout and associated items
        _context.CheckoutItems.RemoveRange(checkout.Items);
        _context.Checkouts.Remove(checkout);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Checkout cancelled successfully" });
    }

    [HttpPost("{checkoutId}/complete")]
    public async Task<IActionResult> CompleteCheckout(int checkoutId)
    {
        var user = HttpContext.Items["User"] as User;
        if (user == null)
        {
            return Unauthorized(new { error = "Unauthorized request" });
        }

        var checkout = await _context.Checkouts
            .Include(c => c.Items)
            .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.Id == checkoutId && c.UserId == user.Id);

        if (checkout == null)
        {
            return NotFound(new { error = "Checkout not found" });
        }

        if (checkout.IsCompleted)
        {
            return BadRequest(new { error = "Checkout is already completed." });
        }

        foreach (var item in checkout.Items)
        {
            if (item.Product!.Quantity < item.Quantity)
            {
                return BadRequest(new { error = $"Not enough stock for product {item.ProductId}." });
            }
        }

        foreach (var item in checkout.Items)
        {
            item.Product!.Quantity -= item.Quantity;
        }

        checkout.IsCompleted = true;
        await _context.SaveChangesAsync();

        var summary = checkout.Items.Select(ci => new
        {
            productId = ci.ProductId,
            productName = ci.Product!.Name,
            quantity = ci.Quantity,
            totalPrice = ci.TotalPrice
        }).ToList();

        return Ok(new
        {
            message = "Checkout completed successfully",
            checkoutId = checkout.Id,
            summary
        });
    }

    [HttpGet("{checkoutId}")]
    public async Task<IActionResult> GetCheckout(int checkoutId)
    {
        var user = HttpContext.Items["User"] as User;
        if (user == null)
        {
            return Unauthorized(new { error = "Unauthorized request" });
        }

        var checkout = await _context.Checkouts
            .Include(c => c.Items)
            .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.Id == checkoutId && c.UserId == user.Id);

        if (checkout == null)
        {
            return NotFound(new { error = "Checkout not found" });
        }

        var response = new CheckoutResponse
        {
            CheckoutId = checkout.Id,
            IsCompleted = checkout.IsCompleted,
            TotalCost = checkout.Items.Sum(i => i.TotalPrice),
            Items = checkout.Items.Select(i => new CheckoutItemResponse
            {
                ProductId = i.ProductId,
                ProductName = i.Product!.Name,
                Quantity = i.Quantity,
                TotalPrice = i.TotalPrice
            }).ToList()
        };

        return Ok(response);
    }


}