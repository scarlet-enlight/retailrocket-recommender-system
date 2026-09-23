using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using RetailRocket.API.Middleware;
using RetailRocket.Application.Interfaces.ML;
using RetailRocket.Application.Interfaces.Shop;
using RetailRocket.Application.Services.JWT;
using RetailRocket.Application.Services.ML;
using RetailRocket.Application.Services.Shop;
using RetailRocket.Application.Mappings.ML;
using RetailRocket.Application.Mappings.Shop;
using RetailRocket.Application.Mappings.Short;
using RetailRocket.Infrastructure.Persistence;
using RetailRocket.Infrastructure.Repositories.ML;
using RetailRocket.Infrastructure.Repositories.Shop;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IRecommendationRuleRepository, RecommendationRuleRepository>();

// Services
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<RecommendationRuleService>();

// JWT Authentification
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });
builder.Services.AddScoped<JwtTokenService>();

// AutoMapper Configurations
builder.Services.AddAutoMapper(typeof(RecommendationRuleMappingProfile).Assembly);
builder.Services.AddAutoMapper(typeof(CartMappingProfile).Assembly);
builder.Services.AddAutoMapper(typeof(OrderMappingProfile).Assembly);
builder.Services.AddAutoMapper(typeof(ProductMappingProfile).Assembly);
builder.Services.AddAutoMapper(typeof(UserMappingProfile).Assembly);
builder.Services.AddAutoMapper(typeof(CategoryShortMappingProfile).Assembly);
builder.Services.AddAutoMapper(typeof(ItemShortMappingProfile).Assembly);
builder.Services.AddAutoMapper(typeof(ProductShortMappingProfile).Assembly);
builder.Services.AddAutoMapper(typeof(UserShortMappingProfile).Assembly);

// CORS Policy
builder.Services.AddCors(options =>
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:7228",
                "https://localhost:6086",
                "http://localhost:5173",
                "https://localhost:5173")
            .WithMethods("GET", "POST", "PUT", "DELETE")
            .AllowAnyHeader();
    })
    );

var app = builder.Build();

// Add exception handler for web errors
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Seed database with example entities
// Mustn't go into production
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await context.Database.MigrateAsync();
    await AppDbContextSeeder.SeedAsync(context);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

// End appliances
app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigin");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();