using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Repositories.Context;
using VaccineScheduleAPI;
using System.Text;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Register HttpClient 
builder.Services.AddHttpClient();

// Add services to the container.
builder.Services.AddControllers();

// Get the connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

//// Register DbContext with MySQL provider (Pomelo)
//builder.Services.AddDbContext<DatabaseContext>(options =>
//    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
//);

// Add Swagger/OpenAPI support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "Enter 'Bearer' [space] and then your token"
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

    // ✅ Add support for file uploads in Swagger
    options.OperationFilter<SwaggerFileOperationFilter>();
});

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Allow your frontend origin
              .AllowAnyMethod() // Allow GET, POST, etc.
              .AllowAnyHeader() // Allow all headers (e.g., Content-Type)
              .AllowCredentials(); // Allow cookies/auth if needed
    });
});
// Register database context
// If you were using SQL Server or another database provider, you could configure it here as needed
// Example:
// builder.Services.AddDbContext<DatabaseContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
// );

// Register custom services and repositories
builder.Services.AddConfig(builder.Configuration); // Registers services and repositories
// Firebase Authentication Configuration
var firebaseIssuer = builder.Configuration["Firebase:Issuer"];
var firebaseAudience = builder.Configuration["Firebase:Audience"];
// Set up JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
Console.WriteLine("JWT Key: " + jwtSettings["Key"]);  // Debugging step

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],

            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // Check if the token starts with "Bearer " and remove it
                if (context.Request.Headers.ContainsKey("Authorization"))
                {
                    var authorizationHeader = context.Request.Headers["Authorization"].ToString();
                    if (authorizationHeader.StartsWith("Bearer ", StringComparison.InvariantCultureIgnoreCase))
                    {
                        context.Token = authorizationHeader.Substring("Bearer ".Length).Trim();
                    }
                    else
                    {
                        context.Token = authorizationHeader;  // Use the token directly if no "Bearer" prefix
                    }
                }

                Console.WriteLine($"🔍 Received Token: {context.Token}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var jwtToken = context.SecurityToken as Microsoft.IdentityModel.JsonWebTokens.JsonWebToken;
                if (jwtToken != null)
                {
                    Console.WriteLine($"✅ Token Issuer: {jwtToken.Issuer}");
                    Console.WriteLine($"✅ Token Audience: {jwtToken.Audiences.FirstOrDefault()}");
                }
                else
                {
                    Console.WriteLine("❌ Failed to parse JWT token.");
                }
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"❌ JWT Authentication Failed: {context.Exception.Message}");
                return Task.CompletedTask;
            }
        };
    });

// Add authorization policy (optional)
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Bearer", policy =>
        policy.RequireAuthenticatedUser());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use HTTPS redirection and authentication/authorization middleware
app.UseHttpsRedirection();
app.UseCors("AllowFrontend"); // Add this line to enable CORS
app.UseAuthentication();  // Added authentication middleware
app.UseAuthorization();   // Add authorization middleware
app.MapControllers();     // Map the controllers
app.Run();                // Run the application
