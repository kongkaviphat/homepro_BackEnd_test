namespace ItBookShop.Models;

public class LikedBook
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;   // ✅ ต้องมีบรรทัดนี้

    public string BookId { get; set; } = null!;
    public Book Book { get; set; } = null!;   // ✅ และต้องมีอันนี้ด้วย
}
