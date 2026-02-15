namespace ItBookShop.Models;

public class User
{
    public int userId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Fullname { get; set; } = string.Empty;

    public List<LikedBook> LikedBooks { get; set; } = new();
}