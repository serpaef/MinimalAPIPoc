using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalAPIPoc.Domain.DTO;
using MinimalAPIPoc.Domain.Entities;
using MinimalAPIPoc.Domain.Interfaces;
using MinimalAPIPoc.Domain.ModelViews;
using MinimalAPIPoc.Domain.Services;
using MinimalAPIPoc.Domain.Enums;
using MinimalAPIPoc.Infrastructure.Db;

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
    catch (BadHttpRequestException)
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
static ValidationErrors validateAdminDTO(AdminDTO adminDTO)
{
    var validationErrors = new ValidationErrors();
    if (string.IsNullOrWhiteSpace(adminDTO.Username))
        validationErrors.Messages.Add("Username is required.");
    if (string.IsNullOrWhiteSpace(adminDTO.Password))
        validationErrors.Messages.Add("Password is required.");
    if (adminDTO.Role == Role.None)
        validationErrors.Messages.Add("Role is required.");
    if (adminDTO.Role != Role.Admin)
        validationErrors.Messages.Add("Role must be Admin");

    return validationErrors;
}

app.MapPost("/admin/login", ([FromBody] LoginDTO loginDTO, IAdminService adminService) =>
{
    if(adminService.Login(loginDTO) != null)
    {
        return Results.Ok("Login successful");
    }
    
    return Results.Unauthorized();
}).WithTags("Admin");

app.MapPost("/admin", ([FromBody] AdminDTO adminDTO, IAdminService adminService) =>
{
    var validationErrors = validateAdminDTO(adminDTO);

    if (validationErrors.Messages.Count > 0)
        return Results.BadRequest(validationErrors);

    var admin = new Admin()
    {
        Username = adminDTO.Username,
        Password = adminDTO.Password,
        Role = adminDTO.Role.ToString()
    };

    adminService.Create(admin);

    return Results.Created($"/admin/{admin.Id}", admin);
}).WithTags("Admin");

app.MapGet("/admin", (HttpContext http, int? page, IAdminService adminService) =>
{
    var admins = adminService.GetAllAdmins(page);
    
    return Results.Ok(admins);
}).WithTags("Admin");

app.MapGet("/admin/{id}", ([FromRoute]int id, IAdminService adminService) =>
{
    var admin = adminService.GetAllAdmins(null).FirstOrDefault(a => a.Id == id);
    
    if(admin == null)
    {
        return Results.NotFound(new { message = "Admin not found" });
    }

    return Results.Ok(admin);
}).WithTags("Admin");
#endregion

#region Vehicle
static ValidationErrors validateVehicleDTO(VehicleDTO vehicleDTO)
{
    var validationErrors = new ValidationErrors();

    if (string.IsNullOrWhiteSpace(vehicleDTO.Name))
        validationErrors.Messages.Add("Name is required.");

    if (string.IsNullOrWhiteSpace(vehicleDTO.Make))
        validationErrors.Messages.Add("Make is required.");

    if (vehicleDTO.Year < 1886 || vehicleDTO.Year > DateTime.Now.Year + 1)
        validationErrors.Messages.Add("Year must be between 1886 and next year.");

    return validationErrors;
}

app.MapPost("/vehicles", ([FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
{
    var validationErrors = validateVehicleDTO(vehicleDTO);
    
    if(validationErrors.Messages.Count > 0)
        return Results.BadRequest(validationErrors);

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

app.MapPut("/vehicles/{id}", ([FromRoute]int id, [FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
{
    var existingVehicle = vehicleService.GetVehicleById(id);
    
    if(existingVehicle == null)
    {
        return Results.NotFound(new { message = "Vehicle not found" });
    }
   
    var validationErrors = validateVehicleDTO(vehicleDTO);

    if (validationErrors.Messages.Count > 0)
        return Results.BadRequest(validationErrors);

    existingVehicle.Name = vehicleDTO.Name;
    existingVehicle.Make = vehicleDTO.Make;
    existingVehicle.Year = vehicleDTO.Year;
    
    vehicleService.UpdateVehicle(existingVehicle);
    
    return Results.Ok(existingVehicle);
}).WithTags("Vehicles");

app.MapDelete("/vehicles/{id}", ([FromRoute]int id, IVehicleService vehicleService) =>
{
    var existingVehicle = vehicleService.GetVehicleById(id);
    
    if(existingVehicle == null)
    {
        return Results.NotFound(new { message = "Vehicle not found" });
    }
    
    vehicleService.DeleteVehicle(id);
    
    return Results.NoContent();
}).WithTags("Vehicles");
#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
#endregion
