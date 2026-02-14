using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

[ApiController]
[Route("/Books")] 
public class BooksController : ControllerBase
{
    private readonly HttpClient _httpClient;

    public BooksController(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient();
    }

    [HttpGet]
    public async Task<IActionResult> GetBooks()
    {
        var response = await _httpClient.GetStringAsync(
            "https://api.itbook.store/1.0/search/mysql");

        var jsonDoc = JsonDocument.Parse(response);

        var books = jsonDoc.RootElement
            .GetProperty("books")
            .EnumerateArray()
            .Select(x => new
            {
                Title = x.GetProperty("title").GetString(),
                Subtitle = x.GetProperty("subtitle").GetString(),
                Isbn13 = x.GetProperty("isbn13").GetString(),
                Price = x.GetProperty("price").GetString(),
                Image = x.GetProperty("image").GetString(),
                Url = x.GetProperty("url").GetString()
            })
            .OrderBy(x => x.Title) // 🔥 เรียง A-Z
            .ToList();

        return Ok(books);
    }
}
