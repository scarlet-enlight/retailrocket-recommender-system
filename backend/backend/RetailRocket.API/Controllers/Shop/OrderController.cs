using Microsoft.AspNetCore.Mvc;
using RetailRocket.Application.DTOs.Request.Shop;
using RetailRocket.Application.DTOs.Response.Shop;
using RetailRocket.Application.Services.Shop;
using RetailRocket.Domain.Entities.Shop;

namespace RetailRocket.API.Controllers.Shop;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly OrderService _orderService;
    
    public OrderController(OrderService orderService) =>
        _orderService = orderService;

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var order = await _orderService.GetOrderAsync(id);
        if (order is null) return NotFound();
        return Ok(new OrderDto
        {
            OrderId =  order.OrderId,
            CreatedAt = order.CreatedAt,
            Total = order.Total
        });
    }
    
    [HttpGet("by-user/{userId}")]
    public async Task<IActionResult> GetAllByUser(Guid userId)
    {
        var orders = await _orderService.GetOrdersByUserAsync(userId);
        var result = orders.Select(o => new OrderDto
        {
            OrderId = o.OrderId,
            CreatedAt = o.CreatedAt,
            Total = o.Total
        });
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
    {
        var order = new Order(dto.UserId, dto.Total);
        await _orderService.AddOrderAsync(order);
        return CreatedAtAction(nameof(GetById), new {id = order.OrderId },  new OrderDto
        {
            OrderId = order.OrderId,
            CreatedAt = order.CreatedAt,
            Total = order.Total
        });
    }

    // WIP
    [HttpPut("by-user/{userId}/price")]
    public async Task<IActionResult> UpdateTotalPrice(Guid id, [FromBody] decimal total)
    {
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var order = await _orderService.GetOrderAsync(id);
        if (order is null) return NotFound();
        await _orderService.DeleteOrderAsync(id);
        return NoContent();
    }
}