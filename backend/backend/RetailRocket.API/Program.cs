using System.Text;
using System.Threading.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RetailRocket.API.Middleware;
using RetailRocket.Application.Interfaces.Mail;
using RetailRocket.Application.Interfaces.ML;
using RetailRocket.Application.Interfaces.Shop;
using RetailRocket.Application.Services.JWT;
using RetailRocket.Application.Services.ML;
using RetailRocket.Application.Services.Shop;
using RetailRocket.Application.Mappings.ML;
using RetailRocket.Application.Mappings.Shop;
using RetailRocket.Application.Mappings.Short;
using RetailRocket.Application.Services.Mail;
using RetailRocket.Application.Services.WebAccess;
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

// CORS Policy
builder.Services.AddCors(options =>
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .WithMethods("GET", "POST", "PUT", "DELETE")
            .AllowAnyHeader()
            .AllowCredentials();
    })
);

// JWT Authentification
var jwt = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
          ?? throw new InvalidOperationException("Missing 'Jwt' configuration section.");

builder.Services.AddOptions<JwtSettings>().BindConfiguration(JwtSettings.SectionName).ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key))
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                ctx.Token = ctx.Request.Cookies["access_token"];
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = ctx =>
            {
                Console.WriteLine($"JWT failed: {ctx.Exception.Message}");
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddScoped<JwtTokenService>();

// SMTP Service
builder.Services.AddOptions<SmtpSettings>().BindConfiguration(SmtpSettings.SectionName).ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddSingleton<IMailerService, MailerService>();

// WebAccess
builder.Services.AddOptions<WebAccessSettings>().BindConfiguration(WebAccessSettings.SectionName).ValidateDataAnnotations().ValidateOnStart();

// AutoMapper Configurations
builder.Services.AddAutoMapper(cfg => { cfg.AddMaps(typeof(RecommendationRuleMappingProfile).Assembly); });
builder.Services.AddAutoMapper(cfg => { cfg.AddMaps(typeof(CartMappingProfile).Assembly); });
builder.Services.AddAutoMapper(cfg => { cfg.AddMaps(typeof(OrderMappingProfile).Assembly); });
builder.Services.AddAutoMapper(cfg => { cfg.AddMaps(typeof(ProductMappingProfile).Assembly); });
builder.Services.AddAutoMapper(cfg => { cfg.AddMaps(typeof(UserMappingProfile).Assembly); });
builder.Services.AddAutoMapper(cfg => { cfg.AddMaps(typeof(CategoryShortMappingProfile).Assembly); });
builder.Services.AddAutoMapper(cfg => { cfg.AddMaps(typeof(ItemShortMappingProfile).Assembly); });
builder.Services.AddAutoMapper(cfg => { cfg.AddMaps(typeof(ProductShortMappingProfile).Assembly); });
builder.Services.AddAutoMapper(cfg => { cfg.AddMaps(typeof(UserShortMappingProfile).Assembly); });

// Rate Limiter
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("mail-service", opt =>
    {
        opt.PermitLimit = 4;
        opt.Window = TimeSpan.FromSeconds(12);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 2;
    });
});

var app = builder.Build();

// Add exception handler for web errors
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Seed database with example entities
// Mustn't go into production
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (app.Environment.IsDevelopment())
    {
        //await context.Database.EnsureDeletedAsync();
        await context.Database.MigrateAsync();
        //await AppDbContextSeeder.SeedAsync(context);
    }
    else
    {
        await context.Database.MigrateAsync();
        //await AppDbContextSeeder.SeedAsync(context);
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

// Rate Limiter appliance
app.UseRouting();
app.UseRateLimiter();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

// End appliances
app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigin");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();