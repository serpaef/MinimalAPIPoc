using MinimalAPIPoc.Domain.DTO;

var builder = WebApplication.CreateBuilder(args);
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
