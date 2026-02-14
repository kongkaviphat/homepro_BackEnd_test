using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ItBookShop.Data;
using ItBookShop.Models;

[ApiController]
[Route("/user/")] 
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
    var user = await _context.Users.FindAsync(dto.UserId);
    if (user == null)
        return NotFound("User not found");

    var existingLike = await _context.LikedBooks
        .FirstOrDefaultAsync(x => 
            x.UserId == dto.UserId && 
            x.BookId == dto.BookId);

    // 🔥 ถ้าเคยไลค์แล้ว → ลบ (Unlike)
    if (existingLike != null)
    {
        _context.LikedBooks.Remove(existingLike);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Book unliked",
            bookId = dto.BookId
        });
    }

    // 🔥 ถ้ายังไม่เคยไลค์ → ไปดึงข้อมูลจาก API
    var client = new HttpClient();
    var book = await client.GetFromJsonAsync<Book>(
        $"https://api.itbook.store/1.0/books/{dto.BookId}"
    );

    if (book == null || string.IsNullOrEmpty(book.Title))
        return NotFound("Book not found from external API");

    var likedBook = new LikedBook
    {
        UserId = dto.UserId,
        BookId = book.Isbn13,
        Title = book.Title,
        Image = book.Image
    };

    _context.LikedBooks.Add(likedBook);
    await _context.SaveChangesAsync();

    return Ok(new
    {
        message = "Book liked",
        likedBook.BookId,
        likedBook.Title,
        likedBook.Image
    });
}

}