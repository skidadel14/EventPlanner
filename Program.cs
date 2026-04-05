using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using EventHubAPI.Data;
using EventHubAPI.Services.Interfaces;
using EventHubAPI.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// 1. AddDbContext<AppDbContext> with UseSqlServer
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. AddScoped for all services
builder.Services.AddScoped<IOrganizerService, OrganizerService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IAttendeeService, AttendeeService>();

// 3. AddAuthentication with JwtBearer
const string jwtKey = "EventHubSuperSecretJwtKeyThatIsAtLeast32Chars!";
const string jwtIssuer = "EventHubAPI";
const string jwtAudience = "EventHubClient";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
        // Read token from "auth_token" cookie instead of Authorization header
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.ContainsKey("auth_token"))
                {
                    context.Token = context.Request.Cookies["auth_token"];
                }
                return Task.CompletedTask;
            }
        };
    });

// 4. AddAuthorization
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "EventHub API", Version = "v1" });

    // Cookie-based Authentication for Swagger
    c.AddSecurityDefinition("cookieAuth", new OpenApiSecurityScheme
    {
        Description = "JWT stored in the 'auth_token' HttpOnly cookie. Login via /api/Auth/organizer/login or /api/Auth/attendee/login to set the cookie automatically.",
        Name = "auth_token",
        In = ParameterLocation.Cookie,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "cookieAuth"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "cookieAuth"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Auto-apply migrations or ensure database creation on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try 
    {
        var db = services.GetRequiredService<AppDbContext>();
        // Using Migrate() is consistent and ensures the database is created
        // and all migrations are applied.
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "EventHub API v1"));
}
else 
{
    app.UseHttpsRedirection();
}

// UseAuthentication MUST come before UseAuthorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
