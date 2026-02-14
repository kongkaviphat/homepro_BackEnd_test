namespace ItBookShop.Models;

public class LikedBook
{
    public int Id { get; set; }
    public string BookId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;

    public int UserId { get; set; }
    public User? User { get; set; }
}
