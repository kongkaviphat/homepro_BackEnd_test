namespace ItBookShop.Models;

public class Book
{
    public string Isbn13 { get; set; } = null!;

    public string Title { get; set; } = null!;
    public string? Subtitle { get; set; }
    public string? Price { get; set; }
    public string? Image { get; set; }
    public string? Url { get; set; }

    public ICollection<LikedBook> LikedBooks { get; set; } = new List<LikedBook>();
}
