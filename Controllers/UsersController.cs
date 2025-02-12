using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly CheckoutContext _context;

    public UsersController(CheckoutContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<UserResponse>> CreateUser([FromBody] CreateUserRequest request)
    {
        // No authentication required
        var user = new User
        {
            Username = request.Username
        };
        _context.Add(user);

        await _context.SaveChangesAsync();

        return Ok(new UserResponse { ApiKey = user.ApiKey });
    }
}
