using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalAPIPoc.Domain.DTO;
using MinimalAPIPoc.Domain.Entities;
using MinimalAPIPoc.Domain.Interfaces;
using MinimalAPIPoc.Domain.ModelViews;
using MinimalAPIPoc.Domain.Services;
using MinimalAPIPoc.Infrastructure.Db;
using static System.Net.WebRequestMethods;

#region Setup
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
    );
});

var app = builder.Build();

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (BadHttpRequestException ex)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/json";

        var result = new
        {
            message = "Bad Request",
        };

        await context.Response.WriteAsJsonAsync(result);
    }
});

app.UseRouting();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
#endregion

#region Admin
app.MapPost("/admin/login", ([FromBody] LoginDTO loginDTO, IAdminService adminService) =>
{
    if(adminService.Login(loginDTO) != null)
    {
        return Results.Ok("Login successful");
    }
    
    return Results.Unauthorized();
}).WithTags("Admin");
#endregion

#region Vehicle
app.MapPost("/vehicles", ([FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
{
    var vehicle = new Vehicle
    {
        Name = vehicleDTO.Name,
        Make = vehicleDTO.Make,
        Year = vehicleDTO.Year
    };

    vehicleService.CreateVehicle(vehicle);

    return Results.Created($"/vehicles/{vehicle.Id}", vehicle);
}).WithTags("Vehicles");

app.MapGet("/vehicles", (HttpContext http, int? page, string? name, string? make, IVehicleService vehicleService) =>
{
    var vehicles = vehicleService.GetAllVehicles(page, name, make);
    
    return Results.Ok(vehicles);
}).WithTags("Vehicles");

app.MapGet("/vehicles/{id}", ([FromRoute]int id, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.GetVehicleById(id);
    
    if(vehicle == null)
    {
        return Results.NotFound(new { message = "Vehicle not found" });
    }
    return Results.Ok(vehicle);
}).WithTags("Vehicles");
#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
#endregion
