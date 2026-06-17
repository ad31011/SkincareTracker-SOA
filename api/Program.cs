using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SkincareTracker.API.Data;
using SkincareTracker.API.Interfaces;
using SkincareTracker.API.Models;
using SkincareTracker.API.Repositories;
using SkincareTracker.API.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IRoutineRepository, RoutineRepository>();
builder.Services.AddScoped<ISkinLogRepository, SkinLogRepository>();
builder.Services.AddScoped<IIngredientRepository, IngredientRepository>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IRoutineService, RoutineService>();
builder.Services.AddScoped<ISkinLogService, SkinLogService>();
builder.Services.AddScoped<IIngredientService, IngredientService>();

var jwtKey = builder.Configuration["Jwt:Key"] ?? "SuperSecretSkincareTrackerKey2024!XYZ";
var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactApp", policy =>
    {
        policy.WithOrigins(
            "http://localhost:5173",
            "http://localhost:3000",
            "https://skincare-tracker-soa.vercel.app"
        )
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Skincare Tracker API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("ReactApp");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    var adminEmail = config["AdminUser:Email"];
    if (!db.Users.Any(u => u.Email == adminEmail))
    {
        db.Users.Add(new User
        {
            Name = config["AdminUser:Name"] ?? "Administrator",
            Email = config["AdminUser:Email"] ?? "admin@skincaretracker.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(config["AdminUser:Password"] ?? "Admin123!"),
            Role = "Admin",
            SkinType = "Normal",
            SkinConcerns = "",
            CreatedAt = DateTime.UtcNow
        });
        db.SaveChanges();
    }
}

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
app.Run();
