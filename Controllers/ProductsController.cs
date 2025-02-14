using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly CheckoutContext _context;

    public ProductsController(CheckoutContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> AddProduct([FromBody] CreateProductRequest request)
    {
        // User must provide a valid API key

        var user = HttpContext.Items["User"] as User;

        var product = new Product
        {
            Name = request.Name!,
            Price = request.Price,
            Quantity = request.Quantity,
            User = user!
        };

        _context.Add(product);
        await _context.SaveChangesAsync();

        return Created(nameof(AddProduct), new { productId = product.Id });
    }

    [HttpGet]
    public async Task<ActionResult<List<ProductResponse>>> GetAllProducts()
    {

        var user = HttpContext.Items["User"] as User;

        var products = await _context.Products
        .Where(p => p.User == user)
        .Select(p => new ProductResponse
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Quantity = p.Quantity
        })
        .ToListAsync();

        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductResponse>> GetProduct(long id)
    {

        var user = HttpContext.Items["User"] as User;

        var product = await _context.Products
        .Where(p => p.Id == id)
        .Select(p => new ProductResponse
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Quantity = p.Quantity
        }).FirstAsync();

        if (product == null)
        {
            return NotFound(new { error = "Product not found" });
        }

        return Ok(product);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutProduct(long id, [FromBody] UpdateProductRequest request)
    {
        var user = HttpContext.Items["User"] as User;

        if (id != request.Id)
        {
            return BadRequest();
        }

        var product = await _context.Products.Where(p => p.Id == id && p.UserId == user!.Id).FirstAsync();
        if (product == null) return NotFound();

        product.Name = request.Name!;
        product.Price = request.Price;
        product.Quantity = request.Quantity;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            product = await _context.Products.FindAsync(id);
            if (product == null || product.UserId != user!.Id)
            {
                return NotFound(new { error = "Product not found or unauthorized" });
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodoItem(long id)
    {
        var todoItem = await _context.Products.FindAsync(id);
        if (todoItem == null)
        {
            return NotFound();
        }

        _context.Products.Remove(todoItem);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("others")]
    public async Task<ActionResult<List<ProductResponse>>> GetOthers()
    {

        var user = HttpContext.Items["User"] as User;

        var products = await _context.Products
        .Where(p => p.User != user)
        .Select(p => new ProductResponse
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Quantity = p.Quantity
        })
        .ToListAsync();

        return Ok(products);
    }

}