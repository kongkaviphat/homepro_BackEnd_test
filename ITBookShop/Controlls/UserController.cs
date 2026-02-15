using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Net.Http.Json;

using ItBookShop.Data;
using ItBookShop.Models;

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

    // ==========================
    // TOGGLE LIKE BOOK
    // ==========================
    [HttpPost("like")]
    public async Task<IActionResult> ToggleLike(LikeBookRequestDto dto)
    {
        // 🔐 1. ดึง userId จาก Token
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
            return Unauthorized("Invalid token");

        int tokenUserId = int.Parse(userIdClaim.Value);

        // 🔥 2. เช็คว่าที่ส่งมาใน body ตรงกับ token ไหม
        if (dto.UserId != tokenUserId)
            return Unauthorized("UserId does not match token");

        int userId = tokenUserId;

        // ==========================
        // 3. หา Book ใน Database
        // ==========================
        var book = await _context.Books
            .FirstOrDefaultAsync(x => x.Isbn13 == dto.BookId);

        // ==========================
        // 4. ถ้าไม่มี → ดึงจาก API
        // ==========================
        if (book == null)
        {
            using var client = new HttpClient();

            var apiBook = await client.GetFromJsonAsync<ApiBookResponse>(
                $"https://api.itbook.store/1.0/books/{dto.BookId}"
            );

            if (apiBook == null || apiBook.Error != "0")
                return NotFound("Book not found from external API");

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

        // ==========================
        // 5. เช็คว่า Like แล้วหรือยัง
        // ==========================
        var existingLike = await _context.LikedBooks
            .FirstOrDefaultAsync(x =>
                x.UserId == userId &&
                x.BookId == dto.BookId);

        // ==========================
        // 6. ถ้าเคย Like → Unlike
        // ==========================
        if (existingLike != null)
        {
            _context.LikedBooks.Remove(existingLike);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                status = "unliked",
                userId = userId,
                bookId = dto.BookId
            });
        }

        // ==========================
        // 7. ถ้ายังไม่เคย Like → เพิ่ม
        // ==========================
        var likedBook = new LikedBook
        {
            UserId = userId,
            BookId = dto.BookId
        };

        _context.LikedBooks.Add(likedBook);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            status = "liked",
            userId = userId,
            bookId = dto.BookId
        });
    }

    // ==========================
    // GET MY LIKED BOOKS
    // ==========================
    [HttpGet("liked")]
    public async Task<IActionResult> GetMyLikedBooks()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
            return Unauthorized("Invalid token");

        int userId = int.Parse(userIdClaim.Value);

        var books = await _context.LikedBooks
            .Where(x => x.UserId == userId)
            .Include(x => x.Book)
            .Select(x => new
            {
                x.Book.Isbn13,
                x.Book.Title,
                x.Book.Subtitle,
                x.Book.Price,
                x.Book.Image,
                x.Book.Url
            })
            .ToListAsync();

        return Ok(books);
    }
}
