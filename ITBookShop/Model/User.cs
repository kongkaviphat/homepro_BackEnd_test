namespace ItBookShop.Models;

public class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Fullname { get; set; } = null!;

    public ICollection<LikedBook> LikedBooks { get; set; } = new List<LikedBook>();

}
