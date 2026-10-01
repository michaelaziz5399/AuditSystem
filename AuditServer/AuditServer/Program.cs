#region Using Directives
using AuditServer.DataBase.DBContext;
using AuditServer.DataBase.Entities;
using AuditServer.Mapper;
using AuditServer.Services.AuthService;
using AuditServer.Services.BackgroundServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;
#endregion

var builder = WebApplication.CreateBuilder(args);


#region Database Context Configuration

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));
#endregion

#region Identity Framework Configuration

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;

    // User settings
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();
#endregion

#region Authentication & JWT Bearer Token Configuration

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["Secret"]
    ?? throw new InvalidOperationException("JWT Secret is not configured.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // set to true in production
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});
#endregion


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
#region Mapper

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(typeof(AuthMappingProfile).Assembly);
});

#endregion

#region تسجيل الخدمات (Scoped & Hosted Services)
builder.Services.AddScoped<IAuthRepo, AuthRepo>();
builder.Services.AddHostedService<TokenCleanupHostedService>();
#endregion
#region CORS Policy Configuration (For Angular Frontend Integration)

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
#endregion

var app = builder.Build();

#region Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // تفعيل OpenAPI و Scalar API UI
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "Audit Management System API Reference";
        options.Theme = ScalarTheme.Purple;
    });
}
#endregion

#region Database Migration and Seeding
app.UseHttpsRedirection();

app.UseCors("AllowAngularDev");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
#endregion