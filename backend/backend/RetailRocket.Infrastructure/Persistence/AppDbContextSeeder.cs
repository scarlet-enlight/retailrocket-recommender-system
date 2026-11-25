using RetailRocket.Domain.Entities.Historical;
using RetailRocket.Domain.Entities.ML;
using RetailRocket.Domain.Entities.Shop;
using RetailRocket.Application.Services.Security;

namespace RetailRocket.Infrastructure.Persistence;

public class AppDbContextSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        await SeedCategoriesAsync(context);
        await SeedVisitorsAsync(context);
        await SeedItemsAsync(context);
        await SeedTransactionsAsync(context);
        await SeedTransactionItemsAsync(context);
        await SeedEventsAsync(context);
        await SeedRecommendationRulesAsync(context);
        await SeedUsersAsync(context);
        await SeedProductsAsync(context);
        await SeedOrdersAsync(context);
        await SeedOrderItemsAsync(context);
        await SeedCartsAsync(context);
    }

    private static async Task SeedCategoriesAsync(AppDbContext context)
    {
        if (context.Categories.Any()) return;

        var root = new Category(1, null, "Kitchen");
        await context.Categories.AddRangeAsync(root);
        await context.SaveChangesAsync();
        
        var categories = new List<Category>
        {
            new Category(2, root.category_id, "Kitchen Appliances"),
            new Category(3, root.category_id, "Utensils"),
        };
        
        await context.Categories.AddRangeAsync(categories);
        await context.SaveChangesAsync();
    }

    private static async Task SeedEventsAsync(AppDbContext context)
    {
        if (context.Events.Any()) return;
        
        var visitorIds = context.Visitors.Select(v => v.visitor_id).ToList();
        var itemIds = context.Items.Select(i => i.item_id).ToList();

        var events = new List<Event>
        {
            new Event(visitorIds[0], itemIds[0], "view", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()),
            new Event(visitorIds[2], itemIds[1], "addtocart", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()),
            new Event(visitorIds[1], itemIds[1], "transaction", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()), 
        };
        
        await context.Events.AddRangeAsync(events);
        await context.SaveChangesAsync();
    }

    private static async Task SeedItemsAsync(AppDbContext context)
    {
        if (context.Items.Any()) return;

        var categoryIds = context.Categories.Select(c => c.category_id).ToList();

        var items = new List<Item>
        {
            new Item(3, categoryIds[1], true),
            new Item(1, categoryIds[2], true),
            new Item(2, categoryIds[2], false)
        };
        
        await context.Items.AddRangeAsync(items);
        await context.SaveChangesAsync();
    }
    
    private static async Task SeedTransactionsAsync(AppDbContext context)
    {
        if (context.Transactions.Any()) return;
        
        var visitorIds = context.Visitors.Select(v => v.visitor_id).ToList();
        
        var transactions = new List<Transaction>
        {
            new Transaction(2, visitorIds[1], DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()),
            new Transaction(3, visitorIds[2], DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()),
            new Transaction(1, visitorIds[0], DateTimeOffset.UtcNow.ToUnixTimeMilliseconds())
        };
        
        await context.Transactions.AddRangeAsync(transactions);
        await context.SaveChangesAsync();
    }
    
    private static async Task SeedTransactionItemsAsync(AppDbContext context)
    {
        if (context.TransactionItems.Any()) return;
        
        var transactionIds = context.Transactions.Select(t => t.transaction_id).ToList();
        var itemIds = context.Items.Select(i => i.item_id).ToList();

        var transactionItems = new List<TransactionItem> 
        {
            new TransactionItem(transactionIds[0], itemIds[1]),
            new TransactionItem(transactionIds[2], itemIds[0]),
            new TransactionItem(transactionIds[2], itemIds[1]),
        };
        
        await context.TransactionItems.AddRangeAsync(transactionItems);
        await context.SaveChangesAsync();
    }
    
    private static async Task SeedVisitorsAsync(AppDbContext context)
    {
        if (context.Visitors.Any()) return;

        var visitors = new List<Visitor>
        {
            new Visitor(1),
            new Visitor(2),
            new Visitor(3)
        };
        
        await context.Visitors.AddRangeAsync(visitors);
        await context.SaveChangesAsync();
    }
    
    private static async Task SeedRecommendationRulesAsync(AppDbContext context)
    {
        if (context.RecommendationRules.Any()) return;
        
        var ifItemIds =  context.Items.Select(i => i.item_id).ToList();
        var thenItemIds = context.Items.Select(i => i.item_id).ToList();

        var recommendationRules = new List<RecommendationRule>
        {
            new RecommendationRule(ifItemIds[2], thenItemIds[0], 0.746, 0.9788, 0.3476),
            new RecommendationRule(ifItemIds[0], thenItemIds[1], 0.89765, 0.6745, 0.8765),
            new RecommendationRule(ifItemIds[1], thenItemIds[2], 0.7685, 0.798765, 0.54978)
        };
        
        await context.RecommendationRules.AddRangeAsync(recommendationRules);
        await context.SaveChangesAsync();
    }
    
    private static async Task SeedCartsAsync(AppDbContext context)
    {
        if (context.Carts.Any()) return;
        
        var userIds = context.Users.Select(u => u.user_id).ToList();
        var productIds = context.Products.Select(p => p.product_id).ToList();

        var carts = new List<Cart>
        {
            new Cart(userIds[0], productIds[0], 12),
            new Cart(userIds[2], productIds[1], 87),
            new Cart(userIds[1], productIds[2], 3),
        };
        
        await context.Carts.AddRangeAsync(carts);
        await context.SaveChangesAsync();
    }
    
    private static async Task SeedOrdersAsync(AppDbContext context)
    {
        if (context.Orders.Any()) return;
        
        var userIds = context.Users.Select(u => u.user_id).ToList();

        var orders = new List<Order>
        {
            new Order(userIds[2], 4578.76m),
            new Order(userIds[1], 645.22m),
            new Order(userIds[0], 21.37m)
        };
        
        await context.Orders.AddRangeAsync(orders);
        await context.SaveChangesAsync();
    }
    
    private static async Task SeedOrderItemsAsync(AppDbContext context)
    {
        if (context.OrderItems.Any()) return;
        
        var orderIds = context.Orders.Select(o => o.order_id).ToList();
        var productIds = context.Products.Select(i => i.product_id).ToList();

        var orderItems = new List<OrderItem>
        {
            new OrderItem(orderIds[0], productIds[1], 6, 657.22m),
            new OrderItem(orderIds[1], productIds[2], 128, 42069.67m),
            new OrderItem(orderIds[2], productIds[0], 1, 59.99m)
        };
        
        await context.OrderItems.AddRangeAsync(orderItems);
        await context.SaveChangesAsync();
    }
    
    private static async Task SeedProductsAsync(AppDbContext context)
    {
        if (context.Products.Any()) return;
        
        var itemIds = context.Items.Select(i => i.item_id).ToList();
        var categoryIds = context.Categories.Select(c => c.category_id).ToList();

        var products = new List<Product>
        {
            new Product(itemIds[1], "Microwave A65FD", 399.99m, categoryIds[1]),
            new Product(itemIds[0], "Cutting Knife", 45.99m, categoryIds[2]),
            new Product(itemIds[2], "Bowl", 23.99m, categoryIds[1])
        };
        
        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();
    }
    
    private static async Task SeedUsersAsync(AppDbContext context)
    {
        if (context.Users.Any()) return;

        var users = new List<User>
        {
            new("bułka", "bułka@retailrocket.pl", PasswordHasherService.Hash("KubaZjadłMasło")),
            new("nugat", "nugat@retailrocket.pl", PasswordHasherService.Hash("KluskiŚląskie2137!")),
            new("krowodrza", "krowodrza@retailrocket.pl", PasswordHasherService.Hash("Xx_P0l1b00d4,P0l1b00d4_xX"))
        };
        
        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();
    }
}