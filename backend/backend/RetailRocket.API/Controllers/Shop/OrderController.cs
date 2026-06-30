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
            User = order.User,
            CreatedAt = order.CreatedAt,
            Total = order.Total
        });
    }
    
    [HttpGet("{by-user}")]
    public async Task<IActionResult> GetAllByUser(Guid userId)
    {
        var orders = await _orderService.GetOrdersByUserAsync(userId);
        var result = orders.Select(o => new OrderDto
        {
            OrderId = o.OrderId,
            User = o.User,
            CreatedAt = o.CreatedAt,
            Total = o.Total
        });
        return Ok(result);
    }

    [HttpPost("{id}")]
    public async Task<IActionResult> Create(Guid id, [FromBody] CreateOrderDto dto)
    {
        var order = new Order(dto.User, dto.Total);
        await _orderService.AddOrderAsync(order);
        return CreatedAtAction(nameof(GetById), new {id = order.OrderId },  new OrderDto
        {
            OrderId = order.OrderId,
            User = order.User,
            CreatedAt = order.CreatedAt,
            Total = order.Total
        });
    }

    // WIP
    [HttpPut("{by-user}/{id}/price")]
    public async Task<IActionResult> UpdateTotalPrice([FromForm] decimal total)
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