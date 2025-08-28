using Microsoft.EntityFrameworkCore;
using MinimalAPIPoc.Domain.DTO;
using MinimalAPIPoc.Infrastructure.Db;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
    );
});

var app = builder.Build();

app.MapGet("/", () => "Entrypoint");

app.MapPost("/login", (LoginDTO loginDTO) =>
{
    if (loginDTO.Username == "adm" && loginDTO.Password == "pwd")
    {
        return Results.Ok("Welcome");
    }
    return Results.Unauthorized();
});

app.Run();
