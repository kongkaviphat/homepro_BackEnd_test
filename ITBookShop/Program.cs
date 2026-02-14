using ItBookShop.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=itbook.db"));

builder.Services.AddControllers();
builder.Services.AddHttpClient();

var app = builder.Build();

app.MapControllers();
app.Run();


public record RegisterDto(string Username, string Password, string Fullname);
public record LoginDto(string Username, string Password);
public record LikeBookRequestDto(int UserId, string BookId);