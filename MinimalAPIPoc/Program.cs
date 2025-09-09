using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalAPIPoc.Domain.DTO;
using MinimalAPIPoc.Domain.Entities;
using MinimalAPIPoc.Domain.Enums;
using MinimalAPIPoc.Domain.Interfaces;
using MinimalAPIPoc.Domain.ModelViews;
using MinimalAPIPoc.Domain.Services;
using MinimalAPIPoc.Infrastructure.Db;
using MinimalAPIPoc.Infrastructure.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

#region Setup
var builder = WebApplication.CreateBuilder(args);

var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.Key ?? "12345"))
    };
});

builder.Services.AddAuthorization();

builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

});

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
app.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");
#endregion

#region Admin

static String GenerateJwtToken(Admin admin, JwtSettings jwtSettings)
{
    if (jwtSettings == null)
        throw new ArgumentNullException(nameof(jwtSettings));

    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>()
    {
        new Claim("username", admin.Username),
        new Claim("role", admin.Role)
    };

    var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: credentials
        );

    return new JwtSecurityTokenHandler().WriteToken(token);
}

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
    var admin = adminService.Login(loginDTO);
    if (admin != null)
    {
        string jwtToken = GenerateJwtToken(admin, jwtSettings!);
        return Results.Ok(new
        {
            username = admin.Username,
            role = admin.Role,
            token = "Bearer " + jwtToken
        });
    }
    
    return Results.Unauthorized();
}).AllowAnonymous().WithTags("Admin");

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

    var adminModelView = new AdminModelView
    {
        Id = admin.Id,
        Username = admin.Username,
        Role = admin.Role
    };

    return Results.Created($"/admin/{adminModelView.Id}", adminModelView);
}).RequireAuthorization().WithTags("Admin");

app.MapGet("/admin", (HttpContext http, int? page, IAdminService adminService) =>
{
    var admins = adminService.GetAllAdmins(page);

    var adminViews = new List<AdminModelView>();

    foreach (var admin in admins)
    {
        adminViews.Add(new AdminModelView
        {
            Id = admin.Id,
            Username = admin.Username,
            Role = admin.Role
        });
    }

    return Results.Ok(adminViews);
}).RequireAuthorization().WithTags("Admin");

app.MapGet("/admin/{id}", ([FromRoute]int id, IAdminService adminService) =>
{
    var admin = adminService.GetAllAdmins(null).FirstOrDefault(a => a.Id == id);
    
    if(admin == null)
    {
        return Results.NotFound(new { message = "Admin not found" });
    }

    return Results.Ok(new AdminModelView()
    {
        Id = admin.Id,
        Username = admin.Username,
        Role = admin.Role
    });
}).RequireAuthorization().WithTags("Admin");
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
}).RequireAuthorization().WithTags("Vehicles");

app.MapGet("/vehicles", (HttpContext http, int? page, string? name, string? make, IVehicleService vehicleService) =>
{
    var vehicles = vehicleService.GetAllVehicles(page, name, make);
    
    return Results.Ok(vehicles);
}).RequireAuthorization().WithTags("Vehicles");

app.MapGet("/vehicles/{id}", ([FromRoute]int id, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.GetVehicleById(id);
    
    if(vehicle == null)
    {
        return Results.NotFound(new { message = "Vehicle not found" });
    }

    return Results.Ok(vehicle);
}).RequireAuthorization().WithTags("Vehicles");

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
}).RequireAuthorization().WithTags("Vehicles");

app.MapDelete("/vehicles/{id}", ([FromRoute]int id, IVehicleService vehicleService) =>
{
    var existingVehicle = vehicleService.GetVehicleById(id);
    
    if(existingVehicle == null)
    {
        return Results.NotFound(new { message = "Vehicle not found" });
    }
    
    vehicleService.DeleteVehicle(id);
    
    return Results.NoContent();
}).RequireAuthorization().WithTags("Vehicles");
#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
#endregion
