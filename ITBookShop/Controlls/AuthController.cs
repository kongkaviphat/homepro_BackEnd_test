using Microsoft.AspNetCore.Mvc;
using ItBookShop.Data;
using ItBookShop.Models;

[ApiController]
[Route("/")] 
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;

    public AuthController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("register")]
    public IActionResult Register(RegisterDto dto)
    {
        if (_context.Users.Any(x => x.Username == dto.Username))
            return BadRequest("Username already exists");

        var user = new User
        {
            Username = dto.Username,
            Password = dto.Password,
            Fullname = dto.Fullname
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        return Ok(user);
    }

    [HttpPost("login")]
    public IActionResult Login(LoginDto dto)
    {
        var user = _context.Users
            .FirstOrDefault(x => x.Username == dto.Username && x.Password == dto.Password);

        if (user == null)
            return Unauthorized("Invalid username or password");

        return Ok(user);
    }
}
