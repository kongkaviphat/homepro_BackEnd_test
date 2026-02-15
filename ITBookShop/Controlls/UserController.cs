using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ItBookShop.Data;
using ItBookShop.Models;
using System.Text.Json.Serialization;

[ApiController]
[Route("user")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly AppDbContext _context;

    public UserController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("like")]
    public async Task<IActionResult> ToggleLike(LikeBookRequestDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized();

        int userId = int.Parse(userIdClaim.Value);

        // 🔎 หา Book ใน DB
        var book = await _context.Books
            .FirstOrDefaultAsync(x => x.Isbn13 == dto.BookId);

        // ❗ ถ้าไม่มี → ไปดึงจาก API
        if (book == null)
        {
            var client = new HttpClient();

            var apiBook = await client.GetFromJsonAsync<ApiBookResponse>(
                $"https://api.itbook.store/1.0/books/{dto.BookId}"
            );

            if (apiBook == null || apiBook.Error != "0")
                return NotFound("Book not found");

            book = new Book
            {
                Isbn13 = apiBook.Isbn13,
                Title = apiBook.Title,
                Subtitle = apiBook.Subtitle,
                Price = apiBook.Price,
                Image = apiBook.Image,
                Url = apiBook.Url
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();
        }

        // 🔎 เช็ค Like
        var existingLike = await _context.LikedBooks
            .FirstOrDefaultAsync(x =>
                x.UserId == userId &&
                x.BookId == dto.BookId);

        if (existingLike != null)
        {
            _context.LikedBooks.Remove(existingLike);
            await _context.SaveChangesAsync();

            return Ok(new { status = "unliked" });
        }

        var likedBook = new LikedBook
        {
            UserId = userId,
            BookId = dto.BookId,
        };

        _context.LikedBooks.Add(likedBook);
        await _context.SaveChangesAsync();

        return Ok(new { status = "liked" });
    }

    [HttpGet("liked")]
    public async Task<IActionResult> GetMyLikedBooks()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        int userId = int.Parse(userIdClaim!.Value);

        var books = await _context.LikedBooks
            .Where(x => x.UserId == userId)
            .Include(x => x.Book)
            .Select(x => x.Book)
            .ToListAsync();

        return Ok(books);
    }
}
